namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a type that can be used for comparison on two <typeparamref name="TSelf"/> instances.
/// </summary>
/// <typeparam name="TSelf">The type of the comparison object. The type must be derived from <see cref="Step"/>.</typeparam>
/// <seealso cref="Step"/>
public interface IEquatableStep<TSelf> where TSelf : Step, IEquatableStep<TSelf>
{
	/// <summary>
	/// Determines whether two <typeparamref name="TSelf"/> instances are considered equal.
	/// </summary>
	/// <param name="left">The first element to be compared.</param>
	/// <param name="right">The second element to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating whether two <typeparamref name="TSelf"/> instances are considered equal.</returns>
	static abstract bool operator ==(TSelf left, TSelf right);

	/// <summary>
	/// Determines whether two <typeparamref name="TSelf"/> instances are not considered equal.
	/// </summary>
	/// <param name="left">The first element to be compared.</param>
	/// <param name="right">The second element to be compared.</param>
	/// <returns>A <see cref="bool"/> result indicating whether two <typeparamref name="TSelf"/> instances are not considered equal.</returns>
	static virtual bool operator !=(TSelf left, TSelf right) => !(left == right);
}
