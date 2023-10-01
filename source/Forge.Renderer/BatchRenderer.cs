using Forge.Graphics;
using Forge.Graphics.Buffers;
using Forge.Renderer.Layouts;
using Forge.Renderer.Utils;
using Forge.Renderer.VertexAssebmlers;
using Silk.NET.OpenGL;

namespace Forge.Renderer;

internal static class BatchConstants
{
	public const int VerticesPerQuad = 4;
	public const int IndicesPerQuad = 6;
}

public class BatchQuadRenderer<TVertex, TRenderComponent> : IDisposable
	where TVertex : unmanaged
{
	private const int MaxQuadsPerBatch = 1000;
	private const int MaxVerticesPerBatch = BatchConstants.VerticesPerQuad * MaxQuadsPerBatch;
	private const int MaxIndicesPerBatch = BatchConstants.IndicesPerQuad * MaxQuadsPerBatch;

	private readonly GraphicsDevice gd;
	private readonly RenderBatch<TVertex, TRenderComponent> renderBatch;

	private VertexArrayBuffer? Vao;
	private Buffer<TVertex>? Vbo;
	private Buffer<uint>? Ebo;

	private bool disposed;

	public BatchQuadRenderer(
		GraphicsDevice gd,
		IVertexLayout vertexLayout,
		IVertexAssembler<TVertex, TRenderComponent> vertexAssembler)
    {
		renderBatch = new RenderBatch<TVertex, TRenderComponent>(
			vertexAssembler, 
			new RenderBatchDescription(MaxVerticesPerBatch));

		renderBatch.OnFull += Flush;

		this.gd = gd;

		InitBuffers(vertexLayout);
	}

	private void InitBuffers(IVertexLayout vertexLayout)
	{
		Vao = new VertexArrayBuffer(gd);
		Vao.Bind();

		Vbo = Graphics.Buffers.Buffer.Vertex.New<TVertex>(gd, MaxVerticesPerBatch, BufferUsageARB.DynamicDraw);
		Vbo.Bind();

		// todo: it is the same for all batches, so we can just reuse it
		Ebo = Graphics.Buffers.Buffer.Index.New(gd, ModelUtils.GenerateQuadIndices(MaxIndicesPerBatch));
		Ebo.Bind();

		vertexLayout.Enable(gd.gl);
	}

	public RenderBatch<TVertex, TRenderComponent> GetBatch()
	{
		return renderBatch;
	}

	public unsafe void Flush()
	{
		var vertices = renderBatch.GetUsedVertices();

		if (vertices.Length > 0)
		{
			uint indicesUsed = (uint)vertices.Length / BatchConstants.VerticesPerQuad * BatchConstants.IndicesPerQuad;

			Vbo!.SetData(vertices);
			Vao!.Bind();
			gd.gl.DrawElements(PrimitiveType.Triangles, indicesUsed, DrawElementsType.UnsignedInt, null);

			renderBatch.Reset();
		}
	}

	public void Dispose()
	{
		if (disposed)
			return;

		renderBatch.OnFull -= Flush;
		Ebo?.Dispose();
		Vbo?.Dispose();
		Vao?.Dispose();

		disposed = true;
	}
}
