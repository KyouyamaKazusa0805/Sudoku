namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Random"/>.
/// </summary>
/// <seealso cref="Random"/>
public static class RandomExtensions
{
	/// <summary>
	/// Try to get the random element in the specified array.
	/// </summary>
	/// <typeparam name="T">The type of the array.</typeparam>
	/// <param name="this">The random number generator.</param>
	/// <param name="array">The array.</param>
	/// <param name="index">The index generated.</param>
	/// <returns>The random element fetched.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T NextInArray<T>(this Random @this, T[] array, out int index) => array[index = @this.Next(0, array.Length - 1)];
}
