using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.Text.Extensions;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	[CodeFixProvider("SD0303")]
	public sealed partial class SD0303CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SD0303);
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), descriptor) = diagnostic;
			var node = root.FindNode(span, getInnermostNodeForTie: true);
			var tags = diagnostic.Properties;

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0303,
					createChangedDocument: async c =>
					{
						string typeName = tags["TypeName"]!;
						string fieldName = tags["PropertyName"]!;
						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.ReplaceNode(
							node,
							SyntaxFactory.MemberAccessExpression(
								SyntaxKind.SimpleMemberAccessExpression,
								SyntaxFactory.IdentifierName(typeName),
								SyntaxFactory.IdentifierName(fieldName)
							)
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SD0303)
				),
				diagnostic
			);
		}
	}
}
