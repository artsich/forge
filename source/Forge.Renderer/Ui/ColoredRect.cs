using Silk.NET.Input;
using Silk.NET.Maths;

namespace Forge.Renderer.Ui
{
	public class ColoredRect : UiElement
	{
		private readonly UiQuadRenderer quadRenderer;

		public Vector2D<float> Size { get; set; }

		public Vector4D<float> Color { get; set; }

		public override Box2D<float> Aabb 
		{
			get
			{
				var min = Transform.Position;
				var max = Transform.Position + Size with { Y = -Size.Y };

				return new(
					Vector2D.Min(min, max),
					Vector2D.Max(min, max));
			}
		}

		public ColoredRect()
		{
			quadRenderer = UiRenderContext.Instance.QuadRenderer;
		}

		internal override void Draw()
		{
			quadRenderer.Push(
				new Components.QuadRenderComponent(Transform.Position, Size, Color));
		}
	}
}
