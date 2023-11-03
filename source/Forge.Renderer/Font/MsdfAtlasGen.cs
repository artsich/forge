using System.Diagnostics;

namespace Forge.Renderer.Font;

public sealed class MsdfAtlasGen
{
	private const int DistanceRange = 2;
	private const string YOrigin = "top";

	private readonly string toolPath;

	public MsdfAtlasGen()
		: this("ThirdParty/msdf-atlas-gen.exe")
	{
	}

	public MsdfAtlasGen(string toolPath)
	{
		this.toolPath = toolPath;
	}

	public string GenerateAtlas(string fontPath, string savePath, int size = 32)
	{
		var fontName = Path.GetFileNameWithoutExtension(fontPath);

		var atlasImagePath = Path.Combine(savePath, $"{fontName}_Atlas.png");
		var atlasMetaPath = Path.Combine(savePath, $"{fontName}_Atlas.json");

		string args = $"""-type sdf -font "{fontPath}" -imageout "{atlasImagePath}" -json "{atlasMetaPath}" -size {size} -yorigin {YOrigin} -pxrange {DistanceRange}""";
		Process.Start(toolPath, args).WaitForExit();

		return fontName;
	}
}
