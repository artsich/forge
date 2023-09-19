using Silk.NET.OpenGL;

namespace Forge.Graphics.Buffers;

public struct BufferDescription : IEquatable<BufferDescription>
{
    public BufferDescription(int sizeInBytes, BufferTargetARB bufferTarget, BufferUsageARB usage, int structureByteStride = 0)
    {
        SizeInBytes = sizeInBytes;
        BufferTarget = bufferTarget;
        Usage = usage;
        StructureByteStride = structureByteStride;
    }

    public int SizeInBytes;

    public BufferTargetARB BufferTarget;

    public BufferUsageARB Usage;

    public int StructureByteStride;

    public bool Equals(BufferDescription other)
    {
        return SizeInBytes == other.SizeInBytes && BufferTarget == other.BufferTarget && Usage == other.Usage && StructureByteStride == other.StructureByteStride;
    }

    public override bool Equals(object? obj)
    {
		if (obj is null) return false;
		return obj is BufferDescription description && Equals(description);
    }

	public override readonly int GetHashCode()
	{
		return HashCode.Combine(SizeInBytes, BufferTarget, Usage, StructureByteStride);
	}

	public static bool operator ==(BufferDescription left, BufferDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BufferDescription left, BufferDescription right)
    {
        return !left.Equals(right);
    }
}
