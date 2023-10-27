using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Runtime.CompilerServices;

namespace Forge.Renderer.Vertices;

public readonly struct GlyphVertex
{
	public readonly Vector2D<float> Position;
	public readonly Vector2D<float> UV;
	public readonly Vector4D<float> Color;

	public GlyphVertex(Vector2D<float> position, Vector2D<float> uv, Vector4D<float> color)
	{
		UV = uv;
		Color = color;
		Position = position;
	}

	public GlyphVertex(float x, float y, float uvX, float uvY, Vector4D<float> color)
	{
		Position = new Vector2D<float>(x, y);
		UV = new Vector2D<float>(uvX, uvY);
		Color = color;
	}

	// todo: Layout shoule be moved to extensions or smth else?

	private static readonly int[] componentsCount = new[]
	{
		2,
		2,
		4
	};

	private static readonly int[] offsets = new int[]
	{
		Unsafe.SizeOf<Vector2D<float>>(),
		Unsafe.SizeOf<Vector2D<float>>(),
		Unsafe.SizeOf<Vector4D<float>>(),
	};

	public unsafe static void Enable(GL gl)
	{
		uint size = (uint)Unsafe.SizeOf<GlyphVertex>();
		int offset = 0;
		for (int i = 0; i < offsets.Length; i++)
		{
			gl.EnableVertexAttribArray((uint)i);
			gl.VertexAttribPointer((uint)i, componentsCount[i], VertexAttribPointerType.Float, false, size, (void*)offset);

			offset += offsets[i];
		}
	}
}
