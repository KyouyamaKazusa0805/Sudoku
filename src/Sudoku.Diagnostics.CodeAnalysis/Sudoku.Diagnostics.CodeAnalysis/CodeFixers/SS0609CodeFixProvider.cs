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
	[CodeFixProvider("SS0609")] // Please update here.
	public sealed partial class SS0609CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0609));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), _) = diagnostic;
			if (
				root.FindNode(span, getInnermostNodeForTie: true) is not SubpatternSyntax
				{
					NameColon: var nameColon,
					Pattern: var pattern
				} node
			)
			{
				return;
			}

			if (nameColon is not null)
			{
				context.RegisterCodeFix(
					CodeAction.Create(
						title: CodeFixTitles.SS0609_2,
						createChangedDocument: async c =>
						{
							var editor = await DocumentEditor.CreateAsync(document, c);
							editor.ReplaceNode(
								node,
								node
								.WithPattern(
									SyntaxFactory.DiscardPattern()
								)
								.WithNameColon(
									null
								)
							);

							return document.WithSyntaxRoot(editor.GetChangedRoot());
						},
						equivalenceKey: nameof(CodeFixTitles.SS0609_2)
					),
					diagnostic
				);
			}

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0609_1,
					createChangedDocument: async c =>
					{
						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.ReplaceNode(
							node,
							node.WithPattern(
								SyntaxFactory.DiscardPattern()
							)
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SS0609_1)
				),
				diagnostic
			);
		}
	}
}
