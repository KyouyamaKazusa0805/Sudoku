using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Extensions;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	[CodeFixProvider("SD0401")]
	public sealed partial class SD0401CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(CodeFixTitles.SD0401));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), _) = diagnostic;
			var node = root.FindNode(span, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0401,
					createChangedDocument: async c =>
					{
						string exprValue = diagnostic.Properties["ExpressionValue"]!;

						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.ReplaceNode(
							node,
							SyntaxFactory.InvocationExpression(
								SyntaxFactory.IdentifierName(
									SyntaxFactory.Identifier(
										SyntaxFactory.TriviaList(),
										SyntaxKind.NameOfKeyword,
										"nameof",
										"nameof",
										SyntaxFactory.TriviaList()
									)
								)
							)
							.WithArgumentList(
								SyntaxFactory.ArgumentList(
									SyntaxFactory.SingletonSeparatedList(
										SyntaxFactory.Argument(
											SyntaxFactory.IdentifierName(
												exprValue
											)
										)
									)
								)
							)
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SD0401)
				),
				diagnostic
			);
		}
	}
}
