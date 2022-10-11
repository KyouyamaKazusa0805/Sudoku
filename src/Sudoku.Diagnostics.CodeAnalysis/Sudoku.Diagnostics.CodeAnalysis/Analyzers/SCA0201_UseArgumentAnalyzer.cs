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
			case IIsPatternOperation
			{
				Pattern: IConstantPatternOperation
				{
					Syntax: ConstantPatternSyntax
					{
						Expression: LiteralExpressionSyntax { RawKind: (int)SyntaxKind.NullLiteralExpression }
					}
				}
			}:
			case IBinaryOperation { OperatorKind: BinaryOperatorKind.Equals, LeftOperand: var l, RightOperand: var r }
			when verifyOperand(l) ^ verifyOperand(r):
			{
				break;
			}
			default:
			{
				return;
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool verifyOperand(IOperation operation)
				=> operation is IConversionOperation { Operand: ILiteralOperation { ConstantValue: { HasValue: true, Value: null } } };
		}

		switch (whenTrueOperation)
		{
			case var _ when throwOperationVerifier(whenTrueOperation, out var exceptionType):
			{
				check(exceptionType);
				break;
			}
			case IBlockOperation { ChildOperations: { Count: 1 } c } when throwOperationVerifier(c.First(), out var exceptionType):
			{
				check(exceptionType);
				break;
			}
			default:
			{
				return;
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool throwOperationVerifier(IOperation operation, out INamedTypeSymbol? exceptionType)
			{
				if (operation is IThrowOperation
					{
						Exception: IConversionOperation
						{
							Operand: IObjectCreationOperation
							{
								Constructor:
								{
									Parameters: [{ Type.SpecialType: SpecialType.System_String }],
									ContainingType: var e
								}
							}
						}
					})
				{
					exceptionType = e;
					return true;
				}
				else
				{
					exceptionType = null;
					return false;
				}
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
	}
}
