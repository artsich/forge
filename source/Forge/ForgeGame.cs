using Forge;
using Forge.Graphics;
using Forge.Graphics.Buffers;
using Forge.Graphics.Shaders;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Numerics;
using System.Runtime.CompilerServices;
using Shader = Forge.Graphics.Shaders.Shader;

public unsafe class ForgeGame : GameBase
{
	private static GL Gl;
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

		uniform vec3 u_Color = vec3(1.0);

        void main()
        {
            vec3 color1 = vec3(0.0, 0.0, 0.0); //red
            vec3 color2 = u_Color; //blue
            
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

	protected override void LoadGame()
	{
		Gl = GraphicsDevice.gl;

		Vao = new Forge.Graphics.Buffers.VertexArray(GraphicsDevice);
		Vao.Bind();

		Vbo = Forge.Graphics.Buffers.Buffer.Vertex.New(GraphicsDevice, Vertices);
		Vbo.Bind();

		Ebo = Forge.Graphics.Buffers.Buffer.Index.New(GraphicsDevice, Indices);
		Ebo.Bind();

		Shader = new Shader(
			GraphicsDevice,
			new Shader.ShaderPart(VertexShaderSource, ShaderType.VertexShader),
			new Shader.ShaderPart(FragmentShaderSource, ShaderType.FragmentShader))
		.Compile();

		Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)Unsafe.SizeOf<Vector3D<float>>(), null);
		Gl.EnableVertexAttribArray(0);
	}

	protected override void Render(double delta)
	{
		Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

		Vao.Bind();
		Shader.Bind();

		Gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
	}

	double globalTime;

	protected override void Update(double elapsedTime)
	{
		globalTime += elapsedTime;

		AddRenderTask(() => 
			Shader["u_Color"]?.SetValue(new Vector3D<float>((float)Math.Sin(globalTime), 0.0f, 0.0f)));
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
		Gl.Viewport(new System.Drawing.Size(obj.X, obj.Y));
	}
}
