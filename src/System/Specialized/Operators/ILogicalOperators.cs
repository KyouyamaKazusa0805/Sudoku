namespace System;

/// <summary>
/// Represents for an abstraction unit that supports for boolean logical operators
/// <see langword="operator"/> <![CDATA[&&]]> and <see langword="operator"/> <![CDATA[||]]>.
/// </summary>
/// <typeparam name="TSelf">The implementation.</typeparam>
public interface ILogicalOperators<TSelf> where TSelf : ILogicalOperators<TSelf>?
{
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
	/// <returns>A <typeparamref name="TSelf"/> result indicating the calculation result.</returns>
	public static abstract TSelf operator &(TSelf left, TSelf right);

	/// <summary>
	/// Equivalent to <c><![CDATA[true(left) ? left : false(left | right)]]></c>.
	/// </summary>
	/// <param name="left">The left-side value.</param>
	/// <param name="right">The right-side value.</param>
	/// <returns>A <typeparamref name="TSelf"/> result indicating the calculation result.</returns>
	public static abstract TSelf operator |(TSelf left, TSelf right);

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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static virtual bool operator false(TSelf value) => !(value ? true : false);
}
