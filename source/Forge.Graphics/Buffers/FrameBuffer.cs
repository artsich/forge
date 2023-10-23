using Silk.NET.OpenGL;

namespace Forge.Graphics.Buffers;

public sealed class FrameBuffer : GraphicsResourceBase
{
	private uint id;

	public uint Texture0 { get; private set; }

	public FrameBuffer(GraphicsDevice gd) : base(gd)
	{
		id = GL.GenFramebuffer();
	}

	public unsafe void Configure(uint w, uint h)
	{
		Texture0 = GL.GenTexture();
		GL.BindTexture(GLEnum.Texture2D, Texture0);
		GL.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgb, w, h, 0, GLEnum.Rgb, GLEnum.UnsignedByte, null);
		GL.TexParameterI(TextureTarget.Texture2D, GLEnum.TextureMinFilter, (int)TextureMagFilter.Nearest);
		GL.TexParameterI(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);

		Bind();
		GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, GLEnum.Texture2D, Texture0, 0);
		Unbind();

		if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
		{
			throw new Exception("Framebuffer is not complete!");
		}
	}

	public void Bind()
	{
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, id);
	}

	public void Unbind()
	{
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}

	protected override void OnDestroy()
	{
		GL.DeleteFramebuffer(id);
	}
}
