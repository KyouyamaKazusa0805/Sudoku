using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Sudoku.Analytics.Rating;

/// <summary>
/// Represents a formula. The formula can help you calculate the target expression.
/// </summary>
/// <param name="expression">Indicates the expression to combine multiple values.</param>
public sealed class Formula(Expression<Func<decimal[], decimal>> expression)
{
	/// <summary>
	/// Try to calculate the final score via the specified arguments.
	/// </summary>
	/// <param name="arguments">The arguments passed in.</param>
	/// <returns>The final score.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public decimal GetScore(decimal[] arguments) => expression.Compile()(arguments);
}
