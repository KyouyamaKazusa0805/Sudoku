namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0208")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), nameof(SyntaxKind.Parameter))]
public sealed partial class SCA0208_ThisParameterNameAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SyntaxNodeAnalysisContext context)
	{
		if (context is not { Node: ParameterSyntax { Modifiers: var modifiers and not [], Identifier: { ValueText: not "this" } identifier } })
		{
			return;
		}

		if (!modifiers.Any(SyntaxKind.ThisKeyword))
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0208, identifier.GetLocation()));
	}
}
