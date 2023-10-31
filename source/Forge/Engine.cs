using Forge.Graphics;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Collections.Concurrent;

namespace Forge;

public class Engine
{
	public static IWindow Window { get; private set; }

	public static GraphicsDevice? GraphicsDevice { get; private set; }

	public static IKeyboard PrimaryKeyboard;

	public static IMouse PrimaryMouse;

	public static Engine Instance { get; private set; }

	private readonly ConcurrentQueue<Action> _renderTasks = new();
	private readonly ILayer[] layers;

	private double totalUpdateTime = 0.0;
	private double totalRenderTime = 0.0;

	public Engine(params ILayer[] layers)
	{
		Instance = this;

		this.layers = layers;

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

		Window = Silk.NET.Windowing.Window.Create(options);

		Window.Load += OnLoad;
		Window.Render += (dt) =>
		{
			totalRenderTime += dt;
			while (_renderTasks.TryDequeue(out var task))
			{
				task();
			}

			foreach (var layer in layers)
			{
				layer.Render(new GameTime((float)totalRenderTime, (float)dt));
			}
		};

		Window.Update += (dt) =>
		{
			totalUpdateTime += dt;
			foreach (var layer in layers)
			{
				layer.Update(new GameTime((float)totalUpdateTime, (float)dt));
			}
		};

		Window.Closing += () =>
		{
			foreach (var layer in layers)
			{
				layer.Unload();
			}
		};
	}

	private void OnLoad()
	{
		IInputContext input = Window.CreateInput();
		PrimaryKeyboard = input.Keyboards[0];
		PrimaryMouse = input.Mice[0];

		Graphics.Graphics.ForceHardwareAcceleratedRendering();
		GraphicsDevice = GraphicsDevice.InitOpengl(Window);

		string version = GraphicsDevice.gl.GetStringS(StringName.Version);
		Console.WriteLine($"OpenGL Version: {version}");

		foreach (var layer in layers)
		{
			layer.Load();
		}
	}

	public void Run()
	{
		Window.Run();
	}

	public static void AddRenderTask(Action task)
	{
		Instance._renderTasks.Enqueue(task);
	}

	public void Stop()
	{
		Window.Close();
	}
}
