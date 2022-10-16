namespace System.Runtime.CompilerServices;

/// <summary>
/// Provides extra methods that is an extension of type <see cref="Unsafe"/>.
/// </summary>
/// <seealso cref="Unsafe"/>
public static class Unsafe2
{
	/// <summary>
	/// Moves the reference to the next position. Simply calls <see cref="AddByteOffset{T}(ref T, nint)"/> with arguments
	/// <paramref name="ref"/> and 1.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	/// <param name="ref">The reference.</param>
	/// <seealso cref="AddByteOffset{T}(ref T, nint)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RefMoveNext<T>(ref T @ref) => AddByteOffset(ref @ref, 1);

	/// <summary>
	/// Moves the reference to the previous position. Simply calls <see cref="SubtractByteOffset{T}(ref T, nint)"/> with arguments
	/// <paramref name="ref"/> and 1.
	/// </summary>
	/// <typeparam name="T">The type of the element.</typeparam>
	/// <param name="ref">The reference.</param>
	/// <seealso cref="SubtractByteOffset{T}(ref T, nint)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RefMovePrevious<T>(ref T @ref) => SubtractByteOffset(ref @ref, 1);
}
