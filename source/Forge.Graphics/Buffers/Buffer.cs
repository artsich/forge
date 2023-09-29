using Silk.NET.OpenGL;
using System.Runtime.CompilerServices;

namespace Forge.Graphics.Buffers;

public unsafe sealed class Buffer<T> : Buffer
	where T : unmanaged
{
	private static readonly int TSize = Unsafe.SizeOf<T>();

	public Buffer(GraphicsDevice gd)
		: base(gd)
	{
	}

	public void SetData(T[] fromData)
	{
		SetData(fromData, new ResourceRegion(0, fromData.Length));
	}

	public void SetData(T[] fromData, int count)
	{
		fixed (void* from = &fromData[0])
			base.SetData(new DataPointer(from, fromData.Length * TSize), new ResourceRegion(0, count * TSize));
	}

	// todo: Broken???
	public unsafe void SetData(T[] fromData, ResourceRegion region)
	{
		fixed (void* from = &fromData[0])
			base.SetData(new DataPointer(from, fromData.Length * TSize), region);
	}
}

public unsafe partial class Buffer : GraphicsResourceBase
{
	internal uint BufferId;
	internal BufferUsageARB BufferUsageHint;
	internal BufferTargetARB BufferTarget;
	internal int SizeInBytes;
	
	public int ElementSize { get; private set; }
	public int ElementCount { get; private set; }

	public Buffer(GraphicsDevice gd)
		: base(gd)
	{
	}

	internal Buffer Initialize(IntPtr dataPointer, BufferDescription description)
	{
		BufferTarget = description.BufferTarget;
		BufferUsageHint = description.Usage;
		SizeInBytes = description.SizeInBytes;
		ElementSize = description.StructureByteStride;
		ElementCount = SizeInBytes / ElementSize;

		Recreate(dataPointer);
		Gd.RegisterBufferMemoryUsage(description.SizeInBytes);
		return this;
	}

	public void Recreate(IntPtr dataPointer)
	{
		GL.GenBuffers(1, out BufferId);
		GL.BindBuffer(BufferTarget, BufferId);
		GL.BufferData(BufferTarget, (nuint)SizeInBytes, (void*)dataPointer, BufferUsageHint);
		GL.BindBuffer(BufferTarget, 0);
	}

	public void Bind()
	{
		if (BufferId == 0)
			throw new InvalidOperationException("Buffer was disposed.");
		GL.BindBuffer(BufferTarget, BufferId);
	}

	public void SetData(DataPointer fromData, ResourceRegion region)
	{
		if (region.Left == 0 && region.Right == SizeInBytes)
		{
			GL.NamedBufferData(BufferId, (UIntPtr)region.Right, (void*)fromData.Pointer, (GLEnum)BufferUsageHint);
		}
		else
		{
			GL.NamedBufferSubData(BufferId, region.Left, (UIntPtr)(region.Right - region.Left), (void*)fromData.Pointer);
		}
	}

	protected override void OnDestroy()
	{
		GL.DeleteBuffers(1, in BufferId);
		BufferId = 0;
		Gd.RegisterBufferMemoryUsage(-SizeInBytes);
	}
}
