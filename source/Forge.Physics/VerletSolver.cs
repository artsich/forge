using Silk.NET.Maths;

namespace Forge.Physics;

public class VerletSolver
{
	private readonly Vector2D<float> gravity = new(0.0f, -981f);

	private readonly IList<VerletCircleObject> objects;
	private readonly IList<Link> links;

	public VerletSolver(IList<VerletCircleObject> objects, IList<Link> links)
	{
		this.objects = objects;
		this.links = links;
	}

	public void Update(float dt)
	{
		var steps = 8;
		var dtStep = dt / steps;

		for (int i = 0; i < steps; i++)
		{
			ApplyGravity();
			ApplyConstraints();
			SolveCollisions();
			UpdatePositions(dtStep);
		}
	}

	private void UpdatePositions(float dt)
	{
		foreach (VerletCircleObject obj in objects)
		{
			obj.UpdatePosition(dt);
		}
	}

	private void ApplyGravity()
	{
		foreach (var obj in objects)
		{
			obj.ApplyForce(gravity);
		}
	}

	private void ApplyConstraints()
	{
		var pos = Vector2D<float>.Zero;
		var radius = 500;
		var myradius = 10f;

		foreach (var obj in objects)
		{
			var to_obj = obj.PositionCurrent - pos;
			var distance = to_obj.Length;

			var r_sum = myradius + radius;
			if (r_sum < distance)
			{
				var n = to_obj / distance;
				obj.PositionCurrent += n * (r_sum - distance);
			}
		}

		foreach (var link in links)
		{
			link.Apply();
		}
	}

	private void SolveCollisions()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			var object_1 = objects[i];
			for (int j = i + 1; j < objects.Count; j++)
			{
				var object_2 = objects[j];

				TryResolve(object_1, object_2);
			}
		}
	}

	private static void TryResolve(VerletCircleObject object_1, VerletCircleObject object_2)
	{
		var collisionAxis = object_1.PositionCurrent - object_2.PositionCurrent;
		var distance = collisionAxis.Length;
		var r_sum = object_1.Radius + object_2.Radius;

		if (distance < r_sum)
		{
			var n = collisionAxis / distance;
			float delta = r_sum - distance;

			object_1.PositionCurrent += n * delta * 0.5f;
			object_2.PositionCurrent -= n * delta * 0.5f;
		}
	}
}
