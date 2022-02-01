using Sudoku.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Defines the bitwise-or operators. The operators are:
/// <list type="bullet">
/// <item><see cref="operator |(in TSelf, in TOther)"/></item>
/// <item><see cref="operator |(in TSelf, TOther)"/></item>
/// <item><see cref="operator |(TSelf, in TOther)"/></item>
/// </list>
/// </summary>
/// <typeparam name="TSelf">The type of the current instance.</typeparam>
/// <typeparam name="TOther">The type that takes part in the operation.</typeparam>
/// <typeparam name="TResult">The type of the result value.</typeparam>
public interface IValueBitwiseOrOperators<[Self] TSelf, TOther, TResult>
	where TSelf : struct, IValueBitwiseOrOperators<TSelf, TOther, TResult>
	where TOther : struct
	where TResult : struct
{
	/// <summary>
	/// Make bitwise-or operation using two instances of type <typeparamref name="TSelf"/>
	/// and <typeparamref name="TOther"/>, and return the result value of type <typeparamref name="TResult"/>.
	/// </summary>
	/// <param name="left">The left-side instance to calculate.</param>
	/// <param name="right">The left-side instance to calculate.</param>
	/// <returns>The result value.</returns>
	static abstract TResult operator |(in TSelf left, in TOther right);

	/// <inheritdoc cref="operator |(in TSelf, in TOther)"/>
	static abstract TResult operator |(in TSelf left, TOther right);

	/// <inheritdoc cref="operator |(in TSelf, in TOther)"/>
	static abstract TResult operator |(TSelf left, in TOther right);
}
