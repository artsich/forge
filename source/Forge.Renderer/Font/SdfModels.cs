namespace Forge.Renderer.Font.Sdf;

public record struct Bounds(float Left, float Bottom, float Right, float Top);

public record Glyph(int Unicode, float Advance, Bounds PlaneBounds, Bounds AtlasBounds);

public record Metrics(int EmSize, float LineHeight, float Ascender, float Descender, float UnderlineY, float UnderlineThickness);

public record Kerning(int Unicode1, int Unicode2, float Advance);

public record Atlas(string Type, int DistanceRange, int Size, int Width, int Height, string YOrigin);

public record Root(Atlas Atlas, Metrics Metrics, List<Glyph> Glyphs, List<Kerning> Kerning);
