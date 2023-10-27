using Forge.Graphics;

namespace Forge.Renderer.Font;

public class SpriteFont : GraphicsResourceBase
{
	private readonly IReadOnlyDictionary<KerningKey, float> kerningPairs;

	public Texture2d Atlas { get; }

	public IReadOnlyDictionary<char, Glyph> Glyphs { get; init; }

	public float Size { get; init; }

	public SpriteFont(
		float size,
		Texture2d atlas,
		IReadOnlyDictionary<char, Glyph> chars,
		IReadOnlyDictionary<KerningKey, float> kerningPairs)
		: base(GraphicsDevice.Current)
	{
		Size = size;
		Atlas = atlas;
		Glyphs = chars;
		this.kerningPairs = kerningPairs;
	}

	/// <summary>
	/// Kerning pairs for the font, which is the adjustment of space between specific character pairs to improve visual aesthetics.
	/// </summary>
	public float GetKerning(char a, char b)
	{
		return kerningPairs.GetValueOrDefault(new KerningKey(a, b));
	}

	protected override void OnDestroy()
	{
		Atlas?.Dispose();
	}
}
