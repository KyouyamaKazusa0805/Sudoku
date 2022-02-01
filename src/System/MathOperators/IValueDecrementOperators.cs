using Sudoku.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Defines the decrement operators. The operators are:
/// <list type="bullet">
/// <item><see cref="operator --(in TSelf)"/></item>
/// </list>
/// </summary>
/// <typeparam name="TSelf">The type of the current instance.</typeparam>
public interface IValueDecrementOperators<[Self] TSelf>
	where TSelf : struct, IDecrementOperators<TSelf>, IValueDecrementOperators<TSelf>
{
	/// <summary>
	/// Decrement the current instance of type <typeparamref name="TSelf"/>.
	/// </summary>
	/// <param name="value">The current value.</param>
	/// <returns>The result value.</returns>
	static abstract TSelf operator --(in TSelf value);
}