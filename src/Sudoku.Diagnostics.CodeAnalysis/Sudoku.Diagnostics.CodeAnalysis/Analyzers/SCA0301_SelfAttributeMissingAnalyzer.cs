namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0301")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.TypeParameter))]
public sealed partial class SCA0301_SelfAttributeMissingAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SyntaxNodeAnalysisContext context)
	{
		if (context is not
			{
				Node: TypeParameterSyntax node,
				SemanticModel: var semanticModel,
				Compilation: var compilation,
				CancellationToken: var ct
			})
		{
			return;
		}

		if (semanticModel.GetDeclaredSymbol(node, ct) is not ITypeParameterSymbol
			{
				TypeParameterKind: TypeParameterKind.Type,
				ConstraintTypes: var constraintTypes and not [],
				HasUnmanagedTypeConstraint: false,
				HasValueTypeConstraint: false,
				DeclaringType: var declaringType,
				Locations: [var location]
			} symbol)
		{
			return;
		}

		if (constraintTypes.All(t => !SymbolEqualityComparer.Default.Equals(t, declaringType)))
		{
			return;
		}

		var selfAttributeType = compilation.GetTypeByMetadataName(SpecialFullTypeNames.SelfAttribute);
		if (selfAttributeType is null)
		{
			//context.ReportDiagnostic(Diagnostic.Create(SCA0001, location));
			return;
		}

		if (symbol.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, selfAttributeType)))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0301, location));
	}
}
