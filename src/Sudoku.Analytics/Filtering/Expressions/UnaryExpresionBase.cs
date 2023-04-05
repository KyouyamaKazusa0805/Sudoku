namespace Sudoku.Filtering.Expressions;

/// <summary>
/// Base class implementation of <see cref="IExpression"/>.
/// </summary>
/// <seealso cref="IExpression"/>
public abstract class UnaryExpressionBase : IExpression
{
	/// <summary>
	/// The inner expression.
	/// </summary>
	protected readonly IExpression _expression;


	/// <summary>
	/// Initializes a new instance of <see cref="UnaryExpressionBase"/>.
	/// </summary>
	/// <param name="expression">The expression.</param>
	protected UnaryExpressionBase(IExpression expression) => _expression = expression;


	/// <inheritdoc/>
	public abstract object Evaluate(IDictionary<string, object> variables);
}
