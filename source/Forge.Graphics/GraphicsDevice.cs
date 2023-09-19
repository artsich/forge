using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Forge.Graphics;

public class GraphicsDevice
{
	private long bufferMemory;
	private long textureMemory;

	public readonly GL gl;

	public static GraphicsDevice Current { get; private set; }

	private GraphicsDevice(GL gl)
	{
		this.gl = gl;
	}

	public static GraphicsDevice InitOpengl(IWindow window)
	{
		var gl = GL.GetApi(window);
		Current = new GraphicsDevice(gl);
		return Current;
	}

	public long BuffersMemory => Interlocked.Read(ref bufferMemory);

	public long TextureMemory => Interlocked.Read(ref textureMemory);

	internal void RegisterTextureMemoryUsage(long memoryChange)
	{
		Interlocked.Add(ref textureMemory, memoryChange);
	}

	internal void RegisterBufferMemoryUsage(long memoryChange)
	{
		Interlocked.Add(ref bufferMemory, memoryChange);
	}
}
