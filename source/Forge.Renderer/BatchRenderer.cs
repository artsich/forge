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
	private int verticesUsed;
	private int indicesUsed;
	private readonly IGeometryAssembler<TVertex, TRenderComponent> vertexAssembler;
	private readonly TVertex[] vertices;

	private VertexArrayBuffer? Vao;
	private Buffer<TVertex>? Vbo;
	private Buffer<uint>? Ebo;

	public BatchRenderer(
		IVertexLayout vertexLayout,
		IGeometryAssembler<TVertex, TRenderComponent> geometryAssembler,
		BatchRendererDescription description)
		: base(GraphicsDevice.Current)
	{
		vertexAssembler = geometryAssembler;
		vertices = new TVertex[description.EntitiesCount * geometryAssembler.VerticesRequired];

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

	public void Push(ref TRenderComponent renderComponent)
	{
		if (verticesUsed + vertexAssembler.VerticesRequired == vertices.Length)
		{
			Flush();
		}

		vertexAssembler.Assemble(GetVerticesToPack(), ref renderComponent);
		verticesUsed += vertexAssembler.VerticesRequired;
		indicesUsed += vertexAssembler.IndicesRequired;
	}

	public unsafe void Flush()
	{
		var vertices = GetUsedVertices();

		if (vertices.Length > 0)
		{
			Vbo!.Bind();
			Vbo!.SetData(vertices);
			Vao!.Bind();

			Gd.gl.DrawElements(PrimitiveType.Triangles, (uint)indicesUsed, DrawElementsType.UnsignedInt, null);

			Reset();
		}
	}

	protected override void OnDestroy()
	{
		Ebo?.Dispose();
		Vbo?.Dispose();
		Vao?.Dispose();
	}

	private Span<TVertex> GetVerticesToPack()
	{
		var start = verticesUsed;
		var end = start + vertexAssembler.VerticesRequired;
		return vertices.AsSpan()[start..end];
	}

	private Span<TVertex> GetUsedVertices() => vertices.AsSpan()[0..verticesUsed];

	private void Reset()
	{
		verticesUsed = 0;
		indicesUsed = 0;
	}
}
