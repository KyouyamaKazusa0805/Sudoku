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
	[CodeFixProvider("SS0601")]
	public sealed partial class SS0601CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0601));
			if (diagnostic.AdditionalLocations.Count != 2)
			{
				return;
			}

			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;

			/*slice-pattern*/
			var (_, patternSpan) = diagnostic.AdditionalLocations[0];
			var pattern = (PatternSyntax)root.FindNode(patternSpan, getInnermostNodeForTie: true);
			var (_, designationSpan) = diagnostic.AdditionalLocations[1];
			var designation = (VariableDesignationSyntax)root.FindNode(designationSpan, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0601,
					createChangedDocument: async c =>
					{
						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.ReplaceNode(
							pattern,
							SyntaxFactory.VarPattern(designation)
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SS0601)
				),
				diagnostic
			);
		}
	}
}
