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
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.Text.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	/// <summary>
	/// Indicates the code fixer for solving the diagnostic result
	/// <a href="https://github.com/SunnieShine/Sudoku/wiki/Rule-SD0101">SD0101</a>.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SD0101CodeFixProvider)), Shared]
	public sealed class SD0101CodeFixProvider : CodeFixProvider
	{
		private static readonly UsingDirectiveSyntax AdditionalUsingDiretive =
			SyntaxFactory.UsingDirective(
				SyntaxFactory.QualifiedName(
					SyntaxFactory.QualifiedName(
						SyntaxFactory.IdentifierName("Sudoku"),
						SyntaxFactory.IdentifierName("Solving")
					),
					SyntaxFactory.IdentifierName("Manual")
				)
			);


		/// <inheritdoc/>
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
			DiagnosticIds.SD0101
		);

		/// <inheritdoc/>
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;


		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SD0101));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), _) = diagnostic;
			var typeDeclaration = (TypeDeclarationSyntax)root.FindNode(span, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SD0101,
					createChangedDocument: async c =>
					{
						var root = (await document.GetSyntaxRootAsync(c).ConfigureAwait(false))!;
						var descendantAndItself = root.DescendantNodesAndSelf();
						var compilationUnit = descendantAndItself.OfType<CompilationUnitSyntax>().First();
						var usings = compilationUnit.Usings;
						bool needAppendUsingDirective = true;
						if (usings.Any(static @using => @using.Name.ToString() == "Sudoku.Solving.Manual"))
						{
							needAppendUsingDirective = false;
						}

						var editor = await DocumentEditor.CreateAsync(document, c);
						if (needAppendUsingDirective)
						{
							editor.ReplaceNode(
								compilationUnit,
								compilationUnit.WithUsings(usings.Add(AdditionalUsingDiretive))
							);
						}

						editor.AddMember(
							typeDeclaration,
							SyntaxFactory.PropertyDeclaration(
								SyntaxFactory.IdentifierName("TechniqueProperties"),
								SyntaxFactory.Identifier("Properties")
							)
							.WithModifiers(
								SyntaxFactory.TokenList(
									new[]
									{
										SyntaxFactory.Token(SyntaxKind.PublicKeyword),
										SyntaxFactory.Token(SyntaxKind.StaticKeyword)
									}
								)
							)
							.WithAccessorList(
								SyntaxFactory.AccessorList(
									SyntaxFactory.SingletonList(
										SyntaxFactory.AccessorDeclaration(
											SyntaxKind.GetAccessorDeclaration
										)
										.WithSemicolonToken(
											SyntaxFactory.Token(SyntaxKind.SemicolonToken)
										)
									)
								)
							)
							.WithInitializer(
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
														SyntaxFactory.LiteralExpression(
															SyntaxKind.StringLiteralExpression,
															SyntaxFactory.Literal(string.Empty)
														)
													)
												}
											)
										)
									)
								)
							)
							.WithSemicolonToken(
								SyntaxFactory.Token(SyntaxKind.SemicolonToken)
							)
						);

						return document.WithSyntaxRoot(editor.GetChangedRoot());
					},
					equivalenceKey: nameof(CodeFixTitles.SD0101)
				),
				diagnostic
			);
		}
	}
}
