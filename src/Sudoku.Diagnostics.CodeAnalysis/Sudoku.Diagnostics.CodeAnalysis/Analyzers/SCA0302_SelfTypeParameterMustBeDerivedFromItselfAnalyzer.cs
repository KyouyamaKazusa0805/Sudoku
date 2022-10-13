namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0302")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.TypeParameter))]
public sealed partial class SCA0302_SelfTypeParameterMustBeDerivedFromItselfAnalyzer : DiagnosticAnalyzer
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

		var selfAttribute = compliation.GetTypeByMetadataName(SpecialFullTypeNames.SelfAttribute);
		if (selfAttribute is null)
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

		context.ReportDiagnostic(Diagnostic.Create(SCA0302, node.GetLocation()));
	}
}
