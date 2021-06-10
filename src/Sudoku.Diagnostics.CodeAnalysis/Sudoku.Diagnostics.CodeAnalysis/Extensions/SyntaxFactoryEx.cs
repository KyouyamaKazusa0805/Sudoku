using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides the additional operations for <see cref="SyntaxFactory"/>.
	/// </summary>
	/// <seealso cref="SyntaxFactory"/>
	public static class SyntaxFactoryEx
	{
		/// <summary>
		/// Creates a syntax node that represents a <c>expr <see langword="is"/> { }</c> syntax.
		/// </summary>
		/// <param name="expression">The expression <c>expr</c> node.</param>
		/// <returns>The syntax node.</returns>
		public static IsPatternExpressionSyntax IsEmptyPropertyPatternExpression(ExpressionSyntax expression) =>
			SyntaxFactory.IsPatternExpression(
				expression,
				SyntaxFactory.RecursivePattern()
				.WithPropertyPatternClause(
					SyntaxFactory.PropertyPatternClause()
					.WithOpenBraceToken(
						SyntaxFactory.Token(
							SyntaxFactory.TriviaList(),
							SyntaxKind.OpenBraceToken,
							SyntaxFactory.TriviaList(
								SyntaxFactory.Space
							)
						)
					)
					.WithCloseBraceToken(
						SyntaxFactory.Token(
							SyntaxFactory.TriviaList(),
							SyntaxKind.CloseBraceToken,
							SyntaxFactory.TriviaList(
								SyntaxFactory.Space
							)
						)
					)
				)
			)
			.WithIsKeyword(
				SyntaxFactory.Token(
					SyntaxFactory.TriviaList(),
					SyntaxKind.IsKeyword,
					SyntaxFactory.TriviaList(
						SyntaxFactory.Space
					)
				)
			);

		/// <summary>
		/// Creates a syntax node that represents a <c>expr <see langword="is"/> { } variable</c> syntax.
		/// </summary>
		/// <param name="expression">The expression <c>expr</c> node.</param>
		/// <param name="variableName">The variable name of the part <c>variable</c>.</param>
		/// <returns>The syntax node.</returns>
		public static IsPatternExpressionSyntax IsEmptyPropertyPatternExpression(
			ExpressionSyntax expression, string variableName) =>
			SyntaxFactory.IsPatternExpression(
				expression,
				SyntaxFactory.RecursivePattern()
				.WithPropertyPatternClause(
					SyntaxFactory.PropertyPatternClause()
					.WithOpenBraceToken(
						SyntaxFactory.Token(
							SyntaxFactory.TriviaList(),
							SyntaxKind.OpenBraceToken,
							SyntaxFactory.TriviaList(
								SyntaxFactory.Space
							)
						)
					)
					.WithCloseBraceToken(
						SyntaxFactory.Token(
							SyntaxFactory.TriviaList(),
							SyntaxKind.CloseBraceToken,
							SyntaxFactory.TriviaList(
								SyntaxFactory.Space
							)
						)
					)
				)
				.WithDesignation(
					SyntaxFactory.SingleVariableDesignation(
						SyntaxFactory.Identifier(
							variableName
						)
					)
				)
			)
			.WithIsKeyword(
				SyntaxFactory.Token(
					SyntaxFactory.TriviaList(),
					SyntaxKind.IsKeyword,
					SyntaxFactory.TriviaList(
						SyntaxFactory.Space
					)
				)
			);
	}
}
