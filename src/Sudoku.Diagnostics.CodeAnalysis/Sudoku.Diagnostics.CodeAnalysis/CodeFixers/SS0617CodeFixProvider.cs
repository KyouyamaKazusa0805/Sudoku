using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Extensions;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	[CodeFixProvider("SS0617")]
	public sealed partial class SS0617CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0617));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), _) = diagnostic;
			var node = root.FindNode(span, getInnermostNodeForTie: true);
			var (_, leftExprSpan) = diagnostic.AdditionalLocations[0];
			var leftExpr = (ExpressionSyntax)root.FindNode(leftExprSpan, getInnermostNodeForTie: true);
			var (_, rightExprSpan) = diagnostic.AdditionalLocations[1];
			var rightExpr = (ExpressionSyntax)root.FindNode(rightExprSpan, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0617,
					createChangedDocument: async c =>
					{
						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.ReplaceNode(
							node,
							SyntaxFactory.BinaryExpression(
								(SyntaxKind)int.Parse(diagnostic.Properties["OperatorToken"]!) switch
								{
									SyntaxKind.GreaterThanToken => SyntaxKind.GreaterThanExpression,
									SyntaxKind.GreaterThanEqualsToken => SyntaxKind.GreaterThanOrEqualExpression,
									SyntaxKind.LessThanToken => SyntaxKind.LessThanExpression,
									SyntaxKind.LessThanEqualsToken => SyntaxKind.LessThanOrEqualExpression
								},
								leftExpr,
								rightExpr
							)
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SS0617)
				),
				diagnostic
			);
		}
	}
}
