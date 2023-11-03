using Forge.Renderer.Components;
using Forge.Renderer.Utils;
using Forge.Renderer.Vertices;
using Silk.NET.Maths;

namespace Forge.Renderer.VertexAssebmlers;

public class CharacterAssembler : IGeometryAssembler<GlyphVertex, CharacterRenderComponent>
{
	public int VerticesRequired => 4;

	public int IndicesRequired => 6;

	private static readonly Vector4D<float>[] QuadVertexPositions = new Vector4D<float>[4]
	{
		new Vector4D<float>(0f, 0f, 0.0f, 1.0f),
		new Vector4D<float>(1f, 0f, 0.0f, 1.0f),
		new Vector4D<float>(1f, -1f, 0.0f, 1.0f),
		new Vector4D<float>(0f, -1f, 0.0f, 1.0f),
	};

	public unsafe void Assemble(Span<GlyphVertex> vertices, ref CharacterRenderComponent glyph)
	{
		if (vertices.Length < VerticesRequired)
			throw new ArgumentException($"Span must have at least {VerticesRequired} elements", nameof(vertices));

		var model = glyph.Model;
		var poss = stackalloc Vector2D<float>[4];

		for (int i = 0; i < 4; i++)
		{
			var v4 = QuadVertexPositions[i] * model;
			poss[i] = new(v4.X, v4.Y);
		}

		vertices[0] = new GlyphVertex(poss[0], glyph.UV.Origin, glyph.Color);

		vertices[1] = new GlyphVertex(
			poss[1].X,
			poss[1].Y,
			glyph.UV.Origin.X + glyph.UV.Size.X,
			glyph.UV.Origin.Y,
			glyph.Color);

		vertices[2] = new GlyphVertex(
			poss[2],
			glyph.UV.Origin + glyph.UV.Size,
			glyph.Color);

		vertices[3] = new GlyphVertex(
			poss[3].X,
			poss[3].Y,
			glyph.UV.Origin.X,
			glyph.UV.Origin.Y + glyph.UV.Size.Y, glyph.Color);
	}

	public uint[] GetIndices(int count)
	{
		return IndicesGenerator.GenerateQuadIndices(count);
	}
}
