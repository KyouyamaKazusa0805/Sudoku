namespace Sudoku.Filtering.Expressions;

/// <summary>
/// Defines the backing implementation of <c><see langword="has"/></c> operator.
/// </summary>
[Obsolete("This type will be removed in the future.", false)]
public sealed class ConclusionHasExpression : BinaryExpressionBase
{
	/// <summary>
	/// Initializes a <see cref="ConclusionHasExpression"/> instance via two expressions, and the inner handling context.
	/// </summary>
	/// <param name="left">The left-side expression.</param>
	/// <param name="right">The right-side expression.</param>
	/// <param name="context">The inner handling context.</param>
	internal ConclusionHasExpression(IExpression left, IExpression right, Context context) : base(left, right, context)
	{
	}


	/// <inheritdoc/>
	protected override object EvaluateImpl(object lhsResult, IExpression rightHandSide, IDictionary<string, object> variables)
	{
		var left = ((string)lhsResult).Split(", ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		var right = (string)rightHandSide.Evaluate(variables);
		return left.Any(s => s.Contains(right));
	}
}
