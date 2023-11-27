using System.Runtime.CompilerServices;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a type that operates with <see cref="IEquatableStep{TSelf}"/> instances.
/// </summary>
/// <seealso cref="IEquatableStep{TSelf}"/>
public static class EquatableStep
{
	/// <summary>
	/// Try to compare elements, removing all duplicated ones in this collection,
	/// which uses the operator <see cref="IEquatableStep{TSelf}.op_Equality(TSelf, TSelf)"/> defined in this interface.
	/// </summary>
	/// <param name="steps">The list of steps to be processed.</param>
	/// <returns>The list of steps.</returns>
	/// <remarks>
	/// This method does not change the ordering of the original list.
	/// In other words, if the original list is in order, the final list after invoking this method will be also in order.
	/// </remarks>
	/// <seealso cref="IEquatableStep{TSelf}.op_Equality(TSelf, TSelf)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IEnumerable<TSelf> Distinct<TSelf>(List<TSelf> steps) where TSelf : Step, IEquatableStep<TSelf>
		=> steps switch
		{
			[] => [],
			[var firstElement] => [firstElement],
			[var a, var b] => a == b ? [a] : [a, b],
			_ => new HashSet<TSelf>(steps, new EqualityComparer<TSelf>())
		};
}

#nullable disable warnings
/// <summary>
/// The internal comparer type for <typeparamref name="T"/> instances.
/// </summary>
/// <typeparam name="T">The type of the step.</typeparam>
file sealed class EqualityComparer<T> : IEqualityComparer<T> where T : Step, IEquatableStep<T>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(T x, T y) => x == y;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetHashCode(T obj) => 0;
}
#nullable restore warnings
