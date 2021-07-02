using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.Text.Extensions;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	[CodeFixProvider("SS0102")]
	public sealed partial class SS0102CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SS0102);
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), _) = diagnostic;
			var node = (InterpolatedStringExpressionSyntax)root.FindNode(span, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0102,
					createChangedDocument: async c =>
					{
						var innerToken = node.Contents;

						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.ReplaceNode(
							node,
							SyntaxFactory.InterpolatedStringExpression(
								SyntaxFactory.Token(SyntaxKind.InterpolatedStringStartToken),
								SyntaxFactory.SingletonList<InterpolatedStringContentSyntax>(
									SyntaxFactory.InterpolatedStringText()
									.WithTextToken(
										SyntaxFactory.Token(
											SyntaxFactory.TriviaList(),
											SyntaxKind.InterpolatedStringTextToken,
											innerToken.Count == 0
											? string.Empty
											: ((InterpolatedStringTextSyntax)innerToken[0]).TextToken.Text,
											innerToken.Count == 0
											? string.Empty
											: ((InterpolatedStringTextSyntax)innerToken[0]).TextToken.ValueText,
											SyntaxFactory.TriviaList()
										)
									)
								),
								SyntaxFactory.Token(SyntaxKind.InterpolatedStringEndToken)
							)
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SS0102)
				),
				diagnostic
			);
		}
	}
}
