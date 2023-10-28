using Silk.NET.OpenGL;
using System.Drawing;

namespace Forge.Graphics;

public enum TextureWrap
{
	Repeat,
	MirroredRepeat,
	ClampToEdge,
	ClampToBorder
}

public enum MinMagFilter
{
	Nearest,
	Linear
}

public sealed class Texture2d : GraphicsResourceBase
{
	private readonly bool mipmap;

	internal uint Handle { get; private set; }

	public Size Size { get; }

	public unsafe Texture2d(uint width, uint height, Span<byte> data = default, bool mipmap = true)
		: base(GraphicsDevice.Current)
	{
		Size = new Size((int)width, (int)height);
		Handle = GL.GenTexture();
		GL.BindTexture(GLEnum.Texture2D, Handle);

		if (!data.IsEmpty)
		{
			fixed (void* d = &data[0])
			{
				GL.TexImage2D(GLEnum.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
			}
		}
		else
		{
			GL.TexImage2D(GLEnum.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);
		}

		if (mipmap)
		{
			GL.GenerateMipmap(TextureTarget.Texture2D);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
		}

		Wrap = TextureWrap.Repeat;
		Filter = MinMagFilter.Nearest;

		GL.BindTexture(GLEnum.Texture2D, 0);
	}

	public TextureWrap Wrap
	{
		set
		{
			Bind();

			var param = value switch
			{
				TextureWrap.Repeat => (int)GLEnum.Repeat,
				TextureWrap.MirroredRepeat => (int)GLEnum.MirroredRepeat,
				TextureWrap.ClampToEdge => (int)GLEnum.ClampToEdge,
				TextureWrap.ClampToBorder => (int)GLEnum.ClampToBorder,
				_ => throw new NotImplementedException(),
			};

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, param);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, param);
		}
	}

	public MinMagFilter Filter
	{
		set
		{
			Bind();

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
				value switch
				{
					MinMagFilter.Nearest => (int)GLEnum.Nearest,
					MinMagFilter.Linear => (int)GLEnum.Linear,
					_ => throw new NotImplementedException()
				});

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
				value switch
				{
					MinMagFilter.Nearest => mipmap ? (int)GLEnum.NearestMipmapNearest : (int)GLEnum.Nearest,
					MinMagFilter.Linear => mipmap ? (int)GLEnum.LinearMipmapLinear : (int)GLEnum.Linear,
					_ => throw new NotImplementedException()
				});
		}
	}

	public void Bind(int slot = 0)
	{
		GL.ActiveTexture(TextureUnit.Texture0 + slot);
		GL.BindTexture(TextureTarget.Texture2D, Handle);
	}

	protected override void OnDestroy()
	{
		GL.DeleteTexture(Handle);
	}
}