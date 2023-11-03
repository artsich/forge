using Silk.NET.Maths;
using System.Data;

namespace Forge.Renderer.Font;

public class FontService : IFontService
{
	private readonly string pathToFontFolder;

	public FontService(string pathToFontFolder)
	{
		this.pathToFontFolder = pathToFontFolder;
	}

	public SpriteFont CreateFont(string fontPath)
	{
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			var name = new MsdfAtlasGen().GenerateAtlas(fontPath, pathToFontFolder);
			return GetFont(name);
		}

		throw new NotImplementedException();
	}

	public bool FontExists(string name)
	{
		var (meta, texture) = GetFontPath(name);

		if (!Path.Exists(meta) || !Path.Exists(texture))
		{
			return false;
		}

		return true;
	}

	public SpriteFont GetFont(string name)
	{
		var (meta, texture) = GetFontPath(name);

		if (!FontExists(name))
		{
			throw new DataException($"Font {name} does not exist");
		}

		return GetSpriteFont(meta, texture);
	}

	private (string meta, string texture) GetFontPath(string name)
	{
		return (
			Path.Combine(pathToFontFolder, name + "_Atlas.json"),
			Path.Combine(pathToFontFolder, name + "_Atlas.png")
		);
	}

	private static SpriteFont GetSpriteFont(string metaPath, string texturePath)
	{
		Root meta = LoadMeta(metaPath);

		var glyphs = new Dictionary<char, Glyph>();

		foreach (var glyphData in meta.Glyphs)
		{
			var glyph = ParseGlyph(meta.Atlas, glyphData);
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
				LoadPixels(texturePath),
				true)
			{
				Filter = Graphics.MinMagFilter.Linear,
				Wrap = Graphics.TextureWrap.ClampToEdge,
			},
			glyphs,
			kerningPairs);
	}

	private static byte[] LoadPixels(string texturePath)
	{
		byte[] textureData;

		using (Image<Rgba32> image = Image.Load<Rgba32>(texturePath))
		{
			textureData = new byte[image.Width * image.Height * 4];

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

	private static Glyph ParseGlyph(Atlas atlas, GlyphInfo glyphData)
	{
		var size = new Vector2D<float>(
			glyphData.AtlasBounds.Right - glyphData.AtlasBounds.Left,
			glyphData.AtlasBounds.Top - glyphData.AtlasBounds.Bottom);
		size = Vector2D.Abs(size);

		Vector2D<float> layoutOffset;
		if (atlas.YOrigin == "top")
		{
			layoutOffset = new Vector2D<float>(
				glyphData.PlaneBounds.Left * atlas.Size,
				-glyphData.PlaneBounds.Top * atlas.Size);
		}
		else
		{
			layoutOffset = new Vector2D<float>(
				glyphData.PlaneBounds.Left * atlas.Size,
				glyphData.PlaneBounds.Bottom * atlas.Size);
		}

		var uv = new Rectangle<float>(
				glyphData.AtlasBounds.Left / atlas.Width,
				glyphData.AtlasBounds.Top / atlas.Height,
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

	private static Root LoadMeta(string metaInfoPath)
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
