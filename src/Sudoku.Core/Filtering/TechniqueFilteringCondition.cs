namespace Sudoku.Filtering;

/// <summary>
/// Defines a way that is used for filtering the gathered step after
/// <see cref="IStepGatherableSearcher.Search(in Grid, IProgress{double}, CancellationToken)"/> invoked.
/// </summary>
/// <seealso cref="IStepGatherableSearcher"/>
public static partial class TechniqueFiltering
{
	/// <summary>
	/// Indicates the difficulty keyword.
	/// </summary>
	private const string DifficultyKeyword = "difficulty";

	/// <summary>
	/// Indicates the name keyword.
	/// </summary>
	private const string NameKeyword = "name";


	/// <summary>
	/// Filters some invalid steps.
	/// </summary>
	/// <param name="steps">The found steps.</param>
	/// <param name="conditionString">The condition.</param>
	/// <exception cref="ExpressiveException">Throws when the expression being evaluated is invalid.</exception>
	public static IEnumerable<IStep> Filter(IEnumerable<IStep> steps, string conditionString)
		=> string.IsNullOrWhiteSpace(conditionString)
			// Special case: If the condition string is empty, just return.
			? steps
			: from step in steps where Parse(conditionString, step).Evaluate<bool>() select step;

	/// <summary>
	/// To parse a condition string, converting the string into a valid <see cref="Expression"/> instance.
	/// </summary>
	/// <param name="conditionString">The condition string.</param>
	/// <param name="step">The step used.</param>
	/// <returns>The <see cref="Expression"/> instance.</returns>
	private static Expression Parse(string conditionString, IStep step)
	{
		var result = new Expression(
			KeywordPattern()
				.Replace(
					conditionString,
					match => match.Value.ToLower() switch
					{
						DifficultyKeyword => $"{step.Difficulty}",
						NameKeyword => $"'{step.Name}'",
						_ => throw new NotSupportedException("The specified match is not supported to be replaced.")
					}
				),
			ExpressiveOptions.IgnoreCaseForParsing
		);

		result.RegisterOperator(NameLikeOperator.Instance);

		return result;
	}

	[GeneratedRegex("""(dif{2}iculty|name)""", RegexOptions.Compiled | RegexOptions.IgnoreCase, 5000)]
	private static partial Regex KeywordPattern();
}

/// <summary>
/// Indicates the name matching operator <c>like</c>.
/// The expected usage is like <c>name like 'Unique\s+Rectangle.*'</c>.
/// </summary>
file sealed class NameLikeOperator : OperatorBase
{
	/// <summary>
	/// Indicates the singleton instance.
	/// </summary>
	public static readonly NameLikeOperator Instance = new();


	/// <summary>
	/// Initializes a <see cref="NameLikeOperator"/> instance.
	/// </summary>
	private NameLikeOperator()
	{
	}


	/// <inheritdoc/>
	public override IEnumerable<string> Tags => new[] { "like" };


	/// <inheritdoc/>
	public override IExpression BuildExpression(Token previousToken, IExpression[] expressions, Context context)
		=> new NameLikeExpression(expressions[0], expressions[1], context);

	/// <inheritdoc/>
	public override OperatorPrecedence GetPrecedence(Token previousToken) => OperatorPrecedence.Equal;
}

/// <summary>
/// Defines the backing implementation of <c>like</c> operator.
/// </summary>
file sealed class NameLikeExpression : BinaryExpressionBase
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
