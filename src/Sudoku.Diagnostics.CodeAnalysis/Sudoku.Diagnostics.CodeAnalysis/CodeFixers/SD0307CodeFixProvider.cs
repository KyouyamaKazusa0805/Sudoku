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
using Microsoft.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.Text.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	/// <summary>
	/// Indicates the code fixer for solving the diagnostic result
	/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/SD0307?sort_id=4042004">
	/// SD0307
	/// </a>.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SD0307CodeFixProvider)), Shared]
	public sealed class SD0307CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
			DiagnosticIds.SD0307
		);

		/// <inheritdoc/>
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;


		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SD0307);
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var (location, descriptor) = diagnostic;
			var node = root.FindNode(location.SourceSpan, getInnermostNodeForTie: true);
			var tags = diagnostic.Properties;
			string realValueStr = tags["RealValue"]!;

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0307,
					createChangedDocument: async c => await Task.Run(() =>
					{
						if (node is not PrefixUnaryExpressionSyntax { RawKind: var kind })
						{
							throw new InvalidOperationException("The specified node is invalid to fix.");
						}

						int realValueToFix = int.Parse(realValueStr);
						ExpressionSyntax operand = kind switch
						{
							(int)SyntaxKind.UnaryPlusExpression =>
								SyntaxFactory.LiteralExpression(
									SyntaxKind.NumericLiteralExpression,
									SyntaxFactory.Literal(realValueToFix)
								),
							(int)SyntaxKind.UnaryMinusExpression =>
								SyntaxFactory.PrefixUnaryExpression(
									SyntaxKind.BitwiseNotExpression,
									SyntaxFactory.LiteralExpression(
										SyntaxKind.NumericLiteralExpression,
										SyntaxFactory.Literal(realValueToFix)
									)
								)
						};

						var newRoot = root.ReplaceNode(node, operand);

						return document.WithSyntaxRoot(newRoot);
					}, c),
					equivalenceKey: nameof(CodeFixTitles.SD0307)
				),
				diagnostic
			);
		}
	}
}
