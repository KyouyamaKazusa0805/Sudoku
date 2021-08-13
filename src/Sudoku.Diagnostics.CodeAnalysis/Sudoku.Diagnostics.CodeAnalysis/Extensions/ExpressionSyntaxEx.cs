namespace Sudoku.Diagnostics.CodeAnalysis.Extensions;

/// <summary>
/// Provides extension methods on <see cref="ExpressionSyntax"/>.
/// </summary>
/// <seealso cref="ExpressionSyntax"/>
public static class ExpressionSyntaxEx
{
	/// <summary>
	/// To check whether the expression is a simple expression.
	/// </summary>
	/// <param name="this">The node.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool IsSimpleExpression(this ExpressionSyntax @this) =>
		@this is LiteralExpressionSyntax or DefaultExpressionSyntax or IdentifierNameSyntax;
}
