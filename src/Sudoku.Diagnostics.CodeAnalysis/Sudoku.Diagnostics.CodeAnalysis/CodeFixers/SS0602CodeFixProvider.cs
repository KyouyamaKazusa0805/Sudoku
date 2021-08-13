namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0602")]
public sealed partial class SS0602CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0602));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var ((_, span), _) = diagnostic;
		var node = root.FindNode(span, getInnermostNodeForTie: true);
		var (_, expressionSpan) = diagnostic.AdditionalLocations[0];
		var expr = (ExpressionSyntax)root.FindNode(expressionSpan, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0602,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						node,
						SyntaxFactory.IsPatternExpression(
							expr,
							SyntaxFactory.ConstantPattern(
								SyntaxFactory.LiteralExpression(
									SyntaxKind.NullLiteralExpression
								)
							)
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0602)
			),
			diagnostic
		);
	}
}
