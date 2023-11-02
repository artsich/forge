using Forge.Graphics;

namespace Forge.Renderer.Font;

public class SpriteFont : GraphicsResourceBase
{
	public FontMetrics FontMetrics { get; }
	
	public Texture2d Atlas { get; }

	public SpriteFont(
		float size,
		Texture2d atlas,
		IReadOnlyDictionary<char, Glyph> glyphs,
		IReadOnlyDictionary<KerningKey, float> kerningPairs)
		: base(GraphicsDevice.Current)
	{
		Atlas = atlas;
		FontMetrics = new FontMetrics(size, kerningPairs, glyphs);
	}

	protected override void OnDestroy()
	{
		Atlas?.Dispose();
	}
}
