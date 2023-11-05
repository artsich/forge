using Forge.Renderer.Components;
using Forge.Renderer.Font;
using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public class TextLabel : UiElement
{
	private float fontSize = 13;
	private readonly FontMetrics fontInfo;
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

	public override Box2D<float> Aabb
	{
		get
		{
			// todo: font Info can be cached??
			var size = fontInfo.MeasureText(Text, FontSize);
			return new Box2D<float>(AdjustedPosition, AdjustedPosition + size);
		}
	}

	// todo: this is a hack, fix this. Position should be the top left corner of the text, not the bottom left
	private Vector2D<float> AdjustedPosition => Transform.Position with
	{
		Y = Transform.Position.Y - fontInfo.MeasureText(Text, FontSize).Y
	};

	public TextLabel(
		FontMetrics fontInfo,
		FontRenderer renderer,
		Transform2d? transform = null)
		: base(transform)
	{
		this.fontInfo = fontInfo;
		this.renderer = renderer;
	}

	internal override void Draw()
	{
		renderer.DrawText(new TextRenderComponent(Text, AdjustedPosition, FontSize, Color));
	}
}
