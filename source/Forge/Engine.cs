using Forge.Graphics;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Collections.Concurrent;

namespace Forge;

public abstract class Engine
{
	protected readonly IWindow _window;

	protected GraphicsDevice? GraphicsDevice;

	private readonly ConcurrentQueue<Action> _renderTasks = new();

	protected IKeyboard? PrimaryKeyboard;
	protected IMouse? PrimaryMouse;

	private double totalTime = 0.0;
	private double fpsCounter = 0;

	public Engine()
	{
		var options = WindowOptions.Default;
		options.API = new GraphicsAPI()
		{
			Profile = ContextProfile.Core,
			API = ContextAPI.OpenGL,
			Flags = ContextFlags.ForwardCompatible | ContextFlags.Debug,
			Version = new APIVersion(4, 1),

		};

		options.Size = new Vector2D<int>(1280, 720);
		options.Title = "Forge";
		options.VSync = false;

		//options.FramesPerSecond = framerate;
		//options.UpdatesPerSecond = framerate;

		_window = Window.Create(options);

		_window.Load += OnLoad;
		_window.Render += (dt) =>
		{
			while (_renderTasks.TryDequeue(out var task))
			{
				task();
			}

			OnRender(dt);
		};

		_window.Update += (dt) =>
		{
			totalTime += dt;
			fpsCounter += dt;
			if (fpsCounter > 1.0)
			{
				var fps = 1.0 / dt;
				var ms = dt * 1000.0;
				_window.Title = "Forge - MS: " + ms.ToString("0.00") + ". FPS: " + fps.ToString("0.00");
				fpsCounter = 0.0;
			}

			OnUpdate(new GameTime((float)totalTime, (float)dt));
		};

		_window.Closing += OnClose;
		_window.Resize += OnResize;
	}

	protected abstract void OnClose();

	protected abstract void OnResize(Vector2D<int> obj);

	private void OnLoad()
	{
		IInputContext input = _window.CreateInput();
		PrimaryKeyboard = input.Keyboards[0];
		PrimaryMouse = input.Mice[0];

		Graphics.Graphics.ForceHardwareAcceleratedRendering();
		GraphicsDevice = GraphicsDevice.InitOpengl(_window);

		string version = GraphicsDevice.gl.GetStringS(StringName.Version);
		Console.WriteLine($"OpenGL Version: {version}");

		LoadGame();
	}

	public void Run()
	{
		_window.Run();
	}

	public void AddRenderTask(Action task)
	{
		_renderTasks.Enqueue(task);
	}

	public void Stop()
	{
		_window.Close();
	}

	protected abstract void LoadGame();

	protected abstract void OnRender(double delta);

	protected abstract void OnUpdate(GameTime time);
}