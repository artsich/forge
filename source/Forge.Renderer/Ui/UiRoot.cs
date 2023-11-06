using Silk.NET.Input;
using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public class UiRoot
{
	private readonly UiElement[] elements;

	public UiRoot(params UiElement[] elements)
	{
		this.elements = elements;
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
