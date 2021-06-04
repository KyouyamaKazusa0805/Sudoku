using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.Text.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	/// <summary>
	/// Indicates the code fixer for solving the diagnostic result
	/// <a href="https://github.com/SunnieShine/Sudoku/wiki/Rule-SS0102">SS0102</a>.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SS0102CodeFixProvider)), Shared]
	public sealed class SS0102CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
			DiagnosticIds.SS0102
		);

		/// <inheritdoc/>
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;


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
