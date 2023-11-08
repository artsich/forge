using Forge.Renderer.Components;
using Forge.Renderer.Font;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public class Input : UiElement
{
	private readonly FontMetrics fontMetrics;
	private readonly UiQuadRenderer quadRenderer;
	private readonly FontRenderer fontRenderer;

	private float fontSize;
	private int startSymbolIndex;

	private Vector2D<float> scaledFontSizeDim;

	public Vector4D<float> Color { get; set; }

	public Vector4D<float> TextColor { get; set; }

	public Vector2D<float> Size { get; set; }

	public float FontSize
	{
		get
		{
			return fontSize;
		}
		set
		{
			fontSize = Math.Max(value, 1);
			scaledFontSizeDim = fontMetrics.GetScaledSize(fontSize);
		}
	}

	// todo: it's should an array of chars
	public string Text { get; set; } = string.Empty;

	public override Box2D<float> Aabb
	{
		get
		{
			var pos = Transform.Position;
			var min = pos;
			var max = new Vector2D<float>()
			{
				X = pos.X + Size.X + Padding.Left + Padding.Right,
				Y = pos.Y - scaledFontSizeDim.Y - (Padding.Top + Padding.Bottom)
			};

			return new Box2D<float>(
				Vector2D.Min(min, max),
				Vector2D.Max(min, max));
		}
	}

	public Input()
	{
		quadRenderer = UiRenderContext.Instance!.QuadRenderer;
		fontRenderer = UiRenderContext.Instance!.FontRenderer;
		fontMetrics = fontRenderer.SpriteFont.FontMetrics;
		FontSize = 13f;
	}

	internal override void Draw()
	{
		var aabb = Aabb;
		var size = Vector2D.Abs(aabb.Max - aabb.Min);
		quadRenderer.Push(new QuadRenderComponent(
				Transform.Position,
				size,
				Color));

		var textPosition = new Vector2D<float>() 
		{
			X = Transform.Position.X + Padding.Left,
			Y = Transform.Position.Y - scaledFontSizeDim.Y - Padding.Top
		};

		var inputWidth = size.X - (Padding.Left + Padding.Right);
		var availableSimbols = (int)Math.Ceiling(inputWidth / scaledFontSizeDim.X);
		if (availableSimbols < Text.Length)
		{
			startSymbolIndex = Text.Length - availableSimbols;
		}

		var textToRender = Text[startSymbolIndex..];
		fontRenderer.DrawText(new TextRenderComponent(
			textToRender,
			textPosition,
			FontSize,
			TextColor));
	}

	internal override void OnKeyDown(Key key)
	{
		if (InFocus)
		{
			var ch = (char)key;

			if (char.IsAscii(ch))
			{
				Text += ch;
			}
			else if (key == Key.Backspace)
			{
				Text = Text[..^1];
			}
		}
	}
}
