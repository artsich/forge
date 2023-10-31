using Forge.Renderer.Components;
using Forge.Renderer.Utils;
using Forge.Renderer.Vertices;
using Silk.NET.Maths;

namespace Forge.Renderer.VertexAssebmlers;

public class QuadAssembler : IGeometryAssembler<QuadVertex, QuadRenderComponent>
{
	public int VerticesRequired => 4;

	public int IndicesRequired => 6;


	private static readonly Vector4D<float>[] QuadVertexPositions = new Vector4D<float>[4]
	{
		new Vector4D<float>(-0.5f, 0.5f, 0.0f, 1.0f),
		new Vector4D<float>(0.5f, 0.5f, 0.0f, 1.0f),
		new Vector4D<float>(0.5f, -0.5f, 0.0f, 1.0f),
		new Vector4D<float>(-0.5f, -0.5f, 0.0f, 1.0f),
	};

	public static readonly Vector2D<float>[] TextureCoords = new Vector2D<float>[4]
	{
		new (0.0f, 0.0f),
		new (1.0f, 0.0f),
		new (1.0f, 1.0f),
		new (0.0f, 1.0f),
	};

	public uint[] GetIndices(int count)
	{
		return IndicesGenerator.GenerateQuadIndices(count);
	}

	public void Assemble(Span<QuadVertex> vertices, ref QuadRenderComponent quad)
	{
		if (vertices.Length < VerticesRequired)
			throw new ArgumentException($"Span must have at least {VerticesRequired} elements", nameof(vertices));

		var model = quad.Model;

		for (int i = 0; i < QuadVertexPositions.Length; i++)
		{
			var p = QuadVertexPositions[i] * model;

			vertices[i] = new QuadVertex
			{
				Position = new Vector3D<float>(p.X, p.Y, p.Z),
				TexCoord = TextureCoords[i],
				Color = quad.Color
			};
		}
	}
}
