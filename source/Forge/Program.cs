using Forge.Graphics;
using Silk.NET.Maths;
using Silk.NET.Windowing;

var options = WindowOptions.Default;

var window = Window.Create(options with { Size = new Vector2D<int>(800, 600), Title = "Forge" });

window.Run();

window.Load += () =>
{
	var gd = GraphicsDevice.InitOpengl(window);

	var buffer = Forge.Graphics.Buffers.Buffer.Vertex.New(gd, new DataPointer(IntPtr.Zero, 128), 0);

	buffer.Bind();

	buffer.Dispose();
};
