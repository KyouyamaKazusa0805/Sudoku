using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Extensions;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers;

[CodeFixProvider("SS0308")]
public sealed partial class SS0308CodeFixProvider : CodeFixProvider
{
	/// <inheritdoc/>
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var document = context.Document;
		var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0308));
		var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
		var (_, fromClauseSpan) = diagnostic.AdditionalLocations[0];
		var fromClause = (FromClauseSyntax)root.FindNode(fromClauseSpan, getInnermostNodeForTie: true);
		var (_, typeToCastSpan) = diagnostic.AdditionalLocations[1];
		var typeToCast = (TypeSyntax)root.FindNode(typeToCastSpan, getInnermostNodeForTie: true);
		var (_, selectClauseSpan) = diagnostic.AdditionalLocations[2];
		var selectClause = (SelectClauseSyntax)root.FindNode(selectClauseSpan, getInnermostNodeForTie: true);
		var (_, innerExpressionSpan) = diagnostic.AdditionalLocations[3];
		var innerExpression = (ExpressionSyntax)root.FindNode(innerExpressionSpan, getInnermostNodeForTie: true);

		context.RegisterCodeFix(
			CodeAction.Create(
				title: CodeFixTitles.SS0308,
				createChangedDocument: async c =>
				{
					var editor = await DocumentEditor.CreateAsync(document, c);
					editor.ReplaceNode(
						fromClause,
						fromClause.WithType(
							typeToCast.WithTrailingTrivia(
								SyntaxFactory.TriviaList(
									SyntaxFactory.Space
								)
							)
						)
					);
					editor.ReplaceNode(
						selectClause,
						selectClause.WithExpression(
							innerExpression
						)
					);

					return document.WithSyntaxRoot(editor.GetChangedRoot());
				},
				equivalenceKey: nameof(CodeFixTitles.SS0308)
			),
			diagnostic
		);
	}
}
