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
	[CodeFixProvider("SS0306")]
	public sealed partial class SS0306CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0306));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var (_, invocationSpan) = diagnostic.AdditionalLocations[0];
			var invocation = root.FindNode(invocationSpan, getInnermostNodeForTie: true);
			var (_, exprSpan) = diagnostic.AdditionalLocations[1];
			var expr = (ExpressionSyntax)root.FindNode(exprSpan, getInnermostNodeForTie: true);
			var (_, argSpan) = diagnostic.AdditionalLocations[2];
			var arg = (ExpressionSyntax)root.FindNode(argSpan, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0306,
					createChangedDocument: async c =>
					{
						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.ReplaceNode(
							invocation,
							SyntaxFactory.ElementAccessExpression(
								expr,
								SyntaxFactory.BracketedArgumentList(
									SyntaxFactory.SingletonSeparatedList(
										SyntaxFactory.Argument(
											arg
										)
									)
								)
							)
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SS0306)
				),
				diagnostic
			);
		}
	}
}
