﻿namespace Sudoku.Filtering.Operators;

/// <summary>
/// Indicates the conclusion matching operator <c>has</c>.
/// The expected usage is like <c>conclusion has 'r3c4'</c>.
/// </summary>
public sealed class ConclusionHasOperator : OperatorBase
{
	/// <summary>
	/// Indicates the singleton instance.
	/// </summary>
	public static readonly ConclusionHasOperator Instance = new();


	/// <summary>
	/// Initializes a <see cref="ConclusionHasOperator"/> instance.
	/// </summary>
	private ConclusionHasOperator()
	{
	}


	/// <inheritdoc/>
	public override IEnumerable<string> Tags => new[] { "has" };


	/// <inheritdoc/>
	public override IExpression BuildExpression(Token previousToken, IExpression[] expressions, Context context)
		=> new ConclusionHasExpression(expressions[0], expressions[1], context);

	/// <inheritdoc/>
	public override OperatorPrecedence GetPrecedence(Token previousToken) => OperatorPrecedence.Equal;
}
