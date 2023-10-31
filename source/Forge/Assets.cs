using Forge.Renderer.Font;

namespace Forge;

public class Assets
{
	private readonly IFontService fontService;
	public string patoToAssets = ".\\Assets";

	public Assets(IFontService fontService)
	{
		this.fontService = fontService;
	}

	public SpriteFont LoadFont(string name)
	{
		return fontService.GetFont(name);
	}
}
