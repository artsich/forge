using Forge.Graphics;
using Forge.Renderer.Components;
using Forge.Renderer.Vertices.Layouts;
using Forge.Renderer.VertexAssebmlers;
using Forge.Renderer.Vertices;

namespace Forge.Renderer;

public class Renderer2D : IDisposable
{
	private readonly BatchRenderer<CircleVertex, CircleRenderComponent> circleRenderer;

	public Renderer2D(GraphicsDevice graphicsDevice)
	{
		circleRenderer = new BatchRenderer<CircleVertex, CircleRenderComponent>(
			new CircleVertexLayout(),
			new CircleAssembler(segmentsCount: 12),
			new BatchRendererDescription(5000));
	}

	// todo: can be moved to extensions?
	// todo: looks not good solution, think about refactoring!
	public void DrawCircle(CircleRenderComponent circle)
	{
		circleRenderer.Push(ref circle);
	}

	public void FlushAll()
	{
		circleRenderer.Flush();
	}

	public void Dispose()
	{
		circleRenderer?.Dispose();
	}
}
