namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0001", "SCA0109", "SCA0110")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterOperationAction), typeof(OperationKind), nameof(OperationKind.FieldReference))]
public sealed partial class SCA0109_FileAccessOnlyAttributeAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(OperationAnalysisContext context)
	{
		if (context is not
			{
				Operation: IFieldReferenceOperation
				{
					Field.DeclaringSyntaxReferences: [{ SyntaxTree.FilePath: var declaraingFilePath }],
					Syntax: { SyntaxTree.FilePath: var referencingFilePath } syntax
				},
				Compilation: var compilation
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

		context.ReportDiagnostic(Diagnostic.Create(SCA0109, location));
	}
}
