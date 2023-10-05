namespace Forge.Physics;

public class Link
{
	public VervletCircleObject Object1 { get; }
	public VervletCircleObject Object2 { get; }

	private float TargetDistance { get; }

    public Link(VervletCircleObject o1, VervletCircleObject o2, float targetDistance)
    {
		Object1 = o1;
		Object2 = o2;
		TargetDistance = targetDistance;
	}

	public void Apply()
	{
		var collisionAxis = Object1.PositionCurrent - Object2.PositionCurrent;
		var distance = collisionAxis.Length;

		var n = collisionAxis / distance;
		float delta = TargetDistance - distance;

		Object1.PositionCurrent += n * delta * 0.5f;
		Object2.PositionCurrent -= n * delta * 0.5f;
	}
}
