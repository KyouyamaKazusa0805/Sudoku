namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0605")]
public sealed partial class SS0605CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0605));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var ((_, span), _) = diagnostic;
		var node = root.FindNode(span, getInnermostNodeForTie: true);
		var (_, leftSpan) = diagnostic.AdditionalLocations[0];
		var leftExpr = (ExpressionSyntax)root.FindNode(leftSpan, getInnermostNodeForTie: true);
		var (_, rightSpan) = diagnostic.AdditionalLocations[1];
		var rightExpr = (ExpressionSyntax)root.FindNode(rightSpan, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0605,
				createChangedDocument: async c =>
				{
					string operatorToken = diagnostic.Properties["OperatorToken"]!;

					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						node,
						SyntaxFactory.IsPatternExpression(
							leftExpr,
							operatorToken switch
							{
								"==" => SyntaxFactory.ConstantPattern(
									rightExpr
								),
								"!=" => SyntaxFactory.UnaryPattern(
									SyntaxFactory.ConstantPattern(
										rightExpr
									)
								)
							}
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0605)
			),
			diagnostic
		);
	}
}
