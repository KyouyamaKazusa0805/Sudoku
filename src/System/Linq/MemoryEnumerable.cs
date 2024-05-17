namespace System.Linq;

/// <summary>
/// Represents a list of methods that are LINQ methods operating with <see cref="ReadOnlyMemory{T}"/> instances.
/// </summary>
/// <seealso cref="ReadOnlyMemory{T}"/>
public static class MemoryEnumerable
{
	/// <inheritdoc cref="SpanEnumerable.Select{T, TResult}(ReadOnlySpan{T}, Func{T, TResult})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<TResult> Select<T, TResult>(this ReadOnlyMemory<T> @this, Func<T, TResult> selector)
		=> from element in @this.Span select selector(element);

	/// <inheritdoc cref="SpanEnumerable.Where{T}(ReadOnlySpan{T}, Func{T, bool})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<T> Where<T>(this ReadOnlyMemory<T> @this, Func<T, bool> predicate)
		=> from element in @this.Span where predicate(element) select element;
}
