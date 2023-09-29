using System.Runtime.InteropServices;

namespace Forge.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 0)]
public readonly struct ResourceRegion
{
	public ResourceRegion(int left, int right) 
		: this(left, right, 0, 0, 0, 0)
	{
	}

	public ResourceRegion(int left, int right, int top, int bottom, int back, int front)
	{
		Left = left;
		Top = top;
		Front = front;
		Right = right;
		Bottom = bottom;
		Back = back;
	}

	public readonly int Left;
	public readonly int Right;
	public readonly int Top;
	public readonly int Bottom;
	public readonly int Front;
	public readonly int Back;
}
