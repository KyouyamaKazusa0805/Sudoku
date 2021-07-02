using System;
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

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	[CodeFixProvider("SS0706")]
	public sealed partial class SS0706CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0706));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), _) = diagnostic;
			var node = root.FindNode(span, getInnermostNodeForTie: true);
			var (_, leftExprSpan) = diagnostic.AdditionalLocations[0];
			var leftExpr = (ExpressionSyntax)root.FindNode(leftExprSpan, getInnermostNodeForTie: true);
			var (_, rightExprSpan) = diagnostic.AdditionalLocations[1];
			var rightExpr = (ExpressionSyntax)root.FindNode(rightExprSpan, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0706,
					createChangedDocument: async c =>
					{
						ExpressionSyntax innerNodeToReplace =
							SyntaxFactory.InvocationExpression(
								SyntaxFactory.IdentifierName("ReferenceEquals")
							)
							.WithArgumentList(
								SyntaxFactory.ArgumentList(
									SyntaxFactory.SeparatedList<ArgumentSyntax>(
										new SyntaxNodeOrToken[]
										{
											SyntaxFactory.Argument(
												leftExpr
											),
											SyntaxFactory.Token(SyntaxKind.CommaToken),
											SyntaxFactory.Argument(
												rightExpr
											)
										}
									)
								)
							);

						bool isEquals = diagnostic.Properties["OperationKind"] == "==";

						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.ReplaceNode(
							node,
							(
								isEquals
								? innerNodeToReplace
								: SyntaxFactory.PrefixUnaryExpression(
									SyntaxKind.LogicalNotExpression,
									innerNodeToReplace
								)
							).NormalizeWhitespace()
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SS0706)
				),
				diagnostic
			);
		}
	}
}
