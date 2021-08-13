namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0303")]
public sealed partial class SS0303CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0303));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var (_, span) = diagnostic.AdditionalLocations[0];
		var node = root.FindNode(span, getInnermostNodeForTie: true);
		var (_, identifierSpan) = diagnostic.AdditionalLocations[1];
		var identifier = root.FindToken(identifierSpan.Start, findInsideTrivia: true);
		var (_, expressionToIterateSpan) = diagnostic.AdditionalLocations[2];
		var expressionToIterate = (ExpressionSyntax)root.FindNode(expressionToIterateSpan, getInnermostNodeForTie: true);
		var (_, conditionExprSpan) = diagnostic.AdditionalLocations[3];
		var conditionExpr = (ExpressionSyntax)root.FindNode(conditionExprSpan, getInnermostNodeForTie: true);

		switch (diagnostic.AdditionalLocations)
		{
			case { Count: 4 }:
			{
				context.RegisterCodeFix(
					CodeAction.Create(
						title: CodeFixTitles.SS0303,
						createChangedDocument: async c =>
						{
							var editor = await DocumentEditor.CreateAsync(document, c);
							editor.ReplaceNode(
								node,
								SyntaxFactory.InvocationExpression(
									SyntaxFactory.MemberAccessExpression(
										SyntaxKind.SimpleMemberAccessExpression,
										expressionToIterate,
										SyntaxFactory.IdentifierName("Any")
									)
								)
								.WithArgumentList(
									SyntaxFactory.ArgumentList(
										SyntaxFactory.SingletonSeparatedList(
											SyntaxFactory.Argument(
												SyntaxFactory.SimpleLambdaExpression(
													SyntaxFactory.Parameter(
														identifier
													)
												)
												.WithExpressionBody(
													conditionExpr
												)
											)
										)
									)
								)
								.NormalizeWhitespace()
							);

							return document.WithSyntaxRoot(editor.GetChangedRoot());
						},
						equivalenceKey: nameof(CodeFixTitles.SS0303)
					),
					diagnostic
				);

				break;
			}

			case { Count: 5 } additionalLocations
			when additionalLocations[4] is var (_, typeToCastSpan)
			&& root.FindNode(typeToCastSpan, getInnermostNodeForTie: true) is TypeSyntax typeName:
			{
				context.RegisterCodeFix(
					CodeAction.Create(
						title: CodeFixTitles.SS0303,
						createChangedDocument: async c =>
						{
							var editor = await DocumentEditor.CreateAsync(document, c);
							editor.ReplaceNode(
								node,
								SyntaxFactory.InvocationExpression(
									SyntaxFactory.MemberAccessExpression(
										SyntaxKind.SimpleMemberAccessExpression,
										SyntaxFactory.InvocationExpression(
											SyntaxFactory.MemberAccessExpression(
												SyntaxKind.SimpleMemberAccessExpression,
												expressionToIterate,
												SyntaxFactory.GenericName(
													SyntaxFactory.Identifier("Cast")
												)
												.WithTypeArgumentList(
													SyntaxFactory.TypeArgumentList(
														SyntaxFactory.SingletonSeparatedList(
															typeName
														)
													)
												)
											)
										),
										SyntaxFactory.IdentifierName("Any")
									)
								)
								.WithArgumentList(
									SyntaxFactory.ArgumentList(
										SyntaxFactory.SingletonSeparatedList(
											SyntaxFactory.Argument(
												SyntaxFactory.SimpleLambdaExpression(
													SyntaxFactory.Parameter(
														identifier
													)
												)
												.WithExpressionBody(
													conditionExpr
												)
											)
										)
									)
								)
								.NormalizeWhitespace()
							);

							return document.WithSyntaxRoot(editor.GetChangedRoot());
						},
						equivalenceKey: nameof(CodeFixTitles.SS0303)
					),
					diagnostic
				);

				break;
			}
		}
	}
}
