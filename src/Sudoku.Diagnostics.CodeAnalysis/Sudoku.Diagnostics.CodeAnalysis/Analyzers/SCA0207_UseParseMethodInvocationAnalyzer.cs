namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0001", "SCA0207")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.CastExpression))]
public sealed partial class SCA0207_UseParseMethodInvocationAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SyntaxNodeAnalysisContext context)
	{
		if (context is not
			{
				Node: CastExpressionSyntax { Expression: var expression } node,
				CancellationToken: var ct,
				Compilation: var compilation,
				SemanticModel: var semanticModel
			})
		{
			return;
		}

		if (semanticModel.GetOperation(node, ct) is not IConversionOperation
			{
				Operand.ConstantValue.HasValue: false,
				OperatorMethod:
				{
					Name: "op_Explicit",
					Parameters: [{ Type.SpecialType: SpecialType.System_String, NullableAnnotation: NullableAnnotation.Annotated }],
					ReturnType: var type
				}
			})
		{
			return;
		}

		var location = expression.GetLocation();
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

		context.ReportDiagnostic(Diagnostic.Create(SCA0207, location));
	}
}
