using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public class HorizontalContainer : Container
{
	public HorizontalContainer(UiQuadRenderer uiQuadRenderer, float distanceBtwElements)
		: base(uiQuadRenderer, distanceBtwElements)
	{
	}

	public override Box2D<float> Aabb
	{
		get
		{
			float height = 0;
			float width = 0;
			foreach (var child in Children)
			{
				var childAabb = child.Aabb;
				width += childAabb.Size.X + DistanceBtwElements;
				height = MathF.Max(height, childAabb.Size.Y) ;
			}

			if (Children.Count > 0)
			{
				width -= DistanceBtwElements;
			}

			var min = Transform.Position;
			var max = Transform.Position + new Vector2D<float>(
				width + (Padding.Left + Padding.Right),
				-height - (Padding.Top + Padding.Bottom));

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
			currentPos.X += childAabb.Size.X + DistanceBtwElements;
		}
	}
}
