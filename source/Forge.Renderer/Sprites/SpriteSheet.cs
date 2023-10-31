using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using System.Text.Json;

namespace Forge.Renderer.Sprites;

public class SpriteSheet
{
	private readonly string pathToSheet;
	private readonly string pathToMeta;
	private readonly string debugOutputPath;

	public IList<ExtractedSpriteRect> ExtractedSprites { get; private set; } = new List<ExtractedSpriteRect>();

	public SpriteSheet(string path)
	{
		pathToSheet = path;
		pathToMeta = System.IO.Path.ChangeExtension(pathToSheet, ".meta.json");
		debugOutputPath = System.IO.Path.ChangeExtension(pathToSheet, ".debug.png");
	}

	public void Load()
	{
		if (!File.Exists(pathToSheet))
		{
			throw new Exception($"Sprite sheet {pathToSheet} does not exist");
		}

		ExtractedSprites = Extract(pathToSheet);
	}

	public void Save()
	{
		File.WriteAllText(
			pathToMeta,
			JsonSerializer.Serialize(ExtractedSprites));
	}

	public void SaveAsHighlightedImage()
	{
		using Image<Rgba32> image = Image.Load<Rgba32>(pathToSheet);
		const int thinkness = 1;

		foreach (var sprite in ExtractedSprites.Select(x => x.Rect))
		{
			image.Mutate(ctx =>
				ctx.Draw(
					Color.Red,
					thinkness,
					new RectangularPolygon(
						sprite.X, sprite.Y, sprite.Width, sprite.Height)));
		}

		image.Save(debugOutputPath);
	}

	private IList<ExtractedSpriteRect> Extract(string imagePath)
	{
		using Image<Rgba32> image = Image.Load<Rgba32>(imagePath);
		string namePattern = "sprite_{0}";

		bool[,] visited = new bool[image.Width, image.Height];
		List<ExtractedSpriteRect> spriteBoxes = new();

		int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
		int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

		var spriteIndex = 0;
		for (int x = 0; x < image.Width; x++)
		{
			for (int y = 0; y < image.Height; y++)
			{
				if (IsTransparent(image, x, y) || visited[x, y]) continue;

				int minX = x, maxX = x, minY = y, maxY = y;

				Queue<(int, int)> queue = new Queue<(int, int)>();
				queue.Enqueue((x, y));

				while (queue.Count > 0)
				{
					var (currentX, currentY) = queue.Dequeue();

					if (currentX < 0 || currentY < 0 || currentX >= image.Width || currentY >= image.Height)
						continue;

					if (visited[currentX, currentY] || IsTransparent(image, currentX, currentY))
						continue;

					visited[currentX, currentY] = true;

					minX = Math.Min(minX, currentX);
					maxX = Math.Max(maxX, currentX);
					minY = Math.Min(minY, currentY);
					maxY = Math.Max(maxY, currentY);

					for (int dir = 0; dir < 8; dir++)
					{
						queue.Enqueue((currentX + dx[dir], currentY + dy[dir]));
					}
				}

				spriteBoxes.Add(
					new ExtractedSpriteRect(
						this,
						string.Format(namePattern, spriteIndex),
						new SpriteRect(
							minX, minY,
							maxX - minX + 1, maxY - minY + 1)));
				spriteIndex++;
			}
		}

		return spriteBoxes.OrderBy(sp => sp.Rect.Y).ToList();
	}

	private static bool IsTransparent(Image<Rgba32> image, int x, int y)
	{
		return image[x, y].A == 0;
	}
}
