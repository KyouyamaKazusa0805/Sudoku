using Sudoku.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Defines the subtraction operators. The operators are:
/// <list type="bullet">
/// <item><see cref="operator -(in TSelf, in TOther)"/></item>
/// <item><see cref="operator -(in TSelf, TOther)"/></item>
/// <item><see cref="operator -(TSelf, in TOther)"/></item>
/// </list>
/// </summary>
/// <typeparam name="TSelf">The type of the current instance.</typeparam>
/// <typeparam name="TOther">The type that takes part in the operation.</typeparam>
/// <typeparam name="TResult">The type of the result value.</typeparam>
public interface IValueSubtractionOperators<[Self] TSelf, TOther, TResult>
	where TSelf :
		struct,
		ISubtractionOperators<TSelf, TOther, TResult>,
		IValueSubtractionOperators<TSelf, TOther, TResult>
	where TOther : struct
	where TResult : struct
{
	/// <summary>
	/// Subtracts the instance of type <typeparamref name="TOther"/> from the instance
	/// of type <typeparamref name="TSelf"/>, and gets the result of type <typeparamref name="TResult"/>.
	/// </summary>
	/// <param name="left">The left-side instance to calculate.</param>
	/// <param name="right">The left-side instance to calculate.</param>
	/// <returns>The result value.</returns>
	static abstract TResult operator -(in TSelf left, in TOther right);

	/// <inheritdoc cref="operator -(in TSelf, in TOther)"/>
	static abstract TResult operator -(in TSelf left, TOther right);

	/// <inheritdoc cref="operator -(in TSelf, in TOther)"/>
	static abstract TResult operator -(TSelf left, in TOther right);
}
