using System.Runtime.CompilerServices;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a type that operates with <see cref="IComparableStep{TSelf}"/> instances.
/// </summary>
/// <seealso cref="IComparableStep{TSelf}"/>
public static class ComparableStep
{
	/// <inheritdoc cref="Order{TSelf}(List{TSelf})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Order<TSelf>(TSelf[] @this) where TSelf : Step, IComparableStep<TSelf> => Array.Sort(@this, TSelf.Compare);

	/// <summary>
	/// Try to order the <typeparamref name="TSelf"/> collection via the specified comparison rule,
	/// if type argument <typeparamref name="TSelf"/> implements <see cref="IComparableStep{TSelf}"/>.
	/// </summary>
	/// <typeparam name="TSelf"><inheritdoc cref="IComparableStep{TSelf}" path="/typeparam[@name='TSelf']"/></typeparam>
	/// <param name="this">A collection.</param>
	/// <seealso cref="Step"/>
	/// <seealso cref="IComparableStep{TSelf}"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Order<TSelf>(List<TSelf> @this) where TSelf : Step, IComparableStep<TSelf> => @this.Sort(TSelf.Compare);
}
