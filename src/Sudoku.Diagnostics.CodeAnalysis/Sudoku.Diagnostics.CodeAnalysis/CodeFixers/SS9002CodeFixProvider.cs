using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.Text.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	/// <summary>
	/// Indicates the code fixer for solving the diagnostic result
	/// <a href="https://github.com/SunnieShine/Sudoku/wiki/Rule-SS9002">SS9002</a>.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SS9002CodeFixProvider)), Shared]
	public sealed class SS9002CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
			DiagnosticIds.SS9002
		);

		/// <inheritdoc/>
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;


		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SS9002);
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var (_, initializerSpan) = diagnostic.AdditionalLocations[0];
			var initializerNode = (InitializerExpressionSyntax)root.FindNode(initializerSpan, getInnermostNodeForTie: true);
			var (_, valueSpan) = diagnostic.AdditionalLocations[1];
			var valueNode = (ExpressionSyntax)root.FindNode(valueSpan, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS9002,
					createChangedDocument: c => RemoveRedundantArrayTypeSyntaxAsync(
						document: document,
						root: root,
						initializerNode: initializerNode,
						valueNode: valueNode,
						cancellationToken: c
					),
					equivalenceKey: nameof(CodeFixTitles.SS9002)
				),
				diagnostic
			);
		}


		/// <summary>
		/// Delegated method that is invoked by <see cref="RegisterCodeFixesAsync(CodeFixContext)"/> above.
		/// </summary>
		/// <param name="document">The current document to fix.</param>
		/// <param name="root">The root of the syntax tree.</param>
		/// <param name="initializerNode">The initializer node.</param>
		/// <param name="valueNode"></param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task that handles this operation.</returns>
		/// <seealso cref="RegisterCodeFixesAsync(CodeFixContext)"/>
		private static async Task<Document> RemoveRedundantArrayTypeSyntaxAsync(
			Document document, SyntaxNode root, InitializerExpressionSyntax initializerNode,
			ExpressionSyntax valueNode, CancellationToken cancellationToken = default) =>
			await Task.Run(() =>
			{
				var newRoot = root.ReplaceNode(valueNode, initializerNode);

				return document.WithSyntaxRoot(newRoot);
			}, cancellationToken);
	}
}
