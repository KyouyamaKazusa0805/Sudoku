namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0114")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.TypeParameter))]
public sealed partial class SCA0114_SelfTypeParameterMustBeDerivedFromItselfAnalyzer : DiagnosticAnalyzer
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

		if (constraintTypes.Any(a => SymbolEqualityComparer.Default.Equals(a, declaringType)))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0114, node.GetLocation()));
	}
}
