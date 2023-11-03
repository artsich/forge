using Silk.NET.Input;
using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public record struct Offset(float Left = 0f, float Top = 0f, float Right = 0f, float Bottom = 0f)
{
	public readonly Vector2D<float> LeftTop => new(Left, Top);

	public readonly Vector2D<float> LeftBottom => new(Left, Bottom);

	public readonly Vector2D<float> RightTop => new (Right, Top);
}

public abstract class UiElement
{
	public event Action<Vector2D<float>, MouseButton>? OnClick;

	public abstract Box2D<float> Aabb { get; }

	public Transform2d Transform { get; set; }

	public Offset Margin { get; set; }

	public Offset Padding { get; set; }

	public UiElement(Transform2d? transform)
	{
		Transform = transform ?? new Transform2d();
	}

	internal abstract void Draw();

	internal void OnMouseDown(Vector2D<float> pos, MouseButton button)
	{
		OnClick?.Invoke(pos, button);
	}
}
