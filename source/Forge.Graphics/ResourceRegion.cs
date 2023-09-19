using System.Runtime.InteropServices;

namespace Forge.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 0)]
public readonly struct ResourceRegion
{
	public ResourceRegion(int left, int top) 
		: this(left, top, 0, 0, 0, 0)
	{
	}

	public ResourceRegion(int left, int top, int front, int right, int bottom, int back)
	{
		Left = left;
		Top = top;
		Front = front;
		Right = right;
		Bottom = bottom;
		Back = back;
	}

	public readonly int Left;
	public readonly int Top;
	public readonly int Front;
	public readonly int Right;
	public readonly int Bottom;
	public readonly int Back;
}
