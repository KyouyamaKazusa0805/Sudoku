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
	/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/SD0309?sort_id=4041633">
	/// SD0309
	/// </a>.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SD0309CodeFixProvider)), Shared]
	public sealed class SD0309CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
			DiagnosticIds.SD0309
		);

		/// <inheritdoc/>
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;


		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SD0309);
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var (location, descriptor) = diagnostic;
			var node = root.FindNode(location.SourceSpan, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0309,
					createChangedDocument: c => UseObjectInitializerAsync(document, root, node, c),
					equivalenceKey: nameof(CodeFixTitles.SD0309)
				),
				diagnostic
			);
		}


		private static async Task<Document> UseObjectInitializerAsync(
			Document document, SyntaxNode root, SyntaxNode node,
			CancellationToken cancellationToken = default) =>
			await Task.Run(() =>
			{
				switch (node)
				{
					case ImplicitArrayCreationExpressionSyntax
					{
						Parent: ArgumentSyntax { Parent: ArgumentListSyntax { Parent: var parent } },
						Initializer: var initializer
					}:
					{
						SyntaxNode replaceNode = parent switch
						{
							ObjectCreationExpressionSyntax { Type: var typeName } =>
								SyntaxFactory.ObjectCreationExpression(typeName)
								.WithInitializer(initializer),
							ImplicitObjectCreationExpressionSyntax =>
								SyntaxFactory.ImplicitObjectCreationExpression()
								.WithInitializer(initializer)
						};

						var newRoot = root.ReplaceNode(parent, replaceNode);

						return document.WithSyntaxRoot(newRoot);
					}
					case ArrayCreationExpressionSyntax
					{
						Parent: ArgumentSyntax { Parent: ArgumentListSyntax { Parent: var parent } },
						Initializer: var initializer
					}:
					{
						SyntaxNode replaceNode = parent switch
						{
							ObjectCreationExpressionSyntax { Type: var typeName } =>
								SyntaxFactory.ObjectCreationExpression(typeName)
								.WithInitializer(initializer),
							ImplicitObjectCreationExpressionSyntax =>
								SyntaxFactory.ImplicitObjectCreationExpression()
								.WithInitializer(initializer)
						};

						var newRoot = root.ReplaceNode(parent, replaceNode);

						return document.WithSyntaxRoot(newRoot);
					}
					case ImplicitStackAllocArrayCreationExpressionSyntax
					{
						Parent: ArgumentSyntax { Parent: ArgumentListSyntax { Parent: var parent } },
						Initializer: var initializer
					}:
					{
						SyntaxNode replaceNode = parent switch
						{
							ObjectCreationExpressionSyntax { Type: var typeName } =>
								SyntaxFactory.ObjectCreationExpression(typeName)
								.WithInitializer(initializer),
							ImplicitObjectCreationExpressionSyntax =>
								SyntaxFactory.ImplicitObjectCreationExpression()
								.WithInitializer(initializer)
						};

						var newRoot = root.ReplaceNode(parent, replaceNode);

						return document.WithSyntaxRoot(newRoot);
					}
					case StackAllocArrayCreationExpressionSyntax
					{
						Parent: ArgumentSyntax { Parent: ArgumentListSyntax { Parent: var parent } },
						Initializer: var initializer
					}:
					{
						SyntaxNode replaceNode = parent switch
						{
							ObjectCreationExpressionSyntax { Type: var typeName } =>
								SyntaxFactory.ObjectCreationExpression(typeName)
								.WithInitializer(initializer),
							ImplicitObjectCreationExpressionSyntax =>
								SyntaxFactory.ImplicitObjectCreationExpression()
								.WithInitializer(initializer)
						};

						var newRoot = root.ReplaceNode(parent, replaceNode);

						return document.WithSyntaxRoot(newRoot);
					}
					default:
					{
						throw new InvalidOperationException("The specified node is invalid.");
					}
				}
			}, cancellationToken);
	}
}
