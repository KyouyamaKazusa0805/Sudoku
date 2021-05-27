using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for the <see langword="null"/>-conditional
	/// operator <c>?</c> and <see langword="null"/>-forgiving operator <c>!</c> usages.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class NullConditionalAndSuppressionAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				AnalyzeSyntaxNode,
				new[] { SyntaxKind.ConditionalAccessExpression, SyntaxKind.SuppressNullableWarningExpression }
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;

			CheckSS0703(context, semanticModel, originalNode);
			CheckSS0704(context, semanticModel, originalNode);
		}

		private static void CheckSS0703(
			SyntaxNodeAnalysisContext context, SemanticModel semanticModel, SyntaxNode originalNode)
		{
			if (
				originalNode is not ConditionalAccessExpressionSyntax
				{
					Parent: not (
						ConditionalAccessExpressionSyntax or MemberAccessExpressionSyntax
						{
							RawKind: (int)SyntaxKind.SimpleAssignmentExpression
						}
					),
					Expression: var expr,
					OperatorToken: var token
				}
			)
			{
				return;
			}

			if (semanticModel.GetOperation(expr) is not { Type: { } type })
			{
				return;
			}

			if (type.IsNullableType())
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0703,
					location: token.GetLocation(),
					messageArgs: null
				)
			);
		}

		private static void CheckSS0704(
			SyntaxNodeAnalysisContext context, SemanticModel semanticModel, SyntaxNode originalNode)
		{
			if (
				originalNode is not PostfixUnaryExpressionSyntax
				{
					Parent: not (
						ConditionalAccessExpressionSyntax
						or InvocationExpressionSyntax
						or MemberAccessExpressionSyntax { RawKind: (int)SyntaxKind.SimpleAssignmentExpression }
					),
					Operand: var expr,
					OperatorToken: var token
				}
			)
			{
				return;
			}

			if (semanticModel.GetOperation(expr) is not { Type: { } type })
			{
				return;
			}

			if (type.IsNullableType())
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SS0704,
					location: token.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
