﻿using Forge.Renderer.VertexAssebmlers;

namespace Forge.Renderer;

public class Batch<TVertex, TRenderComponent>
	where TVertex : unmanaged
{
	private readonly IGeometryBufferAssembler<TVertex, TRenderComponent> vertexAssembler;
	private readonly TVertex[] vertices;
	private int verticesUsed;

	public event Action? OnFull;

	public Batch(
		IGeometryBufferAssembler<TVertex, TRenderComponent> vertexAssembler,
		int entitiesPerBatch)
    {
		this.vertexAssembler = vertexAssembler;
		this.vertices = new TVertex[entitiesPerBatch * vertexAssembler.VerticesRequired];
	}

	public void Add(ref TRenderComponent renderComponent)
	{
		if (verticesUsed + vertexAssembler.VerticesRequired == vertices.Length) 
		{
			Flush();
		}

		vertexAssembler.Assemble(GetVerticesToPack(), ref renderComponent);
		verticesUsed += vertexAssembler.VerticesRequired;
		IndicesUsed += (uint)vertexAssembler.IndicesRequired;
	}

	private void Flush()
	{
		OnFull?.Invoke();
		Reset();
	}

	public void Reset()
	{
		verticesUsed = 0;
		IndicesUsed = 0;
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