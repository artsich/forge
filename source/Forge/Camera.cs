using Forge.Graphics.Shaders;
using Silk.NET.Input;
using Silk.NET.Maths;

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

public class Camera2DController
{
	private readonly IMoveDir moveDir;

	public CameraData CameraData { get; private set; }

	public Vector2D<float> Position { get; set; }

	public float Speed { get; init; } = 1.0f;

    public Camera2DController(CameraData cameraData, IMoveDir moveDir)
    {
		CameraData = cameraData;
		this.moveDir = moveDir;
	}

	public void Update(GameTime gameTime)
	{
		Position += moveDir.GetDir() * gameTime.DeltaTime * Speed;
		ApplyTranslation(Position);
	}

	private void ApplyTranslation(Vector2D<float> translation)
	{
		CameraData.View = Matrix4X4.CreateTranslation(translation.X, translation.Y, 0);
	}
}
