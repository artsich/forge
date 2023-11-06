using Forge.Renderer.Font;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public record UiSettings(int Width, int Height, SpriteFont spriteFont);

public class UiRoot
{
	private UiElement[] elements = Array.Empty<UiElement>();

	public UiRoot(UiSettings settings)
	{
		UiRenderContext.Init(settings.Width, settings.Height, settings.spriteFont);
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

	public void OnMouseDown(Vector2D<float> pos, MouseButton button)
	{
		foreach (var element in elements)
		{
			if (element.Aabb.Contains(pos))
			{
				element.OnMouseDown(pos, button);
				break;
			}
		}
	}
}
