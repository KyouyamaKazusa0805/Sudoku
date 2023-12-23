namespace Sudoku.Filtering.Operators;

/// <summary>
/// Indicates the name matching operator <c>like</c>.
/// The expected usage is like <c>name like 'Unique\s+Rectangle.*'</c>.
/// </summary>
public sealed class NameLikeOperator : OperatorBase
{
	/// <summary>
	/// Indicates the singleton instance.
	/// </summary>
	public static readonly NameLikeOperator Instance = new();


	/// <inheritdoc/>
	public override IEnumerable<string> Tags => ["like"];


	/// <inheritdoc/>
	public override IExpression BuildExpression(Token previousToken, IExpression[] expressions, Context context)
		=> new NameLikeExpression(expressions[0], expressions[1], context);

	/// <inheritdoc/>
	public override OperatorPrecedence GetPrecedence(Token previousToken) => OperatorPrecedence.Equal;
}
