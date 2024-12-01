namespace System.Runtime.CompilerServices;

/// <summary>
/// Provides with extension methods on <see cref="ITuple"/>.
/// </summary>
/// <seealso cref="ITuple"/>
public static class TupleExtensions
{
	/// <summary>
	/// Converts the <see cref="ITuple"/> instance into an array of objects.
	/// </summary>
	/// <typeparam name="TTuple">The type of target tuple.</typeparam>
	/// <param name="this">The instance.</param>
	/// <returns>The array of elements.</returns>
	public static object?[] ToArray<TTuple>(this TTuple @this) where TTuple : ITuple, allows ref struct
	{
		var result = new object?[@this.Length];
		var i = 0;
		foreach (var element in @this)
		{
			result[i++] = element;
		}
		return result;
	}

	/// <summary>
	/// Converts the tuple elements into a valid span of elements of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The unified type for all elements.</typeparam>
	/// <param name="this">The tuple instance.</param>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> instance.</returns>
	public static ReadOnlySpan<T> AsSpan<T>(this ITuple @this)
	{
		var result = new T[@this.Length];
		var i = 0;
		foreach (var element in @this)
		{
			result[i++] = (T)element!;
		}
		return result;
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TupleEnumerator<TTuple> GetEnumerator<TTuple>(this TTuple @this) where TTuple : ITuple?, allows ref struct
		=> new(@this);
}
