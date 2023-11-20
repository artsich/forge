using Forge.Renderer.Components;
using Forge.Renderer.Vertices.Layouts;
using Forge.Renderer.VertexAssebmlers;
using Forge.Renderer.Vertices;
using Forge.Graphics.Shaders;
using Silk.NET.OpenGL;
using Forge.Graphics;

namespace Forge.Renderer;

public class CircleRenderer : BatchRenderer<CircleVertex, CircleRenderComponent>
{
	public CircleRenderer() 
		: base(
			new CircleVertexLayout(), 
			new CircleAssembler(segmentsCount: 12),
			new BatchRendererDescription(5000))
	{
	}
}

public class SpriteRenderer : GraphicsResourceBase
{
	BatchRenderer<QuadVertex, QuadRenderComponent> batchRenderer;

	private readonly CompiledShader shader;
	private readonly Texture2d[] textures = new Texture2d[32];
	private uint texturesUsed;

	private CameraData? currentCamera;

	public SpriteRenderer(CompiledShader shader)
		: base(GraphicsDevice.Current)
	{
		this.shader = shader;
		textures[0] = Texture2d.White;
		batchRenderer = new BatchRenderer<QuadVertex, QuadRenderComponent>(
							new QuadVertexLayout(),
							new QuadAssembler(),
							new BatchRendererDescription(5000));
	}

	public void Begin(CameraData cameraData)
	{
		texturesUsed = 1;
		currentCamera = cameraData;

		Span<int> ids = stackalloc int[32];
		for (int i = 0; i < 32; i++)
		{
			ids[i] = i;
		}

		shader.Bind();
		shader["uTextures[0]"].SetValue(ids);
	}

	public void Push(QuadRenderComponent quad, Texture2d texture)
	{
		var index = Array.IndexOf(textures, texture);
		if (index == -1)
		{
			if (texturesUsed == textures.Length)
			{
				Flush();
				texturesUsed = 1;
			}

			textures[texturesUsed] = texture;
			quad = quad with { TextureId = texturesUsed };
			texturesUsed++;
		}

		batchRenderer.Push(ref quad);
	}

	public unsafe void Flush()
	{
		shader.Bind(currentCamera!);

		for (int i = 0; i < texturesUsed; i++)
		{
			textures[i].Bind(i);
		}

		Gd.gl.Enable(GLEnum.Blend);
		Gd.gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

		batchRenderer.Flush();

		Gd.gl.Disable(GLEnum.Blend);
	}
}
