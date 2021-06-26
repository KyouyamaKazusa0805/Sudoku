using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0308")]
	public sealed partial class AvailableCastAnalyzer : DiagnosticAnalyzer
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
					FromClause: { Type: null, Identifier: { ValueText: var identifier } } fromClause,
					Body:
					{
						Clauses: { Count: 0 },
						SelectOrGroup: SelectClauseSyntax
						{
							Expression: CastExpressionSyntax
							{
								Type: var typeToCast,
								Expression: IdentifierNameSyntax
								{
									Identifier: { ValueText: var identifierToCheck }
								} innerExpression
							} castExpression
						} selectClause
					}
				}
			)
			{
				return;
			}

			if (identifier != identifierToCheck)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0308,
					location: castExpression.GetLocation(),
					messageArgs: null,
					additionalLocations: new[]
					{
						fromClause.GetLocation(),
						typeToCast.GetLocation(),
						selectClause.GetLocation(),
						innerExpression.GetLocation()
					}
				)
			);
		}
	}
}
