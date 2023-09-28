using Silk.NET.OpenGL;

namespace Forge.Graphics.Shaders;

public class CompiledShader : GraphicsResourceBase
{
	internal uint ProgramId { get; private set; }

	private Uniform[] uniformByLocation = Array.Empty<Uniform>();
	private readonly Dictionary<string, Uniform> uniformByName = new();

	internal CompiledShader(uint programId, GraphicsDevice gd) : base(gd)
	{
		ProgramId = programId;
		ReadUniforms();
	}

	public Uniform? this[string name] => uniformByName.TryGetValue(name, out var uniform) ? uniform : null;

	public bool UniformExists(string name) => uniformByName.ContainsKey(name);

	public void Bind()
	{
		GL.UseProgram(ProgramId);
	}

	public void BindUniform(object source)
	{
		AutoUniform.Bind(source, this);
	}

	public void BindUniforms(params object[] sources)
	{
		foreach(var source in sources)
		{
			BindUniform(source);
		}	
	}

	protected override void OnDestroy()
	{
		GL.DeleteProgram(ProgramId);
		ProgramId = 0;
	}

	private void ReadUniforms()
	{
		GL.GetProgram(ProgramId, GLEnum.ActiveUniforms, out int count);

		uniformByLocation = new Uniform[count];

		for (int i = 0; i < uniformByLocation.Length; i++)
		{
			var name = GL.GetActiveUniform(ProgramId, (uint)i, out int size, out UniformType type);

			var uniform = new Uniform(this, i, name, type, size);
			uniformByLocation[i] = uniform;
			uniformByName.Add(name, uniform);
		}
	}
}
