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
		/// <summary>
		/// To provide a way to insert property into the document, via the current order.
		/// </summary>
		private static readonly SyntaxKind[] OrderOfMemberKinds = new[]
		{
			SyntaxKind.FieldDeclaration,
			SyntaxKind.ConstructorDeclaration
		};


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
					createChangedDocument: c => AppendTechniquePropertyAsync(document, typeDeclaration, c),
					equivalenceKey: nameof(CodeFixTitles.SD0101)
				),
				diagnostic
			);
		}


		/// <summary>
		/// Delegated method that is invoked by <see cref="RegisterCodeFixesAsync(CodeFixContext)"/> above.
		/// </summary>
		/// <param name="document">The current document to fix.</param>
		/// <param name="typeDeclaration">The type declarartion node that the diagnostic result occurs.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task that handles this operation.</returns>
		/// <seealso cref="RegisterCodeFixesAsync(CodeFixContext)"/>
		private static async Task<Document> AppendTechniquePropertyAsync(
			Document document, TypeDeclarationSyntax typeDeclaration, CancellationToken cancellationToken)
		{
			var root = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false))!;
			var compilationUnit = root.DescendantNodesAndSelf().OfType<CompilationUnitSyntax>().First();
			var usings = compilationUnit.Usings;
			bool needAppendUsingDirective = true;
			if (usings.Any(static @using => @using.Name.ToString() == "Sudoku.Solving.Manual"))
			{
				needAppendUsingDirective = false;
			}

			var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
			if (needAppendUsingDirective)
			{
				editor.ReplaceNode(
					compilationUnit,
					compilationUnit.WithUsings(
						usings.Add(
							SyntaxFactory.UsingDirective(
								SyntaxFactory.QualifiedName(
									SyntaxFactory.QualifiedName(
										SyntaxFactory.IdentifierName("Sudoku"),
										SyntaxFactory.IdentifierName("Solving")
									),
									SyntaxFactory.IdentifierName("Manual")
								)
							)
						)
					)
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
		}
	}
}
