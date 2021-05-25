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

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.MethodDeclaration });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			if (
				context.Node is not MethodDeclarationSyntax
				{
					Parent: var parentNode,
					Identifier: { ValueText: "Deconstruct" } identifier,
					ParameterList: { Parameters: var parameters },
					Modifiers: var modifiers,
					ReturnType: var returnType
				}
				/*slice-pattern*/
				|| parentNode is ClassDeclarationSyntax { Modifiers: var classModifiers }
				&& classModifiers.Any(SyntaxKind.StaticKeyword)
			)
			{
				return;
			}

			CheckSS0501AndSS0505(context, parameters, identifier);
			CheckSudokuSS0502AndSS0504(context, modifiers, identifier);
			CheckSS0503(context, returnType);
		}


		private static void CheckSS0501AndSS0505(
			SyntaxNodeAnalysisContext context, SeparatedSyntaxList<ParameterSyntax> parameters,
			SyntaxToken identifier)
		{
			if (parameters.Count < 2)
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0501,
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
							descriptor: SS0505,
							location: parameter.GetLocation(),
							messageArgs: null
						)
					);
				}
			}
		}

		private static void CheckSudokuSS0502AndSS0504(
			SyntaxNodeAnalysisContext context, SyntaxTokenList modifiers, SyntaxToken identifier)
		{
			if (modifiers.Any(SyntaxKind.StaticKeyword))
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0502,
						location: identifier.GetLocation(),
						messageArgs: null
					)
				);
			}

			if (modifiers.All(DoesNotContainPublicKeyword))
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0504,
						location: identifier.GetLocation(),
						messageArgs: null
					)
				);
			}
		}

		private static void CheckSS0503(SyntaxNodeAnalysisContext context, TypeSyntax returnType)
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
						descriptor: SS0503,
						location: returnType.GetLocation(),
						messageArgs: null
					)
				);
			}
		}

		private static bool DoesNotContainPublicKeyword(SyntaxToken token) => token.RawKind != (int)SyntaxKind.PublicKeyword;
		private static bool DoesNotContainOutKeyword(SyntaxToken token) => token.RawKind != (int)SyntaxKind.OutKeyword;
	}
}
