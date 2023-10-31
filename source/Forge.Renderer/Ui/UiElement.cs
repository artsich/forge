namespace Forge.Renderer.Ui;

public abstract class UiElement
{
	public Transform2d Transform { get; }

	public UiElement(Transform2d transform)
	{
		Transform = transform;
	}

	internal abstract void Draw();
}
