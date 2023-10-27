using Forge.Renderer.Components;
using Forge.Renderer.Utils;
using Forge.Renderer.Vertices;

namespace Forge.Renderer.VertexAssebmlers;

public class TextAssembler : IGeometryBufferAssembler<GlyphVertex, CharacterRenderComponent>
{
	public int VerticesRequired => 4;

	public int IndicesRequired => 6;

	public void Assemble(Span<GlyphVertex> vertices, ref CharacterRenderComponent glyph)
	{
		if (vertices.Length < VerticesRequired)
			throw new ArgumentException($"Span must have at least {VerticesRequired} elements", nameof(vertices));

		var pos = glyph.Position;
		var size = glyph.Size;

		vertices[0] = new GlyphVertex(pos, glyph.UV.Origin, glyph.Color);

		vertices[1] = new GlyphVertex(
			pos.X + size.X,
			pos.Y,
			glyph.UV.Origin.X + glyph.UV.Size.X,
			glyph.UV.Origin.Y,
			glyph.Color);

		vertices[2] = new GlyphVertex(
			pos + size,
			glyph.UV.Origin + glyph.UV.Size,
			glyph.Color);

		vertices[3] = new GlyphVertex(
			pos.X,
			pos.Y + size.Y,
			glyph.UV.Origin.X,
			glyph.UV.Origin.Y + glyph.UV.Size.Y, glyph.Color);
	}

	public uint[] GetIndices(int count)
	{
		return IndicesGenerator.GenerateQuadIndices(count);
	}
}
