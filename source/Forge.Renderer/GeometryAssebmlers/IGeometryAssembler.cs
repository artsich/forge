namespace Forge.Renderer.VertexAssebmlers;

public interface IGeometryAssembler<TVertex, TRenderComponent>
	where TVertex : unmanaged
{
	int VerticesRequired { get; }

	int IndicesRequired { get; }

	void Assemble(Span<TVertex> vertices, ref TRenderComponent circle);

	uint[] GetIndices(int count);
}
