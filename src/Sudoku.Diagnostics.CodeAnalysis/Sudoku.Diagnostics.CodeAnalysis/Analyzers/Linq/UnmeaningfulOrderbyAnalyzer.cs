using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0311F")]
	public sealed partial class UnmeaningfulOrderbyAnalyzer : DiagnosticAnalyzer
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
			if (context.Node is not QueryExpressionSyntax { Body: { Clauses: { Count: not 0 } clauses } } node)
			{
				return;
			}

			foreach (var clause in clauses)
			{
				if (
					clause is not OrderByClauseSyntax
					{
						Orderings: { Count: var count and not 0 } orderings
					} queryClause
				)
				{
					continue;
				}

				for (int i = 0, iterationCount = count - 1; i < iterationCount; i++)
				{
					for (int j = i + 1; j < count; j++)
					{
						OrderingSyntax former = orderings[i], latter = orderings[j];
						if (
							(Former: former, Latter: latter) is not (
								Former:
								{
									Expression: IdentifierNameSyntax
									{
										Identifier: { ValueText: var formerIdentifier }
									}
								},
								Latter:
								{
									Expression: IdentifierNameSyntax
									{
										Identifier: { ValueText: var latterIdentifier }
									}
								}
							)
						)
						{
							continue;
						}

						if (formerIdentifier != latterIdentifier)
						{
							continue;
						}

						context.ReportDiagnostic(
							Diagnostic.Create(
								descriptor: SS0311,
								location: latter.GetLocation(),
								properties: ImmutableDictionary.CreateRange(
									new KeyValuePair<string, string?>[] { new("Index", j.ToString()) }
								),
								messageArgs: null,
								additionalLocations: new[] { queryClause.GetLocation() }
							)
						);
					}
				}
			}
		}
	}
}
