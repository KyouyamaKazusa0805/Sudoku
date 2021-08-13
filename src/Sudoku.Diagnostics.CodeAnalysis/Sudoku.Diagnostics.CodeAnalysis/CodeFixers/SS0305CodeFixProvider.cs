namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0305")]
public sealed partial class SS0305CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0305));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var ((_, latterClauseSpan), _) = diagnostic;
		var latterClause = (OrderByClauseSyntax)root.FindNode(latterClauseSpan, getInnermostNodeForTie: true);
		var (_, formerClauseSpan) = diagnostic.AdditionalLocations[0];
		var formerClause = (OrderByClauseSyntax)root.FindNode(formerClauseSpan, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0305,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						formerClause,
						formerClause.AddOrderings(
							latterClause.Orderings.ToArray()
						)
					);
					editor.RemoveNode(latterClause);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0305)
			),
			diagnostic
		);
	}
}
