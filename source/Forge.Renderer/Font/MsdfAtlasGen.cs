using System.Diagnostics;

namespace Forge.Renderer.Font;

public sealed class MsdfAtlasGen
{
	private readonly string toolPath;

	public MsdfAtlasGen()
		: this("ThirdParty/msdf-atlas-gen.exe")
	{
	}

	public MsdfAtlasGen(string toolPath)
	{
		this.toolPath = toolPath;
	}

	public IFontService GenerateAtlas(string fontPath, int size = 128)
	{
		// todo: think about cache abstraction.
		TryInitCache();
		var fontName = Path.GetFileNameWithoutExtension(fontPath);

		var atlasImagePath = $"Cache/Font/{fontName}_Atlas.png";
		var atlasMetaPath = $"Cache/Font/{fontName}_Atlas.json";

		if (!(File.Exists(atlasImagePath) && File.Exists(atlasMetaPath)))
		{
			string args = $"""-type sdf -font "{fontPath}" -imageout "{atlasImagePath}" -json "{atlasMetaPath}" -size {size} -yorigin bottom -pxrange 10""";
			Process.Start(toolPath, args).WaitForExit();
		}

		return new FontService(atlasImagePath, atlasMetaPath);
	}

	private static void TryInitCache()
	{
		if (!Directory.Exists("Cache/Font"))
		{
			Directory.CreateDirectory("Cache/Font");
		}
	}
}
