using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0311F")]
	public sealed partial class ContinuousOrderbyAnalyzer : DiagnosticAnalyzer
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

				for (int i = 0; i < count - 1; i++)
				{
					for (int j = i + 1; j < count; j++)
					{
						OrderingSyntax former = orderings[i], latter = orderings[j];
						if (
							(former, latter) is not (
#pragma warning disable IDE0055
								{
									Expression: IdentifierNameSyntax
									{
										Identifier: { ValueText: var formerIdentifier }
									}
								},
								{
									Expression: IdentifierNameSyntax
									{
										Identifier: { ValueText: var latterIdentifier }
									}
								}
#pragma warning restore IDE0055
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
