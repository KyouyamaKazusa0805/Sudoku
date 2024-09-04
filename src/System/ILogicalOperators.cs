namespace System;

/// <summary>
/// Defines a mechanism for computing the logical relation between two instances of type <typeparamref name="TSelf"/>.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
public interface ILogicalOperators<TSelf> where TSelf : ILogicalOperators<TSelf>?, allows ref struct
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

	/// <summary>
	/// Equivalent to <c><![CDATA[false(left) ? left : false(left & right)]]></c>.
	/// </summary>
	/// <param name="left">The left-side value.</param>
	/// <param name="right">The right-side value.</param>
	/// <returns>A instance of type <typeparamref name="TSelf"/>, as the result value.</returns>
	public static abstract TSelf operator &(TSelf left, TSelf right);

	/// <summary>
	/// Equivalent to <c><![CDATA[true(left) ? left : true(left | right)]]></c>.
	/// </summary>
	/// <param name="left">The left-side value.</param>
	/// <param name="right">The right-side value.</param>
	/// <returns><inheritdoc cref="op_BitwiseAnd(TSelf, TSelf)" path="/returns"/></returns>
	public static abstract TSelf operator |(TSelf left, TSelf right);

	/// <summary>
	/// Equivalent to <c><![CDATA[left & ~right | ~left & right]]></c>.
	/// </summary>
	/// <param name="left">The left-side value.</param>
	/// <param name="right">The right-side value.</param>
	/// <returns><inheritdoc cref="op_BitwiseAnd(TSelf, TSelf)" path="/returns"/></returns>
	public static abstract TSelf operator ^(TSelf left, TSelf right);
}
