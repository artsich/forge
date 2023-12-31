﻿using Silk.NET.OpenGL;

namespace Forge.Graphics.Shaders;

public class ShaderSources
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

		internal void Delete(GL gl)
		{
			gl.DeleteShader(Id);
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

	public ShaderSources(GraphicsDevice gd, params ShaderPart[] shaderParts)
	{
		this.gd = gd;
		this.shaderParts = shaderParts;
	}

	public CompiledShader? Compile()
	{
		uint programId = gd.gl.CreateProgram();

		foreach (var shaderPart in shaderParts)
		{
			shaderPart.Compile(gd.gl);
			gd.gl.AttachShader(programId, shaderPart.Id);
		}

		gd.gl.LinkProgram(programId);

		foreach (var shaderPart in shaderParts)
		{
			shaderPart.Delete(gd.gl);
		}

		if (!ValidateProgram(programId))
		{
			return null;
		}

		return new CompiledShader(programId, gd);
	}

	private bool ValidateProgram(uint programId)
	{
		gd.gl.GetProgram(programId, GLEnum.LinkStatus, out int result);
		string infoLog = gd.gl.GetProgramInfoLog(programId);

		if (result == 0)
		{
			Console.WriteLine("Shader program link failed");
			if (!string.IsNullOrWhiteSpace(infoLog))
				Console.WriteLine(infoLog);

			gd.gl.DeleteProgram(programId);

			return false;
		}
		if (!string.IsNullOrWhiteSpace(infoLog))
		{
			Console.WriteLine($"Shader program link result: {infoLog}");
		}

		return true;
	}
}
