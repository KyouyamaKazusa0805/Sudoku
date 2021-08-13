namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0307")]
public sealed partial class SS0307CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0307));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var (_, bodySpan) = diagnostic.AdditionalLocations[0];
		var body = (QueryBodySyntax)root.FindNode(bodySpan, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0307,
				createChangedDocument: async c =>
				{
					int index = int.Parse(diagnostic.Properties["Index"]!);
					var clauses = body.Clauses;
					var newClauses = clauses.Insert(index, clauses[index + 1]).RemoveAt(index + 2);

					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						body,
						body.WithClauses(
							newClauses
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0307)
			),
			diagnostic
		);
	}
}
