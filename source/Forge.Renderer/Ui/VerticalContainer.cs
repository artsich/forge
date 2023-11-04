using Silk.NET.Input;
using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

/*
As a Developer I want to have H/V continer to be able to layout my UI elements

I should be able to add elements to the container and have them be laid out vertically/horizontally
I should be able to set distance between elements

*/

public class VerticalContainer : UiElement
{
	private readonly UiQuadRenderer uiQuadRenderer;
	private readonly float distanceBtwElements;

	private readonly List<UiElement> children = new();

	public List<UiElement> Children {
		private get 
		{ 
			return children;
		}
		init
		{
			children = value;
			AdjustLayoutChildren();
		}
	}

	public Vector4D<float> Color { get; set; }

	public VerticalContainer(
		UiQuadRenderer uiQuadRenderer,
		float distanceBtwElements)
	{
		this.uiQuadRenderer = uiQuadRenderer;
		this.distanceBtwElements = distanceBtwElements;
	}

	public override Box2D<float> Aabb
	{
		get
		{
			float ySize = 0;
			float xMax = float.MinValue;
			foreach (var child in Children)
			{
				var childAabb = child.Aabb;
				xMax = MathF.Max(xMax, childAabb.Max.X);
				ySize += childAabb.Max.Y - childAabb.Min.Y + distanceBtwElements;
			}

			var min = Transform.Position;
			var max = Transform.Position + new Vector2D<float>(
				xMax + Padding.Left + Padding.Right,
				ySize + Padding.Top + Padding.Bottom);

			return new Box2D<float>(
				Vector2D.Min(min, max),
				Vector2D.Max(min, max));
		}
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

	public void AddChild(UiElement child)
	{
		Children.Add(child);
	}

	private void AdjustLayoutChildren()
	{
		Vector2D<float> currentPos = Transform.Position - (Padding.LeftTop * new Vector2D<float>(-1f, 1f));

		foreach (var child in Children)
		{
			child.Transform.Position = currentPos;
			var childAabb = child.Aabb;
			currentPos.Y -= childAabb.Max.Y - childAabb.Min.Y + distanceBtwElements;
		}
	}

	internal override void OnMouseDown(Vector2D<float> pos, MouseButton button)
	{
		var processed = false;
		foreach (var child in Children)
		{
			var localPos = child.Transform.Position;
			if (child.Aabb.Contains(localPos))
			{
				child.OnMouseDown(localPos, button);
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
