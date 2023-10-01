using Forge.Renderer.Components;
using Silk.NET.Maths;

namespace Forge.Renderer.VertexAssebmlers;

public class CircleVertexAssembler : IVertexAssembler<CircleVertexData, CircleRenderComponent>
{
    public int VerticesRequired => 4;

    public unsafe void Pack(Span<CircleVertexData> vertices, ref CircleRenderComponent circle)
    {
        if (vertices.Length < VerticesRequired)
            throw new ArgumentException("Span must have at least 4 elements", nameof(vertices));

        var (pos, radius, col, fade) = circle;

        var offsets = stackalloc (float r1, float r2, float t1, float t2)[]
        {
            (-radius, +radius, 0, 1),
            (+radius, +radius, 1, 1),
            (+radius, -radius, 1, 0),
            (-radius, -radius, 0, 0)
        };

        for (int i = 0; i < 4; i++)
        {
            vertices[i] = new CircleVertexData(
                new Vector3D<float>(pos.X + offsets[i].r1, pos.Y + offsets[i].r2, 0f),
                new Vector2D<float>(offsets[i].t1, offsets[i].t2),
                col,
                fade);
        }
    }
}
