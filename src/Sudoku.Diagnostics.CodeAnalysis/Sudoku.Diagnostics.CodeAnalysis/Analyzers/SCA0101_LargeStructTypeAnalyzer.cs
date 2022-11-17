namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0101")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterOperationAction), typeof(OperationKind), nameof(OperationKind.ObjectCreation))]
public sealed partial class SCA0101_LargeStructTypeAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(OperationAnalysisContext context)
	{
		if (context is not
			{
				CancellationToken: var ct,
				Compilation: var compilation,
				Operation: IObjectCreationOperation
				{
					Type: { TypeKind: TypeKind.Struct, Name: var name and not [] } type,
					Arguments: [],
					Syntax: ObjectCreationExpressionSyntax node
				}
			})
		{
			return;
		}

		var nodeLocation = node.GetLocation();
		if (compilation.GetTypeByMetadataName(SpecialFullTypeNames.IsLargeStructAttribute) is not { } attributeType)
		{
			return;
		}

		if (type.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType)) is not { } a)
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0101, nodeLocation, messageArgs: null));
	}
}
