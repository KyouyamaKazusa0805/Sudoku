namespace System;

/// <summary>
/// Defines a mechanism for computing the logical relation between two instances of type <typeparamref name="TSelf"/>.
/// </summary>
/// <typeparam name="TSelf">The implementation.</typeparam>
public interface ILogicalOperators<TSelf>
	where TSelf :
		ILogicalOperators<TSelf>?
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
{
	/// <summary>
	/// Make logical and for two <typeparamref name="TSelf"/> instances.
	/// </summary>
	public static sealed TSelf LogicalAnd(TSelf left, TSelf right) => left && right;

	/// <summary>
	/// Make logical or for two <typeparamref name="TSelf"/> instances.
	/// </summary>
	public static sealed TSelf LogicalOr(TSelf left, TSelf right) => left || right;


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
