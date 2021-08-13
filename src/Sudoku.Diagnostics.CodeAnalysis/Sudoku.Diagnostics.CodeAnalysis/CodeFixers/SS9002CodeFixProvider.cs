namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS9002")]
public sealed partial class SS9002CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SS9002);
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var (_, initializerSpan) = diagnostic.AdditionalLocations[0];
		var initializerNode = root.FindNode(initializerSpan, getInnermostNodeForTie: true);
		var (_, valueSpan) = diagnostic.AdditionalLocations[1];
		var valueNode = root.FindNode(valueSpan, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS9002,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(valueNode, initializerNode);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS9002)
			),
			diagnostic
		);
	}
}
