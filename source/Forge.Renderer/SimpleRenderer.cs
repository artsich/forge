using Forge.Renderer.Components;
using Forge.Renderer.Vertices.Layouts;
using Forge.Renderer.VertexAssebmlers;
using Forge.Renderer.Vertices;

namespace Forge.Renderer;

public class CircleRenderer : BatchRenderer<CircleVertex, CircleRenderComponent>
{
	public CircleRenderer() 
		: base(
			new CircleVertexLayout(), 
			new CircleAssembler(segmentsCount: 12),
			new BatchRendererDescription(5000))
	{
	}
}

public class QuadRenderer : BatchRenderer<QuadVertex, QuadRenderComponent>
{
	public QuadRenderer()
		: base(
			new QuadVertexLayout(),
			new QuadAssembler(),
			new BatchRendererDescription(5000))
	{
	}
}
