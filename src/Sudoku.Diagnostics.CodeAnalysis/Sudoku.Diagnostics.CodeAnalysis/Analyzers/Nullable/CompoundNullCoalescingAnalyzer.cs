namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0701", "SS0705")]
public sealed partial class CompoundNullCoalescingAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(
			static context =>
			{
				CheckSS0701(context);
				CheckSS0705(context);
			},
			new[] { SyntaxKind.IfStatement, SyntaxKind.CoalesceAssignmentExpression }
		);
	}


	private static void CheckSS0701(SyntaxNodeAnalysisContext context)
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
				innerCheck(semanticModel, statement, leftExpr);

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
				innerCheck(semanticModel, statement, leftExpr);

				break;
			}
		}

		void innerCheck(SemanticModel semanticModel, StatementSyntax statement, ExpressionSyntax leftExpr)
		{
			if (
				semanticModel.GetOperation(leftExpr) is not (
					(FRef or LRef or PRef) and { Type: (_, _, isNullable: true) leftExprType } refOperation
				)
			)
			{
				return;
			}

			if (
				statement switch
				{
					BlockSyntax { Statements: { Count: 1 } statements } => statements[0],
					ExpressionStatementSyntax => statement,
					_ => null
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

			switch ((OperandReference: operandReferenceOperation, Reference: refOperation))
			{
				case (OperandReference: FRef { Field: var field }, Reference: FRef { Field: var fieldOperand })
				when field.ToDisplayString() == fieldOperand.ToDisplayString():
				case (
					OperandReference: PRef { Property: var property },
					Reference: PRef { Property: var propertyOperand }
				)
				when property.ToDisplayString() == propertyOperand.ToDisplayString():
				case (OperandReference: LRef { Local: var local }, Reference: LRef { Local: var localOperand })
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
					messageArgs: new[] { leftOperand.ToString(), rightOperand.ToString() },
					additionalLocations: new[] { leftOperand.GetLocation(), rightOperand.GetLocation() }
				)
			);
		}
	}

	private static void CheckSS0705(SyntaxNodeAnalysisContext context)
	{
		var (semanticModel, _, originalNode) = context;

		if (
			originalNode is not AssignmentExpressionSyntax
			{
				RawKind: (int)SyntaxKind.CoalesceAssignmentExpression,
				Left: var leftExpr
			}
		)
		{
			return;
		}

		if (
			semanticModel.GetOperation(leftExpr) is not (
				(FRef or LRef) and { Type: (_, var isReferenceType, var isNullable) } referenceOperation
			)
		)
		{
			return;
		}

		if (!isReferenceType || isNullable)
		{
			return;
		}

		context.ReportDiagnostic(
			Diagnostic.Create(
				descriptor: SS0705,
				location: originalNode.GetLocation(),
				messageArgs: new[] { leftExpr.ToString() }
			)
		);
	}
}
