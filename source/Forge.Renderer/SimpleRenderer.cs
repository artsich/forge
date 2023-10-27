using Forge.Graphics;
using Forge.Graphics.Shaders;
using Forge.Renderer.Components;
using Forge.Renderer.Font;
using Forge.Renderer.Layouts;
using Forge.Renderer.VertexAssebmlers;
using Forge.Renderer.Vertices;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Forge.Renderer;

public class Renderer2D : IDisposable
{
	private readonly BatchRenderer<CircleVertex, CircleRenderComponent> circleRenderer;

	public Renderer2D(GraphicsDevice graphicsDevice)
	{
		circleRenderer = new BatchRenderer<CircleVertex, CircleRenderComponent>(
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

	public Batch<CircleVertex, CircleRenderComponent> StartDrawCircles()
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
