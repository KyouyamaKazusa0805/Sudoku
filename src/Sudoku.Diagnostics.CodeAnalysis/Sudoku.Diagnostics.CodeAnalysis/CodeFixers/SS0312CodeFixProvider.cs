using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Extensions;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0312")]
public sealed partial class SS0312CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0312));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var (_, bodySpan) = diagnostic.AdditionalLocations[0];
		var body = (QueryBodySyntax)root.FindNode(bodySpan, getInnermostNodeForTie: true);
		var (_, formerSpan) = diagnostic.AdditionalLocations[1];
		var formerExpr = (ExpressionSyntax)root.FindNode(formerSpan, getInnermostNodeForTie: true);
		var (_, latterSpan) = diagnostic.AdditionalLocations[2];
		var latterExpr = (ExpressionSyntax)root.FindNode(latterSpan, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0312,
				createChangedDocument: async c =>
				{
					int index = int.Parse(diagnostic.Properties["Index"]!);
					bool shouldAppendParen = bool.Parse(diagnostic.Properties["ShouldAppendParen"]!);
					var oldClauses = body.Clauses;

					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						body,
						body.WithClauses(
							oldClauses.Replace(
								oldClauses[index],
								SyntaxFactory.WhereClause(
									SyntaxFactory.BinaryExpression(
										SyntaxKind.LogicalAndExpression,
										formerExpr,
										shouldAppendParen
										? SyntaxFactory.ParenthesizedExpression(latterExpr)
										: latterExpr
									)
								)
							).RemoveAt(index + 1)
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0312)
			),
			diagnostic
		);
	}
}
