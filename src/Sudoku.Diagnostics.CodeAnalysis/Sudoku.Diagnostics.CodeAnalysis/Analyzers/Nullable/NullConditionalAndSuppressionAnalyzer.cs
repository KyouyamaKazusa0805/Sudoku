using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGen;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SS0703F", "SS0704F")]
	public sealed partial class NullConditionalAndSuppressionAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				static context =>
				{
					CheckSS0703(context);
					CheckSS0704(context);
				},
				new[] { SyntaxKind.ConditionalAccessExpression, SyntaxKind.SuppressNullableWarningExpression }
			);
		}


		private static void CheckSS0703(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;

			if (
				originalNode is not ConditionalAccessExpressionSyntax
				{
					Expression: var expr,
					OperatorToken: var token
				}
			)
			{
				return;
			}

			if (semanticModel.GetOperation(expr) is not { Type: (_, _, isNullable: false) })
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0703,
					location: token.GetLocation(),
					messageArgs: null,
					additionalLocations: new[] { originalNode.GetLocation() }
				)
			);
		}

		private static void CheckSS0704(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;

			if (
				originalNode is not PostfixUnaryExpressionSyntax
				{
					Operand: var expr,
					OperatorToken: var token
				}
			)
			{
				return;
			}

			if (semanticModel.GetOperation(expr) is not { Type: (_, _, isNullable: false) })
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0704,
					location: token.GetLocation(),
					messageArgs: null,
					additionalLocations: new[] { originalNode.GetLocation() }
				)
			);
		}
	}
}
