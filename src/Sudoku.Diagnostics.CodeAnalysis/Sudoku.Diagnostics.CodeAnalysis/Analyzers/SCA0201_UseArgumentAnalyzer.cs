namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0201")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.IfStatement))]
public sealed partial class SCA0201_UseArgumentAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SyntaxNodeAnalysisContext context)
	{
		if (context is not
			{
				CancellationToken: var ct,
				Compilation: var compilation,
				Node: IfStatementSyntax node,
				SemanticModel: var semanticModel
			})
		{
			return;
		}

		ReportCase_ArgumentNullException(semanticModel, node, ct, compilation, context);
	}

	private static void ReportCase_ArgumentNullException(
		SemanticModel semanticModel, IfStatementSyntax node, CancellationToken ct,
		Compilation compilation, SyntaxNodeAnalysisContext context)
	{
		// .--------------------------------------------------.           .-------------------------------------------------.
		// | if (variable is null)                            |           |                                                 |
		// | {                                                |           |                                                 |
		// |    throw new ArgumentNullException(nameof(arg)); |   ====>   | ArgumentNullException.ThrowIfNull(nameof(arg)); |
		// | }                                                |           |                                                 |
		// '--------------------------------------------------'           '-------------------------------------------------'
		if (semanticModel.GetOperation(node, ct) is not IConditionalOperation
			{
				// To avoid the case that user has already defined 'operator true' and 'operator false'.
				Condition: { Type.SpecialType: SpecialType.System_Boolean } condition,
				WhenTrue: var whenTrueOperation,
				WhenFalse: null,
				IsRef: false
			} operation)
		{
			return;
		}

		switch (condition)
		{
			case IIsPatternOperation { Pattern: IConstantPatternOperation { Syntax: var syntax } } when isNullLiteral(syntax):
			case IBinaryOperation
			{
				OperatorKind: BinaryOperatorKind.Equals,
				LeftOperand: IConversionOperation { Operand: ILiteralOperation { ConstantValue: { HasValue: true, Value: null } } },
				RightOperand: not IConversionOperation { Operand: ILiteralOperation { ConstantValue: { HasValue: true, Value: null } } }
			}:
			case IBinaryOperation
			{
				OperatorKind: BinaryOperatorKind.Equals,
				LeftOperand: not IConversionOperation { Operand: ILiteralOperation { ConstantValue: { HasValue: true, Value: null } } },
				RightOperand: IConversionOperation { Operand: ILiteralOperation { ConstantValue: { HasValue: true, Value: null } } }
			}:
			{
				break;
			}
			default:
			{
				return;
			}
		}

		switch (whenTrueOperation)
		{
			case IThrowOperation
			{
				Exception: IConversionOperation
				{
					Operand: IObjectCreationOperation
					{
						Constructor:
						{
							Parameters: [{ Type.SpecialType: SpecialType.System_String } firstParameterSymbol],
							ContainingType: var exceptionType
						}
					}
				}
			}:
			{
				check(exceptionType);
				break;
			}
			case IBlockOperation { ChildOperations: { Count: 1 } c }
			when c.First() is IThrowOperation
			{
				Exception: IConversionOperation
				{
					Operand: IObjectCreationOperation
					{
						Constructor:
						{
							Parameters: [{ Type.SpecialType: SpecialType.System_String } firstParameterSymbol],
							ContainingType: var exceptionType
						}
					}
				}
			}:
			{
				check(exceptionType);
				break;
			}
			default:
			{
				return;
			}
		}

		void check(ITypeSymbol? exceptionType)
		{
			var argumentNullExceptionSymbol = compilation.GetTypeByMetadataName(typeof(ArgumentNullException).FullName)!;
			if (!SymbolEqualityComparer.Default.Equals(exceptionType, argumentNullExceptionSymbol))
			{
				return;
			}

			context.ReportDiagnostic(Diagnostic.Create(SCA0201, node.GetLocation()));
		}

		static bool isNullLiteral(SyntaxNode node)
			=> node is ConstantPatternSyntax { Expression: LiteralExpressionSyntax { RawKind: (int)SyntaxKind.NullLiteralExpression } };
	}
}
