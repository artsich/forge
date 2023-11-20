using Forge.Graphics;
using Silk.NET.Maths;

namespace Forge.Renderer.Components;

public struct QuadRenderComponent
{
	public float Rotation;
	public uint TextureId;
	public Vector2D<float> Position;
	public Vector2D<float> Size;
	public Vector4D<float> Color;
	public Rectangle<float> UV;

	public readonly Matrix4X4<float> Model =>
		Matrix4X4.CreateScale(new Vector3D<float>(Size, 1)) *
		Matrix4X4.CreateRotationZ(Rotation) *
		Matrix4X4.CreateTranslation(new Vector3D<float>(Position, 0));

	public QuadRenderComponent(
		Vector2D<float> position,
		Vector2D<float> size,
		Vector4D<float> color)
	{
		Position = position;
		Size = size;
		Color = color;
	}

	public QuadRenderComponent(
		Vector2D<float> position,
		Vector2D<float> size,
		Vector4D<float> color,
		Rectangle<float> uv)
		: this(position, size, color)
	{
		UV = uv;
	}
}
