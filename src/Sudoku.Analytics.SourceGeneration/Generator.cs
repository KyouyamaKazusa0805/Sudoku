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
	}


	private void Basic(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(context.CompilationProvider, StepSearcherDefaultImportingHandler.Output);

	private void StepData(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(context.CompilationProvider.Combine(context.AdditionalTextsProvider.Collect()), StepDataHandler.Output);
}
