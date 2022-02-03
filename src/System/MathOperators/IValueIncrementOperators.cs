#if FEATURE_GENERIC_MATH && FEATURE_GENERIC_MATH_IN_ARG
using System.Runtime.Versioning;
using Sudoku.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Defines the increment operators. The operators are:
/// <list type="bullet">
/// <item><see cref="operator ++(in TSelf)"/></item>
/// </list>
/// </summary>
/// <typeparam name="TSelf">The type of the current instance.</typeparam>
[RequiresPreviewFeatures]
public interface IValueIncrementOperators<[Self] TSelf>
	where TSelf : struct, IIncrementOperators<TSelf>, IValueIncrementOperators<TSelf>
{
	/// <summary>
	/// Increment the current instance of type <typeparamref name="TSelf"/>.
	/// </summary>
	/// <param name="value">The current value.</param>
	/// <returns>The result value.</returns>
	static abstract TSelf operator ++(in TSelf value);
}

#endif