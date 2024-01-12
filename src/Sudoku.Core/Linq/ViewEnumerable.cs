namespace Sudoku.Linq;

/// <summary>
/// Represents with LINQ methods for <see cref="View"/> instances.
/// </summary>
/// <seealso cref="View"/>
public static class ViewEnumerable
{
	/// <summary>
	/// Projects with a new transform of elements.
	/// </summary>
	/// <typeparam name="T">The type of target element.</typeparam>
	/// <param name="this">The view.</param>
	/// <param name="selector">The method to transform each element.</param>
	/// <returns>A <see cref="ReadOnlySpan{T}"/> of <typeparamref name="T"/> elements.</returns>
	public static ReadOnlySpan<T> Select<T>(this View @this, Func<ViewNode, T> selector)
	{
		var result = new List<T>(@this.Count);
		foreach (var element in @this)
		{
			result.Add(selector(element));
		}

		return result.AsReadOnlySpan();
	}

	/// <summary>
	/// Filters the view nodes, only returns nodes of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the node.</typeparam>
	/// <returns>The target collection of element type <typeparamref name="T"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ViewNodeIterator<T> OfType<T>(this View @this) where T : ViewNode => new(@this.GetEnumerator());
}
