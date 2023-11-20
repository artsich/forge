using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Runtime.CompilerServices;

namespace Forge.Renderer.Vertices;

public struct QuadVertex
{
	public uint TextureId;
	public Vector2D<float> TexCoord;
	public Vector3D<float> Position;
	public Vector4D<float> Color;

	public QuadVertex(Vector3D<float> position, Vector4D<float> color, Vector2D<float> texCoord)
	{
		Color = color;
		Position = position;
		TexCoord = texCoord;
	}

	private static readonly int[] componentsCount = new int[4]
	{
		1,
		2,
		3,
		4
	};

	private static readonly int[] offsets = new int[4]
	{
		sizeof(float),
		Unsafe.SizeOf<Vector2D<float>>(),
		Unsafe.SizeOf<Vector3D<float>>(),
		Unsafe.SizeOf<Vector4D<float>>(),
	};

	public static unsafe void EnableVertexPointer(GL gl)
	{
		uint size = (uint)Unsafe.SizeOf<QuadVertex>();
		int offset = 0;
		for (int i = 0; i < offsets.Length; i++)
		{
			gl.EnableVertexAttribArray((uint)i);
			gl.VertexAttribPointer((uint)i, componentsCount[i], VertexAttribPointerType.Float, false, size, (void*)offset);

			offset += offsets[i];
		}
	}
}
