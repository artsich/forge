﻿using Silk.NET.Maths;

namespace Forge.Renderer.Components;

public struct CircleRenderComponent
{
	public Vector4D<float> Color;
	public Vector2D<float> Position;
	public float Radius;
	public float Fade;

	public CircleRenderComponent(Vector4D<float> color, Vector2D<float> position, float radius, float fade)
	{
		Color = color;
		Position = position;
		Radius = radius;
		Fade = fade;
	}
}
