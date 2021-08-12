using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0309F")]
public sealed partial class RedundantAscendingKeywordAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.QueryExpression });
	}


	/// <inheritdoc/>
	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		if (context.Node is not QueryExpressionSyntax { Body: { Clauses: { Count: not 0 } clauses } })
		{
			return;
		}

		foreach (var clause in clauses)
		{
			if (clause is not OrderByClauseSyntax { Orderings: { Count: not 0 } orderings })
			{
				return;
			}

			foreach (var ordering in orderings)
			{
				if (
					ordering.AscendingOrDescendingKeyword is not
					{
						RawKind: (int)SyntaxKind.AscendingKeyword
					} possibleAscendingkeyword
				)
				{
					return;
				}

				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0309,
						location: possibleAscendingkeyword.GetLocation(),
						messageArgs: null,
						additionalLocations: new[] { ordering.GetLocation() }
					)
				);
			}
		}
	}
}
