using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeGenerating.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ExpressionSyntax"/>.
	/// </summary>
	/// <seealso cref="ExpressionSyntaxEx"/>
	public static class ExpressionSyntaxEx
	{
		/// <summary>
		/// To check whether the specified expression is a <see langword="nameof"/> expression.
		/// </summary>
		/// <param name="this">The expression node to check.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool IsNameOfExpression(
#if NETSTANDARD2_1_OR_GREATER
			[NotNullWhen(true)]
#endif
			this ExpressionSyntax? @this
		) => @this is InvocationExpressionSyntax
		{
			Expression: IdentifierNameSyntax { Identifier: { ValueText: "nameof" } }
		};
	}
}
