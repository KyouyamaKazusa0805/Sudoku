namespace System;

/// <summary>
/// Represents a type that supports <see langword="operator"/> <c><![CDATA[&&]]></c>
/// and <see langword="operator"/> <c><![CDATA[||]]></c>.
/// </summary>
/// <typeparam name="TSelf">The type of the value.</typeparam>
/// <typeparam name="TOther">The other value to be used for computing the result.</typeparam>
/// <typeparam name="TResult">The result of the value.</typeparam>
public interface ILogicalOperators<[Self] in TSelf, in TOther, out TResult>
	where TSelf : IBooleanOperators<TSelf>, ILogicalOperators<TSelf, TOther, TResult>
{
	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_BitwiseAnd(TSelf, TOther)"/>
	public static abstract TResult operator &(TSelf value, TOther other);

	/// <inheritdoc cref="IBitwiseOperators{TSelf, TOther, TResult}.op_BitwiseOr(TSelf, TOther)"/>
	public static abstract TResult operator |(TSelf value, TOther other);
}
