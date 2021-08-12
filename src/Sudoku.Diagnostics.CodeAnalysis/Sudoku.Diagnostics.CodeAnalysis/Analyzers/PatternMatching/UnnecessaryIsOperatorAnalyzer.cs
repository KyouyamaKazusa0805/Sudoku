using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGenerating;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0617F")]
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
		var (semanticModel, _, originalNode, _, cancellationToken) = context;

		if (
			originalNode is not IsPatternExpressionSyntax
			{
				Expression: var expr,
				Pattern: RelationalPatternSyntax
				{
					OperatorToken: { RawKind: var kind },
					Expression: var constantExpr
				}
			}
		)
		{
			return;
		}

		if (!semanticModel.TypeEquals(expr, constantExpr, cancellationToken: cancellationToken))
		{
			return;
		}

		context.ReportDiagnostic(
			Diagnostic.Create(
				descriptor: SS0617,
				location: originalNode.GetLocation(),
				messageArgs: null,
				properties: ImmutableDictionary.CreateRange(
					new KeyValuePair<string, string?>[] { new("OperatorToken", kind.ToString()) }
				),
				additionalLocations: new[] { expr.GetLocation(), constantExpr.GetLocation() }
			)
		);
	}
}
