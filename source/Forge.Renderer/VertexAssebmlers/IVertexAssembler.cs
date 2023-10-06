namespace Forge.Renderer.VertexAssebmlers;

public interface IVertexAssembler<TVertex, TRenderComponent>
    where TVertex : unmanaged
{
    int VerticesRequired { get; }

    uint IndicesRequired { get; }

    void Pack(Span<TVertex> vertices, ref TRenderComponent circle);
}
