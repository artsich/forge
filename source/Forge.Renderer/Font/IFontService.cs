namespace Forge.Renderer.Font;

public interface IFontService
{
	SpriteFont CreateFont(string path);

	bool FontExists(string name);

	SpriteFont GetFont(string name);
}
