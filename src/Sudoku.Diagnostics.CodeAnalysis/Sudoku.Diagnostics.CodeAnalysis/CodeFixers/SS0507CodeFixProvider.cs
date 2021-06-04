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
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.CodeFixers
{
	/// <summary>
	/// Indicates the code fixer for solving the diagnostic result
	/// <a href="https://github.com/SunnieShine/Sudoku/wiki/Rule-SS0507">SS0507</a>.
	/// </summary>
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SS0507CodeFixProvider)), Shared]
	public sealed class SS0507CodeFixProvider : CodeFixProvider
	{
		/// <inheritdoc/>
		public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
			DiagnosticIds.SS0507
		);

		/// <inheritdoc/>
		public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;


		/// <inheritdoc/>
		public override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var document = context.Document;
			var diagnostic = context.Diagnostics.First(static d => d.Id == nameof(DiagnosticIds.SS0507));
			var root = (await document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false))!;
			var ((_, span), _) = diagnostic;
			var (_, declSpan) = diagnostic.AdditionalLocations[0];
			var parameter = (ParameterSyntax)root.FindNode(span, getInnermostNodeForTie: true);
			var typeDeclaration = (TypeDeclarationSyntax)root.FindNode(declSpan, getInnermostNodeForTie: true);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0507_1,
					createChangedDocument: c => AppendFieldAsync(document, parameter, typeDeclaration, c),
					equivalenceKey: nameof(CodeFixTitles.SS0507_1)
				),
				diagnostic
			);

			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0507_2,
					createChangedDocument: c => AppendPropertyAsync(document, parameter, typeDeclaration, c),
					equivalenceKey: nameof(CodeFixTitles.SS0507_2)
				),
				diagnostic
			);
		}


		/// <summary>
		/// Delegated method that is invoked by <see cref="RegisterCodeFixesAsync(CodeFixContext)"/> above.
		/// </summary>
		/// <param name="document">The current document to fix.</param>
		/// <param name="parameter">The parameter node that the diagnostic result occurs.</param>
		/// <param name="typeDeclaration">The type declaration.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task that handles this operation.</returns>
		/// <seealso cref="RegisterCodeFixesAsync(CodeFixContext)"/>
		private static async Task<Document> AppendFieldAsync(
			Document document, ParameterSyntax parameter, TypeDeclarationSyntax typeDeclaration,
			CancellationToken cancellationToken)
		{
			var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
			editor.AddMember(
				typeDeclaration,
				SyntaxFactory.FieldDeclaration(
					SyntaxFactory.VariableDeclaration(
						SyntaxFactory.IdentifierName(parameter.Type?.ToString() ?? "int")
					)
					.WithVariables(
						SyntaxFactory.SingletonSeparatedList(
							SyntaxFactory.VariableDeclarator(
								SyntaxFactory.Identifier(
									parameter.Identifier.ValueText.ToCamelCase(
										CaseConvertingOption.ReserveLeadingUnderscore
									)
								)
							)
						)
					)
				)
				.WithModifiers(
					SyntaxFactory.TokenList(
						new[]
						{
							SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
							SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
						}
					)
				)
			);

			return document.WithSyntaxRoot(editor.GetChangedRoot());
		}

		/// <summary>
		/// Delegated method that is invoked by <see cref="RegisterCodeFixesAsync(CodeFixContext)"/> above.
		/// </summary>
		/// <param name="document">The current document to fix.</param>
		/// <param name="parameter">The parameter node that the diagnostic result occurs.</param>
		/// <param name="typeDeclaration">The type declaration.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>A task that handles this operation.</returns>
		/// <seealso cref="RegisterCodeFixesAsync(CodeFixContext)"/>
		private static async Task<Document> AppendPropertyAsync(
			Document document, ParameterSyntax parameter, TypeDeclarationSyntax typeDeclaration,
			CancellationToken cancellationToken)
		{
			var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
			editor.AddMember(
				typeDeclaration,
				SyntaxFactory.PropertyDeclaration(
					SyntaxFactory.IdentifierName(parameter.Type?.ToString() ?? "int"),
					SyntaxFactory.Identifier(parameter.Identifier.ValueText.ToPascalCase())
				)
				.WithModifiers(
					SyntaxFactory.TokenList(
						SyntaxFactory.Token(SyntaxKind.PublicKeyword)
					)
				)
				.WithAccessorList(
					SyntaxFactory.AccessorList(
						SyntaxFactory.List(
							new[]
							{
								SyntaxFactory.AccessorDeclaration(
									SyntaxKind.GetAccessorDeclaration
								)
								.WithSemicolonToken(
									SyntaxFactory.Token(SyntaxKind.SemicolonToken)
								),
								SyntaxFactory.AccessorDeclaration(
									SyntaxKind.InitAccessorDeclaration
								)
								.WithSemicolonToken(
									SyntaxFactory.Token(SyntaxKind.SemicolonToken)
								)
							}
						)
					)
				)
			);

			return document.WithSyntaxRoot(editor.GetChangedRoot());
		}
	}
}
