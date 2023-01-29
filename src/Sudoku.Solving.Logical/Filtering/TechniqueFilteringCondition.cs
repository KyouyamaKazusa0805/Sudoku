namespace Sudoku.Filtering;

/// <summary>
/// Defines a way that is used for filtering the gathered step.
/// </summary>
/// <remarks>
/// Supported expressions:
/// <list type="bullet">
/// <item><c><see langword="rating"/> (<![CDATA[>]]>,<![CDATA[>=]]>,<![CDATA[<]]>,<![CDATA[<=]]>,<![CDATA[==]]>,<![CDATA[!=]]>) (value)</c></item>
/// <item><c><see langword="name"/> (<![CDATA[==]]>,<![CDATA[!=]]>) (technique-name)</c></item>
/// <item><c><see langword="name"/> <see langword="like"/> (pattern)</c></item>
/// <item><c><see langword="conclusion"/> (<![CDATA[==]]>,<![CDATA[!=]]>) (conclusion)</c></item>
/// <item><c><see langword="conclusion"/> <see langword="has"/> (coordinate)</c></item>
/// <item><c><see langword="count"/> <see langword="conclusion"/> (<![CDATA[>]]>,<![CDATA[>=]]>,<![CDATA[<]]>,<![CDATA[<=]]>,<![CDATA[==]]>,<![CDATA[!=]]>) (count)</c></item>
/// </list>
/// </remarks>
public static partial class TechniqueFiltering
{
	/// <summary>
	/// Indicates the rating keyword.
	/// </summary>
	private const string RatingKeyword = "rating";

	/// <summary>
	/// Indicates the name keyword.
	/// </summary>
	private const string NameKeyword = "name";

	/// <summary>
	/// Indicates the conclusion keyword.
	/// </summary>
	private const string ConclusionKeyword = "conclusion";


	/// <summary>
	/// Filters some invalid steps.
	/// </summary>
	/// <param name="steps">The found steps.</param>
	/// <param name="conditionString">The condition.</param>
	/// <exception cref="ExpressiveException">Throws when the expression being evaluated is invalid.</exception>
	public static IEnumerable<IStep> Filter(IEnumerable<IStep> steps, string conditionString)
		=> string.IsNullOrWhiteSpace(conditionString)
			? steps // Special case: If the condition string is empty, just return.
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
						RatingKeyword => $"{step.Difficulty}",
						NameKeyword => $"'{step.Name}'",
						ConclusionKeyword => $"'{ConclusionFormatter.Format(step.Conclusions.ToArray(), ", ", true)}'",
						_ => throw new NotSupportedException("The specified match is not supported to be replaced.")
					}
				),
			ExpressiveOptions.IgnoreCaseForParsing
		);

		result.RegisterOperator(NameLikeOperator.Instance);
		result.RegisterOperator(ConclusionHasOperator.Instance);
		result.RegisterOperator(CountOperator.Instance);

		return result;
	}

	[GeneratedRegex("""(rating|name|conclusion)""", RegexOptions.Compiled | RegexOptions.IgnoreCase, 5000)]
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
/// Indicates the conclusion matching operator <c>has</c>.
/// The expected usage is like <c>conclusion has 'r3c4'</c>.
/// </summary>
file sealed class ConclusionHasOperator : OperatorBase
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

/// <summary>
/// Indicates the counter operator <c>count</c> to total up the number of conclusions in a single technique step.
/// The expected usage is like <c>count conclusion</c>.
/// </summary>
file sealed class CountOperator : OperatorBase
{
	/// <summary>
	/// Indicates the singleton instance.
	/// </summary>
	public static readonly CountOperator Instance = new();


	/// <summary>
	/// Initializes a <see cref="CountOperator"/> instance.
	/// </summary>
	private CountOperator()
	{
	}


	/// <inheritdoc/>
	public override IEnumerable<string> Tags => new[] { "count" };


	/// <inheritdoc/>
	public override IExpression BuildExpression(Token previousToken, IExpression[] expressions, Context context)
		=> new CountExpression(expressions[0] ?? expressions[1]);

	/// <inheritdoc/>
	public override OperatorPrecedence GetPrecedence(Token previousToken) => OperatorPrecedence.Not;
}

/// <summary>
/// Defines the backing implementation of <c><see langword="like"/></c> operator.
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

/// <summary>
/// Defines the backing implementation of <c><see langword="has"/></c> operator.
/// </summary>
file sealed class ConclusionHasExpression : BinaryExpressionBase
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
		var left = ((string)lhsResult).SplitRemovingEmpty(", ");
		var right = (string)rightHandSide.Evaluate(variables);
		return left.Any(s => s.Contains(right));
	}
}

/// <summary>
/// Defines the backing implementation of <c><see langword="count"/></c> operator.
/// </summary>
file sealed class CountExpression : UnaryExpressionBase
{
	/// <summary>
	/// Initializes a <see cref="ConclusionHasExpression"/> instance via an expression.
	/// </summary>
	/// <param name="expression">The operated expression.</param>
	internal CountExpression(IExpression expression) : base(expression)
	{
	}


	/// <inheritdoc/>
	public override object Evaluate(IDictionary<string, object> variables)
	{
		var value = ((string)_expression.Evaluate(variables)).SplitRemovingEmpty(", ");
		return value.Length;
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Simply calls <see cref="string.Split(string?, StringSplitOptions)"/>.
	/// </summary>
	/// <seealso cref="string.Split(string?, StringSplitOptions)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string[] SplitRemovingEmpty(this string @this, string separator)
		=> @this.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
