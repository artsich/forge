﻿using Forge.Graphics.Shaders;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Drawing;
using Shader = Forge.Graphics.Shaders.Shader;
using Forge.Renderer.Components;
using Forge.Renderer;

namespace Forge;

public class VervletCircleObject
{
	public Vector2D<float> PositionCurrent;
	public Vector2D<float> PositionOld;
	public Vector2D<float> Acceleration;

	public float Radius;

	public bool IsStatic = false;

	public VervletCircleObject(Vector2D<float> positionCurrent, float radius)
	{
		PositionCurrent = positionCurrent;
		PositionOld = positionCurrent;
		Radius = radius;
	}

	public void UpdatePosition(float dt)
	{
		if (!IsStatic)
		{
			Vector2D<float> velocity = PositionCurrent - PositionOld;
			PositionOld = PositionCurrent;
			PositionCurrent += velocity + Acceleration * dt * dt;

			Acceleration = Vector2D<float>.Zero;
		}
	}

	public void ApplyForce(Vector2D<float> force)
	{
		Acceleration += force;
	}
}


public class Link
{
	public VervletCircleObject Object1;
	public VervletCircleObject Object2;

	public float TargetDistance;

	public void Apply()
	{
		var collisionAxis = Object1.PositionCurrent - Object2.PositionCurrent;
		var distance = collisionAxis.Length;

		var n = collisionAxis / distance;
		float delta = TargetDistance - distance;

		Object1.PositionCurrent += n * delta * 0.5f;
		Object2.PositionCurrent -= n * delta * 0.5f;
	}
}

public class Solver
{
	private readonly IList<VervletCircleObject> objects;
	private readonly IList<Link> links;
	Vector2D<float> gravity = new(0.0f, -1000f);

	public Solver(IList<VervletCircleObject> objects, IList<Link> links)
	{
		this.objects = objects;
		this.links = links;
	}

	public void Update(float dt)
	{
		var steps = 8;
		var dtStep = dt / steps;

		for(int i = 0; i < steps; i++)
		{
			ApplyGravity();
			ApplyConstraints();
			SolveCollisions();
			UpdatePositions(dtStep);
		}
	}

	private void UpdatePositions(float dt)
	{
		foreach (VervletCircleObject obj in objects)
		{
			obj.UpdatePosition(dt);
		}
	}

	private void ApplyGravity()
	{
		foreach(var obj in objects)
		{
			obj.ApplyForce(gravity);
		}
	}

	private void ApplyConstraints()
	{
		var pos = Vector2D<float>.Zero;
		var radius = 500;
		var myradius = 10f;

		foreach(var obj in objects)
		{
			var to_obj = obj.PositionCurrent - pos;
			var distance = to_obj.Length;

			var r_sum = myradius + radius;
			if (r_sum < distance)
			{
				var n = to_obj / distance;

				obj.PositionCurrent += n * (r_sum - distance);
			}
		}

		foreach (var link in links)
		{
			link.Apply();
		}
	}

	private void SolveCollisions()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			var object_1 = objects[i];
			for (int j = i+1; j < objects.Count; j++)
			{
				var object_2 = objects[j];

				TryResolve(object_1, object_2);
			}
		}
	}

	private static void TryResolve(VervletCircleObject object_1, VervletCircleObject object_2)
	{
		var collisionAxis = object_1.PositionCurrent - object_2.PositionCurrent;
		var distance = collisionAxis.Length;
		var r_sum = object_1.Radius + object_2.Radius;

		if (distance < r_sum)
		{
			var n = collisionAxis / distance;
			float delta = r_sum - distance;

			object_1.PositionCurrent += n * delta * 0.5f;
			object_2.PositionCurrent -= n * delta * 0.5f;
		}
	}
}

public class CircleDrawer
{
	private const int CircleCount = 5000;

	private readonly int screenWidth;
	private readonly int screenHeight;

	public CircleDrawer(int screenWidth, int screenHeight)
	{
		this.screenWidth = screenWidth;
		this.screenHeight = screenHeight;
	}

	public void DrawCircles(SimpleRenderer renderer)
	{
		int circlesInXDirection = (int)Math.Sqrt(CircleCount * screenWidth / (float)screenHeight);
		int circlesInYDirection = CircleCount / circlesInXDirection;

		Vector4D<float> color = new(1.0f, 0.5f, 0.5f, 1.0f);
		float fade = 0.04f;
		float radius = 10.0f;

		var batch = renderer.StartDrawCircles();

		float deltaX = radius * 2;
		float deltaY = radius * 2;

		int startX = -screenWidth/2;
		int startY = -screenHeight/2;

		for (int i = 0; i < circlesInXDirection; i++)
		{
			for (int j = 0; j < circlesInYDirection; j++)
			{
				Vector2D<float> position = new(startX + i * deltaX, startY + j * deltaY);
				var circle = new CircleRenderComponent(color, position, radius, fade);
				batch.Add(ref circle);
			}
		}

		renderer.FlushAll();
	}
}

public unsafe class ForgeGame : GameBase
{
	private static GL Gl;

	private static CompiledShader Shader;

	private static readonly string VertexShaderSource = @"
        #version 460
        layout (location = 0) in vec3 vPos;
		layout (location = 1) in vec4 vColor;
		layout (location = 2) in vec2 vTexCoord;
		layout (location = 3) in float vFade;

		uniform mat4 cameraViewProj;
		uniform mat4 model = mat4(1.0);

		out vec2 fragTexCoord;
		out float fragFade;
		out vec4 fragColor;

        void main()
        {
			fragTexCoord = vTexCoord;
			fragFade = vFade;
			fragColor = vColor;

			vec4 pos = model * vec4(vPos, 1.0);
			gl_Position = cameraViewProj * pos;
        }
        ";

	private static readonly string FragmentShaderSource = @"
#version 460

out vec4 FragColor;

in vec2 fragTexCoord;
in float fragFade;
in vec4 fragColor;

uniform float timeTotal;

vec3 lightColor = vec3(1.0, 0.0, 1.0);

const float radius = 0.5;

float circle(vec2 uv, vec2 circleCenter, float r, float blur)
{
    float d = length(uv - circleCenter);
    return smoothstep(r, r-blur, d);
}

void main()
{
	vec2 center = vec2(0.5, 0.5);
	vec2 uv = fragTexCoord;

	float col = circle(uv, center, radius, fragFade);

	vec3 c = mix(fragColor.rgb, lightColor, sin(timeTotal));
	FragColor = col * vec4(c, 1.0);
}
";

	private readonly CameraData camera = new(Matrix4X4.CreateOrthographic(1280, 720, 0.1f, 100.0f));

	private SimpleRenderer? renderer;

	private Camera2DController camera2D;

	private readonly CircleDrawer circleDrawer = new(1920, 1080);

	public static GameTime Time { get; private set; }

	Solver solver;
	List<VervletCircleObject> vervletObjects;
	List<Link> vervletLinks;

	Random r = new Random();

	object locker = new object();

	protected override void LoadGame()
	{
		PrimaryKeyboard.KeyDown += (_, key, _) =>
		{
			if (key == Silk.NET.Input.Key.Space)
			{
				AddGameLogicTask(() =>
				{
					lock (locker)
					{
						vervletObjects.Add(
							new VervletCircleObject(
								new Vector2D<float>(
									r.Next(-200, 200),
									r.Next(-200, 200)),
								r.Next(10, 20)));
					}
				});
			}
		};

		Gl = GraphicsDevice!.gl;
		camera2D = new Camera2DController(camera, PrimaryMouse!)
		{
			Speed = 100f
		};

		renderer = new SimpleRenderer(GraphicsDevice);

		Shader = new Shader(
			GraphicsDevice,
			new Shader.ShaderPart(VertexShaderSource, ShaderType.VertexShader),
			new Shader.ShaderPart(FragmentShaderSource, ShaderType.FragmentShader))
		.Compile() ?? throw new InvalidOperationException("Shader compilation error!");

		vervletObjects = new()
		{
			new VervletCircleObject(new Vector2D<float>(0, 10), 10) { IsStatic = true },
			new VervletCircleObject(new Vector2D<float>(20, 0), 10),
			new VervletCircleObject(new Vector2D<float>(40, 0), 10),
			new VervletCircleObject(new Vector2D<float>(60, 0), 10),
			new VervletCircleObject(new Vector2D<float>(80, 0), 10),
			new VervletCircleObject(new Vector2D<float>(100, 0), 10),
			new VervletCircleObject(new Vector2D<float>(120, 0), 10),
			new VervletCircleObject(new Vector2D<float>(140, 0), 10),
			new VervletCircleObject(new Vector2D<float>(160, 0), 10),
			new VervletCircleObject(new Vector2D<float>(180, 0), 10) { IsStatic = true },
		};

		vervletLinks = new List<Link>()
		{
			new Link()
			{
				Object1 = vervletObjects[0],
				Object2 = vervletObjects[1],
				TargetDistance = 25,
			},
			new Link()
			{
				Object1 = vervletObjects[1],
				Object2 = vervletObjects[2],
				TargetDistance = 25,
			},
			new Link()
			{
				Object1 = vervletObjects[2],
				Object2 = vervletObjects[3],
				TargetDistance = 25,
			},
			new Link()
			{
				Object1 = vervletObjects[3],
				Object2 = vervletObjects[4],
				TargetDistance = 25,
			},
			new Link()
			{
				Object1 = vervletObjects[4],
				Object2 = vervletObjects[5],
				TargetDistance = 25,
			},
			new Link()
			{
				Object1 = vervletObjects[5],
				Object2 = vervletObjects[6],
				TargetDistance = 25,
			},
			new Link()
			{
				Object1 = vervletObjects[6],
				Object2 = vervletObjects[7],
				TargetDistance = 25,
			},
			new Link()
			{
				Object1 = vervletObjects[7],
				Object2 = vervletObjects[8],
				TargetDistance = 25,
			},
			new Link()
			{
				Object1 = vervletObjects[8],
				Object2 = vervletObjects[9],
				TargetDistance = 25,
			},
		};

		solver = new Solver(vervletObjects, vervletLinks);
	}

	protected override void Render(double delta)
	{
		Gl.Clear(ClearBufferMask.ColorBufferBit);

		Shader.Bind();

		var batch = renderer.StartDrawCircles();

		lock (locker)
		{
			foreach (var obj in vervletObjects)
			{
				var renderInfo = new CircleRenderComponent()
				{
					Color = new Vector4D<float>(1.0f, 0.5f, 0.5f, 1.0f),
					Position = obj.PositionCurrent,
					Radius = obj.Radius,
				};

				batch.Add(ref renderInfo);
			}
		}

		renderer.FlushAll();

		//circleDrawer.DrawCircles(renderer!);
	}

	protected override void Update(GameTime time)
	{
		AddRenderTask(() =>
		{
			Shader.BindUniforms(time, camera2D.CameraData);
		});

		Time = time;
		camera2D.Update(time);

		solver.Update(time.DeltaTime);
	}

	protected override void OnClose()
	{
		Shader.Dispose();
	}

	protected override void OnResize(Vector2D<int> obj)
	{
		Gl.Viewport(new Size(obj.X, obj.Y));
		camera.Projection = Matrix4X4.CreateOrthographic(obj.X, obj.Y, 0.1f, 100.0f);
	}
}