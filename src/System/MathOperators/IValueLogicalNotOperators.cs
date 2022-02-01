using Sudoku.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Defines the logical-not operators. The operators are:
/// <list type="bullet">
/// <item><see cref="operator !(in TSelf)"/></item>
/// </list>
/// </summary>
/// <typeparam name="TSelf">The type of the current instance.</typeparam>
public interface IValueLogicalNotOperators<[Self] TSelf> where TSelf : struct, IValueLogicalNotOperators<TSelf>
{
	/// <summary>
	/// Gets the logical-not result value from the instance.
	/// Both are of type <typeparamref name="TSelf"/>.
	/// </summary>
	/// <param name="value">The value itself.</param>
	/// <returns>The result value.</returns>
	static abstract TSelf operator !(in TSelf value);
}