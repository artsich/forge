using Forge.Graphics;
using Forge.Graphics.Buffers;
using Forge.Graphics.Shaders;
using Forge.Physics;
using Forge.Renderer;
using Forge.Renderer.Components;
using Forge.Renderer.Font;
using Forge.Renderer.Ui;
using Forge.Shaders;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Drawing;
using Shader = Forge.Graphics.Shaders.ShaderSources;

namespace Forge;

public interface ILayer
{
	void Load();

	void Unload();

	void Update(GameTime time);

	void Render(GameTime time);
}

public unsafe class ForgeGame : ILayer
{
	public const int Width = 640, Height = 9 * Width / 16;

	private IWindow window;
	private static GL Gl;

	private static CompiledShader Shader;
	private static CompiledShader PostProcessingShader;

	private readonly CameraData gameCamera = new(Matrix4X4.CreateOrthographic(Width, Height, 0.1f, 100.0f));
	private readonly CameraData uiCamera = new(Matrix4X4.CreateOrthographic(Width, Height, 0.1f, 100.0f));

	private Camera2DController camera2D;

	private Texture2d texture;

	private FrameBuffer framebuffer;
	private FrameBuffer defaultFb;

	private float[] quadVertices = { // vertex attributes for a quad that fills the entire screen in Normalized Device Coordinates.
		// positions // texCoords
		-1.0f,  1.0f, 0.0f, 1.0f,
		-1.0f, -1.0f, 0.0f, 0.0f,
		 1.0f, -1.0f, 1.0f, 0.0f,

		-1.0f,  1.0f, 0.0f, 1.0f,
		 1.0f, -1.0f, 1.0f, 0.0f,
		 1.0f,  1.0f, 1.0f, 1.0f
	};

	private VertexArrayBuffer quadVao;
	private Buffer<float> quadVBO;

	public static GameTime Time { get; private set; }

	private VerletSolver solver;
	private List<VerletCircleObject> verletObjects;
	private List<Link> verletLinks;

	private readonly Random r = new();

	private FontRenderer fontRenderer;
	private float timer;
	private float fps;

	private readonly Assets assets = new(new FontService("Assets//Font"));

	private TextLabel fpsLabel;
	private TextLabel zoomLabel;
	private TextLabel entitiesOnScreen;
	private Renderer.Ui.Button animationsButton;
	private TextLabel mousePositionLabel;
	private UiRoot uiRoot;
	private CompiledShader quadShader;

	private CircleRenderer circleRenderer;
	private UiQuadRenderer buttonsRenderer;

	public void Load()
	{
		window = Engine.Window;

		window.Resize += OnResize;

		Gl = Engine.GraphicsDevice!.gl;
		camera2D = new Camera2DController(gameCamera, Engine.PrimaryMouse)
		{
			Speed = 1000f
		};

		Shader = new DefaultShader()
			.Compile() ?? throw new InvalidOperationException("Shader compilation error!");

		PostProcessingShader = new PostProcessingShader()
			.Compile() ?? throw new InvalidOperationException("Shader compilation error!");

		framebuffer = new FrameBuffer(
			new Size(Width, Height),
			new[]
			{
				new Texture2d(Width, Height, mipmap: false)
			});

		defaultFb = new FrameBuffer(new(window.Size.X, window.Size.Y));

		texture = new Texture2d(1, 1, new byte[] { 0, 255, 0, 255 });

		quadVao = new VertexArrayBuffer(GraphicsDevice.Current);
		quadVao.Bind();
		quadVBO = Graphics.Buffers.Buffer.Vertex.New(GraphicsDevice.Current, quadVertices, BufferUsageARB.StaticDraw);
		quadVBO.Bind();

		Gl.EnableVertexAttribArray(0);
		Gl.VertexAttribPointer(0, 2, GLEnum.Float, false, 4 * sizeof(float), (void*)0);
		Gl.EnableVertexAttribArray(1);
		Gl.VertexAttribPointer(1, 2, GLEnum.Float, false, 4 * sizeof(float), (void*)(2 * sizeof(float)));

		quadVao.Unbind();

		verletObjects = new List<VerletCircleObject>
		{
			new VerletCircleObject(new Vector2D<float>(0, 10), 10) { IsStatic = true },
			new VerletCircleObject(new Vector2D<float>(20, 0), 10),
			new VerletCircleObject(new Vector2D<float>(40, 0), 10),
			new VerletCircleObject(new Vector2D<float>(60, 0), 10),
			new VerletCircleObject(new Vector2D<float>(80, 0), 10),
			new VerletCircleObject(new Vector2D<float>(100, 0), 10),
			new VerletCircleObject(new Vector2D<float>(120, 0), 10),
			new VerletCircleObject(new Vector2D<float>(140, 0), 10),
			new VerletCircleObject(new Vector2D<float>(160, 0), 10),
			new VerletCircleObject(new Vector2D<float>(180, 0), 10) { IsStatic = true },
		};

		verletLinks = new List<Link>()
		{
			new Link(verletObjects[0], verletObjects[1], 25f),
			new Link(verletObjects[1], verletObjects[2], 25f),
			new Link(verletObjects[2], verletObjects[3], 25f),
			new Link(verletObjects[3], verletObjects[4], 25f),
			new Link(verletObjects[4], verletObjects[5], 25f),
			new Link(verletObjects[5], verletObjects[6], 25f),
			new Link(verletObjects[6], verletObjects[7], 25f),
			new Link(verletObjects[7], verletObjects[8], 25f),
			new Link(verletObjects[8], verletObjects[9], 25f),
		};

		solver = new VerletSolver(verletObjects, verletLinks);
		quadShader = new QuadShader().Compile() ?? throw new Exception("Shader compilation failed");

		circleRenderer = new CircleRenderer();
		buttonsRenderer = new UiQuadRenderer(quadShader);

		var fontSprite = assets.LoadFont("consola");

		fontRenderer = new FontRenderer(fontSprite, new SdfFontShader());

		fpsLabel = new TextLabel(fontSprite.FontMetrics, fontRenderer)
		{
			Color = new (1f, 0f, 0f, 1f),
		};

		zoomLabel = new TextLabel(fontSprite.FontMetrics, fontRenderer);
		entitiesOnScreen = new TextLabel(fontSprite.FontMetrics, fontRenderer);
		mousePositionLabel = new TextLabel(fontSprite.FontMetrics, fontRenderer);

		animationsButton = new(
			new TextLabel(fontSprite.FontMetrics, fontRenderer)
			{
				Text = "Animations",
				FontSize = 15f,
				Color = new (1f, 0f, 0f, 1f),
			},
			buttonsRenderer)
		{
			Color = new(0f, 0f, 0.7f, 1f),
			Padding = new(10f),
		};

		var addEntities = new Renderer.Ui.Button(
			new TextLabel(
				fontSprite.FontMetrics,
				fontRenderer)
			{
				Text = "Add",
				FontSize = 15f,
				Color = new (1f, 0f, 0f, 1f),
			},
			buttonsRenderer)
		{
			Color = new(0f, 0.8f, 0f, 1f),
			Padding = new(10f),
		};

		var actionsContainer = new VerticalContainer(
			buttonsRenderer,
			10f)
		{
			Transform = new Transform2d(new Vector2D<float>(-Width / 2f + 20, Height / 2f - 20)),
			Children = new List<UiElement>()
			{
				animationsButton,
				addEntities
			},
			Color = new Vector4D<float>(0.1f, 0.1f, 0.1f, 0.5f),
			Padding = new(10f)
		};

		var systemInfoContainer = new HorizontalContainer(
			buttonsRenderer,
			10f)
		{
			Transform = new Transform2d(new Vector2D<float>(-Width / 2f + 20, -Height / 2f + 50)),
			Children = new List<UiElement>()
			{
				entitiesOnScreen,
				mousePositionLabel,
				zoomLabel,
				fpsLabel,
			},
			Color = new Vector4D<float>(0.1f, 0.1f, 0.1f, 0.5f),
			Padding = new(10f)
		};

		zoomLabel.OnClick += (_, _) => Console.WriteLine("Clicked on zoom.");
		fpsLabel.OnClick += (_, _) => Console.WriteLine("Clicked on fps label.");
		animationsButton.OnClick += (_, _) => Console.WriteLine("Animation editor.");
		actionsContainer.OnClick += (_, _) => Console.WriteLine("Clicked on action container.");
		systemInfoContainer.OnClick += (_, _) => Console.WriteLine("Clicked on system info container.");
		addEntities.OnClick += (_, _) => GenerateGameObject();

		uiRoot = new(actionsContainer, systemInfoContainer);

		Engine.PrimaryMouse.MouseDown += (mouse, button) => uiRoot.OnMouseDown(MapToCurrentWindowCoord(mouse.Position.X, mouse.Position.Y), button);
	}

	private Vector2D<float> MapToCurrentWindowCoord(float x, float y)
	{
		var xx = x / window.Size.X;
		var yy = y / window.Size.Y;
		return new(Width * xx - Width / 2f, Height / 2f - Height * yy);
	}

	public void Render(GameTime time)
	{
		var (totalRenderTime, delta) = (time.TotalTime, time.DeltaTime);

		framebuffer.Bind();
		framebuffer.Clear();

		Shader.Bind();
		texture.Bind();

		foreach (var obj in verletObjects)
		{
			var renderInfo = new CircleRenderComponent()
			{
				Color = new Vector4D<float>(1.0f, 0.5f, 0.5f, 1.0f),
				Position = obj.PositionCurrent,
				Radius = obj.Radius,
			};

			circleRenderer.Push(ref renderInfo);
		}
		circleRenderer.Flush();

		quadShader.Bind();
		quadShader["cameraViewProj"].SetValue(gameCamera.ViewProjection);

		timer += delta;
		if (timer > 1)
		{
			timer = 0;
			fps = 1.0f / (float)delta;
		}

		fpsLabel.Text = $"FPS: {fps:0.000}";
		zoomLabel.Text = $"Zoom scale: {camera2D.CurrentZoom:0.0000}";
		entitiesOnScreen.Text = "Entities: " + verletObjects.Count;
		var windowCoord = MapToCurrentWindowCoord((int)Engine.PrimaryMouse.Position.X, (int)Engine.PrimaryMouse.Position.Y);
		var (x, y) = ((int)windowCoord.X, (int)windowCoord.Y);
		mousePositionLabel.Text = $"Mouse pos: {x} : {y}";

		uiRoot.Draw();

		// Move ui render logic to ui root?
		GraphicsDevice.Current.gl.Enable(GLEnum.Blend);
		GraphicsDevice.Current.gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);
		buttonsRenderer.Flush(uiCamera);
		GraphicsDevice.Current.gl.Disable(GLEnum.Blend);

		// Move ui render logic to ui root?
		fontRenderer.Flush(uiCamera);

		framebuffer.Unbind();

		defaultFb.Bind();
		defaultFb.Clear();

		PostProcessingShader.Bind();
		quadVao.Bind();

		// todo: should i make it part of framebuffer?
		Gl.Disable(EnableCap.DepthTest);

		framebuffer.Colors.ElementAt(0).Bind(0);

		Gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
	}

	public void Update(GameTime time)
	{
		// does not make sense to update camera in single thread app
		Engine.AddRenderTask(() =>
		{
			Shader.BindUniforms(camera2D.CameraData);
		});

		Time = time;
		camera2D.Update(time);

		solver.Update(time.DeltaTime);

		if (Engine.PrimaryKeyboard.IsKeyPressed(Key.Space))
		{
			GenerateGameObject();
		}
	}

	private void GenerateGameObject()
	{
		verletObjects.Add(
			new VerletCircleObject(
				new Vector2D<float>(
					r.Next(-200, 200),
					r.Next(-200, 200)),
				r.Next(10, 20)));
	}

	public void Unload()
	{
		Shader.Dispose();
	}

	private void OnResize(Vector2D<int> size)
	{
		defaultFb.Resize(new Size(size.X, size.Y));
	}
}