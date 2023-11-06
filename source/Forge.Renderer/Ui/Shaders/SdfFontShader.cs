using Forge.Graphics;
using Forge.Graphics.Shaders;
using Silk.NET.OpenGL;

namespace Forge.Shaders;

public class SdfFontShader : ShaderSources
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

float effect = 1;

float resolution = 1080.0; // todo: take this value from uniform

float smoothing = 0.1;

float oversample() {
    vec2 pixelSize = vec2(1.0 / resolution);
    vec2 offsets[9] = vec2[](
        vec2(-pixelSize.x, pixelSize.y), // top-left
        vec2(0.0, pixelSize.y), // top-center
        vec2(pixelSize.x, pixelSize.y), // top-right
        vec2(-pixelSize.x, 0.0), // center-left
        vec2(0.0, 0.0), // center-center
        vec2(pixelSize.x, 0.0), // center-right
        vec2(-pixelSize.x, -pixelSize.y), // bottom-left
        vec2(0.0, -pixelSize.y), // bottom-center
        vec2(pixelSize.x, -pixelSize.y)  // bottom-right
    );

    float alpha = 0.0;
    for(int i = 0; i < 9; i++)
    {
        float sample = texture(text, TexCoords + offsets[i]).r;
        alpha += smoothstep(0.5 - smoothing, 0.5 + smoothing, sample);
    }

    alpha /= 9.0;
    return alpha;
}

void main()
{
    if (effect == 1) {
        float alpha = oversample();
        o_color = vec4(Color.rgb, alpha * Color.a);
    }
    else if (effect == 2) {
        float distance = texture(text, TexCoords).r;
        float aaf = smoothing;
        //aaf = fwidth(distance);

        float alpha = smoothstep(0.5 - aaf, 0.5 + aaf, distance);
        o_color = vec4(Color.xyz, alpha);    
    }
    else { // debug effect
        o_color = vec4(Color.rgb, 1.);
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
