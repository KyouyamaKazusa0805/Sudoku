using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0634")]
public sealed partial class SS0634CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0634));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var ((_, span), _) = diagnostic;
		var node = root.FindNode(span, getInnermostNodeForTie: true);
		var (_, exprSpan) = diagnostic.AdditionalLocations[0];
		var expr = (ExpressionSyntax)root.FindNode(exprSpan, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0634,
				createChangedDocument: async c =>
				{
					string variableName = diagnostic.Properties["VariableName"]!;

					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						node,
						SyntaxFactoryEx.IsEmptyPropertyPatternExpression(
							expr,
							variableName
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0634)
			),
			diagnostic
		);
	}
}
