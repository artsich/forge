using Forge.Renderer.Font.Sdf;
using Silk.NET.Maths;

namespace Forge.Renderer.Font;

public class SdfFont : IFontService
{
	private readonly string texturePath;
	private readonly string metaInfoPath;

	public SdfFont(string texturePath, string metaInfoPath)
	{
		this.texturePath = texturePath;
		this.metaInfoPath = metaInfoPath;
	}

	public byte[] GetTextureData()
	{
		return File.ReadAllBytes(texturePath);
	}

	public FontMeta GetMeta()
	{
		var jsonData = File.ReadAllText(metaInfoPath);
		var root = System.Text.Json.JsonSerializer.Deserialize<Root>(jsonData, new System.Text.Json.JsonSerializerOptions()
		{
			PropertyNameCaseInsensitive = true
		});
		var fontData = new Dictionary<char, Glyph>();

		foreach (var glyph in root.Glyphs)
		{
			fontData[(char)glyph.Unicode] = new Glyph
			{
				UVPosition = new Vector2D<float>(glyph.AtlasBounds.Left, glyph.AtlasBounds.Bottom),
				UVSize = new Vector2D<float>((glyph.AtlasBounds.Right - glyph.AtlasBounds.Left) / root.Atlas.Width, (glyph.AtlasBounds.Top - glyph.AtlasBounds.Bottom) / root.Atlas.Height),
				Size = new Vector2D<float>(glyph.PlaneBounds.Right - glyph.PlaneBounds.Left, glyph.PlaneBounds.Top - glyph.PlaneBounds.Bottom),
				Bearing = new Vector2D<float>(glyph.PlaneBounds.Left, glyph.PlaneBounds.Bottom),
				Advance = glyph.Advance
			};
		}

		var kerningPairs = new Dictionary<string, float>();
		foreach (var kern in root.Kerning)
		{
			string key = $"{(char)kern.Unicode1}{(char)kern.Unicode2}";
			kerningPairs[key] = kern.Advance;
		}

		return new FontMeta(fontData, kerningPairs);
	}
}
