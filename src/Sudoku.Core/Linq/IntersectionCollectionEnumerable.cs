namespace Sudoku.Linq;

/// <summary>
/// Represents a list of methods that is used as LINQ methods on type <see cref="IntersectionCollection"/>.
/// </summary>
/// <seealso cref="IntersectionCollection"/>
public static class IntersectionCollectionEnumerable
{
	/// <summary>
	/// Projects each element into a new transform using the specified method to transform.
	/// </summary>
	/// <typeparam name="TResult">The type of result elements.</typeparam>
	/// <param name="this">The instance to be checked..</param>
	/// <param name="selector">The transform method.</param>
	/// <returns>An array of <typeparamref name="TResult"/> results.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TResult[] Select<TResult>(this IntersectionCollection @this, Func<Intersection, TResult> selector)
		=> from e in @this._values select selector(e);
}
