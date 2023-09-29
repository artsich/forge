using Forge.Graphics.Buffers;
using Forge.Graphics.Shaders;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Drawing;
using System.Runtime.CompilerServices;
using Shader = Forge.Graphics.Shaders.Shader;
using Buffer = Forge.Graphics.Buffers.Buffer;

namespace Forge;

public readonly struct CircleVertexData
{
	public readonly Vector3D<float> Position;
	public readonly Vector4D<float> Color;
	public readonly Vector2D<float> TexCoord;
	public readonly float Fade;

	private static readonly int[] componentsCount = new int[4]
	{
		3,
		4,
		2,
		1
	};

	private static readonly int[] offsets = new int[4]
	{
		Unsafe.SizeOf<Vector3D<float>>(),
		Unsafe.SizeOf<Vector4D<float>>(),
		Unsafe.SizeOf<Vector2D<float>>(),
		sizeof(float)
	};

	public CircleVertexData(Vector3D<float> position, Vector2D<float> texCoord, Vector4D<float> color, float fade)
	{
		Position = position;
		TexCoord = texCoord;
		Color = color;
		Fade = fade;
	}

	public static unsafe void EnableVertexPointer(GL gl)
	{
		uint size = (uint)Unsafe.SizeOf<CircleVertexData>();
		int offset = 0;
		for (int i = 0; i < offsets.Length; i++)
		{
			gl.EnableVertexAttribArray((uint)i);
			gl.VertexAttribPointer((uint)i, componentsCount[i], VertexAttribPointerType.Float, false, size, (void*)offset);

			offset += offsets[i];
		}
	}
}

public unsafe class ForgeGame : GameBase
{
	private static GL Gl;
	private static Buffer<CircleVertexData> Vbo;
	private static Buffer<uint> Ebo;
	private static VertexArrayBuffer Vao;
	private static CompiledShader Shader;

	private static readonly string VertexShaderSource = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec3 vPos;
		layout (location = 1) in vec4 vColor;
		layout (location = 2) in vec2 vTexCoord;
		layout (location = 3) in float vFade;

		uniform mat4 cameraProj;
		uniform mat4 cameraView;
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
			gl_Position = cameraProj * cameraView * pos;
        }
        ";

	private static readonly string FragmentShaderSource = @"
#version 330 core

out vec4 FragColor;

in vec2 fragTexCoord;
in float fragFade;
in vec4 fragColor;

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
	FragColor = col * fragColor;
}
";

	private static readonly CircleVertexData[] Vertices =
	{
		new CircleVertexData(new ( 1f,  1f, 0f), new (1.0f, 1.0f), Vector4D<float>.UnitX, 0.00f),
		new CircleVertexData(new ( 1f, -1f, 0f), new (1.0f, 0.0f), Vector4D<float>.UnitX, 0.00f),
		new CircleVertexData(new (-1f, -1f, 0f), new (0.0f, 0.0f), Vector4D<float>.UnitX, 0.00f),
		new CircleVertexData(new (-1f,  1f, 0f), new (0.0f, 1.0f), Vector4D<float>.UnitZ, 0.00f),
	};

	private static readonly uint[] Indices =
	{
		0, 1, 3,
		1, 2, 3
	};

	private readonly Camera camera = new(Matrix4X4.CreateOrthographic(1280, 720, 0.1f, 100.0f));

	protected override void LoadGame()
	{
		Gl = GraphicsDevice!.gl;

		Vao = new VertexArrayBuffer(GraphicsDevice);
		Vao.Bind();

		Vbo = Buffer.Vertex.New<CircleVertexData>(GraphicsDevice, Vertices);
		Vbo.Bind();

		Ebo = Buffer.Index.New<uint>(GraphicsDevice, Indices);
		Ebo.Bind();

		Shader = new Shader(
			GraphicsDevice,
			new Shader.ShaderPart(VertexShaderSource, ShaderType.VertexShader),
			new Shader.ShaderPart(FragmentShaderSource, ShaderType.FragmentShader))
		.Compile() ?? throw new InvalidOperationException("Shader compilation error!");

		CircleVertexData.EnableVertexPointer(Gl);
	}

	protected override void Render(double delta)
	{
		Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

		Vao.Bind();
		Shader.Bind();

		Gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
	}

	protected override void Update(GameTime time)
	{
		AddRenderTask(() =>
		{
			Shader.BindUniforms(time, camera);
			Shader["model"]!.SetValue(Matrix4X4.CreateScale(100f));
		});
	}

	protected override void OnClose()
	{
		Vbo.Dispose();
		Ebo.Dispose();
		Vao.Dispose();
		Shader.Dispose();
	}

	protected override void OnResize(Vector2D<int> obj)
	{
		Gl.Viewport(new Size(obj.X, obj.Y));
		camera.Projection = Matrix4X4.CreateOrthographic(obj.X, obj.Y, 0.1f, 100.0f);
	}
}