namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0112")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSymbolAction), typeof(SymbolKind), new[] { nameof(SymbolKind.Field), nameof(SymbolKind.Method) })]
public sealed partial class SCA0112_NoNeedFileAccessOnlyAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SymbolAnalysisContext context)
	{
		if (context is not
			{
				Symbol:
				{
					ContainingType.IsFileLocal: true,
					DeclaringSyntaxReferences: [var syntaxRef]
				} symbol and (IFieldSymbol or IMethodSymbol { Name: ".ctor" }),
				CancellationToken: var ct,
				Compilation: var compilation
			})
		{
			return;
		}

		var attribute = compilation.GetTypeByMetadataName(SpecialFullTypeNames.FileAccessOnlyAttribute);
		if (attribute is null)
		{
			return;
		}

		if (symbol.GetAttributes().All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
		{
			return;
		}

		var location = syntaxRef.GetSyntax(ct) switch
		{
			ConstructorDeclarationSyntax { Identifier: var identifier } => identifier.GetLocation(),
			VariableDeclaratorSyntax { Identifier: var identifier } => identifier.GetLocation()
		};
		context.ReportDiagnostic(Diagnostic.Create(SCA0112, location));
	}
}
