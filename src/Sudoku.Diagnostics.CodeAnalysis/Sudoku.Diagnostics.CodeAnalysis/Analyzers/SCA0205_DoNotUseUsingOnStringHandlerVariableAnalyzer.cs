namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0001", "SCA0205")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.VariableDeclarator))]
public sealed partial class SCA0205_DoNotUseUsingOnStringHandlerVariableAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SyntaxNodeAnalysisContext context)
	{
		if (context is not
			{
				Compilation: var compilation,
				CancellationToken: var ct,
				SemanticModel: var semanticModel,
				Node: VariableDeclaratorSyntax
				{
					Identifier: var identifier,
					Parent: VariableDeclarationSyntax { Parent: LocalDeclarationStatementSyntax { Modifiers: var modifiers and not [] } }
				} node
			})
		{
			return;
		}

		if (semanticModel.GetOperation(node, ct) is not IVariableDeclaratorOperation { Symbol.Type: var localType })
		{
			return;
		}

		var location = identifier.GetLocation();
		var stringHandlerType = compilation.GetTypeByMetadataName(SpecialFullTypeNames.StringHandler);
		if (stringHandlerType is null)
		{
			context.ReportDiagnostic(Diagnostic.Create(SCA0001, location, messageArgs: new[] { SpecialFullTypeNames.StringHandler }));
			return;
		}

		if (!SymbolEqualityComparer.Default.Equals(localType, stringHandlerType))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0205, location));
	}
}
