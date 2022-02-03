#if FEATURE_GENERIC_MATH && FEATURE_GENERIC_MATH_IN_ARG
using System.Runtime.Versioning;
using Sudoku.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Defines the shift operators. The operators are:
/// <list type="bullet">
/// <item><see cref="operator }}(in TSelf, int)"/></item>
/// <item><see cref="operator {{(in TSelf, int)"/></item>
/// </list>
/// </summary>
/// <typeparam name="TSelf">The type of the current instance.</typeparam>
/// <typeparam name="TResult">The type of the result value.</typeparam>
[RequiresPreviewFeatures]
public interface IValueShiftOperators<[Self] TSelf, TResult>
	where TSelf : struct, IShiftOperators<TSelf, TResult>, IValueShiftOperators<TSelf, TResult>
	where TResult : struct
{
	/// <summary>
	/// Gets the right-shift result from the specified value of type <typeparamref name="TSelf"/>
	/// and the shift amount.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="shiftAmount">The amount of the shift.</param>
	/// <returns>The result value.</returns>
	static abstract TResult operator >>(in TSelf value, int shiftAmount);

	/// <summary>
	/// Gets the left-shift result from the specified value of type <typeparamref name="TSelf"/>
	/// and the shift amount.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="shiftAmount">The amount of the shift.</param>
	/// <returns>The result value.</returns>
	static abstract TResult operator <<(in TSelf value, int shiftAmount);
}

#endif