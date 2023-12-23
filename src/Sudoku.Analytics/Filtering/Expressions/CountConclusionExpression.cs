namespace Sudoku.Filtering.Expressions;

/// <summary>
/// Defines the backing implementation of <c><see langword="count"/></c> operator.
/// </summary>
public sealed class CountConclusionExpression : UnaryExpressionBase
{
	/// <summary>
	/// Initializes a <see cref="ConclusionHasExpression"/> instance via an expression.
	/// </summary>
	/// <param name="expression">The operated expression.</param>
	internal CountConclusionExpression(IExpression expression) : base(expression)
	{
	}


	/// <inheritdoc/>
	public override object Evaluate(IDictionary<string, object> variables)
	{
		var value = ((string)_expression.Evaluate(variables)).Split(", ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		return value.Length;
	}
}
