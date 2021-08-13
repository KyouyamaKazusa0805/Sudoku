namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0633")]
public sealed partial class SS0633CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0633));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var ((_, span), _) = diagnostic;
		var node = root.FindNode(span, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0633,
				createChangedDocument: async c =>
				{
					bool isSwitchExpr = bool.Parse(diagnostic.Properties["IsSwitchExpression"]!);

					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						node,
						isSwitchExpr
						? SyntaxFactory.DiscardPattern()
						: SyntaxFactory.DefaultSwitchLabel()
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0633)
			),
			diagnostic
		);
	}
}
