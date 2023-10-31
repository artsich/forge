using Forge.Graphics;
using Silk.NET.OpenGL;
using Shader = Forge.Graphics.Shaders.Shader;

namespace Forge.Shaders
{
	public class DefaultShader : Shader
	{
		public DefaultShader() :
			base(GraphicsDevice.Current,
				new ShaderPart(FragmentShaderSource, ShaderType.FragmentShader),
				new ShaderPart(VertexShaderSource, ShaderType.VertexShader)
			)
		{
		}

		private static readonly string VertexShaderSource = @"
#version 330 core
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec4 vColor;
layout (location = 2) in vec2 vTexCoord;
layout (location = 3) in float vFade;

uniform mat4 cameraViewProj;
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
	gl_Position = cameraViewProj * pos;
}
";

		private static readonly string FragmentShaderSource = @"
#version 330 core

out vec4 FragColor;

in vec2 fragTexCoord;
in float fragFade;
in vec4 fragColor;

uniform float timeTotal;

uniform sampler2D texture1;

vec3 lightColor = vec3(1.0, 0.0, 1.0);

void main()
{
	vec3 texColor = texture(texture1, fragTexCoord).xyz;
	vec3 c = mix(fragColor.rgb, lightColor, sin(timeTotal)) * texColor;
	FragColor = vec4(c, 1.0);
}
";
	}
}
