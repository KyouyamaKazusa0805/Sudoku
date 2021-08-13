namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SD0313")]
public sealed partial class SD0313CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SD0313));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var (_, span) = diagnostic.AdditionalLocations[0];
		var expr = (ExpressionSyntax)root.FindNode(span, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SD0313,
				createChangedDocument: async c =>
				{
					int length = int.Parse(diagnostic.Properties["Length"]!);

					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						expr,
						SyntaxFactory.LiteralExpression(
							SyntaxKind.NumericLiteralExpression,
							SyntaxFactory.Literal(length)
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SD0313)
			),
			diagnostic
		);
	}
}
