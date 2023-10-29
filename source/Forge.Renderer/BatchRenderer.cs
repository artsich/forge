using Forge.Graphics;
using Forge.Graphics.Buffers;
using Forge.Renderer.Layouts;
using Forge.Renderer.VertexAssebmlers;
using Silk.NET.OpenGL;

namespace Forge.Renderer;

public readonly struct BatchRendererDescription
{
	public readonly int EntitiesCount;

	public BatchRendererDescription(int entitiesCount)
	{
		EntitiesCount = entitiesCount;
	}
}

public class BatchRenderer<TVertex, TRenderComponent> : GraphicsResourceBase
	where TVertex : unmanaged
{
	private readonly Batch<TVertex, TRenderComponent> renderBatch;

	private VertexArrayBuffer? Vao;
	private Buffer<TVertex>? Vbo;
	private Buffer<uint>? Ebo;

	public BatchRenderer(
		IVertexLayout vertexLayout,
		IGeometryBufferAssembler<TVertex, TRenderComponent> geometryAssembler,
		BatchRendererDescription description)
		: base(GraphicsDevice.Current)
	{
		renderBatch = new Batch<TVertex, TRenderComponent>(
			geometryAssembler,
			description.EntitiesCount);

		renderBatch.OnFull += Flush;

		InitBuffers(vertexLayout,
			description.EntitiesCount * geometryAssembler.VerticesRequired,
			geometryAssembler.GetIndices(description.EntitiesCount * (int)geometryAssembler.IndicesRequired));
	}

	private void InitBuffers(IVertexLayout vertexLayout, int maxVerticexPerBatch, uint[] indices)
	{
		Vao = new VertexArrayBuffer(Gd);
		Vao.Bind();

		Vbo = Graphics.Buffers.Buffer.Vertex.New<TVertex>(Gd, maxVerticexPerBatch, BufferUsageARB.DynamicDraw);
		Vbo.Bind();

		Ebo = Graphics.Buffers.Buffer.Index.New(Gd, indices);
		Ebo.Bind();

		vertexLayout.Enable(Gd.gl);
	}

	public Batch<TVertex, TRenderComponent> GetBatch()
	{
		return renderBatch;
	}

	public unsafe void Flush()
	{
		var vertices = renderBatch.GetUsedVertices();

		if (vertices.Length > 0)
		{
			Vbo!.Bind();
			Vbo!.SetData(vertices);
			Vao!.Bind();

			Gd.gl.DrawElements(PrimitiveType.Triangles, renderBatch.IndicesUsed, DrawElementsType.UnsignedInt, null);

			renderBatch.Reset();
		}
	}

	protected override void OnDestroy()
	{
		renderBatch.OnFull -= Flush;
		Ebo?.Dispose();
		Vbo?.Dispose();
		Vao?.Dispose();
	}
}
