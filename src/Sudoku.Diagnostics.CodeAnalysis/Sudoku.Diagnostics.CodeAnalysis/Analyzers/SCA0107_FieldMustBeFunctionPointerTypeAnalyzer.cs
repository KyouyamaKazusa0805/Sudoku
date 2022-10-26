namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0107")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSymbolAction), typeof(SymbolKind), nameof(SymbolKind.Field))]
public sealed partial class SCA0107_FieldMustBeFunctionPointerTypeAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SymbolAnalysisContext context)
	{
		if (context is not
			{
				Symbol: IFieldSymbol
				{
					DeclaringSyntaxReferences: [var syntaxRef],
					Type: not (IPointerTypeSymbol { PointedAtType.SpecialType: SpecialType.System_Void } or IFunctionPointerTypeSymbol),
				} targetField,
				Compilation: var compilation,
				CancellationToken: var ct
			})
		{
			return;
		}

		var syntaxNode = syntaxRef.GetSyntax(ct);
		var location = syntaxNode.GetLocation();
		var attribute = compilation.GetTypeByMetadataName(SpecialFullTypeNames.DisallowFunctionPointerInvocationAttribute);
		if (attribute is null)
		{
			return;
		}

		if (targetField.GetAttributes().All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0107, location));
	}
}
