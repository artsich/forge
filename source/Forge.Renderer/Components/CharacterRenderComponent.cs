using Silk.NET.Maths;

namespace Forge.Renderer.Components;

public struct CharacterRenderComponent
{
	public Vector2D<float> Position;
	public Vector2D<float> Size;
	public Rectangle<float> UV;
	public Vector4D<float> Color;
}
