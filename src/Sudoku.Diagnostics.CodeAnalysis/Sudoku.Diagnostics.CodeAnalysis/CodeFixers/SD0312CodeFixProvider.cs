namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SD0312")]
public sealed partial class SD0312CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SD0312));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var (_, span) = diagnostic.AdditionalLocations[0];
		var expr = (ExpressionSyntax)root.FindNode(span, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SD0312,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, context.CancellationToken);
					editor.ReplaceNode(
						expr,
						expr switch
						{
							ArrayCreationExpressionSyntax { Type: var type, Initializer: var initializer } =>
								SyntaxFactory.StackAllocArrayCreationExpression(
									type,
									initializer
								),
							ImplicitArrayCreationExpressionSyntax { Initializer: var initializer } =>
								SyntaxFactory.ImplicitStackAllocArrayCreationExpression(
									initializer
								)
						}
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SD0312)
			),
			diagnostic
		);
	}
}
