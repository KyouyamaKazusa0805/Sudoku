using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0605")]
public sealed partial class NullableValueTypeConstantPatternAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(
			AnalyzeSyntaxNode,
			new[] { SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression }
		);
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		var (semanticModel, _, originalNode) = context;

		if (
			originalNode is not BinaryExpressionSyntax
			{
				RawKind: var kind,
				Left: var leftExpr,
				Right: var rightExpr
			}
		)
		{
			return;
		}

		if (
			semanticModel.GetOperation(leftExpr) is not
			{
				Type: (isValueType: true, _, isNullable: true) leftExprType
			}
		)
		{
			return;
		}

		if (
			semanticModel.GetOperation(rightExpr) is not
			{
				Type: { } rightExprType,
				ConstantValue: { HasValue: true }
			}
		)
		{
			return;
		}

		string leftType = leftExprType.ToDisplayString();
		string rightType = rightExprType.ToDisplayString();
		if (leftType.Substring(0, leftType.Length - 1) != rightType)
		{
			return;
		}

		context.ReportDiagnostic(
			Diagnostic.Create(
				descriptor: SS0605,
				location: originalNode.GetLocation(),
				messageArgs: new[]
				{
					leftExpr.ToString(),
					kind == (int)SyntaxKind.EqualsExpression ? "is" : "is not",
					rightExpr.ToString()
				},
				properties: ImmutableDictionary.CreateRange(
					new KeyValuePair<string, string?>[]
					{
						new("OperatorToken", kind == (int)SyntaxKind.EqualsExpression ? "==" : "!=")
					}
				),
				additionalLocations: new[] { leftExpr.GetLocation(), rightExpr.GetLocation() }
			)
		);
	}
}
