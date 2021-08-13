namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0703")]
public sealed partial class SS0703CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0703));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var (_, exprSpan) = diagnostic.AdditionalLocations[0];
		if (
			root.FindNode(exprSpan, getInnermostNodeForTie: true) is not ConditionalAccessExpressionSyntax
			{
				Expression: var baseExpr,
				WhenNotNull: var whenNotNull
			} expr
		)
		{
			return;
		}

		ExpressionSyntax? exprToReplace = whenNotNull switch
		{
			// local.Method(parameterList)
			InvocationExpressionSyntax
			{
				Expression: MemberBindingExpressionSyntax { Name: var bindingName },
				ArgumentList: var args
			} => SyntaxFactory.InvocationExpression(
				SyntaxFactory.MemberAccessExpression(
					SyntaxKind.SimpleMemberAccessExpression,
					baseExpr,
					bindingName
				),
				args
			),

			// local.Property
			MemberBindingExpressionSyntax { Name: var bindingName } =>
				SyntaxFactory.MemberAccessExpression(
					SyntaxKind.SimpleMemberAccessExpression,
					baseExpr,
					bindingName
				),

			// local[indexerList]
			ElementBindingExpressionSyntax { ArgumentList: var argumentList } =>
				SyntaxFactory.ElementAccessExpression(
					baseExpr,
					argumentList
				),

			_ => null
		};
		if (exprToReplace is null)
		{
			return;
		}

		// Check whether the expression is in an assignment statement.
		// i.e. expression 'int? a = inst?[0]' -> 'int a = inst[0]'.
		TypeSyntax? typeNode = null, typeNodeToReplace = null;
		if (
			baseExpr is IdentifierNameSyntax
			{
				Parent: ConditionalAccessExpressionSyntax
				{
					Parent: EqualsValueClauseSyntax
					{
						Parent: VariableDeclaratorSyntax
						{
							Parent: VariableDeclarationSyntax
							{
								Type: NullableTypeSyntax { ElementType: var innerType } innerTypeNode,
								Variables: { Count: 1 }
							}
						}
					}
				}
			}
		)
		{
			typeNode = innerTypeNode;
			typeNodeToReplace = innerType;
		}

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0703,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(expr, exprToReplace);
					if (typeNode is not null && typeNodeToReplace is not null)
					{
						editor.ReplaceNode(
							typeNode,
							typeNodeToReplace.WithTrailingTrivia(
								new[]
								{
									SyntaxFactory.SyntaxTrivia(
										SyntaxKind.WhitespaceTrivia,
										" "
									)
								}
							)
						);
					}

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0703)
			),
			diagnostic
		);
	}
}
