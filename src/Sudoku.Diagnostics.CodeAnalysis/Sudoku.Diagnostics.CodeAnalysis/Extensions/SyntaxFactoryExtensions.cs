namespace Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Provides the additional operations for <see cref="SyntaxFactory"/>.
/// </summary>
/// <seealso cref="SyntaxFactory"/>
public static class SyntaxFactoryExtensions
{
	/// <summary>
	/// Creates a syntax node that represents a <c>expr <see langword="is"/> { }</c>
	/// or <c>expr <see langword="is not"/> { }</c> syntax.
	/// </summary>
	/// <param name="expression">The expression <c>expr</c> node.</param>
	/// <param name="isNegated">
	/// Indicates whether the pattern should be negated. The result expression may be as follows:
	/// <list type="table">
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description><c>expr <see langword="is"/> { }</c></description>
	/// </item>
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description><c>expr <see langword="is not"/> { }</c></description>
	/// </item>
	/// </list>
	/// </param>
	/// <returns>The syntax node.</returns>
	public static IsPatternExpressionSyntax IsEmptyPropertyPatternExpression(
		ExpressionSyntax expression, bool isNegated = false)
	{
		var propertyPattern =
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
						SyntaxFactory.TriviaList()
					)
				)
			);

		return isNegated
			? SyntaxFactory.IsPatternExpression(
				expression,
				SyntaxFactory.UnaryPattern(
					SyntaxFactory.Token(
						SyntaxFactory.TriviaList(),
						SyntaxKind.NotKeyword,
						SyntaxFactory.TriviaList(
							SyntaxFactory.Space
						)
					),
					propertyPattern
				)
			)
			: SyntaxFactory.IsPatternExpression(
				expression,
				propertyPattern
			).WithIsKeyword(
				SyntaxFactory.Token(
					SyntaxFactory.TriviaList(),
					SyntaxKind.IsKeyword,
					SyntaxFactory.TriviaList(
						SyntaxFactory.Space
					)
				)
			);
	}

	/// <summary>
	/// Creates a syntax node that represents a <c>expr <see langword="is"/> { } variable</c>
	/// or <c>expr <see langword="is not"/> { } variable</c> syntax.
	/// </summary>
	/// <param name="expression">The expression <c>expr</c> node.</param>
	/// <param name="variableName">The variable name of the part <c>variable</c>.</param>
	/// <param name="isNegated">
	/// Indicates whether the pattern should be negated. The result expression may be as follows:
	/// <list type="table">
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description><c>expr <see langword="is"/> { }</c></description>
	/// </item>
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description><c>expr <see langword="is not"/> { }</c></description>
	/// </item>
	/// </list>
	/// </param>
	/// <returns>The syntax node.</returns>
	public static IsPatternExpressionSyntax IsEmptyPropertyPatternExpression(
		ExpressionSyntax expression, string variableName, bool isNegated = false)
	{
		var propertyPattern =
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
						SyntaxFactory.TriviaList()
					)
				)
			)
			.WithDesignation(
				SyntaxFactory.SingleVariableDesignation(
					SyntaxFactory.Identifier(
						variableName
					)
				)
			);

		return isNegated
			? SyntaxFactory.IsPatternExpression(
				expression,
				SyntaxFactory.UnaryPattern(
					SyntaxFactory.Token(
						SyntaxFactory.TriviaList(),
						SyntaxKind.NotKeyword,
						SyntaxFactory.TriviaList(
							SyntaxFactory.Space
						)
					),
					propertyPattern
				)
			)
			: SyntaxFactory.IsPatternExpression(
				expression,
				propertyPattern
			).WithIsKeyword(
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
