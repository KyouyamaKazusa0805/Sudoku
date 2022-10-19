namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0001", "SCA0206")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.InvocationExpression))]
public sealed partial class SCA0206_UseExplicitCastAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SyntaxNodeAnalysisContext context)
	{
		if (context is not
			{
				Node: InvocationExpressionSyntax node,
				CancellationToken: var ct,
				Compilation: var compilation,
				SemanticModel: var semanticModel
			})
		{
			return;
		}

		if (semanticModel.GetOperation(node, ct) is not IInvocationOperation
			{
				Instance: null,
				TargetMethod:
				{
					Name: "Parse",
					IsStatic: true,
					Parameters: [{ Type.SpecialType: SpecialType.System_String }],
					TypeParameters: [],
					ContainingType: var type
				},
				Arguments: [{ Value.ConstantValue: { HasValue: true, Value: string } }]
			})
		{
			return;
		}

		var location = node.GetLocation();
		var gridType = compilation.GetTypeByMetadataName(SpecialFullTypeNames.Grid);
		if (gridType is null)
		{
			context.ReportDiagnostic(Diagnostic.Create(SCA0001, location, messageArgs: new[] { SpecialFullTypeNames.Grid }));
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(gridType, type))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0206, location));
	}
}
