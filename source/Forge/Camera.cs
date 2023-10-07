using Forge.Graphics.Shaders;
using Silk.NET.Input;
using Silk.NET.Maths;
using System.Numerics;

namespace Forge;

[UniformPrefix("camera")]
public class CameraData
{
	[Uniform("Proj")]
	public Matrix4X4<float> Projection { get; set; }

	[Uniform("View")]
	public Matrix4X4<float> View { get; set; }

	[Uniform("ViewProj")]
	public Matrix4X4<float> ViewProjection => View * Projection;

	public CameraData(Matrix4X4<float> projection) 
		: this(projection, Matrix4X4<float>.Identity)
	{
	}

	public CameraData(Matrix4X4<float> projection, Matrix4X4<float> view)
	{
		Projection = projection;
		View = view;
	}
}

public class Camera2DController
{
	private readonly IMouse mouse;

	private float currentZoom = 1.0f;
	private Vector2D<float> startMovePosition;

	public CameraData CameraData { get; private set; }

	public float Speed { get; init; } = 1.0f;

	public float ZoomSpeed { get; init; } = 0.1f;

	public Camera2DController(CameraData cameraData, IMouse mouse)
    {
		CameraData = cameraData;

		this.mouse = mouse;
		this.mouse.MouseDown += OnMouseBtnDown;
		this.mouse.MouseMove += OnMouseMove;
		this.mouse.Scroll += OnMouseScroll;
	}

	public void Update(GameTime gameTime)
	{
	}

	private void OnMouseScroll(IMouse mouse, ScrollWheel scroll)
	{
		float zoomAmount = 1 + ZoomSpeed * scroll.Y;
		currentZoom *= zoomAmount;
		var zoomMatrix = Matrix4X4.CreateScale(zoomAmount, zoomAmount, 1);
		CameraData.Projection *= zoomMatrix;
	}

	private void OnMouseMove(IMouse mouse, Vector2 vector)
	{
		if (mouse.IsButtonPressed(MouseButton.Middle))
		{
			var curMousePosition = new Vector2D<float>(mouse.Position.X, mouse.Position.Y);
			if (curMousePosition == startMovePosition)
			{
				return;
			}

			var dir = Vector2D.Normalize(curMousePosition - startMovePosition) * new Vector2D<float>(1, -1);
			var actualSpeed = Speed / currentZoom;
			ApplyTranslation(dir * actualSpeed * ForgeGame.Time.DeltaTime);
			startMovePosition = curMousePosition;
		}
	}

	private void OnMouseBtnDown(IMouse mouse, MouseButton button)
	{
		if (button == MouseButton.Middle)
		{
			startMovePosition = new (mouse.Position.X, mouse.Position.Y);
		}
	}

	private void ApplyTranslation(Vector2D<float> translation)
	{
		var translationMatrix = Matrix4X4.CreateTranslation(translation.X, translation.Y, 0);
		CameraData.View *= translationMatrix;
	}
}
