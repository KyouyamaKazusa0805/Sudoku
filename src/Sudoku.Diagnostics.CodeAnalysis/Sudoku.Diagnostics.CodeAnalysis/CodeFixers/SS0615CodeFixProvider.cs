using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0615")]
public sealed partial class SS0615CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0615));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var ((_, span), _) = diagnostic;
		var node = root.FindNode(span, getInnermostNodeForTie: true);
		var (_, exprSpan) = diagnostic.AdditionalLocations[0];
		var expr = (ExpressionSyntax)root.FindNode(exprSpan, getInnermostNodeForTie: true);
		bool isNull = bool.Parse(diagnostic.Properties["IsNull"]!);
		bool isHasValue = bool.Parse(diagnostic.Properties["IsHasValue"]!);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0615_1,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						node,
						SyntaxFactory.IsPatternExpression(
							isHasValue
							? expr.WithTrailingTrivia(
								SyntaxFactory.ParseLeadingTrivia(" ")
							)
							: expr,
							isNull
							? SyntaxFactory.ConstantPattern(
								SyntaxFactory.LiteralExpression(
									SyntaxKind.NullLiteralExpression
								)
							)
							: SyntaxFactory.UnaryPattern(
								SyntaxFactory.ConstantPattern(
									SyntaxFactory.LiteralExpression(
										SyntaxKind.NullLiteralExpression
									)
								)
							)
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0615_1)
			),
			diagnostic
		);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0615_2,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						node,
						SyntaxFactoryEx.IsEmptyPropertyPatternExpression(
							isHasValue
							? expr.WithTrailingTrivia(
								SyntaxFactory.ParseLeadingTrivia(" ")
							)
							: expr,
							isNull
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0615_2)
			),
			diagnostic
		);
	}
}
