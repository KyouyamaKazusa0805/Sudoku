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
	/// Filters a <see cref="View"/>, only reserves the <see cref="ViewNode"/> instances satisfying the specified condition.
	/// </summary>
	/// <param name="this">The view.</param>
	/// <param name="predicate">The filter.</param>
	/// <returns>A list of <see cref="ViewNode"/> filtered.</returns>
	public static ReadOnlySpan<ViewNode> Where(this View @this, Func<ViewNode, bool> predicate)
	{
		var result = new List<ViewNode>(@this.Count);
		foreach (var element in @this)
		{
			if (predicate(element))
			{
				result.Add(element);
			}
		}
		return result.AsReadOnlySpan();
	}

	/// <summary>
	/// Filters the view nodes, only returns nodes of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the node.</typeparam>
	/// <returns>The target collection of element type <typeparamref name="T"/>.</returns>
	public static ReadOnlySpan<T> OfType<T>(this View @this) where T : ViewNode
	{
		var result = new List<T>();
		foreach (var node in @this)
		{
			if (node is T casted)
			{
				result.Add(casted);
			}
		}

		return result.AsReadOnlySpan();
	}

	/// <returns>
	/// The first element that matches the conditions defined by the specified predicate, if found;
	/// otherwise, throw an <see cref="InvalidOperationException"/>.
	/// </returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the sequence has no elements satisfying the specified rule.
	/// </exception>
	/// <inheritdoc cref="FirstOrDefault(View, Func{ViewNode, bool})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ViewNode First(this View @this, Func<ViewNode, bool> match)
		=> @this.FirstOrDefault(match) ?? throw new InvalidOperationException("Sequence has no elements.");

	/// <summary>
	/// Searches for an element that matches the conditions defined by the specified predicate,
	/// and returns the first occurrence within the entire <see cref="View"/>.
	/// </summary>
	/// <param name="this">The view to be checked.</param>
	/// <param name="match">The <see cref="Func{T, TResult}"/> delegate that defines the conditions of the element to search for.</param>
	/// <returns>
	/// The first element that matches the conditions defined by the specified predicate, if found; otherwise, <see langword="null"/>.
	/// </returns>
	public static ViewNode? FirstOrDefault(this View @this, Func<ViewNode, bool> match)
	{
		foreach (var element in @this)
		{
			if (match(element))
			{
				return element;
			}
		}

		return null;
	}
}
