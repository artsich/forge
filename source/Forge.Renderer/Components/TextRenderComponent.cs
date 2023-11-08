using Silk.NET.Maths;

namespace Forge.Renderer.Components;

public readonly struct TextRenderComponent
{
	// todo: I think this should be an array of chars
	public readonly string String;
	public readonly float Scale;
	public readonly Vector2D<float> Position;
	public readonly Vector4D<float> Color;


	public TextRenderComponent(string str, Vector2D<float> position, float scale, Vector4D<float>? color = null)
	{
		Position = position;
		Scale = Math.Max(1f, scale);
		String = str;
		Color = color ?? Vector4D<float>.One;
	}

	public TextRenderComponent()
	{
		Scale = 1f;
		String = string.Empty;
		Position = Vector2D<float>.Zero;
		Color = Vector4D<float>.One;
	}
}
