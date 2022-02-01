using Sudoku.Diagnostics.CodeAnalysis;

namespace System;

/// <summary>
/// Defines the bitwise operators. The operators are:
/// <list type="bullet">
/// <item><see cref="IValueBitwiseNotOperators{TSelf, TResult}.operator ~(in TSelf)"/></item>
/// <item><see cref="IValueBitwiseAndOperators{TSelf, TOther, TResult}.operator &amp;(in TSelf, in TOther)"/></item>
/// <item><see cref="IValueBitwiseAndOperators{TSelf, TOther, TResult}.operator &amp;(in TSelf, TOther)"/></item>
/// <item><see cref="IValueBitwiseAndOperators{TSelf, TOther, TResult}.operator &amp;(TSelf, in TOther)"/></item>
/// <item><see cref="IValueBitwiseOrOperators{TSelf, TOther, TResult}.operator |(in TSelf, in TOther)"/></item>
/// <item><see cref="IValueBitwiseOrOperators{TSelf, TOther, TResult}.operator |(in TSelf, TOther)"/></item>
/// <item><see cref="IValueBitwiseOrOperators{TSelf, TOther, TResult}.operator |(TSelf, in TOther)"/></item>
/// <item><see cref="IValueBitwiseExclusiveOrOperators{TSelf, TOther, TResult}.operator ^(in TSelf, in TOther)"/></item>
/// <item><see cref="IValueBitwiseExclusiveOrOperators{TSelf, TOther, TResult}.operator ^(in TSelf, TOther)"/></item>
/// <item><see cref="IValueBitwiseExclusiveOrOperators{TSelf, TOther, TResult}.operator ^(TSelf, in TOther)"/></item>
/// </list>
/// </summary>
/// <typeparam name="TSelf">The type of the current instance.</typeparam>
/// <typeparam name="TOther">The type that takes part in the operation.</typeparam>
/// <typeparam name="TResult">The type of the result value.</typeparam>
public interface IValueBitwiseOperators<[Self] TSelf, TOther, TResult>
	where TSelf :
		struct,
		IValueBitwiseNotOperators<TSelf, TResult>,
		IValueBitwiseAndOperators<TSelf, TOther, TResult>,
		IValueBitwiseOrOperators<TSelf, TOther, TResult>,
		IValueBitwiseExclusiveOrOperators<TSelf, TOther, TResult>,
		IBitwiseOperators<TSelf, TOther, TResult>,
		IValueBitwiseOperators<TSelf, TOther, TResult>
	where TOther : struct
	where TResult : struct
{
}