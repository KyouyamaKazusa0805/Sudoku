namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SD0307")]
public sealed partial class SD0307CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SD0307);
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var ((_, span), descriptor) = diagnostic;
		var node = (PrefixUnaryExpressionSyntax)root.FindNode(span, getInnermostNodeForTie: true);
		var tags = diagnostic.Properties;

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SD0307,
				createChangedDocument: async c =>
				{
					int realValueToFix = int.Parse(tags["RealValue"]!);
					ExpressionSyntax operand = node.RawKind switch
					{
						(int)SyntaxKind.UnaryPlusExpression =>
							SyntaxFactory.LiteralExpression(
								SyntaxKind.NumericLiteralExpression,
								SyntaxFactory.Literal(realValueToFix)
							),
						(int)SyntaxKind.UnaryMinusExpression =>
							SyntaxFactory.PrefixUnaryExpression(
								SyntaxKind.BitwiseNotExpression,
								SyntaxFactory.LiteralExpression(
									SyntaxKind.NumericLiteralExpression,
									SyntaxFactory.Literal(realValueToFix)
								)
							)
					};

					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(node, operand);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SD0307)
			),
			diagnostic
		);
	}
}
