using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for <see langword="is"/> pattern.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class TypePatternAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.IsPatternExpression });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;

			if (
				originalNode is not IsPatternExpressionSyntax
				{
					Expression: var expression,
					Pattern: DeclarationPatternSyntax { Type: var declarationType } pattern
				} node
			)
			{
				return;
			}

			if (semanticModel.GetOperation(expression) is not { Type: var definedType })
			{
				return;
			}

			if (
				semanticModel.GetOperation(pattern) is not IDeclarationPatternOperation
				{
					MatchedType: { } matchedType
				}
			)
			{
				return;
			}

			if (!SymbolEqualityComparer.Default.Equals(definedType, matchedType))
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0601,
					location: declarationType.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
