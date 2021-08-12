using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0303")]
public sealed partial class AvailableSimplificationAnyMethodAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.InvocationExpression });
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		if (
			context.Node is not InvocationExpressionSyntax
			{
				Expression: MemberAccessExpressionSyntax
				{
					RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
					Expression: ParenthesizedExpressionSyntax
					{
						Expression: QueryExpressionSyntax
						{
							FromClause:
							{
								Type: var typeToCast,
								Expression: var expressionToIterate,
								Identifier: { ValueText: var identifier } identifierToken
							},
							Body:
							{
								Clauses: { Count: 1 } clauses,
								SelectOrGroup: SelectClauseSyntax
								{
									Expression: IdentifierNameSyntax
									{
										Identifier: { ValueText: var identifierInSelectClause }
									}
								}
							}
						}
					},
					Name: { Identifier: { ValueText: "Any" } anyToken }
				},
				ArgumentList: { Arguments: { Count: 0 } }
			} node
		)
		{
			return;
		}

		if (clauses[0] is not WhereClauseSyntax { Condition: var conditionExpr })
		{
			return;
		}

		if (identifier != identifierInSelectClause)
		{
			return;
		}

		context.ReportDiagnostic(
			Diagnostic.Create(
				descriptor: SS0303,
				location: anyToken.GetLocation(),
				properties: ImmutableDictionary.CreateRange(
					new KeyValuePair<string, string?>[] { new("IdentifierName", identifier) }
				),
				messageArgs: null,
				additionalLocations: typeToCast is null
					? new[]
					{
						node.GetLocation(),
						identifierToken.GetLocation(),
						expressionToIterate.GetLocation(),
						conditionExpr.GetLocation()
					}
					: new[]
					{
						node.GetLocation(),
						identifierToken.GetLocation(),
						expressionToIterate.GetLocation(),
						conditionExpr.GetLocation(),
						typeToCast.GetLocation()
					}
			)
		);
	}
}
