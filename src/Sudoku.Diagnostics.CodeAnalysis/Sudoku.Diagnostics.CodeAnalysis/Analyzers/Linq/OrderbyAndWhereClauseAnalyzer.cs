using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text.Extensions;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0307")]
	public sealed partial class OrderbyAndWhereClauseAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.QueryExpression });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			if (
				context.Node is not QueryExpressionSyntax
				{
					SyntaxTree: var tree,
					Body: { Clauses: { Count: not 0 } clauses }
				}
			)
			{
				return;
			}

			for (int i = 0, count = clauses.Count; i < count - 1; i++)
			{
				if ((clauses[i], clauses[i + 1]) is not (OrderByClauseSyntax l, WhereClauseSyntax r))
				{
					continue;
				}

				Location leftLocation = l.GetLocation(), rightLocation = r.GetLocation();
				var (_, (startLocation, _)) = leftLocation;
				var (_, (_, endLocation)) = rightLocation;
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0307,
						location: Location.Create(
							syntaxTree: tree,
							textSpan: TextSpan.FromBounds(startLocation, endLocation)
						),
						messageArgs: null,
						additionalLocations: new[] { leftLocation, rightLocation }
					)
				);
			}
		}
	}
}
