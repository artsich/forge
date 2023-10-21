using Silk.NET.OpenGL;

namespace Forge.Graphics;

public class Texture2d : GraphicsResourceBase
{
	private readonly uint _handle;

	public unsafe Texture2d(GraphicsDevice gd, Span<byte> data, uint width, uint height)
		: base(gd)
	{
		_handle = GL.GenTexture();
		var target = TextureTarget.Texture2D;
		GL.BindTexture(target, _handle);

		fixed (void* d = &data[0])
		{
			GL.TexImage2D(target, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
		}

		SetParameters();
	}

	private void SetParameters()
	{
		GL.GenerateMipmap(TextureTarget.Texture2D);

		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
		GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.NearestMipmapNearest);

		//GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
		//GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
	}

	public void Bind(int slot = 0)
	{
		GL.BindTextureUnit((uint)slot, _handle);
	}

	protected override void OnDestroy()
	{
		GL.DeleteTexture(_handle);
	}
}