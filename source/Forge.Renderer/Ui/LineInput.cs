using Forge.Renderer.Components;
using Forge.Renderer.Font;
using Silk.NET.Input;
using Silk.NET.Maths;
using static System.Math;

namespace Forge.Renderer.Ui;

public class LineInput : UiElement
{
	private readonly FontMetrics fontMetrics;
	private readonly UiQuadRenderer quadRenderer;
	private readonly FontRenderer fontRenderer;

	private float fontSize;
	private Vector2D<float> scaledFontSizeDim;

	private int cursorIndex;
	private int startTextWindowIndex;
	private int endTextWindowIndex;

	public Vector4D<float> Color { get; set; }

	public Vector4D<float> TextColor { get; set; }

	public float Width { get; set; }

	public float FontSize
	{
		get
		{
			return fontSize;
		}
		set
		{
			fontSize = Max(value, 1);
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
			var max = pos + new Vector2D<float>()
			{
				X = Width,
				Y = -scaledFontSizeDim.Y - (Padding.Top + Padding.Bottom)
			};

			return new Box2D<float>(
				Vector2D.Min(min, max),
				Vector2D.Max(min, max));
		}
	}

	private Vector2D<float> AabbSize => Vector2D.Abs(Aabb.Max - Aabb.Min);

	public LineInput()
	{
		quadRenderer = UiRenderContext.Instance!.QuadRenderer;
		fontRenderer = UiRenderContext.Instance!.FontRenderer;
		fontMetrics = fontRenderer.SpriteFont.FontMetrics;
		FontSize = 19f;
	}

	internal override void Draw()
	{
		var size = AabbSize;
		quadRenderer.Push(new QuadRenderComponent(
				Transform.Position,
				size,
				Color));

		var textPosition = Transform.Position + new Vector2D<float>()
		{
			X = Padding.Left,
			Y = -scaledFontSizeDim.Y - Padding.Top
		};

		UpdateTextWindow(size.X);

		var textToRender = Text[startTextWindowIndex..endTextWindowIndex];
		fontRenderer.DrawText(new TextRenderComponent(
			textToRender,
			textPosition,
			FontSize,
			TextColor));

		if (InFocus)
		{
			var textSize = fontMetrics.MeasureText(Text[startTextWindowIndex..cursorIndex], FontSize);
			var cursorPosition = textPosition +
				new Vector2D<float>()
				{
					X = textSize.X,
					Y = scaledFontSizeDim.Y
				};

			quadRenderer.Push(new QuadRenderComponent(
				cursorPosition,
				new Vector2D<float>()
				{
					X = 1f,
					Y = scaledFontSizeDim.Y
				},
				Vector4D<float>.One));
		}
	}

	internal override void OnKeyDown(Key key)
	{
		if (InFocus)
		{
			var keyboard = UiRoot.Instance.Keyboard;

			if (!HandleCursorActions(key, keyboard))
			{
				var ch = (char)key;
				if (char.IsAscii(ch))
				{
					ch = IsUpperCase(keyboard) ? char.ToUpper(ch) : char.ToLower(ch);
					Text = Text.Insert(cursorIndex, ch.ToString());
					cursorIndex++;
				}
			}
		}
	}

	private bool HandleCursorActions(Key key, IKeyboard keyboard)
	{
		var leftCtrl = keyboard.IsKeyPressed(Key.ControlLeft);
		switch (key)
		{
			case Key.Left:
				{
					MoveCursorLeft(leftCtrl);
					break;
				}
			case Key.Right:
				{
					MoveCursorRight(leftCtrl);
					break;
				}
			case Key.Home:
				{
					cursorIndex = 0;
					break;
				}
			case Key.End:
				{
					cursorIndex = Text.Length;
					break;
				}
			case Key.V:
				{
					if (leftCtrl)
					{
						Text = Text.Insert(cursorIndex, keyboard.ClipboardText);
						cursorIndex += keyboard.ClipboardText.Length;
					}
					else
					{
						return false;
					}
					break;
				}
			case Key.Delete:
				{
					if (cursorIndex < Text.Length)
					{
						Text = Text.Remove(cursorIndex, 1);
					}
					break;
				}
			case Key.Backspace:
				{
					if (Text.Length > 0 && cursorIndex > 0)
					{
						Text = Text.Remove(cursorIndex - 1, 1);
						cursorIndex--;
					}
					break;
				}
			default:
				return false;
		}

		return true;
	}

	private void MoveCursorRight(bool leftCtrl)
	{
		if (leftCtrl)
		{
			MoveCursorToRightWord();
		}
		else
		{
			cursorIndex = Min(cursorIndex + 1, Text.Length);
		}
	}

	private void MoveCursorToRightWord()
	{
		if (cursorIndex < Text.Length && Text[cursorIndex] == ' ')
		{
			while (++cursorIndex < Text.Length && Text[cursorIndex] == ' ') { }
		}
		while (cursorIndex < Text.Length)
		{
			if (Text[cursorIndex] == ' ')
			{
				break;
			}
			cursorIndex++;
		}
	}

	private void MoveCursorLeft(bool leftCtrl)
	{
		if (leftCtrl)
		{
			MoveCursorToLeftWord();
		}
		else
		{
			cursorIndex = Max(cursorIndex - 1, 0);
		}
	}

	private void MoveCursorToLeftWord()
	{
		if (cursorIndex > 0 && Text[cursorIndex - 1] == ' ')
		{
			while (--cursorIndex > 0 && Text[cursorIndex] == ' ') { }
		}
		while (cursorIndex > 0)
		{
			if (Text[cursorIndex - 1] == ' ')
			{
				break;
			}
			cursorIndex--;
		}
	}

	private void UpdateTextWindow(float windowWidth)
	{
		var inputWidth = windowWidth - (Padding.Left + Padding.Right);
		var availableSimbols = (int)Ceiling(inputWidth / scaledFontSizeDim.X);

		if (startTextWindowIndex > cursorIndex)
		{
			startTextWindowIndex = Max(cursorIndex, 0);
		}

		if (cursorIndex == startTextWindowIndex)
		{
			startTextWindowIndex = Max(cursorIndex - (availableSimbols / 2), 0);
		}

		if (cursorIndex > endTextWindowIndex && endTextWindowIndex - startTextWindowIndex >= availableSimbols)
		{
			startTextWindowIndex = Max(cursorIndex - availableSimbols, 0);
		}

		endTextWindowIndex = Min(startTextWindowIndex + availableSimbols, Text.Length);
	}

#pragma warning disable CA1416 // Validate platform compatibility
	private static bool IsUpperCase(IKeyboard keyboard)
	{
		var isUpperCase = keyboard.IsKeyPressed(Key.ShiftLeft);
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			isUpperCase |= Console.CapsLock;
		}
		return isUpperCase;
	}
#pragma warning restore CA1416 // Validate platform compatibility
}
