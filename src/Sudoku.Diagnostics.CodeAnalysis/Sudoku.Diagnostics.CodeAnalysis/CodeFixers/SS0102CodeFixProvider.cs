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
			var (location, _) = diagnostic;
			var node = (InterpolatedStringExpressionSyntax)root.FindNode(location.SourceSpan, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0102,
					createChangedDocument: c => RemoveRedundantDollarMarkAsync(document, root, node, c),
					equivalenceKey: nameof(CodeFixTitles.SS0102)
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
		private static async Task<Document> RemoveRedundantDollarMarkAsync(
			Document document, SyntaxNode root, InterpolatedStringExpressionSyntax node,
			CancellationToken cancellationToken = default) => await Task.Run(() =>
			{
				var innerToken = node.Contents;

				var newRoot = root.ReplaceNode(
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

				return document.WithSyntaxRoot(newRoot);
			}, cancellationToken);
	}
}
