using Forge.Graphics;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Forge;

public abstract class GameBase
{
	private readonly IWindow _window;

	protected GraphicsDevice? GraphicsDevice;

	private readonly ConcurrentQueue<Action> _renderTasks = new();
	private readonly ConcurrentQueue<Action> _gameLogicTasks = new();
	private Task _gameLogicTask = Task.CompletedTask;
	private bool _running = true;
	private readonly Stopwatch stopwatch = new();

	private const double framerate = 144.0;
	private readonly double targetFrameTime = 1.0 / framerate;

	protected IKeyboard? PrimaryKeyboard;
	protected IMouse? PrimaryMouse;

	public GameBase()
	{
		var options = WindowOptions.Default;
//		options.API = new GraphicsAPI() 
//		{ 
//			Profile  = ContextProfile.Core,
//			API = ContextAPI.OpenGL,
//#if DEBUG
//			Flags = ContextFlags.Debug,
//#endif
//			Version = new APIVersion(4, 1) 
//		};

		options.Size = new Vector2D<int>(1280, 720);
		options.Title = "Forge";
		options.VSync = false;

		//options.FramesPerSecond = framerate;
		//options.UpdatesPerSecond = framerate;

		_window = Window.Create(options);

		_window.Load += OnLoad;
		_window.Render += OnRender;
		_window.Closing += OnClose;
		_window.Resize += OnResize;
	}

	protected abstract void OnClose();

	protected abstract void OnResize(Vector2D<int> obj);

	private void OnLoad()
	{
		IInputContext input = _window.CreateInput();
		PrimaryKeyboard = input.Keyboards.First();
		PrimaryMouse = input.Mice.First();

		Forge.Graphics.Graphics.ForceHardwareAcceleratedRendering();
		GraphicsDevice = GraphicsDevice.InitOpengl(_window);

		string version = GraphicsDevice.gl.GetStringS(StringName.Version);
		Console.WriteLine($"OpenGL Version: {version}");

		LoadGame();

		_gameLogicTask = Task.Run(GameLogicLoop);
	}

	private void OnRender(double delta)
	{
		while (_renderTasks.TryDequeue(out var task))
		{
			task.Invoke();
		}

		Render(delta);
	}

	public void Run()
	{
		_running = true;
		_window.Run();
	}

	private void GameLogicLoop()
	{
		stopwatch.Start();

		double totalTime = stopwatch.Elapsed.TotalSeconds;

		while (_running)
		{
			double currentTime = stopwatch.Elapsed.TotalSeconds;
			double elapsedTime = currentTime - totalTime;

			while (_gameLogicTasks.TryDequeue(out var task))
			{
				task.Invoke();
			}

			totalTime = currentTime;

			Update(new GameTime((float)totalTime, (float)elapsedTime));

			double sleepTime = targetFrameTime - (stopwatch.Elapsed.TotalSeconds - currentTime);

			if (sleepTime > 0)
			{
				Thread.Sleep((int)(sleepTime * 1000));
			}
		}

		stopwatch.Stop();
	}

	public void AddGameLogicTask(Action task)
	{
		_gameLogicTasks.Enqueue(task);
	}

	public void AddRenderTask(Action task)
	{
		_renderTasks.Enqueue(task);
	}

	public void Stop()
	{
		_running = false;
		_window.Close();
	}

	protected abstract void LoadGame();

	protected abstract void Render(double delta);

	protected abstract void Update(GameTime time);
}