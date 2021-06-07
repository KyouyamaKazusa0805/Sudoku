using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0610")]
	public sealed partial class DiscardedPositionalPatternAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.PositionalPatternClause });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			if (
				context.Node is not PositionalPatternClauseSyntax
				{
					Subpatterns: { Count: >= 2 } subpatterns
				} node
			)
			{
				return;
			}

			/*slice-pattern*/
			if (subpatterns.Any(static subpattern => subpattern.Pattern is not DiscardPatternSyntax))
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0610,
					location: node.GetLocation(),
					messageArgs: new[] { node.ToString(), "is { }", "is not null" }
				)
			);
		}
	}
}
