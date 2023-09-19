using Silk.NET.OpenGL;

namespace Forge.Graphics;

public abstract class GraphicsResourceBase : IDisposable
{
	protected readonly GraphicsDevice Gd;
	protected internal GL GL;

	protected GraphicsResourceBase(GraphicsDevice gd)
	{
		GL = gd.gl;
		Gd = gd;
	}

	public bool IsDisposed { get; private set; }

	public void Dispose()
	{
		if (!IsDisposed)
		{
			OnDestroy();
			GC.SuppressFinalize(this);
		}
	}

	protected virtual void OnDestroy() { }
}