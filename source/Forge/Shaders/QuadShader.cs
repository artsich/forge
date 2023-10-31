using Forge.Graphics;
using Forge.Graphics.Shaders;
using Silk.NET.OpenGL;

namespace Forge.Shaders;

internal class QuadShader : ShaderSources
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
	float edge = 0.12;
	float width = 0.1;

	vec2 smoothedStart = smoothstep(vec2(edge - width), vec2(edge), uv);
	vec2 smoothedEnd = smoothstep(vec2(1.0 - edge + width), vec2(1.0 - edge), uv);

	float alpha = smoothedStart.x * smoothedEnd.x * smoothedStart.y * smoothedEnd.y;
	FragColor = vec4(Color.xyz * alpha, alpha);
}
";

	public QuadShader() :
		base(GraphicsDevice.Current,
			new ShaderPart(Vertex, ShaderType.VertexShader),
			new ShaderPart(Fragment, ShaderType.FragmentShader))
	{
	}
}
