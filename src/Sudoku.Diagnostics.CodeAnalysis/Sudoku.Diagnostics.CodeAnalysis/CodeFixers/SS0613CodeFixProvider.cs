namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0613")]
public sealed partial class SS0613CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0613));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;

		var nodesToTemove = (
			from location in diagnostic.AdditionalLocations
			select root.FindNode(location.SourceSpan, getInnermostNodeForTie: true)
		).ToList();
		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0613_1,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, c);
					nodesToTemove.ForEach(editor.RemoveNode);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0613_1)
			),
			diagnostic
		);

		var ((_, span), _) = diagnostic;
		var node = root.FindNode(span, getInnermostNodeForTie: true);
		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0613_2,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.RemoveNode(node);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0613_2)
			),
			diagnostic
		);
	}
}
