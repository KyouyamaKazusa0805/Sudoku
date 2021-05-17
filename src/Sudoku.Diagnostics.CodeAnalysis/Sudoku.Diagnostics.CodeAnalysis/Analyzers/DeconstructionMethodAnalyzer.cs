using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for deconstruction methods.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class DeconstructionMethodAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				AnalyzeSyntaxNode,
				new[] { SyntaxKind.MethodDeclaration }
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			if (
				context.Node is not MethodDeclarationSyntax
				{
					Identifier: { ValueText: "Deconstruct" } identifier,
					ParameterList: { Parameters: var parameters },
					Modifiers: var modifiers,
					ReturnType: var returnType
				}
			)
			{
				return;
			}

			CheckSudoku025And029(context, parameters, identifier);
			CheckSudoku026And028(context, modifiers, identifier);
			CheckSudoku027(context, returnType);
		}


		private static void CheckSudoku025And029(
			SyntaxNodeAnalysisContext context, SeparatedSyntaxList<ParameterSyntax> parameters,
			SyntaxToken identifier)
		{
			if (parameters.Count < 2)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: new(
							id: DiagnosticIds.Sudoku025,
							title: Titles.Sudoku025,
							messageFormat: Messages.Sudoku025,
							category: Categories.Design,
							defaultSeverity: DiagnosticSeverity.Warning,
							isEnabledByDefault: true,
							helpLinkUri: HelpLinks.Sudoku025
						),
						location: identifier.GetLocation(),
						messageArgs: null
					)
				);
			}

			foreach (var parameter in parameters)
			{
				if (parameter.Modifiers.All(DoesNotContainOutKeyword))
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: new(
								id: DiagnosticIds.Sudoku029,
								title: Titles.Sudoku029,
								messageFormat: Messages.Sudoku029,
								category: Categories.Design,
								defaultSeverity: DiagnosticSeverity.Warning,
								isEnabledByDefault: true,
								helpLinkUri: HelpLinks.Sudoku029
							),
							location: parameter.GetLocation(),
							messageArgs: null
						)
					);
				}
			}
		}

		private static void CheckSudoku026And028(
			SyntaxNodeAnalysisContext context, SyntaxTokenList modifiers, SyntaxToken identifier)
		{
			if (modifiers.Any(MustBeStatic))
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: new(
							id: DiagnosticIds.Sudoku026,
							title: Titles.Sudoku026,
							messageFormat: Messages.Sudoku026,
							category: Categories.Design,
							defaultSeverity: DiagnosticSeverity.Warning,
							isEnabledByDefault: true,
							helpLinkUri: HelpLinks.Sudoku026
						),
						location: identifier.GetLocation(),
						messageArgs: null
					)
				);
			}

			if (modifiers.All(DoesNotContainPublicKeyword))
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: new(
							id: DiagnosticIds.Sudoku028,
							title: Titles.Sudoku028,
							messageFormat: Messages.Sudoku028,
							category: Categories.Design,
							defaultSeverity: DiagnosticSeverity.Warning,
							isEnabledByDefault: true,
							helpLinkUri: HelpLinks.Sudoku028
						),
						location: identifier.GetLocation(),
						messageArgs: null
					)
				);
			}
		}

		private static void CheckSudoku027(SyntaxNodeAnalysisContext context, TypeSyntax returnType)
		{
			if (
				context.SemanticModel.GetOperation(returnType) is { Type: { } type }
				&& SymbolEqualityComparer.Default.Equals(
					type,
					context.Compilation.GetSpecialType(SpecialType.System_Void)
				)
			)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: new(
							id: DiagnosticIds.Sudoku027,
							title: Titles.Sudoku027,
							messageFormat: Messages.Sudoku027,
							category: Categories.Design,
							defaultSeverity: DiagnosticSeverity.Warning,
							isEnabledByDefault: true,
							helpLinkUri: HelpLinks.Sudoku027
						),
						location: returnType.GetLocation(),
						messageArgs: null
					)
				);
			}
		}

		private static bool DoesNotContainPublicKeyword(SyntaxToken token) => token.RawKind != (int)SyntaxKind.PublicKeyword;
		private static bool MustBeStatic(SyntaxToken token) => token.RawKind == (int)SyntaxKind.StaticKeyword;
		private static bool DoesNotContainOutKeyword(SyntaxToken token) => token.RawKind != (int)SyntaxKind.OutKeyword;
	}
}
