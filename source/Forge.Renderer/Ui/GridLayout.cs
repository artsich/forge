using Silk.NET.Input;
using Silk.NET.Maths;
using System.Diagnostics;

namespace Forge.Renderer.Ui;

public sealed class GridLayout : UiElement
{
	private readonly Row[] rows;

	public record struct Row(Column[] Columns, Offset Padding = default, float Gap = 0f)
	{
		public Row(Column column, Offset padding = default, float gap = 0f)
			: this(new[] { column }, padding, gap)
		{
		}

		public readonly Vector2D<float> Size
		{
			get
			{
				var size = Vector2D<float>.Zero;
				foreach (var column in Columns)
				{
					var elementSize = column.Element.Aabb.Size;
					size.X += elementSize.X + Gap;
					size.Y = Math.Max(size.Y, elementSize.Y);
				}

				if (Columns.Length > 0)
				{
					size.X -= Gap;
				}

				size.Y += Padding.Top + Padding.Bottom;
				return size;
			}
		}
	}

	public record struct Column(UiElement Element);

	public override Box2D<float> Aabb
	{
		get
		{
			var width = rows.Max(x => x.Size.X);
			var height = rows.Sum(x => x.Size.Y);

			var min = Transform.Position;
			var max = Transform.Position + new Vector2D<float>(width, -height);

			return new Box2D<float>(
				Vector2D.Min(min, max),
				Vector2D.Max(min, max));
		}
	}

	public GridLayout(params Row[] rows)
	{
		this.rows = rows;
	}

	internal override void Draw()
	{
		var startRowPosition = Transform.Position;
		foreach (var row in rows)
		{
			var columnPosition = startRowPosition + new Vector2D<float>(row.Padding.Left, -row.Padding.Top);
			foreach (var column in row.Columns)
			{
				column.Element.Transform.Position = columnPosition;
				column.Element.Draw();
				columnPosition.X += column.Element.Aabb.Size.X + row.Gap;
			}

			startRowPosition.Y -= row.Size.Y;
		}
	}

	internal override void OnMouseDown(Vector2D<float> pos, MouseButton button)
	{
		var processed = false;
		// todo: can be optimized by using aabb for the row
		var children = rows.SelectMany(row => row.Columns.Select(c => c.Element));
		foreach (var child in children)
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
