namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0211")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSymbolAction), typeof(SymbolKind), nameof(SymbolKind.NamedType))]
public sealed partial class SCA0211_NamespaceShouldMatchExtensionMethodThisParameterTypeAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SymbolAnalysisContext context)
	{
		if (context is not
			{
				Symbol: INamedTypeSymbol { IsStatic: true, DeclaringSyntaxReferences: [var syntaxRef] } symbol,
				CancellationToken: var ct
			})
		{
			return;
		}

		if (symbol.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(static m => m.IsExtensionMethod) is not
			{
				ContainingType.ContainingNamespace: var @namespace,
				Parameters: [{ Type.ContainingNamespace: var containingNamespace }]
			} method)
		{
			return;
		}

		if (SymbolEqualityComparer.Default.Equals(@namespace, containingNamespace))
		{
			return;
		}

		if (syntaxRef.GetSyntax(ct) is not BaseTypeDeclarationSyntax { Identifier: var identifier })
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0211, identifier.GetLocation()));
	}
}
