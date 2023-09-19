namespace Forge.Graphics;

public readonly struct DataPointer
{
	public unsafe DataPointer(void* pointer, int size)
	{
		Pointer = (IntPtr)pointer;
		Size = size;
	}

	public DataPointer(IntPtr pointer, int size)
	{
		Pointer = pointer;
		Size = size;
	}

	public readonly IntPtr Pointer;

	public readonly int Size;
}
