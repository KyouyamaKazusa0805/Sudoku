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

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	/// <summary>
	/// Indicates the code fixer for solving the diagnostic result
	/// <a href="https://github.com/SunnieShine/Sudoku/wiki/Rule-SD0104">SD0104</a>.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SD0104CodeFixProvider)), Shared]
	public sealed class SD0104CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
			DiagnosticIds.SD0104
		);

		/// <inheritdoc/>
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;


		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SD0104));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var (_, span) = diagnostic.AdditionalLocations[0];
			var node = (PropertyDeclarationSyntax)root.FindNode(span, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0104,
					createChangedDocument: async c =>
					{
						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.ReplaceNode(
							node,
							node.WithAccessorList(
								SyntaxFactory.AccessorList(
									SyntaxFactory.SingletonList(
										SyntaxFactory.AccessorDeclaration(
											SyntaxKind.GetAccessorDeclaration
										)
										.WithSemicolonToken(
											SyntaxFactory.Token(SyntaxKind.SemicolonToken)
										)
									)
								)
							)
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SD0104)
				),
				diagnostic
			);
		}
	}
}
