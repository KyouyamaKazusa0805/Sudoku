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
							id: DiagnosticIds.SS0501,
							title: Titles.SS0501,
							messageFormat: Messages.SS0501,
							category: Categories.Design,
							defaultSeverity: DiagnosticSeverity.Warning,
							isEnabledByDefault: true,
							helpLinkUri: HelpLinks.SS0501
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
								id: DiagnosticIds.SS0505,
								title: Titles.SS0505,
								messageFormat: Messages.SS0505,
								category: Categories.Design,
								defaultSeverity: DiagnosticSeverity.Warning,
								isEnabledByDefault: true,
								helpLinkUri: HelpLinks.SS0505
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
							id: DiagnosticIds.SS0502,
							title: Titles.SS0502,
							messageFormat: Messages.SS0502,
							category: Categories.Design,
							defaultSeverity: DiagnosticSeverity.Warning,
							isEnabledByDefault: true,
							helpLinkUri: HelpLinks.SS0502
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
							id: DiagnosticIds.SS0504,
							title: Titles.SS0504,
							messageFormat: Messages.SS0504,
							category: Categories.Design,
							defaultSeverity: DiagnosticSeverity.Warning,
							isEnabledByDefault: true,
							helpLinkUri: HelpLinks.SS0504
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
							id: DiagnosticIds.SS0503,
							title: Titles.SS0503,
							messageFormat: Messages.SS0503,
							category: Categories.Design,
							defaultSeverity: DiagnosticSeverity.Warning,
							isEnabledByDefault: true,
							helpLinkUri: HelpLinks.SS0503
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
