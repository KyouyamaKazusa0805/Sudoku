namespace System;

/// <summary>
/// Provides with extension methods that extended from the <see langword="static class"/> <see cref="Unsafe"/>.
/// </summary>
/// <seealso cref="Unsafe"/>
public static unsafe class UnsafeExtensions
{
	/// <summary>
	/// Same as <see cref="Unsafe.InitBlock(void*, byte, uint)"/>, but using generic.
	/// </summary>
	/// <typeparam name="TUnmanaged">The type of the elements in the block.</typeparam>
	/// <param name="startAddress">The start address of the block.</param>
	/// <param name="value">The value to initialize.</param>
	/// <param name="elementCount">Indicates how many elements is in the block.</param>
	/// <seealso cref="Unsafe.InitBlock(void*, byte, uint)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void InitBlock<TUnmanaged>(TUnmanaged* startAddress, byte value, uint elementCount)
		where TUnmanaged : unmanaged =>
		Unsafe.InitBlock(startAddress, value, (uint)(sizeof(TUnmanaged) * elementCount));

	/// <summary>
	/// Same as <see cref="Unsafe.CopyBlock(void*, void*, uint)"/>, but using generic.
	/// </summary>
	/// <typeparam name="TUnmanaged">The type of the elements in the block.</typeparam>
	/// <param name="destination">The start address of destination block.</param>
	/// <param name="source">The start adressof the source block.</param>
	/// <param name="elementCount">Indicates how many elements is in the block.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void CopyBlock<TUnmanaged>(TUnmanaged* destination, TUnmanaged* source, uint elementCount)
		where TUnmanaged : unmanaged =>
		Unsafe.CopyBlock(destination, source, (uint)(sizeof(TUnmanaged) * elementCount));
}
