namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0615", "SS0616")]
public sealed partial class NullableTypesPatternMatchingSuggestionAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(
			AnalyzeSyntaxNode,
			new[]
			{
				SyntaxKind.SimpleMemberAccessExpression,
				SyntaxKind.EqualsExpression,
				SyntaxKind.NotEqualsExpression,
				SyntaxKind.IsPatternExpression,
				SyntaxKind.LogicalNotExpression
			}
		);
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		var (semanticModel, _, originalNode) = context;

		switch (originalNode)
		{
			// !obj.HasValue
			case PrefixUnaryExpressionSyntax
			{
				RawKind: (int)SyntaxKind.LogicalNotExpression,
				Operand: MemberAccessExpressionSyntax
				{
					Expression: var expr,
					Name: { Identifier: { ValueText: "HasValue" } }
				}
			}
			when semanticModel.GetOperation(expr) is { Type: (isValueType: true, _, isNullable: true) }:
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0615,
						location: originalNode.GetLocation(),
						messageArgs: null,
						properties: ImmutableDictionary.CreateRange(
							new KeyValuePair<string, string?>[]
							{
								new("IsNull", "True"),
								new("IsHasValue", "True")
							}
						),
						additionalLocations: new[] { expr.GetLocation() }
					)
				);

				break;
			}

			// obj.HasValue
			case MemberAccessExpressionSyntax
			{
				Parent: not PrefixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.LogicalNotExpression },
				Expression: var expr,
				Name: { Identifier: { ValueText: "HasValue" } }
			}
			when semanticModel.GetOperation(expr) is { Type: (isValueType: true, _, isNullable: true) }:
			{
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0615,
						location: originalNode.GetLocation(),
						messageArgs: null,
						properties: ImmutableDictionary.CreateRange(
							new KeyValuePair<string, string?>[]
							{
								new("IsNull", "False"),
								new("IsHasValue", "True")
							}
						),
						additionalLocations: new[] { expr.GetLocation() }
					)
				);

				break;
			}

			// obj == null
			// obj != null
			case BinaryExpressionSyntax
			{
				RawKind: var kind and (
					(int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression
				),
				Left: var leftExpr,
				Right: var rightExpr
			} binaryExpression
			when semanticModel.GetOperation(binaryExpression) is IBinaryOperation { OperatorMethod: null }:
			{
				ExpressionSyntax? instanceExpr = null, nullExpr = null;
				bool? isNullableValueType = null;
				foreach (var (a, b) in new[] { (leftExpr, rightExpr), (rightExpr, leftExpr) })
				{
					if (
						semanticModel.GetOperation(a) is
						{
							Type: (isValueType: var isValueType, _, isNullable: true)
						}
						&& b is LiteralExpressionSyntax { RawKind: (int)SyntaxKind.NullLiteralExpression }
					)
					{
						instanceExpr = a;
						nullExpr = b;
						isNullableValueType = isValueType;

						break;
					}
				}
				if (
					(
						InstanceExpressionNode: instanceExpr,
						NullExpressionNode: nullExpr,
						IsNvt: isNullableValueType
					) is not (InstanceExpressionNode: not null, NullExpressionNode: not null, IsNvt: { } isNvt)
				)
				{
					return;
				}

				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: isNvt ? SS0615 : SS0616,
						location: originalNode.GetLocation(),
						messageArgs: null,
						properties: ImmutableDictionary.CreateRange(
							new KeyValuePair<string, string?>[]
							{
								new("IsNull", (kind == (int)SyntaxKind.EqualsExpression).ToString()),
								new("IsHasValue", "False")
							}
						),
						additionalLocations: new[] { instanceExpr.GetLocation() }
					)
				);

				break;
			}
		}
	}
}
