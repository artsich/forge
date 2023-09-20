using Forge.Graphics;
using Forge.Graphics.Shaders;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using static Forge.Graphics.Shaders.Shader;

var options = WindowOptions.Default;

var window = Window.Create(options with { Size = new Vector2D<int>(800, 600), Title = "Forge" });

CompiledShader compiledProgram = null;
Forge.Graphics.Buffers.Buffer? vbo = null;
var gd = GraphicsDevice.InitOpengl(window);

window.Load += () =>
{

	var vertexShader = new ShaderPart(@"
#version 330 core //Using version GLSL version 3.3
layout (location = 0) in vec4 vPos;
        
void main()
{
    gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
}
", ShaderType.VertexShader);

	var fragmentShader = new ShaderPart(@"
#version 330 core
out vec4 FragColor;

void main()
{
    FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
}
", ShaderType.FragmentShader);

	compiledProgram = new Forge.Graphics.Shaders.Shader(gd, vertexShader, fragmentShader).Compile();
	InitBuffer();
};


unsafe void InitBuffer()
{
	vbo = Forge.Graphics.Buffers.Buffer.Vertex.New(gd, new DataPointer(IntPtr.Zero, 128), 8);
	vbo.Bind();

	//Tell opengl how to give the data to the shaders.
	gd.gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
	gd.gl.EnableVertexAttribArray(0);

}

window.Render += (dt) =>
{
	compiledProgram.Bind();

	vbo?.Bind();
};


window.Run();

vbo?.Dispose();

