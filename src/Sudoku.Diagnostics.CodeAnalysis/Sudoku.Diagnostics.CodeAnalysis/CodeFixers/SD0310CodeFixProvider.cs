namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SD0310")]
public sealed partial class SD0310CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(CodeFixTitles.SD0310));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var (_, span) = diagnostic.AdditionalLocations[0];
		var methodInvocation = (InvocationExpressionSyntax)root.FindNode(span, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SD0310,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						methodInvocation,
						methodInvocation.WithArgumentList(
							SyntaxFactory.ArgumentList(
								SyntaxFactory.SingletonSeparatedList(
									SyntaxFactory.Argument(
										SyntaxFactory.LiteralExpression(
											SyntaxKind.StringLiteralExpression,
											SyntaxFactory.Literal("#")
										)
									)
								)
							)
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SD0310)
			),
			diagnostic
		);
	}
}
