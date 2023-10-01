using Silk.NET.OpenGL;

namespace Forge.Renderer.Layouts;

public sealed class CircleVertexLayout : IVertexLayout
{
    public void Enable(GL gl)
    {
        CircleVertexData.EnableVertexPointer(gl);
    }
}
