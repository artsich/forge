using Silk.NET.Input;
using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public abstract class UiElement
{
	public event Action<Vector2D<float>, MouseButton>? OnClick;

	public abstract Box2D<float> Aabb { get; }

	public Transform2d Transform { get; }

	public UiElement(Transform2d transform)
	{
		Transform = transform;
	}

	internal abstract void Draw();

	internal void OnMouseDown(Vector2D<float> pos, MouseButton button)
	{
		OnClick?.Invoke(pos, button);
	}
}
