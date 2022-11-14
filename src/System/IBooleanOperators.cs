namespace System;

/// <summary>
/// Represents a type whose instance of this type can be used as a predicate-like expression.
/// </summary>
/// <typeparam name="TSelf">The type of the target value.</typeparam>
public interface IBooleanOperators<[Self] in TSelf> where TSelf : IBooleanOperators<TSelf>
{
	/// <summary>
	/// Indicates whether the specified value of type <typeparamref name="TSelf"/> satisfies a fixed condition.
	/// </summary>
	/// <param name="value">The value of type <typeparamref name="TSelf"/>.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	static abstract bool operator true(TSelf value);

	/// <summary>
	/// Indicates whether the specified value of type <typeparamref name="TSelf"/> doesn't satisfy a fixed condition.
	/// </summary>
	/// <param name="value">The value of type <typeparamref name="TSelf"/>.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	static virtual bool operator false(TSelf value) => value ? false : true;
}
