namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SD0306")]
public sealed partial class SD0306CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SD0306);
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var ((_, span), descriptor) = diagnostic;
		var node = (PrefixUnaryExpressionSyntax)root.FindNode(span, getInnermostNodeForTie: true);
		var tags = diagnostic.Properties;

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SD0306_1,
				createChangedDocument: async c =>
				{
					if (node is not { RawKind: (int)SyntaxKind.BitwiseNotExpression, Operand: var operand })
					{
						throw new InvalidOperationException("The specified node is invalid to fix.");
					}

					var editor = await DocumentEditor.CreateAsync(document);
					editor.ReplaceNode(node, operand);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SD0306_1)
			),
			diagnostic
		);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SD0306_2,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.RemoveNode(node, SyntaxRemoveOptions.KeepTrailingTrivia);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SD0306_2)
			),
			diagnostic
		);
	}
}
