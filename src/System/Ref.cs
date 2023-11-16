using System.Runtime.CompilerServices;

namespace System;

/// <summary>
/// Represents a list of methods that can check for the concept "References" defined in C#.
/// </summary>
public static class Ref
{
	/// <summary>
	/// Swaps for two elements.
	/// </summary>
	/// <typeparam name="T">The type of both two arguments.</typeparam>
	/// <param name="left">The first element to be swapped.</param>
	/// <param name="right">The second element to be swapped.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Swap<T>(scoped ref T left, scoped ref T right)
	{
		if (MemoryLocationAreSame(in left, in right))
		{
			return;
		}

		var temp = left;
		left = right;
		right = temp;
	}

	/// <summary>
	/// Determines whether the current reference points to <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">The type of the referenced element.</typeparam>
	/// <param name="reference">The reference to be checked.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNullReference<T>(scoped ref readonly T reference) => Unsafe.IsNullRef(in reference);

	/// <summary>
	/// Check whether two references point to a same memory location.
	/// </summary>
	/// <typeparam name="T">The type of both two arguments.</typeparam>
	/// <param name="left">The first element to be checked.</param>
	/// <param name="right">The second element to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool MemoryLocationAreSame<T>(scoped ref readonly T left, scoped ref readonly T right)
		=> Unsafe.AreSame(in left, in right);

	/// <summary>
	/// Returns a reference that points to <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the element the reference points to if this reference were not <see langword="null"/>.
	/// </typeparam>
	/// <returns>A read-only reference that points to <see langword="null"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ref readonly T MakeNullReference<T>() => ref Unsafe.NullRef<T>();

	/// <summary>
	/// Throws an <see cref="ArgumentNullRefException"/> if the argument points to <see langword="null"/>.
	/// </summary>
	/// <typeparam name="T">The type of the referenced element.</typeparam>
	/// <param name="reference">The reference to the target element, or maybe a <see langword="null"/> reference.</param>
	/// <exception cref="ArgumentNullRefException">Throws if the argument is a <see langword="null"/> reference.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNullRef<T>(scoped ref readonly T reference)
	{
		if (IsNullReference(in reference))
		{
			throw new ArgumentNullRefException(nameof(reference));
		}
	}
}
