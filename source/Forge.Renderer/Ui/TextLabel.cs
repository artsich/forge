using Forge.Renderer.Components;
using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public class TextLabel : UiElement
{
	private float fontSize;
	private readonly FontRenderer renderer;

	public string Text { get; set; } = string.Empty;

	public float FontSize
	{
		get => fontSize;
		set
		{
			if (value <= 0)
			{
				throw new InvalidOperationException("Font size must be greater than 0");
			}
			fontSize = value;
		}
	}

	public Vector4D<float> Color { get; set; } = Vector4D<float>.One;

	public TextLabel(
		FontRenderer renderer,
		Transform2d transform)
		: base(transform)
	{
		this.renderer = renderer;
	}

	internal override void Draw()
	{
		renderer.DrawText(new TextRenderComponent(Text, Transform.Position, FontSize, Color));
	}
}
