namespace System;

/// <summary>
/// Extension methods for generic type parameters.
/// </summary>
public static class GenericExtensions
{
	/// <summary>
	/// Move the pointer to the next element.
	/// </summary>
	/// <typeparam name="T">The type of the instance that the reference points to.</typeparam>
	/// <param name="ref">The reference points to an instance of type <typeparamref name="T"/>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void MoveNext<T>(this scoped ref T @ref) where T : struct => Add(ref @ref, 1);

	/// <summary>
	/// Move the pointer to the previous element.
	/// </summary>
	/// <typeparam name="T">The type of the instance that the reference points to.</typeparam>
	/// <param name="ref">The reference points to an instance of type <typeparamref name="T"/>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void MovePrevious<T>(this scoped ref T @ref) where T : struct => Add(ref @ref, -1);
}
