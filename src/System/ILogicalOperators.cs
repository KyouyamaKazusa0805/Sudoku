namespace System;

/// <summary>
/// Defines a mechanism for computing the logical relation between two instances of type <typeparamref name="TSelf"/>.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
public interface ILogicalOperators<TSelf> : IBitwiseOperators<TSelf, TSelf, TSelf> where TSelf : ILogicalOperators<TSelf>?
{
	/// <summary>
	/// Determine whether the specified object is determined <see langword="true"/>.
	/// </summary>
	/// <param name="value">The value to be determined.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static abstract bool operator true(TSelf value);

	/// <summary>
	/// Determine whether the specified object is determined <see langword="false"/>.
	/// </summary>
	/// <param name="value">The value to be determined.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static virtual bool operator false(TSelf value) => !(value ? true : false);

	/// <summary>
	/// Negates the current instance, and makes the result to be negated one.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static abstract bool operator !(TSelf value);
}
