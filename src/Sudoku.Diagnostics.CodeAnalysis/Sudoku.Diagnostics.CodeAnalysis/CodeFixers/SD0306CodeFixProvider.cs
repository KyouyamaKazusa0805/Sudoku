using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.Text.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	/// <summary>
	/// Indicates the code fixer for solving the diagnostic result
	/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/SD0306?sort_id=4041904">
	/// SD0306
	/// </a>.
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
			var (location, descriptor) = diagnostic;
			var node = root.FindNode(location.SourceSpan, getInnermostNodeForTie: true);
			var tags = diagnostic.Properties;

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0306_1,
					createChangedDocument: c => RemoveOperatorBitwiseNotAsync(
						document: document,
						root: root,
						node: node,
						cancellationToken: c
					),
					equivalenceKey: nameof(CodeFixTitles.SD0306_1)
				),
				diagnostic
			);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0306_2,
					createChangedDocument: c => RemoveExpressionAsync(
						document: document,
						root: root,
						node: node,
						cancellationToken: c
					),
					equivalenceKey: nameof(CodeFixTitles.SD0306_2)
				),
				diagnostic
			);
		}


		/// <summary>
		/// Delegated method that is invoked by <see cref="RegisterCodeFixesAsync(CodeFixContext)"/> above.
		/// </summary>
		/// <param name="document">The current document to fix.</param>
		/// <param name="root">The syntax root node.</param>
		/// <param name="node">
		/// The interpolted string expression node that the diagnostic result occurs.
		/// </param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task that handles this operation.</returns>
		/// <seealso cref="RegisterCodeFixesAsync(CodeFixContext)"/>
		/// <exception cref="InvalidOperationException">
		/// Throws when the current node is invalid to fix.
		/// </exception>
		private static async Task<Document> RemoveOperatorBitwiseNotAsync(
			Document document, SyntaxNode root, SyntaxNode node,
			CancellationToken cancellationToken = default) => await Task.Run(() =>
			{
				if (
					node is not PrefixUnaryExpressionSyntax
					{
						RawKind: (int)SyntaxKind.BitwiseNotExpression,
						Operand: var operand
					}
				)
				{
					throw new InvalidOperationException("The specified node is invalid to fix.");
				}

				var newRoot = root.ReplaceNode(node, operand);

				return document.WithSyntaxRoot(newRoot);
			}, cancellationToken);

		/// <summary>
		/// Delegated method that is invoked by <see cref="RegisterCodeFixesAsync(CodeFixContext)"/> above.
		/// </summary>
		/// <param name="document">The current document to fix.</param>
		/// <param name="root">The syntax root node.</param>
		/// <param name="node">
		/// The interpolted string expression node that the diagnostic result occurs.
		/// </param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task that handles this operation.</returns>
		/// <seealso cref="RegisterCodeFixesAsync(CodeFixContext)"/>
		private static async Task<Document> RemoveExpressionAsync(
			Document document, SyntaxNode root, SyntaxNode node,
			CancellationToken cancellationToken = default) => await Task.Run(() =>
			{
				var newRoot = root.RemoveNode(node, SyntaxRemoveOptions.KeepTrailingTrivia)!;

				return document.WithSyntaxRoot(newRoot);
			}, cancellationToken);
	}
}
