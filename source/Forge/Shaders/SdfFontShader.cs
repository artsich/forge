using Forge.Graphics;
using Forge.Graphics.Shaders;
using Silk.NET.OpenGL;

namespace Forge.Shaders;

internal class SdfFontShader : ShaderSources
{
	private const string Vertex = @"
#version 330 core

layout(location = 0) in vec2 v_pos;
layout(location = 1) in vec2 v_uv;
layout(location = 2) in vec4 v_color;

out vec2 TexCoords;
out vec4 Color;

uniform mat4 cameraViewProj;

void main()
{
	TexCoords = v_uv;
	Color = v_color;
	gl_Position = cameraViewProj * vec4(v_pos, 0.0, 1.0);
}

";
	private const string Fragment = @"
#version 330 core

in vec2 TexCoords;
in vec4 Color;

out vec4 o_color;

uniform sampler2D text;
uniform vec3 textColor = vec3(1.0);

const float smoothing = 0.15;

const int effect = 0;

void main()
{
	if (effect == 0) {
		float distanceRange = 2;
		float distance = texture(text, TexCoords).r;
		float alpha = smoothstep(0.5 - smoothing, 0.5 + smoothing, distance);

		o_color = vec4(Color.xyz, alpha);
	} 
	else if (effect == 1) {
		o_color = vec4(Color.xyz, 1.);
	}
}
";

	public SdfFontShader() :
		base(GraphicsDevice.Current,
			new ShaderPart(Vertex, ShaderType.VertexShader),
			new ShaderPart(Fragment, ShaderType.FragmentShader))
	{
	}
}
