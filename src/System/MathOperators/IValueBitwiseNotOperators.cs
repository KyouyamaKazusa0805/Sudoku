#if FEATURE_GENERIC_MATH && FEATURE_GENERIC_MATH_IN_ARG
using System.Runtime.Versioning;
using Sudoku.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Defines the bitwise-not operators. The operators are:
/// <list type="bullet">
/// <item><see cref="operator ~(in TSelf)"/></item>
/// </list>
/// </summary>
/// <typeparam name="TSelf">The type of the current instance.</typeparam>
/// <typeparam name="TResult">The type of the result value.</typeparam>
[RequiresPreviewFeatures]
public interface IValueBitwiseNotOperators<[Self] TSelf, TResult>
	where TSelf : struct, IValueBitwiseNotOperators<TSelf, TResult>
	where TResult : struct
{
	/// <summary>
	/// Gets the bitwise-not result value of type <typeparamref name="TResult"/>
	/// from the instance of type <typeparamref name="TSelf"/>.
	/// </summary>
	/// <param name="value">The value itself.</param>
	/// <returns>The result value.</returns>
	static abstract TResult operator ~(in TSelf value);
}

#endif