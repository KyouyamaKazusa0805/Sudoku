using System;
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
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	[CodeFixProvider("SD0309")]
	public sealed partial class SD0309CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SD0309);
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), _) = diagnostic;
			var node = root.FindNode(span, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0309,
					createChangedDocument: async c =>
					{
						var (parent, replaceNode) = node switch
						{
							ImplicitArrayCreationExpressionSyntax
							{
								Parent: ArgumentSyntax { Parent: ArgumentListSyntax { Parent: var p } },
								Initializer: var initializer
							} => (p, z(p, initializer)),

							ArrayCreationExpressionSyntax
							{
								Parent: ArgumentSyntax { Parent: ArgumentListSyntax { Parent: var p } },
								Initializer: var initializer
							} => (p, z(p, initializer)),

							ImplicitStackAllocArrayCreationExpressionSyntax
							{
								Parent: ArgumentSyntax { Parent: ArgumentListSyntax { Parent: var p } },
								Initializer: var initializer
							} => (p, z(p, initializer)),

							StackAllocArrayCreationExpressionSyntax
							{
								Parent: ArgumentSyntax { Parent: ArgumentListSyntax { Parent: var p } },
								Initializer: var initializer
							} => (p, z(p, initializer))
						};

						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.ReplaceNode(parent!, replaceNode);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SD0309)
				),
				diagnostic
			);

			static SyntaxNode z(SyntaxNode? p, InitializerExpressionSyntax? initializer) => p switch
			{
				ObjectCreationExpressionSyntax { Type: var typeName } =>
					SyntaxFactory.ObjectCreationExpression(typeName)
					.WithInitializer(initializer),
				ImplicitObjectCreationExpressionSyntax =>
					SyntaxFactory.ImplicitObjectCreationExpression()
					.WithInitializer(initializer)
			};
		}
	}
}
