namespace Sudoku.SourceGeneration;

/// <summary>
/// Represents a source generator type that runs multiple different usage of source output services on compiling code.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class Generator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		Basic(context);
		StepData(context);
		SyntaxExpression(context);
		FunctionRawString(context);
	}


	private void Basic(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(context.CompilationProvider, StepSearcherDefaultImportingHandler.Output);

	private void StepData(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(context.CompilationProvider.Combine(context.AdditionalTextsProvider.Collect()), StepDataHandler.Output);

	private void SyntaxExpression(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.CreateSyntaxProvider(static (node, _) => node is ClassDeclarationSyntax, SyntaxExpressionHandler.Check)
				.Where(static value => value is not null)
				.Select(static (value, _) => value!.Value)
				.Collect(),
			SyntaxExpressionHandler.Output
		);

	private void FunctionRawString(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.SyntaxProvider
				.ForAttributeWithMetadataName(
					"Sudoku.Measuring.ExportFunctionAttribute",
					static (node, _) => node is MethodDeclarationSyntax,
					FunctionRawStringHandler.Check
				)
				.Where(static value => value is not null)
				.Select(static (value, _) => value!.Value)
				.Collect(),
			FunctionRawStringHandler.Output
		);
}
