using Forge.Graphics.Shaders;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Drawing;
using Forge.Graphics.Buffers;
using Shader = Forge.Graphics.Shaders.Shader;
using Buffer = Forge.Graphics.Buffers.Buffer;

namespace Forge;

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
		uniform mat4 model;

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

	private readonly Camera camera = new(Matrix4X4.CreateOrthographic(1280, 720, 0.1f, 100.0f));

	private BatchRenderer? renderer;

	private readonly CircleRenderComponent[] circles = new CircleRenderComponent[]
	{
		new CircleRenderComponent(new (1f, 0f, 0f, 0f), new (0f, 0f), 20, 0.05f),
		new CircleRenderComponent(new (1f, 0f, 0f, 0f), new (-40f, 40f), 20, 0.05f),
		new CircleRenderComponent(new (1f, 0f, 0f, 0f), new (-40f, -40f), 20, 0.05f),
		new CircleRenderComponent(new (1f, 0f, 0f, 0f), new (40f, 40f), 20, 0.05f),
		new CircleRenderComponent(new (1f, 0f, 0f, 0f), new (40f, -40f), 20, 0.05f),
	};

	protected override void LoadGame()
	{
		Gl = GraphicsDevice!.gl;

		renderer = new BatchRenderer(GraphicsDevice);

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

		for (int i = 0; i < circles.Length ; i++) 
		{
			ref var circle = ref circles[i];
			renderer!.AddCircle(circle);
		}

		renderer!.EndBatch();
	}

	protected override void Update(GameTime time)
	{
		AddRenderTask(() =>
		{
			Shader.BindUniforms(time, camera);
			Shader["model"]!.SetValue(Matrix4X4.CreateScale(2f));
		});
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