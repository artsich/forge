using Silk.NET.OpenGL;

namespace Forge.Graphics.Buffers;

public sealed class VertexArray : GraphicsResourceBase
{
	private uint id;

	/// <summary>
	/// Instantiates a new vertex array object.
	/// </summary>
	public VertexArray(GraphicsDevice gd) : base(gd)
	{
		id = GL.GenVertexArray();
	}

	public void Bind()
	{
		GL.BindVertexArray(id);
	}

	public void Unbind()
	{
		GL.BindVertexArray(0);
	}

	protected override void OnDestroy() 
	{
		if (id != 0)
		{
			GL.DeleteVertexArray(id);
			id = 0;
		}
	}
}
