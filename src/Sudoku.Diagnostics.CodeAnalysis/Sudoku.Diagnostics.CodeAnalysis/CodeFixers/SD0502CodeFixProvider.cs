namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SD0502")]
public sealed partial class SD0502CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SD0502));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var (_, span) = diagnostic.AdditionalLocations[0];
		var node = (ParameterSyntax)root.FindNode(span, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SD0501,
				createChangedDocument: async c =>
				{
					var identifierToken = node.Identifier;

					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						node,
						node.WithIdentifier(
							SyntaxFactory.Token(
								identifierToken.LeadingTrivia,
								SyntaxKind.IdentifierToken,
								"@this",
								"this",
								identifierToken.TrailingTrivia
							)
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SD0501)
			),
			diagnostic
		);
	}
}
