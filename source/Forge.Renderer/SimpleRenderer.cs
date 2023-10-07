using Forge.Graphics;
using Forge.Renderer.Components;
using Forge.Renderer.Layouts;
using Forge.Renderer.VertexAssebmlers;

namespace Forge.Renderer;

public class SimpleRenderer : IDisposable
{
	private readonly BatchRenderer<CircleVertexData, CircleRenderComponent> circleRenderer;

	public SimpleRenderer(GraphicsDevice graphicsDevice)
	{
		circleRenderer = new BatchRenderer<CircleVertexData, CircleRenderComponent>(
			graphicsDevice,
			new CircleVertexLayout(), 
			new CircleBufferAssembler(segmentsCount: 12),
			new BatchRendererDescription(5000));
	}
	
	// todo: can be moved to extensions?
	// todo: looks not good solution, think about refactoring!
	public void AddCircle(CircleRenderComponent circle)
	{
		circleRenderer.GetBatch().Add(ref circle);
	}

	public Batch<CircleVertexData, CircleRenderComponent> StartDrawCircles()
	{
		return circleRenderer.GetBatch();
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
