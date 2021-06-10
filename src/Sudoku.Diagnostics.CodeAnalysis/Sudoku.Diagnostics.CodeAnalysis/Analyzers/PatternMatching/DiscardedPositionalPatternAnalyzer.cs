using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGen;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

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
			var (semanticModel, _, originalNode) = context;
			if (
				originalNode is not PositionalPatternClauseSyntax
				{
					Parent: RecursivePatternSyntax
					{
						Parent: IsPatternExpressionSyntax { Expression: var expr } node,
						Designation: var variable
					},
					Subpatterns: { Count: >= 2 } subpatterns,
				}
			)
			{
				return;
			}

			/*slice-pattern*/
			if (subpatterns.Any(static subpattern => subpattern.Pattern is not DiscardPatternSyntax))
			{
				return;
			}

			if (semanticModel.GetOperation(expr) is not { Type: (_, _, isNullable: var isNullable) })
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0610,
					location: originalNode.GetLocation(),
					messageArgs: new[] { originalNode.ToString(), "is { }", "is not null" },
					properties: ImmutableDictionary.CreateRange(
						new KeyValuePair<string, string?>[] { new("IsNullable", isNullable.ToString()) }
					),
					additionalLocations: variable is not null
						? new[] { expr.GetLocation(), node.GetLocation(), variable.GetLocation() }
						: new[] { expr.GetLocation(), node.GetLocation() }
				)
			);
		}
	}
}
