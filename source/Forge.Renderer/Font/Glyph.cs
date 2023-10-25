using Silk.NET.Maths;

namespace Forge.Renderer.Font;

public class Glyph
{
	public required Vector2D<float> UVPosition { get; init; }
	public required Vector2D<float> UVSize { get; init; }
	public required Vector2D<float> Size { get; init; }
	public required Vector2D<float> Bearing { get; init; }

	/// <summary>
	/// The horizontal advance after drawing the glyph.
	/// </summary>
	public required float Advance { get; init; }
}
