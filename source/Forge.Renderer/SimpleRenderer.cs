using Forge.Renderer.Components;
using Forge.Renderer.Vertices.Layouts;
using Forge.Renderer.VertexAssebmlers;
using Forge.Renderer.Vertices;
using Forge.Graphics.Shaders;
using Silk.NET.OpenGL;
using Silk.NET.Maths;

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

public class ButtonRenderer : BatchRenderer<QuadVertex, QuadRenderComponent>
{
	private readonly CompiledShader shader;

	public ButtonRenderer(CompiledShader shader)
		: base(
			new QuadVertexLayout(),
			new QuadAssembler(),
			new BatchRendererDescription(5000))
	{
		this.shader = shader;
	}

	public void Push(QuadRenderComponent quad)
	{
		Push(ref quad);
	}

	public void Flush(CameraData cameraData)
	{
		shader.Bind(cameraData);

		Gd.gl.Enable(GLEnum.Blend);
		Gd.gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

		Flush();

		Gd.gl.Disable(GLEnum.Blend);
	}
}