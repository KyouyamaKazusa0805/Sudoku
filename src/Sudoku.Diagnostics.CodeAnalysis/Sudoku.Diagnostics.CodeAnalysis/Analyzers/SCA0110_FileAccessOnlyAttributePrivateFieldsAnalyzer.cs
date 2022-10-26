namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0110")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.FieldDeclaration))]
public sealed partial class SCA0110_FileAccessOnlyAttributePrivateFieldsAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SyntaxNodeAnalysisContext context)
	{
		if (context is not
			{
				Node: FieldDeclarationSyntax { Declaration.Variables: var declarations and not [] },
				SemanticModel: var semanticModel,
				Compilation: var compilation,
				CancellationToken: var ct
			})
		{
			return;
		}

		var attribute = compilation.GetTypeByMetadataName(SpecialFullTypeNames.FileAccessOnlyAttribute);
		if (attribute is null)
		{
			return;
		}

		foreach (var declaration in declarations)
		{
			if (declaration is not { Identifier: { ValueText: var name } identifier })
			{
				continue;
			}

			if (semanticModel.GetDeclaredSymbol(declaration, ct) is not IFieldSymbol { DeclaredAccessibility: Accessibility.Private } symbol)
			{
				continue;
			}

			if (symbol.GetAttributes().All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
			{
				continue;
			}

			context.ReportDiagnostic(Diagnostic.Create(SCA0110, identifier.GetLocation(), messageArgs: new[] { name }));
		}
	}
}
