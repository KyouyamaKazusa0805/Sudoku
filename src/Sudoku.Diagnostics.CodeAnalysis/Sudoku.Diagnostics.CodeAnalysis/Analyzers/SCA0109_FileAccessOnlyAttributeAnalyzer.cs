namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0001", "SCA0109")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterOperationAction), typeof(OperationKind), nameof(OperationKind.FieldReference))]
public sealed partial class SCA0109_FileAccessOnlyAttributeAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(OperationAnalysisContext context)
	{
		if (context is not
			{
				Operation: IFieldReferenceOperation
				{
					Field.DeclaringSyntaxReferences: [{ SyntaxTree.FilePath: var declaraingFilePath } syntaxRef],
					Syntax: { SyntaxTree.FilePath: var referencingFilePath } syntax,
					SemanticModel: { } semanticModel
				},
				Compilation: var compilation,
				CancellationToken: var ct
			})
		{
			return;
		}

		var location = syntax.GetLocation();
		var fileAccessOnlyAttribute = compilation.GetTypeByMetadataName(SpecialFullTypeNames.FileAccessOnlyAttribute);
		if (fileAccessOnlyAttribute is null)
		{
			context.ReportDiagnostic(Diagnostic.Create(SCA0001, location, messageArgs: new[] { SpecialFullTypeNames.FileAccessOnlyAttribute }));
			return;
		}

		if (declaraingFilePath == referencingFilePath)
		{
			return;
		}

		var fieldDeclarationSyntax = (FieldDeclarationSyntax)syntaxRef.GetSyntax(ct);
		var fieldSymbol = semanticModel.GetDeclaredSymbol(fieldDeclarationSyntax, ct)!;
		if (fieldSymbol.GetAttributes().All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, fileAccessOnlyAttribute)))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0109, location));
	}
}
