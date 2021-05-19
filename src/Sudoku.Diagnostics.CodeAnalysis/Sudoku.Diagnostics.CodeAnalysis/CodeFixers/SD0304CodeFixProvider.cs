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
using Microsoft.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.Text.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	/// <summary>
	/// Indicates the code fixer for solving the diagnostic result
	/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/SD0304?sort_id=4035455">
	/// SD0304
	/// </a>.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SD0304CodeFixProvider)), Shared]
	public sealed class SD0304CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
			DiagnosticIds.SD0304
		);

		/// <inheritdoc/>
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;


		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SD0304);
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var (location, descriptor) = diagnostic;
			var node = root.FindNode(location.SourceSpan);
			var tags = diagnostic.Properties;

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0304,
					createChangedDocument: c => UseDefaultFieldInsteadAsync(
						document: document,
						root: root,
						node: node,
						left: tags["Operator"]!,
						notEqualsToken: tags["Variable"]!,
						fieldName: tags["PropertyName"]!,
						cancellationToken: c
					),
					equivalenceKey: nameof(CodeFixTitles.SD0304)
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
		/// <param name="left">The left-side expression or variable.</param>
		/// <param name="notEqualsToken">
		/// The string that stores the symbol <c>!</c> or <see cref="string.Empty"/> to indicate
		/// the which kind of the expression is, equals expression or not equals expression.
		/// </param>
		/// <param name="fieldName">The member name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task that handles this operation.</returns>
		/// <seealso cref="RegisterCodeFixesAsync(CodeFixContext)"/>
		private static async Task<Document> UseDefaultFieldInsteadAsync(
			Document document, SyntaxNode root, SyntaxNode node, string left,
			string notEqualsToken, string fieldName, CancellationToken cancellationToken = default) =>
			await Task.Run(() =>
			{
				var invocationExpr = SyntaxFactory.MemberAccessExpression(
					SyntaxKind.SimpleMemberAccessExpression,
					SyntaxFactory.IdentifierName(left),
					SyntaxFactory.IdentifierName(fieldName)
				);

				var newRoot = root.ReplaceNode(
					node,
					SyntaxFactory.ExpressionStatement(
						notEqualsToken == string.Empty
						? invocationExpr
						: SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, invocationExpr)
					)
					.WithSemicolonToken(
						SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken)
					)
				);

				return document.WithSyntaxRoot(newRoot);
			}, cancellationToken);
	}
}
