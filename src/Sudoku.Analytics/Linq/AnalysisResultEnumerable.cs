namespace Sudoku.Linq;

/// <summary>
/// Provides with some LINQ methods for type <see cref="AnalysisResult"/>.
/// </summary>
/// <seealso cref="AnalysisResult"/>
public static class AnalysisResultEnumerable
{
	/// <summary>
	/// Determines whether all <see cref="Step"/> instances satisfy the specified condition.
	/// </summary>
	/// <param name="this">The instance to be checked.</param>
	/// <param name="predicate">The match method.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool All(this AnalysisResult @this, Func<Step, bool> predicate)
	{
		foreach (var step in @this)
		{
			if (!predicate(step))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Determines whether at least one <see cref="Step"/> instance satisfies the specified condition.
	/// </summary>
	/// <param name="this">The instance to be checked.</param>
	/// <param name="predicate">The match method.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool Any(this AnalysisResult @this, Func<Step, bool> predicate)
	{
		foreach (var step in @this)
		{
			if (predicate(step))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Cast the object into a <see cref="ReadOnlySpan{T}"/> of <typeparamref name="T"/> instances.
	/// </summary>
	/// <typeparam name="T">The type of each element casted.</typeparam>
	/// <param name="this">The instance to be casted.</param>
	/// <returns>A list of <typeparamref name="T"/> instances.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlySpan<T> Cast<T>(this AnalysisResult @this) where T : Step => from element in @this select (T)element;

	/// <summary>
	/// Filters the current collection, preserving <see cref="Step"/> instances that are satisfied the specified condition.
	/// </summary>
	/// <param name="this">The instance.</param>
	/// <param name="condition">The condition to be satisfied.</param>
	/// <returns>An array of <see cref="Step"/> instances.</returns>
	public static ReadOnlySpan<Step> Where(this AnalysisResult @this, Func<Step, bool> condition)
	{
		if (@this.InterimSteps is not { Length: var stepsCount } steps)
		{
			return [];
		}

		var result = new List<Step>(stepsCount);
		foreach (var step in steps)
		{
			if (condition(step))
			{
				result.Add(step);
			}
		}

		return result.AsReadOnlySpan();
	}

	/// <summary>
	/// Projects the collection, to an immutable result of target type.
	/// </summary>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	/// <param name="this">The instance.</param>
	/// <param name="selector">
	/// The selector to project the <see cref="Step"/> instance into type <typeparamref name="TResult"/>.
	/// </param>
	/// <returns>The projected collection of element type <typeparamref name="TResult"/>.</returns>
	public static ReadOnlySpan<TResult> Select<TResult>(this AnalysisResult @this, Func<Step, TResult> selector)
	{
		if (@this.InterimSteps is not { Length: var stepsCount } steps)
		{
			return [];
		}

		var arr = new TResult[stepsCount];
		var i = 0;
		foreach (var step in steps)
		{
			arr[i++] = selector(step);
		}

		return arr;
	}

	/// <summary>
	/// Filters the current collection, preserving steps that are of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the step you want to get.</typeparam>
	/// <param name="this">The instance.</param>
	/// <returns>An array of <typeparamref name="T"/> instances.</returns>
	public static ReadOnlySpan<T> OfType<T>(this AnalysisResult @this) where T : Step
	{
		if (@this.InterimSteps is not { Length: var stepsCount } steps)
		{
			return [];
		}

		var list = new List<T>(stepsCount);
		foreach (var element in steps)
		{
			if (element is T current)
			{
				list.Add(current);
			}
		}

		return list.AsReadOnlySpan();
	}
}
