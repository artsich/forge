using Forge.Graphics;
using Forge.Graphics.Shaders;
using Forge.Renderer.Components;
using Forge.Renderer.Font;
using Forge.Renderer.Layouts;
using Forge.Renderer.VertexAssebmlers;
using Forge.Renderer.Vertices;
using Silk.NET.OpenGL;

namespace Forge.Renderer;

public class FontRenderer : GraphicsResourceBase
{
	private readonly SpriteFont font;
	private readonly CompiledShader shader;
	private readonly BatchRenderer<GlyphVertex, CharacterRenderComponent> batchRenderer;

	public FontRenderer(SpriteFont font, CompiledShader shader)
		: this(
			font,
			shader,
			new BatchRenderer<GlyphVertex, CharacterRenderComponent>(
				new FontQuadLayout(),
				new CharacterAssembler(),
				new BatchRendererDescription(5000))
			)
	{
	}

	private FontRenderer(
		SpriteFont font,
		CompiledShader shader,
		BatchRenderer<GlyphVertex, CharacterRenderComponent> batchRenderer)
		: base(GraphicsDevice.Current)
	{
		this.font = font;
		this.shader = shader;
		this.batchRenderer = batchRenderer;
	}

	public void DrawText(TextRenderComponent text)
	{
		var position = text.Position;
		var scaleFactor = text.Scale / font.Size;

		char? lastChar = null;
		foreach (var character in text.String)
		{
			if (font.Glyphs.TryGetValue(character, out var glyph))
			{
				if (lastChar.HasValue)
				{
					position.X += font.GetKerning(lastChar.Value, character) * scaleFactor;
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
		font.Atlas.Bind(0);

		Gd.gl.Enable(GLEnum.Blend);
		Gd.gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

		batchRenderer.Flush();

		Gd.gl.Disable(GLEnum.Blend);
	}

	protected override void OnDestroy()
	{
		batchRenderer?.Dispose();
	}
}
