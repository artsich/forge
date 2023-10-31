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
using Shader = Forge.Graphics.Shaders.Shader;

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
	public const int Width = 640, Height = 360;

	private IWindow window;
	private static GL Gl;

	private static CompiledShader Shader;
	private static CompiledShader PostProcessingShader;

	private readonly CameraData gameCamera = new(Matrix4X4.CreateOrthographic(Width, Height, 0.1f, 100.0f));
	private readonly CameraData uiCamera = new(Matrix4X4.CreateOrthographic(Width, Height, 0.1f, 100.0f));

	private Renderer2D? renderer;

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

	private UiElements uiElements;

	public void Load()
	{
		window = Engine.Window;

		window.Resize += OnResize;

		Gl = Engine.GraphicsDevice!.gl;
		camera2D = new Camera2DController(gameCamera, Engine.PrimaryMouse)
		{
			Speed = 1000f
		};

		renderer = new Renderer2D();

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

		var fontShader = new Shader(GraphicsDevice.Current,
			new Shader.ShaderPart(
@"
#version 330 core

layout(location = 0) in vec2 v_pos;
layout(location = 1) in vec2 v_uv;
layout(location = 2) in vec4 v_color;

out vec2 TexCoords;
out vec4 Color;

uniform mat4 cameraViewProj;

void main()
{
	TexCoords = v_uv;
	Color = v_color;
	gl_Position = cameraViewProj * vec4(v_pos, 0.0, 1.0);
}

", ShaderType.VertexShader),
			new Shader.ShaderPart(
@"
#version 330 core

in vec2 TexCoords;
in vec4 Color;

out vec4 o_color;

uniform sampler2D text;
uniform vec3 textColor = vec3(1.0);

const float width = 0.4;
const float edge = 0.1;

void main()
{
	float distanceRange = 2;
    float distance = texture(text, TexCoords).r;
    float alpha = smoothstep(width - edge, width + edge, distance);
	
	o_color = vec4(Color.xyz, alpha);
}

", ShaderType.FragmentShader))
			.Compile() ?? throw new InvalidOperationException("Shader compilation error!");

		//var fontSprite = new MsdfAtlasGen()
		//		.GenerateAtlas("C:\\Windows\\Fonts\\consola.ttf")
		//		.GetSpriteFont();
		fontRenderer = new FontRenderer(assets.LoadFont("consola"), fontShader);

		fpsLabel = new TextLabel(
			fontRenderer,
			new Transform2d(new (-Width / 2f + 20, Height / 2f - 20)))
		{
			FontSize = 11f,
			Color = new Vector4D<float>(1f, 0f, 0f, 1f),
		};

		zoomLabel = new TextLabel(
			fontRenderer,
			new Transform2d(new(-Width / 2f + 20, Height / 2f - 40)))
		{
			FontSize = 13f,
			Color = new Vector4D<float>(1f, 0f, 0f, 1f),
		};

		uiElements = new(
			fpsLabel,
			zoomLabel
		);
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

			renderer!.DrawCircle(renderInfo);
		}

		renderer!.FlushAll();

		timer += delta;
		if (timer > 1)
		{
			timer = 0;
			fps = 1.0f / (float)delta;
		}

		fpsLabel.Text = $"FPS: {fps:0.000}";
		zoomLabel.Text = $"Zoom scale: {camera2D.CurrentZoom:0.0000}";

		uiElements.Draw();
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
			verletObjects.Add(
				new VerletCircleObject(
					new Vector2D<float>(
						r.Next(-200, 200),
						r.Next(-200, 200)),
					r.Next(10, 20)));
		}
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