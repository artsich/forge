using Forge.Graphics;
using Forge.Renderer.Components;
using Forge.Renderer.Layouts;
using Forge.Renderer.VertexAssebmlers;

namespace Forge.Renderer;

public class SimpleRenderer : IDisposable
{
	private readonly BatchQuadRenderer<CircleVertexData, CircleRenderComponent> circleRenderer;

	public SimpleRenderer(GraphicsDevice graphicsDevice)
	{
		circleRenderer = new BatchQuadRenderer<CircleVertexData, CircleRenderComponent>(
			graphicsDevice,
			new CircleVertexLayout(), 
			new CircleVertexAssembler());
	}

	public void AddCircle(CircleRenderComponent circle)
	{
		circleRenderer.GetBatch().Add(ref circle);
	}

	public RenderBatch<CircleVertexData, CircleRenderComponent> StartDrawCircles()
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
