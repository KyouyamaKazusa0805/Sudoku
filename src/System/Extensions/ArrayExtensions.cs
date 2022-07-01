namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Array"/>.
/// </summary>
/// <seealso cref="Array"/>
public static class ArrayExtensions
{
	/// <summary>
	/// Simply encapsulates a method to call method <see cref="Array.FindIndex{T}(T[], Predicate{T})"/>
	/// and returns a <see cref="bool"/> value indicating whether the result is not -1.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">An array with one dimension.</param>
	/// <param name="predicate">
	/// The predicate that determines whether an element satisfies the user's requirement.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating whether such element can be found.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool CanFind<T>(this T[] @this, Predicate<T> predicate) => Array.FindIndex(@this, predicate) != -1;
}
