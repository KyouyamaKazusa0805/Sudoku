namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

/// <summary>
/// Indicates the analyzer that checks for the parameter applied <see cref="IsDiscardAttribute"/>.
/// </summary>
/// <seealso cref="IsDiscardAttribute"/>
[Generator(LanguageNames.CSharp)]
public sealed class DiscardParameterAnalyzer : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context) =>
		((DiscardParameterSyntaxChecker)context.SyntaxContextReceiver!).Diagnostics.ForEach(context.ReportDiagnostic);

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new DiscardParameterSyntaxChecker(context.CancellationToken));
}
