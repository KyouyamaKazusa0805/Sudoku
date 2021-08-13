namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0603")]
public sealed partial class SS0603CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0603));
		if (diagnostic.AdditionalLocations.Count == 0)
		{
			return;
		}

		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var ((_, span), _) = diagnostic;
		var node = root.FindNode(span, getInnermostNodeForTie: true);
		var (_, exprSpan) = diagnostic.AdditionalLocations[0];
		var expr = (ExpressionSyntax)root.FindNode(exprSpan, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0603,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						node,
						SyntaxFactory.IsPatternExpression(
							expr,
							SyntaxFactory.UnaryPattern(
								SyntaxFactory.ConstantPattern(
									SyntaxFactory.LiteralExpression(
										SyntaxKind.NullLiteralExpression
									)
								)
							)
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0603)
			),
			diagnostic
		);
	}
}
