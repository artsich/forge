using Forge.Renderer.VertexAssebmlers;

namespace Forge.Renderer;

public readonly struct RenderBatchDescription
{
	public readonly int MaxVerticesPerBatch;

	public RenderBatchDescription(int maxVerticesPerBatch)
	{
		MaxVerticesPerBatch = maxVerticesPerBatch;
	}
}

public class Batch<TVertex, TRenderComponent>
	where TVertex : unmanaged
{
	private readonly IVertexAssembler<TVertex, TRenderComponent> vertexAssembler;
	private readonly TVertex[] vertices;
	private int verticesUsed;

	public event Action? OnFull;

	public Batch(
		IVertexAssembler<TVertex, TRenderComponent> vertexAssembler,
		RenderBatchDescription description)
    {
		this.vertexAssembler = vertexAssembler;
		this.vertices = new TVertex[description.MaxVerticesPerBatch];
	}

	public void Add(ref TRenderComponent renderComponent)
	{
		if (verticesUsed + vertexAssembler.VerticesRequired == vertices.Length) 
		{
			Flush();
		}

		vertexAssembler.Pack(GetVerticesToPack(), ref renderComponent);
		verticesUsed += vertexAssembler.VerticesRequired;
		IndicesUsed += vertexAssembler.IndicesRequired;
	}

	private void Flush()
	{
		OnFull?.Invoke();
		Reset();
	}

	public void Reset()
	{
		verticesUsed = 0;
	}

	public Span<TVertex> GetUsedVertices() => vertices.AsSpan()[0..verticesUsed];

	public uint IndicesUsed { get; private set; }

	private Span<TVertex> GetVerticesToPack()
	{
		var start = verticesUsed;
		var end = start + vertexAssembler.VerticesRequired;
		return vertices.AsSpan()[start..end];
	}
}
