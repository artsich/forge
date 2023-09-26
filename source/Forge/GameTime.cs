namespace Forge;

public readonly struct GameTime
{
	internal GameTime(float totalTime, float deltaTime)
	{
		TotalTime = totalTime;
		DeltaTime = deltaTime;
	}

	public readonly float TotalTime;

	public readonly float DeltaTime;
}
