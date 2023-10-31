using Forge.Renderer.Vertices;
using Silk.NET.OpenGL;

namespace Forge.Renderer.Vertices.Layouts
{
    public sealed class FontQuadLayout : IVertexLayout
    {
        public void Enable(GL gl)
        {
            GlyphVertex.Enable(gl);
        }
    }
}
