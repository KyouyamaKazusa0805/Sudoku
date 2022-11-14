namespace System.Collections.Immutable;

/// <summary>
/// Provides with extension methods on <see cref="ImmutableArray{T}"/>.
/// </summary>
/// <seealso cref="ImmutableArray{T}"/>
internal static class ImmutableArrayExtensions
{
	/// <summary>
	/// Casts the current array elements into target typed values.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array to be casted.</param>
	/// <returns>The casted array.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ImmutableArray<T> CastToNotNull<T>(this ImmutableArray<T?> @this) where T : struct
		=> ImmutableArray.CreateRange(from element in @this select element!.Value);

	/// <summary>
	/// Casts the current array elements into target typed values.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The array to be casted.</param>
	/// <returns>The casted array.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ImmutableArray<T> CastToNotNull<T>(this ImmutableArray<T?> @this) where T : class
		=> ImmutableArray.CreateRange(from element in @this select element!);
}
