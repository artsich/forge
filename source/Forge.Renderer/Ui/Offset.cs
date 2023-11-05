using Silk.NET.Maths;

namespace Forge.Renderer.Ui;

public record struct Offset(float Left = 0f, float Top = 0f, float Right = 0f, float Bottom = 0f)
{
	public Offset(float val)
		: this(val, val, val, val)
	{
	}

	public readonly Vector2D<float> LeftTop => new(Left, Top);

	public readonly Vector2D<float> LeftBottom => new(Left, Bottom);

	public readonly Vector2D<float> RightTop => new(Right, Top);
}
