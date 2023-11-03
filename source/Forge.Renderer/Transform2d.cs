using Silk.NET.Maths;

namespace Forge.Renderer;

public class Transform2d
{
	public float Rotation { get; set; }

	public Vector2D<float> Position { get; set; }

	public Vector2D<float> Scale { get; set; }

	public Matrix4X4<float> Model => Matrix4X4.CreateScale(Scale.X, Scale.Y, 1) *
									 Matrix4X4.CreateRotationZ(Rotation) *
									 Matrix4X4.CreateTranslation(Position.X, Position.Y, 0);

	public Transform2d()
	{
	}

	public Transform2d(Vector2D<float> position)
		: this(position, Vector2D<float>.One, 0)
	{
	}

	public Transform2d(Vector2D<float> position, Vector2D<float> scale, float rotation)
	{
		Scale = scale;
		Rotation = rotation;
		Position = position;
	}
}
