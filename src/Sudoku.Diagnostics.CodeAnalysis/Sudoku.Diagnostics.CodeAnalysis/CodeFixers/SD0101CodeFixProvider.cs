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
	/// <a href="https://gitee.com/SunnieShine/Sudoku/wikis/SD0101?sort_id=3599824">
	/// SD0101
	/// </a>.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SD0101CodeFixProvider))]
	[Shared]
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

		#region NodesToAppend
		/// <summary>
		/// Indicates the node to append.
		/// </summary>
		private static readonly SyntaxList<SyntaxNode> NodesToAppend =
			SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
				SyntaxFactory.PropertyDeclaration(
					SyntaxFactory.IdentifierName("TechniqueProperties"),
					SyntaxFactory.Identifier("Properties")
				)
				.WithModifiers(
					SyntaxFactory.TokenList(
						new[]
						{
							SyntaxFactory.Token(
								SyntaxFactory.TriviaList(
									SyntaxFactory.Trivia(
										SyntaxFactory.DocumentationCommentTrivia(
											SyntaxKind.SingleLineDocumentationCommentTrivia,
											SyntaxFactory.List(
												new XmlNodeSyntax[]
												{
													SyntaxFactory.XmlText()
													.WithTextTokens(
														SyntaxFactory.TokenList(
															SyntaxFactory.XmlTextLiteral(
																SyntaxFactory.TriviaList(
																	SyntaxFactory.DocumentationCommentExterior("///")
																),
																" ",
																" ",
																SyntaxFactory.TriviaList()
															)
														)
													),
													SyntaxFactory.XmlNullKeywordElement()
													.WithName(
														SyntaxFactory.XmlName(
															SyntaxFactory.Identifier("inheritdoc")
														)
													)
													.WithAttributes(
														SyntaxFactory.SingletonList<XmlAttributeSyntax>(
															SyntaxFactory.XmlCrefAttribute(
																SyntaxFactory.NameMemberCref(
																	SyntaxFactory.IdentifierName("SearchingProperties")
																)
															)
														)
													),
													SyntaxFactory.XmlText()
													.WithTextTokens(
														SyntaxFactory.TokenList(
															SyntaxFactory.XmlTextNewLine(
																SyntaxFactory.TriviaList(),
																Environment.NewLine,
																Environment.NewLine,
																SyntaxFactory.TriviaList()
															)
														)
													)
												}
											)
										)
									)
								),
								SyntaxKind.PublicKeyword,
								SyntaxFactory.TriviaList()
							),
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
									new SyntaxNodeOrToken[]{
										SyntaxFactory.Argument(
											SyntaxFactory.LiteralExpression(
												SyntaxKind.DefaultLiteralExpression,
												SyntaxFactory.Token(SyntaxKind.DefaultKeyword)
											)
										)
										.WithNameColon(
											SyntaxFactory.NameColon(
												SyntaxFactory.IdentifierName("priority")
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
										.WithNameColon(
											SyntaxFactory.NameColon(
												SyntaxFactory.IdentifierName("displayLabel")
											)
										)
									}
								)
							)
						)
						.WithInitializer(
							SyntaxFactory.InitializerExpression(
								SyntaxKind.ObjectInitializerExpression,
								SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(
									SyntaxFactory.AssignmentExpression(
										SyntaxKind.SimpleAssignmentExpression,
										SyntaxFactory.IdentifierName("DisplayLevel"),
										SyntaxFactory.LiteralExpression(
											SyntaxKind.DefaultLiteralExpression,
											SyntaxFactory.Token(
												SyntaxFactory.TriviaList(),
												SyntaxKind.DefaultKeyword,
												SyntaxFactory.TriviaList(
													SyntaxFactory.Comment("// Please check and replace this value.")
												)
											)
										)
									)
								)
							)
						)
					)
				)
				.WithSemicolonToken(
					SyntaxFactory.Token(SyntaxKind.SemicolonToken)
				)
				.NormalizeWhitespace()
			);
		#endregion


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
			var diagnostic = context.Diagnostics.First();
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var (location, _) = diagnostic;
			var typeDeclaration = root
				.FindToken(location.SourceSpan.Start)
				.Parent!
				.AncestorsAndSelf()
				.OfType<TypeDeclarationSyntax>()
				.First();

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
			var declarations = typeDeclaration.DescendantNodes().OfType<MemberDeclarationSyntax>();

			int i = Enumerable.Range(1, OrderOfMemberKinds.Length).Reverse().FirstOrDefault(
				i => declarations.Any(d => d.RawKind == (int)OrderOfMemberKinds[i - 1])
			) - 1;
			var descendants = typeDeclaration.DescendantNodes();
			var newRoot = root.InsertNodesAfter(
				i switch
				{
					-1 when descendants.First() is var firstDescendant => firstDescendant,
					_ when descendants.Last(n => n.IsKind(OrderOfMemberKinds[i])) is var lastMember => lastMember
				},
				NodesToAppend
			);

			return document.WithSyntaxRoot(newRoot);
		}
	}
}
