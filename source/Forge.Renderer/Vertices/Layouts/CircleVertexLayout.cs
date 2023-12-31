﻿using Silk.NET.OpenGL;

namespace Forge.Renderer.Vertices.Layouts;

public sealed class CircleVertexLayout : IVertexLayout
{
	public void Enable(GL gl)
	{
		CircleVertex.EnableVertexPointer(gl);
	}
}
