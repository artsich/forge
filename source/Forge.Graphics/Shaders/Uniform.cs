using Silk.NET.OpenGL;

namespace Forge.Graphics.Shaders;

public partial class Uniform
{
	private readonly CompiledShader cs;
	private readonly GL gl;

	public int Location { get; }

	public string Name { get; }

	public UniformType Type { get; }

	public int Size { get; }

	public Uniform(CompiledShader cs, int location, string name, UniformType type, int size)
    {
		this.cs = cs;

		gl = cs.GL;
		Location = location;
		Name = name;
		Type = type;
		Size = size;
	}

	private void ValidateType(UniformType type)
	{
		if (Type != type)
			throw new InvalidOperationException("The value you passed does not fit the uniforms type.");
	}
}
