namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0215")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.NotEqualsExpression))]
public sealed partial class SCA0215_ExpressionSimplifyAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SyntaxNodeAnalysisContext context)
	{
		if (context is not
			{
				Node: ExpressionSyntax { RawKind: (int)SyntaxKind.NotEqualsExpression } node,
				SemanticModel: var semanticModel,
				Compilation: var compilation,
				CancellationToken: var ct
			})
		{
			return;
		}

		if (semanticModel.GetOperation(node, ct) is not IBinaryOperation
			{
				LeftOperand: var lOperand,
				RightOperand: var rOperand,
				Parent: not IBinaryOperation { OperatorKind: BinaryOperatorKind.ConditionalAnd or BinaryOperatorKind.ConditionalOr }
			} operation)
		{
			return;
		}

		var location = node.GetLocation();
		var cellMapType = compilation.GetTypeByMetadataName(SpecialFullTypeNames.CellMap);
		if (cellMapType is null)
		{
			return;
		}

		if (!(p(lOperand, rOperand) ^ p(rOperand, lOperand)))
		{
			return;
		}

		var propertyName = (lOperand, rOperand) switch
		{
			(IPropertyReferenceOperation { Instance.Syntax: var referencingNode }, _) => r(referencingNode),
			(_, IPropertyReferenceOperation { Instance.Syntax: var referencingNode }) => r(referencingNode)
		};

		context.ReportDiagnostic(Diagnostic.Create(SCA0215, location, messageArgs: new[] { propertyName }));


		bool p(IOperation l, IOperation r)
			=> (l, r) is (
				IPropertyReferenceOperation { Property: { Name: "Count", ContainingType: var propertyContainingType, IsIndexer: false } },
				ILiteralOperation { ConstantValue: { HasValue: true, Value: 0 } }
			) && SymbolEqualityComparer.Default.Equals(propertyContainingType, cellMapType);

		string r(SyntaxNode referencingNode)
			=> referencingNode switch { IdentifierNameSyntax => referencingNode.ToString(), _ => $"({referencingNode})" };
	}
}
