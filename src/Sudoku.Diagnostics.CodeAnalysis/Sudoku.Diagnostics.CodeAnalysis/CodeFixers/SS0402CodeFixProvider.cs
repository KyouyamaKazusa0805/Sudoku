namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0402")]
public sealed partial class SS0402CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0402));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var ((_, span), _) = diagnostic;
		var fieldDecl = (EnumMemberDeclarationSyntax)root.FindNode(span, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0402,
				createChangedDocument: async c =>
				{
					string nextFlagStr = diagnostic.Properties["NextPossibleFlag"]!;
					long nextFlag = long.Parse(nextFlagStr);

					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						fieldDecl,
						fieldDecl.WithEqualsValue(
							SyntaxFactory.EqualsValueClause(
								SyntaxFactory.LiteralExpression(
									SyntaxKind.NumericLiteralExpression,
									SyntaxFactory.Literal(
										SyntaxFactory.TriviaList(),
										nextFlagStr,
										nextFlag,
										SyntaxFactory.TriviaList(
											SyntaxFactory.Trivia(
												SyntaxFactory.SkippedTokensTrivia()
											)
										)
									)
								)
							)
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0402)
			),
			diagnostic
		);
	}
}
