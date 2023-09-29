using Forge.Graphics;
using Forge.Graphics.Buffers;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Forge;

public class BatchRenderer : IDisposable
{
	private const int MaxQuadsPerBatch = 1000;
	private const int MaxVerticesPerBatch = 4 * MaxQuadsPerBatch;
	private const int MaxIndices = 6 * MaxQuadsPerBatch;
	private readonly CircleVertexData[] vertices = new CircleVertexData[MaxVerticesPerBatch];

	private uint indicesToDraw;
	private uint verticesCount;

	private VertexArrayBuffer Vao;
	private Buffer<CircleVertexData> Vbo;
	private Buffer<uint> Ebo;

	private readonly GraphicsDevice gd;

	public BatchRenderer(GraphicsDevice gd)
	{
		this.gd = gd;
		InitializeRenderer(gd);
	}

	private void InitializeRenderer(GraphicsDevice gd)
	{
		Vao = new VertexArrayBuffer(gd);
		Vao.Bind();

		Vbo = Graphics.Buffers.Buffer.Vertex.New<CircleVertexData>(gd, MaxVerticesPerBatch, BufferUsageARB.DynamicDraw);
		Vbo.Bind();

		Ebo = Graphics.Buffers.Buffer.Index.New(gd, GenerateIndices(MaxQuadsPerBatch * 6));
		Ebo.Bind();

		CircleVertexData.EnableVertexPointer(gd.gl);
	}

	public void AddCircle(CircleRenderComponent circle)
	{
		if (verticesCount + 4 > MaxVerticesPerBatch)
			EndBatch();

		AddCircleVertices(ref circle);
	}

	public void EndBatch()
	{
		Vbo.SetData(vertices, (int)verticesCount);
		Render();

		indicesToDraw = 0;
		verticesCount = 0;
	}

	private unsafe void Render()
	{
		Vao.Bind();

		gd.gl.DrawElements(PrimitiveType.Triangles, indicesToDraw, DrawElementsType.UnsignedInt, null);
	}

	private void AddCircleVertices(ref CircleRenderComponent circle)
	{
		var pos = circle.Position;
		var radius = circle.Radius;
		var col = circle.Color;
		var fade = circle.Fade;

		var offsets = new (float r1, float r2, float t1, float t2)[]
		{
			(-radius, +radius, 0, 1),
			(+radius, +radius, 1, 1),
			(+radius, -radius, 1, 0),
			(-radius, -radius, 0, 0)
		};

		for (int i = 0; i < 4; i++)
		{
			vertices[verticesCount + i] = new CircleVertexData(
				new Vector3D<float>(pos.X + offsets[i].r1, pos.Y + offsets[i].r2, 0f),
				new Vector2D<float>(offsets[i].t1, offsets[i].t2),
				col,
				fade);
		}

		verticesCount += 4;
		indicesToDraw += 6;
	}

	private static uint[] GenerateIndices(int count)
	{
		var quadIndices = new uint[MaxIndices];

		var offset = 0u;
		for (uint i = 0; i < count; i += 6)
		{
			quadIndices[i + 0] = offset + 0;
			quadIndices[i + 1] = offset + 1;
			quadIndices[i + 2] = offset + 2;

			quadIndices[i + 3] = offset + 2;
			quadIndices[i + 4] = offset + 3;
			quadIndices[i + 5] = offset + 0;

			offset += 4;
		}

		return quadIndices;
	}

	public void Dispose()
	{
		Vbo.Dispose();
		Ebo.Dispose();
		Vao.Dispose();
	}
}
