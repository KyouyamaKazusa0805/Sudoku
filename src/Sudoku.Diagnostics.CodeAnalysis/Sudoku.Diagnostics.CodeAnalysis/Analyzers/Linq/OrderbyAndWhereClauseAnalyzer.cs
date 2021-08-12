using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

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
		if (context.Node is not QueryExpressionSyntax { Body: { Clauses: { Count: not 0 } clauses } body })
		{
			return;
		}

		for (int i = 0, iterationCount = clauses.Count - 1; i < iterationCount; i++)
		{
			if (
				(Left: clauses[i], Right: clauses[i + 1]) is not (
					Left: OrderByClauseSyntax l,
					Right: WhereClauseSyntax
				)
			)
			{
				continue;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0307,
					location: l.GetLocation(),
					properties: ImmutableDictionary.CreateRange(
						new KeyValuePair<string, string?>[] { new("Index", i.ToString()) }
					),
					messageArgs: null,
					additionalLocations: new[] { body.GetLocation() }
				)
			);
		}
	}
}
