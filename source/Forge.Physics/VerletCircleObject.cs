using Silk.NET.Maths;

namespace Forge.Physics;

public class VerletCircleObject
{
	private Vector2D<float> positionCurrent;
	private Vector2D<float> positionOld;
	private Vector2D<float> acceleration;

	public Vector2D<float> PositionCurrent
	{
		get => positionCurrent;
		set
		{
			if (!IsStatic)
				positionCurrent = value;
		}
	}

	public float Radius { get; }
	public bool IsStatic = false;

	public VerletCircleObject(Vector2D<float> positionCurrent, float radius)
	{
		PositionCurrent = positionCurrent;
		positionOld = PositionCurrent;
		Radius = radius;
	}

	public void UpdatePosition(float dt)
	{
		Vector2D<float> velocity = PositionCurrent - positionOld;
		positionOld = PositionCurrent;
		PositionCurrent += velocity + acceleration * dt * dt;

		acceleration = Vector2D<float>.Zero;
	}

	public void ApplyForce(Vector2D<float> force)
	{
		acceleration += force;
	}
}
