namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0311")]
public sealed partial class SS0311CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0311));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var (_, span) = diagnostic.AdditionalLocations[0];
		var node = (OrderByClauseSyntax)root.FindNode(span, getInnermostNodeForTie: true);
		if (node is not { Orderings: { Count: var countOfOrderings and >= 2 } oldOrderings })
		{
			return;
		}

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0311,
				createChangedDocument: async c =>
				{
					int index = int.Parse(diagnostic.Properties["Index"]!);

					var editor = await DocumentEditor.CreateAsync(document, c);
					switch (countOfOrderings - 1)
					{
						case 1:
						{
							editor.RemoveNode(oldOrderings[1]);
							editor.ReplaceNode(
								oldOrderings[0],
								oldOrderings[0].WithTrailingTrivia(
									SyntaxFactory.ParseLeadingTrivia(" ")
								)
							);

							break;
						}
						default:
						{
							var newOrderings = oldOrderings.RemoveAt(index);
							var syntaxNodeOrToken = SyntaxFactory.NodeOrTokenList();
							for (int i = 0, count = newOrderings.Count; i < count; i++)
							{
								var newOrdering = newOrderings[i];
								syntaxNodeOrToken = syntaxNodeOrToken.Add(newOrdering);

								if (i != count - 1)
								{
									syntaxNodeOrToken = syntaxNodeOrToken.Add(
										SyntaxFactory.Token(SyntaxKind.CommaToken)
									);
								}
							}

							editor.ReplaceNode(
								node,
								SyntaxFactory.OrderByClause(
									SyntaxFactory.SeparatedList<OrderingSyntax>(
										syntaxNodeOrToken
									)
								)
							);

							break;
						}
					}

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0311)
			),
			diagnostic
		);
	}
}
