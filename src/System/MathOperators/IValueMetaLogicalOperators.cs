#if FEATURE_GENERIC_MATH && FEATURE_GENERIC_MATH_IN_ARG
using System.Runtime.Versioning;
using Sudoku.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Defines the meta logical operators. The operators are:
/// <list type="bullet">
/// <item><see cref="operator true(in TSelf)"/></item>
/// <item><see cref="operator false(in TSelf)"/></item>
/// </list>
/// </summary>
/// <typeparam name="TSelf">The type of the current instance.</typeparam>
/// <remarks>
/// The meta logical operators can be used as a part of the full expansion
/// of operators <c><![CDATA[&&]]></c> and <c><![CDATA[||]]></c>. For more information, you can visit
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/true-false-operators">
/// this link
/// </see>.
/// </remarks>
[RequiresPreviewFeatures]
public interface IValueMetaLogicalOperators<[Self] TSelf>
	where TSelf : struct, IValueMetaLogicalOperators<TSelf>
{
	/// <summary>
	/// <para>
	/// Checks whether the current instance is at the <see langword="true"/> status.
	/// The operator will be invoked by <c><![CDATA[operator ||]]></c>, which cannot be overloadable directly.
	/// </para>
	/// <para>
	/// The expression <c><![CDATA[a || b]]></c> will be expanded to <c><![CDATA[false(a) ? a : a | b]]></c>,
	/// where the variable <c>a</c> corresponds to the arguement <paramref name="value"/>.
	/// </para>
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>
	/// The <see cref="bool"/> result indicating whether the current instance is
	/// at the <see langword="true"/> status.
	/// </returns>
	static abstract bool operator true(in TSelf value);

	/// <summary>
	/// <para>
	/// Checks whether the current instance is at the <see langword="false"/> status.
	/// The operator will be invoked by <c><![CDATA[operator &&]]></c>, which cannot be overloadable directly.
	/// </para>
	/// <para>
	/// The expression <c><![CDATA[a && b]]></c> will be expanded to <c><![CDATA[false(a) ? a : a & b]]></c>,
	/// where the variable <c>a</c> corresponds to the arguement <paramref name="value"/>.
	/// </para>
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>
	/// The <see cref="bool"/> result indicating whether the current instance is
	/// at the <see langword="false"/> status.
	/// </returns>
	static abstract bool operator false(in TSelf value);
}

#endif