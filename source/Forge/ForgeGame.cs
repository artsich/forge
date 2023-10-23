using Forge.Graphics;
using Forge.Graphics.Buffers;
using Forge.Graphics.Shaders;
using Forge.Physics;
using Forge.Renderer;
using Forge.Renderer.Components;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Drawing;
using Shader = Forge.Graphics.Shaders.Shader;

namespace Forge;

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

		int startX = -screenWidth / 2;
		int startY = -screenHeight / 2;

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
	private static CompiledShader PostProcessingShader;

	private static readonly string VertexShaderSource = @"
        #version 330 core
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
#version 330 core

out vec4 FragColor;

in vec2 fragTexCoord;
in float fragFade;
in vec4 fragColor;

uniform float timeTotal;

uniform sampler2D texture1;

vec3 lightColor = vec3(1.0, 0.0, 1.0);

void main()
{
	vec3 texColor = texture(texture1, fragTexCoord).xyz;
	vec3 c = mix(fragColor.rgb, lightColor, sin(timeTotal)) * texColor;
	FragColor = vec4(c, 1.0);
}
";

	private static readonly string PostProcesssingVertexShader = @"
#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTexCoords;

out vec2 TexCoords;

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, 0.0, 1.0); 
    TexCoords = aTexCoords;
}
";

	private static readonly string PostProcesssingFragmentShader = @"
#version 330 core
out vec4 FragColor;
  
in vec2 TexCoords;

uniform sampler2D screenTexture;

void main()
{ 
    FragColor = texture(screenTexture, TexCoords);
}
";

	private readonly CameraData camera = new(Matrix4X4.CreateOrthographic(1280, 720, 0.1f, 100.0f));

	private SimpleRenderer? renderer;

	private Camera2DController camera2D;

	private Texture2d texture;

	private FrameBuffer framebuffer;

	private float[] quadVertices = { // vertex attributes for a quad that fills the entire screen in Normalized Device Coordinates.
		// positions // texCoords
		-1.0f,  1.0f, 0.0f, 1.0f,
		-1.0f, -1.0f, 0.0f, 0.0f,
		 1.0f, -1.0f, 1.0f, 0.0f,

		-1.0f,  1.0f, 0.0f, 1.0f,
		 1.0f, -1.0f, 1.0f, 0.0f,
		 1.0f,  1.0f, 1.0f, 1.0f
	};

	private VertexArrayBuffer quadVao;
	private Buffer<float> quadVBO;

	public static GameTime Time { get; private set; }

	private VerletSolver solver;
	private List<VerletCircleObject> verletObjects;
	private List<Link> verletLinks;

	private readonly Random r = new();

	protected override void LoadGame()
	{
		Gl = GraphicsDevice!.gl;
		camera2D = new Camera2DController(camera, PrimaryMouse!)
		{
			Speed = 2000f
		};

		renderer = new SimpleRenderer(GraphicsDevice);

		Shader = new Shader(
			GraphicsDevice,
			new Shader.ShaderPart(VertexShaderSource, ShaderType.VertexShader),
			new Shader.ShaderPart(FragmentShaderSource, ShaderType.FragmentShader))
		.Compile() ?? throw new InvalidOperationException("Shader compilation error!");

		PostProcessingShader = new Shader(
			GraphicsDevice,
				new Shader.ShaderPart(PostProcesssingVertexShader, ShaderType.VertexShader),
				new Shader.ShaderPart(PostProcesssingFragmentShader, ShaderType.FragmentShader))
			.Compile() ?? throw new InvalidOperationException("Shader compilation error!");

		framebuffer = new FrameBuffer(GraphicsDevice);
		framebuffer.Configure(1280, 720);

		texture = new Texture2d(GraphicsDevice, new byte[] { 0, 255, 0, 255 }, 1, 1);

		quadVao = new VertexArrayBuffer(GraphicsDevice);
		quadVao.Bind();
		quadVBO = Graphics.Buffers.Buffer.Vertex.New(GraphicsDevice, quadVertices, BufferUsageARB.StaticDraw);
		quadVBO.Bind();

		Gl.EnableVertexAttribArray(0);
		Gl.VertexAttribPointer(0, 2, GLEnum.Float, false, 4 * sizeof(float), (void*)0);
		Gl.EnableVertexAttribArray(1);
		Gl.VertexAttribPointer(1, 2, GLEnum.Float, false, 4 * sizeof(float), (void*)(2 * sizeof(float)));

		quadVao.Unbind();

		verletObjects = new List<VerletCircleObject>
		{
			new VerletCircleObject(new Vector2D<float>(0, 10), 10) { IsStatic = true },
			new VerletCircleObject(new Vector2D<float>(20, 0), 10),
			new VerletCircleObject(new Vector2D<float>(40, 0), 10),
			new VerletCircleObject(new Vector2D<float>(60, 0), 10),
			new VerletCircleObject(new Vector2D<float>(80, 0), 10),
			new VerletCircleObject(new Vector2D<float>(100, 0), 10),
			new VerletCircleObject(new Vector2D<float>(120, 0), 10),
			new VerletCircleObject(new Vector2D<float>(140, 0), 10),
			new VerletCircleObject(new Vector2D<float>(160, 0), 10),
			new VerletCircleObject(new Vector2D<float>(180, 0), 10) { IsStatic = true },
		};

		verletLinks = new List<Link>()
		{
			new Link(verletObjects[0], verletObjects[1], 25f),
			new Link(verletObjects[1], verletObjects[2], 25f),
			new Link(verletObjects[2], verletObjects[3], 25f),
			new Link(verletObjects[3], verletObjects[4], 25f),
			new Link(verletObjects[4], verletObjects[5], 25f),
			new Link(verletObjects[5], verletObjects[6], 25f),
			new Link(verletObjects[6], verletObjects[7], 25f),
			new Link(verletObjects[7], verletObjects[8], 25f),
			new Link(verletObjects[8], verletObjects[9], 25f),
		};

		solver = new VerletSolver(verletObjects, verletLinks);
	}

	protected override void OnRender(double delta)
	{
		framebuffer.Bind();
		Gl.Clear(ClearBufferMask.ColorBufferBit);

		Shader.Bind();
		texture.Bind();

		Gl.Viewport(new Size(1280, 720));

		var batch = renderer!.StartDrawCircles();

		foreach (var obj in verletObjects)
		{
			var renderInfo = new CircleRenderComponent()
			{
				Color = new Vector4D<float>(1.0f, 0.5f, 0.5f, 1.0f),
				Position = obj.PositionCurrent,
				Radius = obj.Radius,
			};

			batch.Add(ref renderInfo);
		}

		renderer.FlushAll();
		framebuffer.Unbind();

		Gl.Clear(ClearBufferMask.ColorBufferBit);

		Gl.Viewport(_window.Size);

		PostProcessingShader.Bind();
		quadVao.Bind();
		Gl.Disable(EnableCap.DepthTest);
		Gl.BindTexture(TextureTarget.Texture2D, framebuffer.Texture0);
		Gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
	}

	protected override void OnUpdate(GameTime time)
	{
		AddRenderTask(() =>
		{
			Shader.BindUniforms(camera2D.CameraData);
		});

		Time = time;
		camera2D.Update(time);

		solver.Update(time.DeltaTime);

		if (PrimaryKeyboard!.IsKeyPressed(Key.Space))
		{
			verletObjects.Add(
				new VerletCircleObject(
					new Vector2D<float>(
						r.Next(-200, 200),
						r.Next(-200, 200)),
					r.Next(10, 20)));
		}
	}

	protected override void OnClose()
	{
		Shader.Dispose();
	}

	protected override void OnResize(Vector2D<int> size)
	{
	}
}