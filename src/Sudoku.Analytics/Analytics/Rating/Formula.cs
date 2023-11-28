using System.Runtime.CompilerServices;
using System.SourceGeneration;

namespace Sudoku.Analytics.Rating;

/// <summary>
/// Represents a formula. The formula can help you calculate the target expression.
/// </summary>
/// <param name="expression">Indicates the expression to combine multiple values.</param>
/// <param name="expressionString">
/// <para>Indicates the detail string text of the argument <paramref name="expression"/>.</para>
/// <include file="../../global-doc-comments.xml" path="g/csharp10/feature[@name='caller-argument-expression']" />
/// </param>
public sealed partial class Formula(
	Func<decimal[], decimal> expression,
	[Data, CallerArgumentExpression(nameof(expression))] string expressionString = null!
)
{
	/// <summary>
	/// Try to calculate the final score via the specified arguments.
	/// </summary>
	/// <param name="arguments">The arguments passed in.</param>
	/// <returns>The final score.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public decimal GetScore(decimal[] arguments) => expression(arguments);
}
