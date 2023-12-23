namespace Sudoku.Filtering.Operators;

/// <summary>
/// Indicates the counter operator <c>count</c> to total up the number of conclusions in a single technique step.
/// The expected usage is like <c>count conclusion</c>.
/// </summary>
public sealed class CountConclusionOperator : OperatorBase
{
	/// <summary>
	/// Indicates the singleton instance.
	/// </summary>
	public static readonly CountConclusionOperator Instance = new();


	/// <inheritdoc/>
	public override IEnumerable<string> Tags => ["count"];


	/// <inheritdoc/>
	public override IExpression BuildExpression(Token previousToken, IExpression[] expressions, Context context)
		=> new CountConclusionExpression(expressions[0] ?? expressions[1]);

	/// <inheritdoc/>
	public override OperatorPrecedence GetPrecedence(Token previousToken) => OperatorPrecedence.Not;
}
