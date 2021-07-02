using Microsoft.CodeAnalysis.CodeActions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.CodeGenerating;
using Microsoft.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.CSharp;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	[CodeFixProvider("SS9008")]
	public sealed partial class SS9008CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS9008));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var (_, span) = diagnostic.AdditionalLocations[0];
			var node = (PropertyDeclarationSyntax)root.FindNode(span, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS9008,
					createChangedDocument: async c =>
					{
						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.ReplaceNode(
							node,
							node.AddModifiers(
								SyntaxFactory.Token(
									SyntaxKind.ReadOnlyKeyword
								)
							)
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SS9008)
				),
				diagnostic
			);
		}
	}
}
