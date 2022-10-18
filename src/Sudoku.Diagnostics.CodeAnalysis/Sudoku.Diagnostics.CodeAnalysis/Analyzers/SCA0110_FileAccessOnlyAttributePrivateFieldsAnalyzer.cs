namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0001", "SCA0110")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.FieldDeclaration))]
public sealed partial class SCA0110_FileAccessOnlyAttributePrivateFieldsAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SyntaxNodeAnalysisContext context)
	{
		if (context is not
			{
				Node: FieldDeclarationSyntax
				{

				} node,
				SemanticModel: var semanticModel,
				Compilation: var compilation,
				CancellationToken: var ct
			})
		{
			return;
		}

		var location = node.GetLocation();
		var attribute = compilation.GetTypeByMetadataName(SpecialFullTypeNames.FileAccessOnlyAttribute);
		if (attribute is null)
		{
			context.ReportDiagnostic(Diagnostic.Create(SCA0001, null, messageArgs: new[] { SpecialFullTypeNames.FileAccessOnlyAttribute }));
			return;
		}

		var symbol = semanticModel.GetDeclaredSymbol(node, ct);
		if (symbol is not IFieldSymbol { DeclaredAccessibility: Accessibility.Private })
		{
			return;
		}

		if (symbol.GetAttributes().All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0110, location));
	}
}
