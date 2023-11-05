using Silk.NET.Input;
using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public abstract class Container : UiElement
{
	private readonly UiQuadRenderer uiQuadRenderer;

	public float DistanceBtwElements { get; set;}

	public IList<UiElement> Children { get; set; } = new List<UiElement>();

	public Vector4D<float> Color { get; set; }

	public Container(
		UiQuadRenderer uiQuadRenderer,
		float distanceBtwElements)
	{
		this.uiQuadRenderer = uiQuadRenderer;
		DistanceBtwElements = distanceBtwElements;
	}

	internal override void Draw()
	{
		var aabb = Aabb;
		var size = Vector2D.Abs(aabb.Max - aabb.Min);

		uiQuadRenderer.Push(new Components.QuadRenderComponent()
		{
			Color = Color,
			Position = Transform.Position,
			Size = size
		});

		AdjustLayoutChildren();

		foreach (var child in Children)
		{
			child.Draw();
		}
	}

	protected abstract void AdjustLayoutChildren();

	internal override void OnMouseDown(Vector2D<float> pos, MouseButton button)
	{
		var processed = false;
		foreach (var child in Children)
		{
			if (child.Aabb.Contains(pos))
			{
				child.OnMouseDown(pos, button);
				processed = true;
				break;
			}
		}

		if (!processed)
		{
			base.OnMouseDown(pos, button);
		}
	}
}
