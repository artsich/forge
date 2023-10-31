namespace Forge.Renderer.Ui;

public class UiElements
{
	private readonly UiElement[] elements;

	public UiElements(params UiElement[] elements)
	{
		this.elements = elements;
	}

	public void Draw()
	{
		foreach (var element in elements)
		{
			element.Draw();
		}
	}
}
