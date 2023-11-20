using Forge.Graphics;
using Forge.Graphics.Shaders;
using Silk.NET.OpenGL;

namespace Forge.Renderer.Shaders;

public class SpriteShader : ShaderSources
{
	private const string Vertex =
@"
#version 410 core
layout (location = 0) in int vTexId;
layout (location = 1) in vec2 vTexPos;
layout (location = 2) in vec3 vPosition;
layout (location = 3) in vec4 vColor;

out vec4 Color;
out vec2 TexPos;
flat out int TexId;

uniform mat4 cameraViewProj;

void main() {
    Color = vColor;
    TexPos = vTexPos;
    TexId = vTexId;
	gl_Position = cameraViewProj * vec4(vPosition, 1.0);
}
";
	private const string Fragment =
@"
#version 410 core

out vec4 FragColor;

in vec4 Color;
in vec2 TexPos;
flat in int TexId;

uniform sampler2D uTextures[32];

void main() {
	vec2 uv = TexPos;
	vec4 texColor = texture(uTextures[TexId], TexPos);
	FragColor = Color * texColor;
}
";

	public SpriteShader() :
		base(GraphicsDevice.Current,
			new ShaderPart(Vertex, ShaderType.VertexShader),
			new ShaderPart(Fragment, ShaderType.FragmentShader))
	{
	}
}
