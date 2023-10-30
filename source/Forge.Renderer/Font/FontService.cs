using Silk.NET.Maths;
using System.Data;

namespace Forge.Renderer.Font;

public class FontService : IFontService
{
	private readonly string texturePath;
	private readonly string metaInfoPath;

	public FontService(string texturePath, string metaInfoPath)
	{
		this.texturePath = texturePath;
		this.metaInfoPath = metaInfoPath;
	}

	public SpriteFont GetSpriteFont()
	{
		Root meta = LoadMeta();

		var glyphs = new Dictionary<char, Glyph>();

		foreach (var glyphData in meta.Glyphs)
		{
			var glyph = ParseGlyph(glyphs, meta.Atlas, glyphData);
			glyphs[glyph.Character] = glyph;
		}

		var kerningPairs = new Dictionary<KerningKey, float>();
		foreach (var kern in meta.Kerning)
		{
			kerningPairs[new(kern.Unicode1, kern.Unicode2)] = kern.Advance;
		}

		return new SpriteFont(
			meta.Atlas.Size,
			new Graphics.Texture2d(
				(uint)meta.Atlas.Width, (uint)meta.Atlas.Height,
				LoadPixels(),
				true)
			{
				Filter = Graphics.MinMagFilter.Linear,
				Wrap = Graphics.TextureWrap.ClampToEdge,
			},
			glyphs,
			kerningPairs);
	}

	private byte[] LoadPixels()
	{
		byte[] textureData;

		using (Image<Rgba32> image = Image.Load<Rgba32>(texturePath))
		{
			// Flip the image vertically
			image.Mutate(ctx => ctx.Flip(FlipMode.Vertical));

			// Convert image to byte array
			textureData = new byte[image.Width * image.Height * 4]; // Assuming RGBA format

			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					Rgba32 pixel = image[x, y];
					int index = y * image.Width * 4 + x * 4;

					textureData[index] = pixel.R;
					textureData[index + 1] = pixel.G;
					textureData[index + 2] = pixel.B;
					textureData[index + 3] = pixel.A;
				}
			}
		}

		return textureData;
	}

	private static Glyph ParseGlyph(Dictionary<char, Glyph> glyphs, Atlas atlas, GlyphInfo glyphData)
	{
		var size = new Vector2D<float>(
			glyphData.AtlasBounds.Right - glyphData.AtlasBounds.Left,
			glyphData.AtlasBounds.Top - glyphData.AtlasBounds.Bottom);

		var layoutOffset = new Vector2D<float>(
			glyphData.PlaneBounds.Left * atlas.Size,
			glyphData.PlaneBounds.Bottom * atlas.Size);

		var uv = new Rectangle<float>(
				glyphData.AtlasBounds.Left / atlas.Width,
				glyphData.AtlasBounds.Bottom / atlas.Height,
				size.X / atlas.Width,
				size.Y / atlas.Height);

		return new Glyph()
		{
			Character = (char)glyphData.Unicode,
			Size = size,
			UV = uv,
			LayoutOffset = layoutOffset,
			XAdvance = glyphData.Advance * atlas.Size,
		};
	}

	private Root LoadMeta()
	{
		var jsonData = File.ReadAllText(metaInfoPath);
		var root = System.Text.Json.JsonSerializer.Deserialize<Root>(jsonData, new System.Text.Json.JsonSerializerOptions()
		{
			PropertyNameCaseInsensitive = true
		}) ?? throw new DataException("Failed to deserialize font meta info");
		return root;
	}
}

record struct Bounds(float Left, float Bottom, float Right, float Top);

record GlyphInfo(int Unicode, float Advance, Bounds PlaneBounds, Bounds AtlasBounds);

record Metrics(int EmSize, float LineHeight, float Ascender, float Descender, float UnderlineY, float UnderlineThickness);

record Kerning(int Unicode1, int Unicode2, float Advance);

record Atlas(string Type, int DistanceRange, int Size, int Width, int Height, string YOrigin);

record Root(Atlas Atlas, Metrics Metrics, List<GlyphInfo> Glyphs, List<Kerning> Kerning);
