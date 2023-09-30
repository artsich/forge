using Forge.Graphics.Shaders;
using Silk.NET.Maths;

namespace Forge;

[UniformPrefix("camera")]
public class Camera
{
	[Uniform("Proj")]
	public Matrix4X4<float> Projection { get; set; }

	[Uniform("View")]
	public Matrix4X4<float> View { get; set; }

	[Uniform("ViewProj")]
	public Matrix4X4<float> ViewProjection => View * Projection;

	public Camera(Matrix4X4<float> projection) 
		: this(projection, Matrix4X4<float>.Identity)
	{
	}

	public Camera(Matrix4X4<float> projection, Matrix4X4<float> view)
	{
		Projection = projection;
		View = view;
	}
}
