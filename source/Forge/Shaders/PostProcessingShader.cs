using Forge.Graphics;
using Silk.NET.OpenGL;
using Shader = Forge.Graphics.Shaders.Shader;

namespace Forge.Shaders;

public class PostProcessingShader : Shader
{
	public PostProcessingShader()
		: base(GraphicsDevice.Current,
			new ShaderPart(PostProcesssingFragmentShader, ShaderType.FragmentShader),
			new ShaderPart(PostProcesssingVertexShader, ShaderType.VertexShader))
	{
	}

	private static readonly string PostProcesssingVertexShader = @"
#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTexCoords;

out vec2 TexCoords;

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, 0.0, 1.0); 
    TexCoords = aTexCoords;
}
";

	private static readonly string PostProcesssingFragmentShader = @"
#version 330 core
out vec4 FragColor;
  
in vec2 TexCoords;

uniform sampler2D screenTexture;

void main()
{ 
    FragColor = texture(screenTexture, TexCoords);
}
";
}
