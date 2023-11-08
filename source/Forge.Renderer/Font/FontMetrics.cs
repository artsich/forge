using Silk.NET.Maths;

namespace Forge.Renderer.Font;

public sealed class FontMetrics
{
	private readonly IReadOnlyDictionary<KerningKey, float> kerningPairs;

	private readonly IReadOnlyDictionary<char, Glyph> glyphs;

	public float Size { get; }

	private float MaxX { get; }

	private float MaxY { get; }

	public FontMetrics(float size, IReadOnlyDictionary<KerningKey, float> kerningPairs, IReadOnlyDictionary<char, Glyph> glyphs)
	{
		Size = size;
		this.kerningPairs = kerningPairs;
		this.glyphs = glyphs;
		MaxX = glyphs.Max(x => x.Value.Size.X);
		MaxY = glyphs.Max(x => x.Value.Size.Y);
	}

	public Vector2D<float> GetScaledSize(float fontSize)
	{
		var scaleFactor = fontSize / Size;
		return new(MaxX * scaleFactor, MaxY * scaleFactor);
	}

	public bool TryGetGlyph(char character, out Glyph? glyph)
	{
		return glyphs.TryGetValue(character, out glyph);
	}

	/// <summary>
	/// Kerning pairs for the font, which is the adjustment of space between specific character pairs to improve visual aesthetics.
	/// </summary>
	public float GetKerning(char a, char b)
	{
		return kerningPairs.GetValueOrDefault(new KerningKey(a, b));
	}

	// todo: can i use this code in FontRenderer.cs?
	public Vector2D<float> MeasureText(string text, float scale)
	{
		var width = 0f;
		var height = 0f; // todo: i can you maxY here
		var scaleFactor = scale / Size;

		char? lastChar = null;
		foreach (var character in text)
		{
			if (glyphs.TryGetValue(character, out var glyph))
			{
				if (lastChar.HasValue)
				{
					width += GetKerning(lastChar.Value, character) * scaleFactor;
				}

				width += glyph.XAdvance * scaleFactor;
				height = MathF.Max(height, glyph.Size.Y * scaleFactor);
				lastChar = character;
			}
		}

		return new(width, height);
	}
}
