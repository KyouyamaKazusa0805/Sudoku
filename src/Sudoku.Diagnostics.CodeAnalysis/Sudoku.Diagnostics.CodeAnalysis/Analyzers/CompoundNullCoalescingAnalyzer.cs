using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.Diagnostics.CodeAnalysis.Extensions;
using FRef = Microsoft.CodeAnalysis.Operations.IFieldReferenceOperation;
using LRef = Microsoft.CodeAnalysis.Operations.ILocalReferenceOperation;
using PRef = Microsoft.CodeAnalysis.Operations.IPropertyReferenceOperation;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for available simplification for <c>operator ??=</c>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class CompoundNullCoalescingAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.IfStatement });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;

			if (
				originalNode is not IfStatementSyntax
				{
					Condition: var condition and (BinaryExpressionSyntax or IsPatternExpressionSyntax),
					Statement: var statement and (BlockSyntax or ExpressionStatementSyntax),
					Else: null
				}
			)
			{
				return;
			}

			switch (condition)
			{
				case BinaryExpressionSyntax
				{
					RawKind: (int)SyntaxKind.EqualsExpression,
					Left: var leftExpr,
					Right: LiteralExpressionSyntax { RawKind: (int)SyntaxKind.NullLiteralExpression }
				}:
				{
					innerCheck_Phase1(semanticModel, statement, leftExpr);

					break;
				}
				case IsPatternExpressionSyntax
				{
					Expression: var leftExpr,
					Pattern: ConstantPatternSyntax
					{
						Expression: { RawKind: (int)SyntaxKind.NullLiteralExpression }
					}
				}:
				{
					innerCheck_Phase1(semanticModel, statement, leftExpr);

					break;
				}
			}

			void innerCheck_Phase1(
				SemanticModel semanticModel, StatementSyntax statement, ExpressionSyntax leftExpr)
			{
				if (
					semanticModel.GetOperation(leftExpr) is not (
						var referenceOperation and (FRef or LRef or PRef) and { Type: { } leftExprType }
					)
				)
				{
					return;
				}

				if (!leftExprType.IsNullableType())
				{
					return;
				}

				if (
					statement switch
					{
						/*slice-pattern*/
						BlockSyntax { Statements: { Count: 1 } statements } => statements[0],
						ExpressionStatementSyntax => statement
					} is not ExpressionStatementSyntax
					{
						Expression: AssignmentExpressionSyntax
						{
							RawKind: (int)SyntaxKind.SimpleAssignmentExpression,
							Left: var leftOperand,
							Right: var rightOperand
						}
					}
				)
				{
					return;
				}

				if (
					semanticModel.GetOperation(leftOperand) is not (
						var operandReferenceOperation and (FRef or LRef or PRef)
					)
				)
				{
					return;
				}

				switch ((operandReferenceOperation, referenceOperation))
				{
					case (FRef { Field: var field }, FRef { Field: var fieldOperand })
					when field.ToDisplayString() == fieldOperand.ToDisplayString():
					case (PRef { Property: var property }, PRef { Property: var propertyOperand })
					when property.ToDisplayString() == propertyOperand.ToDisplayString():
					case (LRef { Local: var local }, LRef { Local: var localOperand })
					when local.ToDisplayString() == localOperand.ToDisplayString():
					{
						break;
					}
					default:
					{
						return;
					}
				}

				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: SS0701,
						location: originalNode.GetLocation(),
						messageArgs: new[] { leftOperand.ToString(), rightOperand.ToString() }
					)
				);
			}
		}
	}
}
