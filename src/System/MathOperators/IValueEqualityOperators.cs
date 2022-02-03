#if FEATURE_GENERIC_MATH && FEATURE_GENERIC_MATH_IN_ARG
using System.Runtime.Versioning;
using Sudoku.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Defines the equality operators. The operators are:
/// <list type="bullet">
/// <item><see cref="operator ==(in TSelf, in TOther)"/></item>
/// <item><see cref="operator ==(in TSelf, TOther)"/></item>
/// <item><see cref="operator ==(TSelf, in TOther)"/></item>
/// <item><see cref="operator !=(in TSelf, in TOther)"/></item>
/// <item><see cref="operator !=(in TSelf, TOther)"/></item>
/// <item><see cref="operator !=(TSelf, in TOther)"/></item>
/// </list>
/// </summary>
/// <typeparam name="TSelf">The type of the current instance.</typeparam>
/// <typeparam name="TOther">The type that takes part in the operation.</typeparam>
[RequiresPreviewFeatures]
public interface IValueEqualityOperators<[Self] TSelf, TOther>
	where TSelf : struct, IEqualityOperators<TSelf, TOther>, IValueEqualityOperators<TSelf, TOther>
	where TOther : struct
{
	/// <summary>
	/// Check whether two instances of type <typeparamref name="TSelf"/> and <typeparamref name="TOther"/>
	/// are considered equal.
	/// </summary>
	/// <param name="left">The left-side instance to calculate.</param>
	/// <param name="right">The left-side instance to calculate.</param>
	/// <returns>The result value.</returns>
	static abstract bool operator ==(in TSelf left, in TOther right);

	/// <inheritdoc cref="operator ==(in TSelf, in TOther)"/>
	static abstract bool operator ==(in TSelf left, TOther right);

	/// <inheritdoc cref="operator ==(in TSelf, in TOther)"/>
	static abstract bool operator ==(TSelf left, in TOther right);

	/// <summary>
	/// Check whether two instances of type <typeparamref name="TSelf"/> and <typeparamref name="TOther"/>
	/// are not considered equal.
	/// </summary>
	/// <param name="left">The left-side instance to calculate.</param>
	/// <param name="right">The left-side instance to calculate.</param>
	/// <returns>The result value.</returns>
	static abstract bool operator !=(in TSelf left, in TOther right);

	/// <inheritdoc cref="operator !=(in TSelf, in TOther)"/>
	static abstract bool operator !=(in TSelf left, TOther right);

	/// <inheritdoc cref="operator !=(in TSelf, in TOther)"/>
	static abstract bool operator !=(TSelf left, in TOther right);
}

#endif