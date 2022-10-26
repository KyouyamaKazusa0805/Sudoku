namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0301")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSymbolAction), typeof(SymbolKind), nameof(SymbolKind.NamedType))]
public sealed partial class SCA0301_MissingSupportedArgumentsAttributeAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SymbolAnalysisContext context)
	{
		if (context is not
			{
				Symbol: INamedTypeSymbol { DeclaringSyntaxReferences: [var syntaxRef], AllInterfaces: var allInterfaces and not [] } type,
				Compilation: var compilation,
				CancellationToken: var ct
			})
		{
			return;
		}

		if (syntaxRef.GetSyntax(ct) is not BaseTypeDeclarationSyntax { Identifier: var identifier })
		{
			return;
		}

		var location = identifier.GetLocation();
		var attributeType = compilation.GetTypeByMetadataName(SpecialFullTypeNames.SupportedArgumentsAttribute);
		if (attributeType is null)
		{
			return;
		}

		var attributesData = type.GetAttributes();
		if (attributesData.Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType)))
		{
			return;
		}

		var executableType = compilation.GetTypeByMetadataName(SpecialFullTypeNames.IExecutable);
		if (executableType is null)
		{
			return;
		}

		if (allInterfaces.All(a => !SymbolEqualityComparer.Default.Equals(a, executableType)))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0301, location));
	}
}
