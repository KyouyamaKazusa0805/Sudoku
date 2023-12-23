namespace Sudoku.Filtering.Expressions;

/// <summary>
/// Defines the backing implementation of <c><see langword="like"/></c> operator.
/// </summary>
public sealed class NameLikeExpression : BinaryExpressionBase
{
	/// <summary>
	/// Initializes a <see cref="NameLikeExpression"/> instance via two expressions, and the inner handling context.
	/// </summary>
	/// <param name="left">The left-side expression.</param>
	/// <param name="right">The right-side expression.</param>
	/// <param name="context">The inner handling context.</param>
	internal NameLikeExpression(IExpression left, IExpression right, Context context) : base(left, right, context)
	{
	}


	/// <inheritdoc/>
	protected override object EvaluateImpl(object lhsResult, IExpression rightHandSide, IDictionary<string, object> variables)
	{
		var left = (string)lhsResult;
		var right = (string)rightHandSide.Evaluate(variables);
		return right.IsRegexPattern() && Regex.IsMatch(left, right, RegexOptions.IgnoreCase);
	}
}
