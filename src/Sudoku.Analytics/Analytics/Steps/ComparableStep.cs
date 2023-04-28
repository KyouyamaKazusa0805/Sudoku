namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a type that operates with <see cref="IComparableStep{TSelf}"/> instances.
/// </summary>
/// <seealso cref="IComparableStep{TSelf}"/>
public static class ComparableStep
{
	/// <summary>
	/// Try to order the collection via the specified comparison rule.
	/// </summary>
	/// <typeparam name="TSelf"><inheritdoc cref="IComparableStep{TSelf}" path="/typeparam[@name='TSelf']"/></typeparam>
	/// <param name="this">A collection.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Order<TSelf>(this List<TSelf> @this) where TSelf : Step, IComparableStep<TSelf> => @this.Sort(TSelf.Compare);
}
