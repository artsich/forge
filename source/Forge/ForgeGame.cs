using Forge.Graphics.Shaders;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Drawing;
using Shader = Forge.Graphics.Shaders.Shader;
using Forge.Renderer.Components;
using Forge.Renderer;

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

	protected override void LoadGame()
	{
		Gl = GraphicsDevice!.gl;
		camera2D = new Camera2DController(camera, new CameraMoveDir(PrimaryKeyboard!))
		{
			Speed = 100f
		};

		renderer = new SimpleRenderer(GraphicsDevice);

		Shader = new Shader(
			GraphicsDevice,
			new Shader.ShaderPart(VertexShaderSource, ShaderType.VertexShader),
			new Shader.ShaderPart(FragmentShaderSource, ShaderType.FragmentShader))
		.Compile() ?? throw new InvalidOperationException("Shader compilation error!");
	}

	protected override void Render(double delta)
	{
		Gl.Clear(ClearBufferMask.ColorBufferBit);

		Shader.Bind();

		circleDrawer.DrawCircles(renderer!);
	}

	protected override void Update(GameTime time)
	{
		AddRenderTask(() =>
		{
			Shader.BindUniforms(time, camera2D.CameraData);
		});

		camera2D.Update(time);
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