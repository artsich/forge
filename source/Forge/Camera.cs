using Forge.Graphics.Shaders;
using Silk.NET.Input;
using Silk.NET.Maths;
using System;
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

public interface IMoveDir
{
	Vector2D<float> GetDir();
}

public class CameraMoveDir : IMoveDir
{
	private readonly IKeyboard keyboard;

	public CameraMoveDir(IKeyboard keyboard)
    {
		this.keyboard = keyboard;
	}

    public Vector2D<float> GetDir()
	{
		Vector2D<float> dir = Vector2D<float>.Zero;
		if (keyboard.IsKeyPressed(Key.W))
		{
			dir.Y = -1;
		}
		else if (keyboard.IsKeyPressed(Key.S))
		{
			dir.Y = 1;
		}

		if (keyboard.IsKeyPressed(Key.A))
		{
			dir.X = -1;
		}
		else if (keyboard.IsKeyPressed(Key.D))
		{
			dir.X = 1;
		}

		return dir;
	}
}

public enum CameraControllerState
{
	Idle,
	Moving,
}

public class Camera2DController
{
	private readonly IMoveDir moveDir;
	private readonly IMouse mouse;

	public CameraData CameraData { get; private set; }

	public float Speed { get; init; } = 1.0f;

	private CameraControllerState state;

	private Vector2D<float> startMovePosition;

	public Camera2DController(CameraData cameraData, IMoveDir moveDir, IMouse mouse)
    {
		CameraData = cameraData;
		this.moveDir = moveDir;
		this.mouse = mouse;

		this.mouse.MouseDown += OnMouseBtnDown;
		this.mouse.MouseUp += OnMouseBtnUp;
		this.mouse.MouseMove += OnMouseMove;
	}

	private void OnMouseMove(IMouse mouse, Vector2 vector)
	{
		if (state == CameraControllerState.Moving)
		{
			var curMousePosition = new Vector2D<float>(mouse.Position.X, mouse.Position.Y);
			if (curMousePosition == startMovePosition)
			{
				return;
			}

			var dir = Vector2D.Normalize(curMousePosition - startMovePosition) * new Vector2D<float>(1, -1);
			var shift = dir * Speed * ForgeGame.Time.DeltaTime;
			startMovePosition = curMousePosition;

			ApplyTranslation(shift);
		}
	}

	private void OnMouseBtnUp(IMouse mouse, MouseButton button)
	{
		if (button == MouseButton.Middle)
		{
			state = CameraControllerState.Idle;
		}
	}

	private void OnMouseBtnDown(IMouse mouse, MouseButton button)
	{
		if (button == MouseButton.Middle)
		{
			state = CameraControllerState.Moving;
			startMovePosition = new (mouse.Position.X, mouse.Position.Y);
		}
	}

	public void Update(GameTime gameTime)
	{
	}

	private void ApplyTranslation(Vector2D<float> translation)
	{
		var translationMatrix = Matrix4X4.CreateTranslation(translation.X, translation.Y, 0);
		CameraData.View *= translationMatrix;
	}
}
