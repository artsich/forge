using Forge.Graphics;
using Forge.Graphics.Shaders;
using Silk.NET.OpenGL;

namespace Forge.Shaders;

public class QuadShader : ShaderSources
{
	private const string Vertex =
@"
#version 410 core
layout (location = 1) in vec2 vTexPos; 
layout (location = 2) in vec3 vPosition;
layout (location = 3) in vec4 vColor;

out vec4 Color;
out vec2 TexPos;

uniform mat4 cameraViewProj;

void main() {
	Color = vColor;
	TexPos = vTexPos;
	gl_Position = cameraViewProj * vec4(vPosition, 1.0);
}
";
	private const string Fragment =
@"
in vec4 Color;
in vec2 TexPos;
out vec4 FragColor;

void main() {
	vec2 uv = TexPos;
	FragColor = Color;
}
";

	public QuadShader() :
		base(GraphicsDevice.Current,
			new ShaderPart(Vertex, ShaderType.VertexShader),
			new ShaderPart(Fragment, ShaderType.FragmentShader))
	{
	}
}
