using Silk.NET.OpenGL;
using System.Drawing;

namespace Forge.Graphics.Buffers;

public sealed class FrameBuffer : GraphicsResourceBase
{
	private readonly uint id;
	private readonly ClearBufferMask clearBufferMask;
	private readonly bool useDepthStencil;

	private Size size;

	public IReadOnlyCollection<Texture2d> Colors { get; }

	public FrameBuffer(Size size)
		: base(GraphicsDevice.Current)
	{
		this.size = size;
		Colors = Array.Empty<Texture2d>();

		clearBufferMask = ClearBufferMask.ColorBufferBit;
		clearBufferMask |= ClearBufferMask.DepthBufferBit;
		clearBufferMask |= ClearBufferMask.StencilBufferBit;
	}

	public FrameBuffer(Size size, IReadOnlyCollection<Texture2d>? colors = null, bool useDepthStencil = false)
		: base(GraphicsDevice.Current)
	{
		if (colors != null
			&& colors.Any()
			&& !colors.All(color => color.Size == colors.First().Size))
		{
			throw new InvalidOperationException("All color textures must have the same size!");
		}

		id = GL.GenFramebuffer();
		this.size = size;
		this.useDepthStencil = useDepthStencil;

		Colors = colors ?? Array.Empty<Texture2d>();
		clearBufferMask = Colors.Count > 0 ? ClearBufferMask.ColorBufferBit : ClearBufferMask.None;
		clearBufferMask |= useDepthStencil ? (ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit) : ClearBufferMask.None;

		Configure();
	}

	public void Resize(Size size)
	{
		if (size == this.size)
		{
			return;
		}

		if (id == 0)
		{
			this.size = size;
		}
		else
		{
			// todo: not sure how to resize a framebuffer for not default FB
			throw new NotImplementedException();
		}
	}
	
	public void Bind()
	{
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, id);
		GL.Viewport(size);
	}

	public void Unbind()
	{
		GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}

	public void Clear()
	{
		GL.Clear(clearBufferMask);
	}

	protected override void OnDestroy()
	{
		GL.DeleteFramebuffer(id);
	}

	private unsafe void Configure()
	{
		Bind();

		for (var i = 0; i < Colors.Count; i++)
		{
			var color = Colors.ElementAt(i);
			color.Bind();
			GL.FramebufferTexture2D(
				FramebufferTarget.Framebuffer,
				FramebufferAttachment.ColorAttachment0 + i,
				GLEnum.Texture2D,
				color.Handle,
				0);
		}

		if (useDepthStencil)
		{
			uint rbo = GL.GenRenderbuffer();
			GL.BindRenderbuffer(GLEnum.Renderbuffer, rbo);
			GL.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.Depth24Stencil8, (uint)size.Width, (uint)size.Height);
			GL.BindRenderbuffer(GLEnum.Renderbuffer, 0);
			GL.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.Depth24Stencil8, GLEnum.Renderbuffer, rbo);
		}

		if (Colors.Count > 0)
		{
			var i = 0;
			GL.DrawBuffers(Colors.Select(_ => (GLEnum.ColorAttachment0 + i++)).ToArray());
		}
		else
		{
			GL.DrawBuffer(DrawBufferMode.None);
		}

		Unbind();

		if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
		{
			throw new Exception("Framebuffer is not complete!");
		}
	}
}
