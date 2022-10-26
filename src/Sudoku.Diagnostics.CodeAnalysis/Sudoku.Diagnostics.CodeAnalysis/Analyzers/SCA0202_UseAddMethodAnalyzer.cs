namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0202")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterOperationAction), typeof(OperationKind), nameof(OperationKind.CompoundAssignment))]
public sealed partial class SCA0202_UseAddMethodAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(OperationAnalysisContext context)
	{
		if (context is not
			{
				Compilation: var compilation,
				Operation: ICompoundAssignmentOperation
				{
					OperatorKind: BinaryOperatorKind.Add,
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
		var cellMapType = compilation.GetTypeByMetadataName(SpecialFullTypeNames.CellMap);
		if (cellMapType is null)
		{
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(type, cellMapType)
			|| !SymbolEqualityComparer.Default.Equals(firstArgType, cellMapType)
			|| !SymbolEqualityComparer.Default.Equals(returnType, cellMapType))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0202, nodeLocation));
	}
}
