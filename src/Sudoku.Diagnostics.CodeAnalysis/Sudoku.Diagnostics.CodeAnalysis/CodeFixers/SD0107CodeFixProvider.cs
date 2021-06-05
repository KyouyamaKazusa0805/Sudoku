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
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	[CodeFixProvider("SD0107")]
	public sealed partial class SD0107CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SD0107));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), _) = diagnostic;
			var propertyDecl = (PropertyDeclarationSyntax)root.FindNode(span, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0107,
					createChangedDocument: async c =>
					{
						var editor = await DocumentEditor.CreateAsync(document, c);
						editor.ReplaceNode(
							propertyDecl,
							propertyDecl.WithInitializer(
								SyntaxFactory.EqualsValueClause(
									SyntaxFactory.ImplicitObjectCreationExpression()
									.WithArgumentList(
										SyntaxFactory.ArgumentList(
											SyntaxFactory.SeparatedList<ArgumentSyntax>(
												new SyntaxNodeOrToken[]
												{
													SyntaxFactory.Argument(
														SyntaxFactory.LiteralExpression(
															SyntaxKind.DefaultLiteralExpression,
															SyntaxFactory.Token(SyntaxKind.DefaultKeyword)
														)
													),
													SyntaxFactory.Token(SyntaxKind.CommaToken),
													SyntaxFactory.Argument(
														SyntaxFactory.MemberAccessExpression(
															SyntaxKind.SimpleMemberAccessExpression,
															SyntaxFactory.PredefinedType(
																SyntaxFactory.Token(SyntaxKind.StringKeyword)
															),
															SyntaxFactory.IdentifierName("Empty")
														)
													)
												}
											)
										)
									)
								)
							)
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SD0107)
				),
				diagnostic
			);
		}
	}
}
