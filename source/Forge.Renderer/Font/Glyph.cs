using Silk.NET.Maths;

namespace Forge.Renderer.Font;

public class Glyph
{
	public char Character { get; init; }

	public Vector2D<float> Size { get; init; }

	public Rectangle<float> UV { get; init; }

	public Vector2D<float> LayoutOffset { get; init; }

	public float XAdvance { get; init; }
}
