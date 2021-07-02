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
	[CodeFixProvider("SS0607")]
	public sealed partial class SS0607CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0607));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), _) = diagnostic;
			if (
				root.FindNode(span, getInnermostNodeForTie: true) is not
				{
					Parent: PositionalPatternClauseSyntax { Subpatterns: var subpatterns } clause
				}
			)
			{
				return;
			}

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0607,
					createChangedDocument: async c =>
					{
						int indexToRemove = int.Parse(diagnostic.Properties["BoundParameterIndex"]!);

						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.ReplaceNode(
							clause,
							clause.WithSubpatterns(
								SyntaxFactory.SeparatedList(
									subpatterns.RemoveAt(indexToRemove)
								)
							)
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SS0607)
				),
				diagnostic
			);
		}
	}
}
