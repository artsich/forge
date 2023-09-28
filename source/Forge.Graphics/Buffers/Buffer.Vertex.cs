using Silk.NET.OpenGL;
using System.Runtime.CompilerServices;

namespace Forge.Graphics.Buffers;

public unsafe partial class Buffer
{
	public static class Vertex
	{
		public static Buffer New(GraphicsDevice gd, int bufferSize, int elementSize, BufferUsageARB usage = BufferUsageARB.StaticDraw)
		{
			// TODO: data pointer is wrong in this case
			return New(gd, new DataPointer(IntPtr.Zero, bufferSize), elementSize, usage);
		}

		public static Buffer<T> New<T>(GraphicsDevice gd, int bufferSize, BufferUsageARB usage = BufferUsageARB.StaticDraw)
			where T : unmanaged
		{
			int elementSize = Unsafe.SizeOf<T>();
			return (Buffer<T>)new Buffer<T>(gd).Initialize(IntPtr.Zero, NewDescription(bufferSize, elementSize, usage));
		}

		public static Buffer<T> New<T>(GraphicsDevice gd, T[] initialValue, BufferUsageARB usage = BufferUsageARB.StaticDraw)
			where T : unmanaged
		{
			int bufferSize = Unsafe.SizeOf<T>() * initialValue.Length;
			int elementSize = Unsafe.SizeOf<T>();

			fixed (void* initial = &initialValue[0])
				return (Buffer<T>)new Buffer<T>(gd).Initialize((IntPtr)initial, NewDescription(bufferSize, elementSize, usage));
		}

		public static Buffer New(GraphicsDevice gd, DataPointer dataPointer, int elementSize, BufferUsageARB usage = BufferUsageARB.StaticDraw)
		{
			BufferDescription bufferDescription = NewDescription(dataPointer.Size, elementSize, usage);
			return new Buffer(gd).Initialize(dataPointer.Pointer, bufferDescription);
		}

		private static BufferDescription NewDescription(int sizeInBytes, int elementSize, BufferUsageARB usage)
		{
			return new BufferDescription()
			{
				BufferTarget = BufferTargetARB.ArrayBuffer,
				SizeInBytes = sizeInBytes,
				StructureByteStride = elementSize,
				Usage = usage,
			};
		}
	}
}
