using Forge.Renderer.Components;
using Forge.Renderer.Utils;
using Silk.NET.Maths;

namespace Forge.Renderer.VertexAssebmlers;

public class CircleBufferAssembler : IGeometryBufferAssembler<CircleVertexData, CircleRenderComponent>
{
	private readonly int segmentsCount;

	public int VerticesRequired { get; }

	public int IndicesRequired { get; }

	public CircleBufferAssembler(int segmentsCount)
	{
		this.segmentsCount = segmentsCount;
		VerticesRequired = segmentsCount + 1;
		IndicesRequired = segmentsCount * 3;
	}

	// todo: should be moved to extensions or smth else?
	public uint[] GetIndices(int count)
	{
		return IndicesGenerator.GenerateCircleIndices(count, segmentsCount);
	}

	public void Assemble(Span<CircleVertexData> vertices, ref CircleRenderComponent circle)
	{
		if (vertices.Length < VerticesRequired)
			throw new ArgumentException($"Span must have at least {VerticesRequired} elements", nameof(vertices));

		vertices[0] = new CircleVertexData(
			new Vector3D<float>(circle.Position.X, circle.Position.Y, 1.0f),
			new Vector2D<float>(0.5f),
			circle.Color,
			circle.Fade);

		float angleIncrement = MathF.PI * 2.0f / segmentsCount;

		for (int i = 1; i <= segmentsCount; i++)
		{
			var theta = i * angleIncrement;
			var cos_theta = MathF.Cos(theta);
			var sin_theta = MathF.Sin(theta);

			var position = new Vector3D<float>(
				circle.Position.X + circle.Radius * sin_theta,
				circle.Position.Y + circle.Radius * cos_theta,
				1.0f);

			var texCoord = new Vector2D<float>(
				0.5f + 0.5f * cos_theta,
				0.5f + 0.5f * sin_theta);

			vertices[i] = new CircleVertexData(position, texCoord, circle.Color, circle.Fade);
		}
	}
}
