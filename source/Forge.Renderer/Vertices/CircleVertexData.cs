using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Runtime.CompilerServices;

namespace Forge.Renderer.Vertices;

public readonly struct CircleVertex
{
	public readonly Vector3D<float> Position;
	public readonly Vector4D<float> Color;
	public readonly Vector2D<float> TexCoord;
	public readonly float Fade;

	private static readonly int[] componentsCount = new int[4]
	{
		3,
		4,
		2,
		1
	};

	private static readonly int[] offsets = new int[4]
	{
		Unsafe.SizeOf<Vector3D<float>>(),
		Unsafe.SizeOf<Vector4D<float>>(),
		Unsafe.SizeOf<Vector2D<float>>(),
		sizeof(float)
	};

	public CircleVertex(Vector3D<float> position, Vector2D<float> texCoord, Vector4D<float> color, float fade)
	{
		Position = position;
		TexCoord = texCoord;
		Color = color;
		Fade = fade;
	}

	public static unsafe void EnableVertexPointer(GL gl)
	{
		uint size = (uint)Unsafe.SizeOf<CircleVertex>();
		int offset = 0;
		for (int i = 0; i < offsets.Length; i++)
		{
			gl.EnableVertexAttribArray((uint)i);
			gl.VertexAttribPointer((uint)i, componentsCount[i], VertexAttribPointerType.Float, false, size, (void*)offset);

			offset += offsets[i];
		}
	}
}
