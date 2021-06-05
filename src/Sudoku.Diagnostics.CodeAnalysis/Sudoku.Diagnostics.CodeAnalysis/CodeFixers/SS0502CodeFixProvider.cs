using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Extensions;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	[CodeFixProvider("SS0502")]
	public sealed partial class SS0502CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0502));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), _) = diagnostic;
			var node = root.FindNode(span, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0502,
					createChangedDocument: async c =>
					{
						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.SetModifiers(node, DeclarationModifiers.None);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SS0502)
				),
				diagnostic
			);
		}
	}
}
