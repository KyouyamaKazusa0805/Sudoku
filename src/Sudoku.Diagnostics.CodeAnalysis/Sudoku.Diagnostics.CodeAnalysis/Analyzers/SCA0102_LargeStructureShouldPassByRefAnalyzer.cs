namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0102")]
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

		if (semanticModel.GetDeclaredSymbol(node, ct) is not IParameterSymbol
			{
				ContainingSymbol: IMethodSymbol { ExplicitInterfaceImplementations: [] },
				Type: var parameterType,
				RefKind: RefKind.None
			} parameterSymbol)
		{
			return;
		}

		var nodeLocation = node.GetLocation();
		var attributeType = compilation.GetTypeByMetadataName(SpecialFullTypeNames.IsLargeStructAttribute);
		if (attributeType is null)
		{
			return;
		}

		var a = parameterType.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType));
		if (a is null)
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0102, nodeLocation));
	}
}
