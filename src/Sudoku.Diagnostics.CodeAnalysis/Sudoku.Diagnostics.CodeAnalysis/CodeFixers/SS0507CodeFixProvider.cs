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

			// Append field.
			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0507_1,
					createChangedDocument: async c =>
					{
						var editor = await DocumentEditor.CreateAsync(document, c);
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
					},
					equivalenceKey: nameof(CodeFixTitles.SS0507_1)
				),
				diagnostic
			);

			// Append property.
			context.RegisterCodeFix(
				CodeAction.Create(
					title: CodeFixTitles.SS0507_2,
					createChangedDocument: async c =>
					{
						var editor = await DocumentEditor.CreateAsync(document, c);
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
					},
					equivalenceKey: nameof(CodeFixTitles.SS0507_2)
				),
				diagnostic
			);
		}
	}
}
