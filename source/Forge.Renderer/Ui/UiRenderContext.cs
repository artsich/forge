using Forge.Renderer.Font;
using Forge.Shaders;
using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public sealed class UiRenderContext : IDisposable
{
	internal readonly UiQuadRenderer QuadRenderer;
	internal readonly FontRenderer FontRenderer;
	private readonly CameraData camera;

	private static UiRenderContext? instance;

	internal static UiRenderContext Instance
	{
		get
		{
			_ = instance ?? throw new Exception("UiRenderContext has not been initialized");
			return instance;
		}
	}

	private UiRenderContext(int width, int height, SpriteFont spriteFont)
	{
		QuadRenderer = new UiQuadRenderer(new QuadShader().Compile() ?? throw new Exception("Shader compilation failed"));
		FontRenderer = new FontRenderer(spriteFont, new SdfFontShader());

		camera = new(Matrix4X4.CreateOrthographic(width, height, 0.1f, 100.0f));
	}

	private void FlushAll()
	{
		QuadRenderer.Flush(camera);
		FontRenderer.Flush(camera);
	}

	public void Dispose()
	{
		FontRenderer.Dispose();
		QuadRenderer.Dispose();
	}

	public static void Init(int width, int height, SpriteFont spriteFont)
	{
		instance ??= new UiRenderContext(width, height, spriteFont);
	}

	public static void Flush()
	{
		Instance!.FlushAll();
	}

	public static void Destroy()
	{
		Instance!.Dispose();
	}
}
