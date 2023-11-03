using Forge.Graphics;
using Forge.Graphics.Shaders;
using Forge.Renderer.Components;
using Forge.Renderer.Font;
using Forge.Renderer.Vertices.Layouts;
using Forge.Renderer.VertexAssebmlers;
using Forge.Renderer.Vertices;
using Silk.NET.OpenGL;

namespace Forge.Renderer;

public class FontRenderer : GraphicsResourceBase
{
	private readonly SpriteFont fontSprite;
	private readonly CompiledShader shader;
	private readonly BatchRenderer<GlyphVertex, CharacterRenderComponent> batchRenderer;

	public FontRenderer(SpriteFont fontSprite, ShaderSources shader)
		: this(
			fontSprite,
			shader,
			new BatchRenderer<GlyphVertex, CharacterRenderComponent>(
				new FontQuadLayout(),
				new CharacterAssembler(),
				new BatchRendererDescription(5000))
			)
	{
	}

	private FontRenderer(
		SpriteFont fontSprite,
		ShaderSources shader,
		BatchRenderer<GlyphVertex, CharacterRenderComponent> batchRenderer)
		: base(GraphicsDevice.Current)
	{
		this.fontSprite = fontSprite;
		this.shader = shader.Compile() ?? throw new Exception("Shader compilation failed");
		this.batchRenderer = batchRenderer;
	}

	public void DrawText(TextRenderComponent text)
	{
		var metrics = fontSprite.FontMetrics;
		var position = text.Position;
		var scaleFactor = text.Scale / metrics.Size;

		char? lastChar = null;
		foreach (var character in text.String)
		{
			if (metrics.TryGetGlyph(character, out var glyph) && glyph != null)
			{
				if (lastChar.HasValue)
				{
					position.X += metrics.GetKerning(lastChar.Value, character) * scaleFactor;
				}
				var characterComponent = new CharacterRenderComponent
				{
					Position = position + glyph.LayoutOffset * scaleFactor,
					Size = glyph.Size * scaleFactor,
					UV = glyph.UV,
					Color = text.Color
				};

				batchRenderer.Push(ref characterComponent);

				position.X += glyph.XAdvance * scaleFactor;
				lastChar = character;
			}
		}
	}

	public void Flush(CameraData camera)
	{
		shader.Bind(camera);
		fontSprite.Atlas.Bind(0);

		Gd.gl.Enable(GLEnum.Blend);
		Gd.gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

		batchRenderer.Flush();

		Gd.gl.Disable(GLEnum.Blend);
	}

	protected override void OnDestroy()
	{
		batchRenderer?.Dispose();
		shader?.Dispose();
	}
}
