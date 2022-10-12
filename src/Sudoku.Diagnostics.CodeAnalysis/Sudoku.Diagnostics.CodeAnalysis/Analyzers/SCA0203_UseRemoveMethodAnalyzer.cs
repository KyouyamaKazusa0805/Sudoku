namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0001", "SCA0203")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterOperationAction), typeof(OperationKind), nameof(OperationKind.CompoundAssignment))]
public sealed partial class SCA0203_UseRemoveMethodAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(OperationAnalysisContext context)
	{
		if (context is not
			{
				Compilation: var compilation,
				Operation: ICompoundAssignmentOperation
				{
					OperatorKind: BinaryOperatorKind.Subtract,
					Type: var type,
					OperatorMethod:
					{
						Parameters: [{ Type: var firstArgType }, { Type.SpecialType: SpecialType.System_Int32 }],
						ReturnType: var returnType
					},
					Syntax: var syntax
				}
			})
		{
			return;
		}

		var nodeLocation = syntax.GetLocation();
		var cellMapType = compilation.GetTypeByMetadataName("Sudoku.Concepts.CellMap");
		if (cellMapType is null)
		{
			context.ReportDiagnostic(Diagnostic.Create(SCA0001, nodeLocation, messageArgs: new[] { "CellMap" }));
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(type, cellMapType)
			|| !SymbolEqualityComparer.Default.Equals(firstArgType, cellMapType)
			|| !SymbolEqualityComparer.Default.Equals(returnType, cellMapType))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0203, nodeLocation));
	}
}
