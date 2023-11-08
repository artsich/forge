using Forge.Renderer.Font;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public record UiSettings(int Width, int Height, SpriteFont spriteFont);

public class UiRoot
{
	internal static UiRoot? instance;

	internal static UiRoot Instance
	{
		get
		{
			if (instance == null)
			{
				throw new Exception("UiRoot has not been initialized");
			}
			return instance;
		}
	}

	private UiElement[] elements = Array.Empty<UiElement>();

	private UiElement? focusedElement = null;

	public UiRoot(UiSettings settings)
	{
		UiRenderContext.Init(settings.Width, settings.Height, settings.spriteFont);
		instance = this;
	}

	public void AddChilds(params UiElement[] elements)
	{
		this.elements = elements.Concat(elements).ToArray();
	}

	public void Draw()
	{
		foreach (var element in elements)
		{
			element.Draw();
		}

		UiRenderContext.Flush();
	}

	internal void SetFocusedElement(UiElement? element)
	{
		focusedElement?.OutOfFocus();
		focusedElement = element;
	}

	public void OnMouseDown(Vector2D<float> pos, MouseButton button)
	{
		if (focusedElement != null && focusedElement.Aabb.Contains(pos))
		{
			focusedElement.OnMouseDown(pos, button);
			return;
		}

		bool processed = false;
		foreach (var element in elements)
		{
			if (element.Aabb.Contains(pos))
			{
				element.OnMouseDown(pos, button);
				processed = true;
				break;
			}
		}

		if (!processed)
		{
			SetFocusedElement(null);
		}
	}

	public void OnKeyDown(Key key)
	{
		focusedElement?.OnKeyDown(key);
	}
}
