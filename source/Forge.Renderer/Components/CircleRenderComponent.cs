using Silk.NET.Maths;

namespace Forge.Renderer.Components;

public struct QuadRenderComponent
{
	public float Rotation;
	public Vector2D<float> Position;
	public Vector2D<float> Size;
	public Vector4D<float> Color;

	public Matrix4X4<float> Model => Matrix4X4.CreateTranslation(new Vector3D<float>(Position, 0)) *
									 Matrix4X4.CreateRotationZ(Rotation) *
									 Matrix4X4.CreateScale(new Vector3D<float>(Size, 1));

	public QuadRenderComponent(Vector2D<float> position, Vector2D<float> size, Vector4D<float> color)
	{
		Position = position;
		Size = size;
		Color = color;
	}
}

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
