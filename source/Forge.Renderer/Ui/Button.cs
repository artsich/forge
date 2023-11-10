using Forge.Renderer.Components;
using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public class Button : UiElement
{
	private readonly TextLabel textLabel;
	private readonly UiQuadRenderer quadRenderer;

	public Vector4D<float> Color { get; set; }

	public Vector2D<float> Size { get; set; }

	public Button(TextLabel textLabel, Transform2d? transform = null)
		: this(textLabel, UiRenderContext.Instance!.QuadRenderer, transform)
	{
	}

	public Button(
		TextLabel textLabel,
		UiQuadRenderer quadRenderer,
		Transform2d? transform = null)
		: base(transform)
	{
		this.textLabel = textLabel;
		this.quadRenderer = quadRenderer;
		Padding = new Offset(10f);
	}

	public override Box2D<float> Aabb
	{
		get
		{
			var textAabb = textLabel.Aabb;
			var size = Vector2D.Abs(textAabb.Max - textAabb.Min);
			size = Vector2D.Max(size, Size);

			var pos = Transform.Position;

			var min = pos;
			var max = new Vector2D<float>()
			{
				X = pos.X + size.X + Padding.Left + Padding.Right,
				Y = pos.Y - size.Y - (Padding.Top + Padding.Bottom)
			};

			return new Box2D<float>(
				Vector2D.Min(min, max),
				Vector2D.Max(min, max));
		}
	}

	internal override void Draw()
	{
		var aabb = Aabb;
		var size = Vector2D.Abs(aabb.Max - aabb.Min);
		quadRenderer.Push(new QuadRenderComponent(
				Transform.Position,
				size,
				Color));

		var paddedPosition = Transform.Position with { X = Transform.Position.X + Padding.Left };
		paddedPosition.Y = Transform.Position.Y - Padding.Top;

		textLabel.Transform.Position = paddedPosition;
		textLabel.Draw();
	}
}
