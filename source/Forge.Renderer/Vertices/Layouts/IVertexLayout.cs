using Silk.NET.OpenGL;

namespace Forge.Renderer.Vertices.Layouts;

// todo: Replace with auto layout
public interface IVertexLayout
{
    void Enable(GL gl);
}

public class QuadVertexLayout : IVertexLayout
{
	public void Enable(GL gl)
	{
		QuadVertex.EnableVertexPointer(gl);
	}
}