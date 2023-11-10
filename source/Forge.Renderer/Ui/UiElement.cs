using Silk.NET.Input;
using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public abstract class UiElement
{
	public event Action<Vector2D<float>, MouseButton>? OnClick;

	public event Action OnFocus;
	public event Action OnUnfocus;

	public abstract Box2D<float> Aabb { get; }

	public Offset Padding { get; set; } = new Offset(10f);

	public Transform2d Transform { get; set; }

	public bool InFocus { get; private set; }

	public UiElement(Transform2d? transform = null)
	{
		Transform = transform ?? new Transform2d();
	}

	internal abstract void Draw();

	internal virtual void OnMouseDown(Vector2D<float> pos, MouseButton button)
	{
		OnClick?.Invoke(pos, button);

		UiRoot.Instance.SetFocusedElement(this);

		if (!InFocus)
		{
			OnFocus?.Invoke();
			InFocus = true;
		}
	}

	internal virtual void OnKeyDown(Key key)
	{
	}

	public void OutOfFocus()
	{
		if (InFocus)
		{
			InFocus = false;
			OnUnfocus?.Invoke();
		}
	}
}
