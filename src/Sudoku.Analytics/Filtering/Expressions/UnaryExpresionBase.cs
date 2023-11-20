using System.SourceGeneration;
using Expressive.Expressions;

namespace Sudoku.Filtering.Expressions;

/// <summary>
/// Base class implementation of <see cref="IExpression"/>.
/// </summary>
/// <param name="expression">The expression.</param>
/// <seealso cref="IExpression"/>
public abstract partial class UnaryExpressionBase([Data(DataMemberKinds.Field, Accessibility = "protected")] IExpression expression) : IExpression
{
	/// <inheritdoc/>
	public abstract object Evaluate(IDictionary<string, object> variables);
}
