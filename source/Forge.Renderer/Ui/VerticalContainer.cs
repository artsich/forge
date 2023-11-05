using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public class VerticalContainer : Container
{
	public VerticalContainer(UiQuadRenderer uiQuadRenderer, float distanceBtwElements)
		: base(uiQuadRenderer, distanceBtwElements)
	{
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
				xMax = MathF.Max(xMax, childAabb.Size.X);
				ySize += childAabb.Size.Y + DistanceBtwElements;
			}

			if (Children.Count > 0)
			{
				ySize -= DistanceBtwElements;
			}

			var min = Transform.Position;
			var max = Transform.Position + new Vector2D<float>(
				xMax + Padding.Left + Padding.Right,
				-ySize - (Padding.Top + Padding.Bottom));

			return new Box2D<float>(
				Vector2D.Min(min, max),
				Vector2D.Max(min, max));
		}
	}

	protected override void AdjustLayoutChildren()
	{
		Vector2D<float> currentPos = Transform.Position - (Padding.LeftTop * new Vector2D<float>(-1f, 1f));

		foreach (var child in this.Children)
		{
			child.Transform.Position = currentPos;
			var childAabb = child.Aabb;
			currentPos.Y -= childAabb.Max.Y - childAabb.Min.Y + DistanceBtwElements;
		}
	}
}
