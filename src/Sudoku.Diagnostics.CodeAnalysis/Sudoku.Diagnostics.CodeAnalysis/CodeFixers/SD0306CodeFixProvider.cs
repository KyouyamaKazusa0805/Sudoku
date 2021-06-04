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
	/// <a href="https://github.com/SunnieShine/Sudoku/wiki/Rule-SD0306">SD0306</a>.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SD0306CodeFixProvider)), Shared]
	public sealed class SD0306CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
			DiagnosticIds.SD0306
		);

		/// <inheritdoc/>
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;


		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SD0306);
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), descriptor) = diagnostic;
			var node = (PrefixUnaryExpressionSyntax)root.FindNode(span, getInnermostNodeForTie: true);
			var tags = diagnostic.Properties;

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0306_1,
					createChangedDocument: async c =>
					{
						if (node is not { RawKind: (int)SyntaxKind.BitwiseNotExpression, Operand: var operand })
						{
							throw new InvalidOperationException("The specified node is invalid to fix.");
						}

						var editor = await DocumentEditor.CreateAsync(document);
						editor.ReplaceNode(node, operand);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SD0306_1)
				),
				diagnostic
			);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0306_2,
					createChangedDocument: async c =>
					{
						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.RemoveNode(node, SyntaxRemoveOptions.KeepTrailingTrivia);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SD0306_2)
				),
				diagnostic
			);
		}
	}
}
