using System.Text.Json.Serialization;

namespace Forge.Renderer.Sprites;

public record struct SpriteRect(
	int X,
	int Y,
	int Width,
	int Height);

public record ExtractedSpriteRect(
	[property: JsonIgnore] SpriteSheet Source,
	string Name,
	SpriteRect Rect,
	string GroupName = "");