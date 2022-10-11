namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0001", "SCA0102")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.Parameter))]
public sealed partial class SCA0102_LargeStructureShouldPassByRefAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SyntaxNodeAnalysisContext context)
	{
		if (context is not
			{
				CancellationToken: var ct,
				Compilation: var compilation,
				Node: ParameterSyntax node,
				SemanticModel: var semanticModel
			})
		{
			return;
		}

		var parameterSymbol = semanticModel.GetDeclaredSymbol(node, ct);
		if (parameterSymbol is not IParameterSymbol { Type: var parameterType, RefKind: RefKind.None })
		{
			return;
		}

		var nodeLocation = node.GetLocation();
		var attributeType = compilation.GetTypeByMetadataName(SpecialFullTypeNames.IsLargeStructAttribute);
		if (attributeType is null)
		{
			context.ReportDiagnostic(Diagnostic.Create(SCA0001, nodeLocation, messageArgs: SpecialFullTypeNames.IsLargeStructAttribute));
			return;
		}

		var attributesData = parameterType.GetAttributes();
		if (attributesData.Length == 0)
		{
			return;
		}

		var a = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType));
		if (a is null)
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0102, nodeLocation));
	}
}
