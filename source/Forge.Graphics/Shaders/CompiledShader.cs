﻿using Silk.NET.OpenGL;

namespace Forge.Graphics.Shaders;

public class Shader
{
	private readonly GraphicsDevice gd;
	private readonly ShaderPart[] shaderParts;

	public class ShaderPart
	{
		public string Source = string.Empty;
		public ShaderType Type;

		internal uint Id;

        public ShaderPart(string source, ShaderType type)
        {
			Source = source;
			Type = type;
		}

		internal void Compile(GL gl)
		{
			Id = gl.CreateShader(Type);
			gl.ShaderSource(Id, Source);
			gl.CompileShader(Id);

			var infoLog = gl.GetShaderInfoLog(Id);
			gl.GetShader(Id, GLEnum.CompileStatus, out int status);

			if (status == 0)
			{
				Console.Write("Shader compilation fail: ");

				if (!string.IsNullOrWhiteSpace(infoLog))
				{
					Console.WriteLine(infoLog);
				}
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(infoLog))
				{
					Console.WriteLine(Type);
					Console.WriteLine(infoLog);
				}
			}
		}
	}

    public Shader(GraphicsDevice gd, params ShaderPart[] shaderParts)
    {
		this.gd = gd;
		this.shaderParts = shaderParts;
	}

	public CompiledShader Compile()
	{
		uint programId = gd.gl.CreateProgram();

		foreach(var shaderPart in shaderParts)
		{
			shaderPart.Compile(gd.gl);
			gd.gl.AttachShader(programId, shaderPart.Id);
		}
		gd.gl.LinkProgram(programId);

		gd.gl.GetProgram(programId, GLEnum.LinkStatus, out int result);
		string infoLog = gd.gl.GetProgramInfoLog(programId);

		if (result == 0)
		{
			Console.WriteLine("Shader program link failed");
			if (!string.IsNullOrWhiteSpace(infoLog))
				Console.WriteLine(infoLog);
			gd.gl.DeleteProgram(programId);

			return null;
		}
		else
		{
			if (!string.IsNullOrWhiteSpace(infoLog))
			{
				Console.WriteLine($"Shader program link result: {infoLog}");
			}
		}

		return new CompiledShader(programId, gd);
	}
}

public class CompiledShader : GraphicsResourceBase
{
	internal uint ProgramId { get; private set; }

	private Uniform[] uniformByLocation = Array.Empty<Uniform>();
	private Dictionary<string, Uniform> uniformByName;

	internal CompiledShader(uint programId, GraphicsDevice gd) : base(gd)
	{
		ProgramId = programId;

		// todo: Should perform in Opengl thread
		ReadUniforms();
	}

	public Uniform? this[string name] => uniformByName.TryGetValue(name, out var uniform) ? uniform : null;

	public void Bind()
	{
		// should i verify that it is opengl thread?
		GL.UseProgram(ProgramId);
		// todo: create auto set uniform mechanism
	}

	private void ReadUniforms()
	{
		GL.GetProgram(ProgramId, GLEnum.ActiveUniforms, out int count);

		uniformByLocation = new Uniform[count];
		uniformByName = new Dictionary<string, Uniform>();

		for (int i = 0; i < uniformByLocation.Length; i++)
		{
			var name = GL.GetActiveUniform(ProgramId, (uint)i, out int size, out UniformType type);

			var uniform = new Uniform(this, i, name, type, size);
			uniformByLocation[i] = uniform;
			uniformByName.Add(name, uniform);
		}
	}

	protected override void OnDestroy()
	{
		GL.DeleteProgram(ProgramId);
		ProgramId = 0;
	}
}
