namespace System.Linq;

/// <summary>
/// Represents LINQ methods used by <see cref="LinkedList{T}"/>.
/// </summary>
/// <seealso cref="LinkedList{T}"/>
public static class LinkedListEnumerable
{
	/// <summary>
	/// Projects each element into a new form.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <typeparam name="TResult">The type of the transformed value.</typeparam>
	/// <param name="this">A <see cref="LinkedList{T}"/> instance.</param>
	/// <param name="selector">The function to transform items.</param>
	/// <returns>The projected values.</returns>
	public static ReadOnlySpan<TResult> Select<T, TResult>(this LinkedList<T> @this, Func<T, TResult> selector)
	{
		var result = new TResult[@this.Count];
		var i = 0;
		foreach (var element in @this)
		{
			result[i++] = selector(element);
		}
		return result;
	}

	/// <inheritdoc cref="Select{T, TResult}(LinkedList{T}, Func{T, TResult})"/>
	public static ReadOnlySpan<TResult> Select<T, TResult>(this LinkedListReversed<T> @this, Func<T, TResult> selector)
	{
		var result = new TResult[@this.Count];
		var i = 0;
		foreach (var element in @this)
		{
			result[i++] = selector(element);
		}
		return result;
	}

	/// <summary>
	/// Reverse the enumeration on <see cref="LinkedList{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">A <see cref="LinkedList{T}"/> instance.</param>
	/// <returns>The reversed enumerator-provider instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static LinkedListReversed<T> Reverse<T>(this LinkedList<T> @this) => new(@this);
}
