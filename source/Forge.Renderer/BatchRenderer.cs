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

public sealed class BatchRenderer<TVertex, TRenderComponent> : IDisposable
	where TVertex : unmanaged
{
	private readonly GraphicsDevice gd;
	private readonly Batch<TVertex, TRenderComponent> renderBatch;

	private VertexArrayBuffer? Vao;
	private Buffer<TVertex>? Vbo;
	private Buffer<uint>? Ebo;

	private bool disposed;

	public BatchRenderer(
		GraphicsDevice gd,
		IVertexLayout vertexLayout,
		IGeometryBufferAssembler<TVertex, TRenderComponent> geometryAssembler,
		BatchRendererDescription description)
	{
		renderBatch = new Batch<TVertex, TRenderComponent>(
			geometryAssembler,
			description.EntitiesCount);

		renderBatch.OnFull += Flush;

		this.gd = gd;

		InitBuffers(vertexLayout,
			description.EntitiesCount * geometryAssembler.VerticesRequired,
			geometryAssembler.GetIndices(description.EntitiesCount * (int)geometryAssembler.IndicesRequired));
	}

	private void InitBuffers(IVertexLayout vertexLayout, int maxVerticexPerBatch, uint[] indices)
	{
		Vao = new VertexArrayBuffer(gd);
		Vao.Bind();

		Vbo = Graphics.Buffers.Buffer.Vertex.New<TVertex>(gd, maxVerticexPerBatch, BufferUsageARB.DynamicDraw);
		Vbo.Bind();

		Ebo = Graphics.Buffers.Buffer.Index.New(gd, indices);
		Ebo.Bind();

		vertexLayout.Enable(gd.gl);
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
			gd.gl.DrawElements(PrimitiveType.Triangles, renderBatch.IndicesUsed, DrawElementsType.UnsignedInt, null);

			renderBatch.Reset();
		}
	}

	public void Dispose()
	{
		if (!disposed)
		{
			renderBatch.OnFull -= Flush;
			Ebo?.Dispose();
			Vbo?.Dispose();
			Vao?.Dispose();

			disposed = true;
		}
	}
}
