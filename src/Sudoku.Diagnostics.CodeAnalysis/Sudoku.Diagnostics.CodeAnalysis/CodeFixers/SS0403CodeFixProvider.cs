using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Extensions;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	[CodeFixProvider("SS0403")]
	public sealed partial class SS0403CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0403));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), _) = diagnostic;
			var fieldDecl = (EnumMemberDeclarationSyntax)root.FindNode(span, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0403,
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
					equivalenceKey: nameof(CodeFixTitles.SS0403)
				),
				diagnostic
			);
		}
	}
}
