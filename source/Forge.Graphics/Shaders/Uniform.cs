using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Forge.Graphics.Shaders;

public class Uniform
{
	private readonly CompiledShader cs;
	private readonly GL gl;

	public int Location { get; init; }

	public string Name { get; init; }

	public UniformType Type { get; init; }

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

	public void SetValue(Vector3D<float> value)
	{
		ValidateType(UniformType.FloatVec3);
		gl.ProgramUniform3(cs.ProgramId, this.Location, value.X, value.Y, value.Z);
	}

	public void SetValue(Vector4D<float> value)
	{
		ValidateType(UniformType.FloatVec4);
		gl.ProgramUniform4(cs.ProgramId, this.Location, value.X, value.Y, value.Z, value.W);
	}

	private void ValidateType(UniformType type)
	{
		if (Type != type)
			throw new InvalidOperationException("The value you passed does not fit the uniforms type.");
	}
}
