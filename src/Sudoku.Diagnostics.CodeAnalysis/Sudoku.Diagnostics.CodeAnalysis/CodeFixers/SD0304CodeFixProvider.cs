namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SD0304")]
public sealed partial class SD0304CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SD0304);
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var ((_, span), descriptor) = diagnostic;
		var node = root.FindNode(span, getInnermostNodeForTie: true);
		var tags = diagnostic.Properties;

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SD0304,
				createChangedDocument: async c =>
				{
					string left = tags["Variable"]!;
					string notEqualsToken = tags["Operator"]!;
					string fieldName = tags["PropertyName"]!;

					var accessExpr = SyntaxFactory.MemberAccessExpression(
						SyntaxKind.SimpleMemberAccessExpression,
						SyntaxFactory.IdentifierName(left),
						SyntaxFactory.IdentifierName(fieldName)
					);

					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						node,
						notEqualsToken == string.Empty
						? accessExpr
						: SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, accessExpr)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SD0304)
			),
			diagnostic
		);
	}
}
