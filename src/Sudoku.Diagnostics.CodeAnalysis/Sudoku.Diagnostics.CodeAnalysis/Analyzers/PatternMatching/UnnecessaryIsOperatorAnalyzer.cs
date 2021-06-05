using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGen;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for the unncessary <c>operator <see langword="is"/></c>.
	/// </summary>
	[CodeAnalyzer("SS0617")]
	public sealed partial class UnnecessaryIsOperatorAnalyzer : DiagnosticAnalyzer
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
					Expression: var expr,
					IsKeyword: var isKeywordToken,
					Pattern: RelationalPatternSyntax { Expression: var constantExpr }
				}
			)
			{
				return;
			}

			if (!semanticModel.TypeEquals(expr, constantExpr))
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0617,
					location: isKeywordToken.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
