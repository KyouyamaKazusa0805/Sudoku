using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGen;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
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
					SyntaxKind.IsPatternExpression
				}
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;

			switch (originalNode)
			{
				// obj.HasValue
				case MemberAccessExpressionSyntax
				{
					Expression: var expr,
					Name: { Identifier: { ValueText: "HasValue" } }
				}
				when semanticModel.GetOperation(expr) is { Type: (isValueType: true, _, isNullable: true) }:
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SS0615,
							location: originalNode.GetLocation(),
							messageArgs: null
						)
					);

					break;
				}

				// obj == null
				// obj != null
				case BinaryExpressionSyntax
				{
					RawKind: (int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression,
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
					if ((instanceExpr, nullExpr, isNullableValueType) is not (not null, not null, { } isNvt))
					{
						return;
					}

					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: isNvt ? SS0615 : SS0616,
							location: originalNode.GetLocation(),
							messageArgs: null
						)
					);

					break;
				}
			}
		}
	}
}
