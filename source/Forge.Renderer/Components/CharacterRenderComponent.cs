using Silk.NET.Maths;

namespace Forge.Renderer.Components;

public struct CharacterRenderComponent
{
	public Vector2D<float> Position;
	public Vector2D<float> Size;
	public Rectangle<float> UV;
	public Vector4D<float> Color;

	public readonly Matrix4X4<float> Model =>
		Matrix4X4.CreateScale(Size.X, Size.Y, 1f)
		* Matrix4X4.CreateTranslation(Position.X, Position.Y, 0f);
}
