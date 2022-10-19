namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0115")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.TypeParameter))]
public sealed partial class SCA0115_SelfTypeParameterShouldNameTSelfAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SyntaxNodeAnalysisContext context)
	{
		if (context is not
			{
				CancellationToken: var ct,
				Node: TypeParameterSyntax node,
				SemanticModel: var semanticModel,
				Compilation: var compliation
			})
		{
			return;
		}

		if (semanticModel.GetDeclaredSymbol(node, ct) is not ITypeParameterSymbol
			{
				TypeParameterKind: TypeParameterKind.Type,
				Name: var name,
				ConstraintTypes: var constraintTypes,
				DeclaringType: { } declaringType,
				Locations: [var location]
			} symbol)
		{
			return;
		}

		if (compliation.GetTypeByMetadataName(SpecialFullTypeNames.SelfAttribute) is not { } selfAttribute)
		{
			return;
		}

		if (symbol.GetAttributes().All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, selfAttribute)))
		{
			return;
		}

		if (name == "TSelf")
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0115, node.GetLocation()));
	}
}
