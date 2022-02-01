using Sudoku.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Defines the greater-than-or-equals and less-than-or-equals operators. The operators are:
/// <list type="bullet">
/// <item><see cref="operator }=(in TSelf, in TSelf)"/></item>
/// <item><see cref="operator }=(in TSelf, TSelf)"/></item>
/// <item><see cref="operator }=(TSelf, in TSelf)"/></item>
/// <item><see cref="operator {=(in TSelf, in TSelf)"/></item>
/// <item><see cref="operator {=(in TSelf, TSelf)"/></item>
/// <item><see cref="operator {=(TSelf, in TSelf)"/></item>
/// </list>
/// </summary>
/// <typeparam name="TSelf">The type of the current instance.</typeparam>
/// <typeparam name="TOther">The type that takes part in the operation.</typeparam>
public interface IValueGreaterThanOrEqualsOrLessThanOrEqualsOperators<[Self] TSelf, TOther>
	where TSelf : struct, IValueGreaterThanOrEqualsOrLessThanOrEqualsOperators<TSelf, TOther>
	where TOther : struct
{
	/// <summary>
	/// Compare two instances of type <typeparamref name="TSelf"/> and <typeparamref name="TOther"/>,
	/// and gets the result of which one is greater. If the argument <paramref name="left"/> is greater
	/// or equals to the argument <paramref name="right"/>, the return value will be <see langword="true"/>;
	/// otherwise, <see langword="false"/>.
	/// </summary>
	/// <param name="left">The left-side instance to calculate.</param>
	/// <param name="right">The left-side instance to calculate.</param>
	/// <returns>The result value.</returns>
	static abstract bool operator >=(in TSelf left, in TSelf right);

	/// <inheritdoc cref="operator {=(in TSelf, in TSelf)"/>
	static abstract bool operator >=(in TSelf left, TSelf right);

	/// <inheritdoc cref="operator {=(in TSelf, in TSelf)"/>
	static abstract bool operator >=(TSelf left, in TSelf right);

	/// <summary>
	/// Compare two instances of type <typeparamref name="TSelf"/> and <typeparamref name="TOther"/>,
	/// and gets the result of which one is less. If the argument <paramref name="left"/> is less
	/// or equals to the argument <paramref name="right"/>, the return value will be <see langword="true"/>;
	/// otherwise, <see langword="false"/>.
	/// </summary>
	/// <param name="left">The left-side instance to calculate.</param>
	/// <param name="right">The left-side instance to calculate.</param>
	/// <returns>The result value.</returns>
	static abstract bool operator <=(in TSelf left, in TSelf right);

	/// <inheritdoc cref="operator }=(in TSelf, in TSelf)"/>
	static abstract bool operator <=(in TSelf left, TSelf right);

	/// <inheritdoc cref="operator }=(in TSelf, in TSelf)"/>
	static abstract bool operator <=(TSelf left, in TSelf right);
}
