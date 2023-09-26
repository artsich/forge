using Forge.Graphics.Shaders;

namespace Forge;

[UniformPrefix("time")]
public readonly struct GameTime
{
	internal GameTime(float totalTime, float deltaTime)
	{
		TotalTime = totalTime;
		DeltaTime = deltaTime;
	}

	[Uniform("Total")]
	public readonly float TotalTime;

	[Uniform("Delta")]
	public readonly float DeltaTime;
}
