using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for the constant pattern matching
	/// of <see cref="Nullable{T}"/>.
	/// </summary>
	/// <seealso cref="Nullable{T}"/>
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
					Left: var leftExpr,
					OperatorToken: var token,
					Right: var rightExpr
				} binaryExpression)
			{
				return;
			}

			if (semanticModel.GetOperation(leftExpr) is not { Type: { IsValueType: true } leftExprType })
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
			if (leftType[leftType.Length - 1] != '?' || leftType.Substring(0, leftType.Length - 1) != rightType)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0605,
					location: token.GetLocation(),
					messageArgs: new[]
					{
						leftExpr.ToString(),
						binaryExpression.RawKind == (int)SyntaxKind.EqualsExpression ? "is" : "is not",
						rightExpr.ToString()
					}
				)
			);
		}
	}
}
