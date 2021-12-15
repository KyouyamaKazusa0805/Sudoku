namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

/// <summary>
/// Indicates the analyzer that checks for the lambda discard parameter applied <see cref="IsDiscardAttribute"/>.
/// </summary>
/// <seealso cref="IsDiscardAttribute"/>
[Generator(LanguageNames.CSharp)]
public sealed class LambdaDiscardParameterAnalyzer : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context) =>
		((LambdaDiscardParameterSyntaxChecker)context.SyntaxContextReceiver!).Diagnostics.ForEach(context.ReportDiagnostic);

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context) =>
		context.RegisterForSyntaxNotifications(() => new LambdaDiscardParameterSyntaxChecker(context.CancellationToken));
}
