using Forge.Graphics;
using Forge.Graphics.Buffers;
using Forge.Graphics.Shaders;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Runtime.CompilerServices;
using Shader = Forge.Graphics.Shaders.Shader;

namespace Forge;

class Program
{
	private static IWindow window;
	private static GL Gl;

	private static GraphicsDevice Gd;
	private static Buffer<Vector3D<float>> Vbo;
	private static Buffer<uint> Ebo;
	private static Forge.Graphics.Buffers.VertexArray Vao;
	private static CompiledShader Shader;

	private static readonly string VertexShaderSource = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec4 vPos;
        
        void main()
        {
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";

	private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

        void main()
        {
            vec3 color1 = vec3(1.0, 0.0, 0.0); //red
            vec3 color2 = vec3(0.0, 0.0, 1.0); //blue
            
            vec3 finalColor = mix(color1, color2, gl_FragCoord.y / 600.0);
            FragColor = vec4(finalColor, 1.0);
        }
        ";

	private static readonly Vector3D<float>[] Vertices =
	{
        new (0.5f,  0.5f, 0.0f),
		new ( 0.5f, -0.5f, 0.0f),
		new (-0.5f, -0.5f, 0.0f),
		new (-0.5f,  0.5f, 0.5f)
	};

	private static readonly uint[] Indices =
	{
		0, 1, 3,
		1, 2, 3
	};

	private static void Main(string[] args)
	{
		var options = WindowOptions.Default;
		options.Size = new Vector2D<int>(1280, 720);
		options.Title = "LearnOpenGL with Silk.NET";
		window = Window.Create(options);

		window.Load += OnLoad;
		window.Render += OnRender;
		window.Update += OnUpdate;
		window.Closing += OnClose;
		window.Resize += OnResize;

		window.Run();

		window.Dispose();
	}

	private static void OnResize(Vector2D<int> obj)
	{
		Gl.Viewport(new System.Drawing.Size(obj.X, obj.Y));
	}

	private static unsafe void OnLoad()
	{
		Gd = GraphicsDevice.InitOpengl(window);
		Gl = Gd.gl;

		Vao = new Graphics.Buffers.VertexArray(Gd);
		Vao.Bind();

		Vbo = Graphics.Buffers.Buffer.Vertex.New(Gd, Vertices);
		Vbo.Bind();

		Ebo = Graphics.Buffers.Buffer.Index.New(Gd, Indices);
		Ebo.Bind();

		Shader = new Shader(
			Gd,
			new Shader.ShaderPart(VertexShaderSource, ShaderType.VertexShader),
			new Shader.ShaderPart(FragmentShaderSource, ShaderType.FragmentShader))
		.Compile();

		Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)Unsafe.SizeOf<Vector3D<float>>(), null);
		Gl.EnableVertexAttribArray(0);
	}

	private static unsafe void OnRender(double obj)
	{
		Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

		Vao.Bind();
		Shader.Bind();

		Gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
	}

	private static void OnUpdate(double obj)
	{
	}

	private static void OnClose()
	{
		Vbo.Dispose();
		Ebo.Dispose();
		Vao.Dispose();
		Shader.Dispose();
	}
}