namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a type that can be used for comparison on two <typeparamref name="TSelf"/> instances.
/// </summary>
/// <typeparam name="TSelf">The type of the comparison object. The type must be derived from <see cref="Step"/>.</typeparam>
/// <seealso cref="Step"/>
public interface IComparableStep<in TSelf> where TSelf : Step, IComparableStep<TSelf>
{
	/// <summary>
	/// Compares two <typeparamref name="TSelf"/> instance, and returns an <see cref="int"/> indicating which value is greater.
	/// </summary>
	/// <param name="left">The left-side value to be compared.</param>
	/// <param name="right">The right-side value to be compared.</param>
	/// <returns>An <see cref="int"/> value indicating which is greater.</returns>
	/// <inheritdoc/>
	static abstract int Compare(TSelf left, TSelf right);
}
