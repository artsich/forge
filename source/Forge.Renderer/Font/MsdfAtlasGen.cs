using System.Diagnostics;

namespace Forge.Renderer.Font
{
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

		public IFontService GenerateAtlas(string fontPath, int size = 32)
		{
			TryInitCache();
			var atlasImagePath = "Cache/Font/Atlas.png";
			var atlasMetaPath = "Cache/Font/Atlas.json";

			string args = $"""-type sdf -font "{fontPath}" -imageout {atlasImagePath} -json {atlasMetaPath} -size {size}""";
			Process.Start(toolPath, args).WaitForExit();
			return new SdfFont(atlasImagePath, atlasMetaPath);
		}

		private void TryInitCache()
		{
			if (!Directory.Exists("Cache/Font"))
			{
				Directory.CreateDirectory("Cache/Font");
			}
		}
	}
}
