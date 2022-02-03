#if FEATURE_GENERIC_MATH && FEATURE_GENERIC_MATH_IN_ARG
using System.Runtime.Versioning;
using Sudoku.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Defines the logical operators. The operators are:
/// <list type="bullet">
/// <item><see cref="IValueLogicalNotOperators{TSelf}.operator !(in TSelf)"/></item>
/// <item><see cref="IValueLogicalAndOperators{TSelf}.operator &amp;(in TSelf, in TSelf)"/></item>
/// <item><see cref="IValueLogicalAndOperators{TSelf}.operator &amp;(in TSelf, TSelf)"/></item>
/// <item><see cref="IValueLogicalAndOperators{TSelf}.operator &amp;(TSelf, in TSelf)"/></item>
/// <item><see cref="IValueLogicalOrOperators{TSelf}.operator |(in TSelf, in TSelf)"/></item>
/// <item><see cref="IValueLogicalOrOperators{TSelf}.operator |(in TSelf, TSelf)"/></item>
/// <item><see cref="IValueLogicalOrOperators{TSelf}.operator |(TSelf, in TSelf)"/></item>
/// <item><see cref="IValueLogicalExclusiveOrOperators{TSelf}.operator ^(in TSelf, in TSelf)"/></item>
/// <item><see cref="IValueLogicalExclusiveOrOperators{TSelf}.operator ^(in TSelf, TSelf)"/></item>
/// <item><see cref="IValueLogicalExclusiveOrOperators{TSelf}.operator ^(TSelf, in TSelf)"/></item>
/// <item><see cref="IValueMetaLogicalOperators{TSelf}.operator true(in TSelf)"/></item>
/// <item><see cref="IValueMetaLogicalOperators{TSelf}.operator false(in TSelf)"/></item>
/// </list>
/// Those operators will be helpful to overload <c><see langword="operator"/> <![CDATA[&&]]></c>
/// or <c><see langword="operator"/> <![CDATA[||]]></c>, where:
/// <list type="bullet">
/// <item>
/// <c><see langword="operator"/> <![CDATA[&&]]></c> is equivalent to <c><![CDATA[false(a) ? a : a & b]]></c>.
/// </item>
/// <item>
/// <c><see langword="operator"/> <![CDATA[||]]></c> is equivalent to <c><![CDATA[true(a) ? a : a | b]]></c>.
/// </item>
/// </list>
/// </summary>
/// <typeparam name="TSelf">The type of the current instance.</typeparam>
[RequiresPreviewFeatures]
public interface IValueLogicalOperators<[Self] TSelf>
	where TSelf :
		struct,
		IValueLogicalNotOperators<TSelf>,
		IValueLogicalAndOperators<TSelf>,
		IValueLogicalOrOperators<TSelf>,
		IValueLogicalExclusiveOrOperators<TSelf>,
		IValueLogicalOperators<TSelf>,
		IValueMetaLogicalOperators<TSelf>
{
}

#endif