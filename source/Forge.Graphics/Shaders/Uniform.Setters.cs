using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Forge.Graphics.Shaders;

public partial class Uniform
{
	public void SetValue(int[] values)
	{
		ValidateType(UniformType.IntSampler1DArray);
		gl.ProgramUniform1(cs.ProgramId, Location, values);
	}

	public void SetValue(Span<int> values)
	{
		//ValidateType(UniformType.UnsignedIntSampler1DArray);
		gl.ProgramUniform1(cs.ProgramId, Location, values);
	}

	public void SetValue(float value)
	{
		ValidateType(UniformType.Float);
		gl.ProgramUniform1(cs.ProgramId, Location, value);
	}

	public void SetValue(Vector3D<float> value)
	{
		ValidateType(UniformType.FloatVec3);
		gl.ProgramUniform3(cs.ProgramId, Location, value.X, value.Y, value.Z);
	}

	public void SetValue(Vector4D<float> value)
	{
		ValidateType(UniformType.FloatVec4);
		gl.ProgramUniform4(cs.ProgramId, Location, value.X, value.Y, value.Z, value.W);
	}

	public unsafe void SetValue(Matrix4X4<float> value)
	{
		ValidateType(UniformType.FloatMat4);
		gl.ProgramUniformMatrix4(cs.ProgramId, Location, 1, false, (float*)&value);
	}
}
