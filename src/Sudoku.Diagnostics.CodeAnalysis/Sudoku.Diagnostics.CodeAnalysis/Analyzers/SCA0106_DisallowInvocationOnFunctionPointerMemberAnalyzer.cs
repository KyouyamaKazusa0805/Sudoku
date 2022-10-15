namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0001", "SCA0106")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterOperationAction), typeof(OperationKind), nameof(OperationKind.FunctionPointerInvocation))]
public sealed partial class SCA0106_DisallowInvocationOnFunctionPointerMemberAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(OperationAnalysisContext context)
	{
		if (context is not
			{
				Operation: IFunctionPointerInvocationOperation
				{
					Target: IFieldReferenceOperation { Field: { DeclaringSyntaxReferences: [var fieldSyntaxRef] } targetField },
					Syntax: var syntaxNode
				},
				Compilation: var compilation,
				CancellationToken: var ct
			})
		{
			return;
		}

		var location = syntaxNode.GetLocation();
		var attribute = compilation.GetTypeByMetadataName(SpecialFullTypeNames.DisallowFunctionPointerInvocationAttribute);
		if (attribute is null)
		{
			context.ReportDiagnostic(Diagnostic.Create(SCA0001, location));
			return;
		}

		if (targetField.GetAttributes().All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
		{
			return;
		}

		// Special case: Check whether the specified invocation node is in the containing type of the function pointer field.
		foreach (var n1 in fieldSyntaxRef.GetSyntax(ct).Ancestors().OfType<TypeDeclarationSyntax>())
		{
			foreach (var n2 in syntaxNode.Ancestors().OfType<TypeDeclarationSyntax>())
			{
				if (n1.IsEquivalentTo(n2))
				{
					return;
				}
			}
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0106, location));
	}
}
