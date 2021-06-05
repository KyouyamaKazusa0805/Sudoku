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
	[CodeFixProvider("SD0302")]
	public sealed partial class SD0302CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == DiagnosticIds.SD0302);
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), _) = diagnostic;
			var node = root.FindNode(span, getInnermostNodeForTie: true);
			var tags = diagnostic.Properties;

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0302,
					createChangedDocument: async c =>
					{
						string nodeName = tags["NodeName"]!;
						string @operator = tags["Operator"]!;

						var editor = await DocumentEditor.CreateAsync(document, c);
						var accessExpr = SyntaxFactory.MemberAccessExpression(
							SyntaxKind.SimpleMemberAccessExpression,
							SyntaxFactory.IdentifierName(nodeName),
							SyntaxFactory.IdentifierName("IsEmpty")
						);

						editor.ReplaceNode(
							node,
							@operator == string.Empty
							? accessExpr
							: SyntaxFactory.PrefixUnaryExpression(
								SyntaxKind.LogicalNotExpression,
								accessExpr
							)
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SD0302)
				),
				diagnostic
			);
		}
	}
}
