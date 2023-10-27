using Forge.Graphics;
using Forge.Graphics.Shaders;
using Forge.Renderer.Components;
using Forge.Renderer.Font;
using Forge.Renderer.Layouts;
using Forge.Renderer.VertexAssebmlers;
using Forge.Renderer.Vertices;

namespace Forge.Renderer;

public class FontRenderer : BatchRenderer<GlyphVertex, CharacterRenderComponent>
{
	private readonly SpriteFont font;
	private readonly CompiledShader shader;

	public FontRenderer(SpriteFont font, CompiledShader shader)
		: base(
			GraphicsDevice.Current,
			new FontQuadLayout(),
			new TextAssembler(),
			new BatchRendererDescription(1000))
	{
		this.font = font;
		this.shader = shader;
	}

	public void Push(TextRenderComponent text)
	{
		var batch = GetBatch();

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
				batch.Add(ref characterComponent);

				position.X += glyph.XAdvance * scaleFactor;
				lastChar = character;
			}
		}
	}

	public void Flush(CameraData camera)
	{
		shader.Bind();
		shader["cameraProj"].SetValue(camera.Projection);
		shader["cameraView"].SetValue(camera.View);
		font.Atlas.Bind(0);

		Flush();
	}
}
