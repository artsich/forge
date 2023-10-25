namespace Forge.Renderer.Font;

public class FontMeta
{
	private readonly IReadOnlyDictionary<string, float> kerningPairs;

	public IReadOnlyDictionary<char, Glyph> Chars { get; init; }

	public FontMeta(IReadOnlyDictionary<char, Glyph> chars,
		IReadOnlyDictionary<string, float> kerningPairs)
	{
		Chars = chars;
		this.kerningPairs = kerningPairs;
	}

	/// <summary>
	/// Kerning pairs for the font, which is the adjustment of space between specific character pairs to improve visual aesthetics.
	/// </summary>
	public float GetKerning(char a, char b)
	{
		return kerningPairs.GetValueOrDefault($"{a}{b}");
	}
}
