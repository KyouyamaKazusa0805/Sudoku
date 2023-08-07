namespace Sudoku.SourceGeneration.Handlers;

/// <summary>
/// Represents a file-local constraint for generators,
/// which can be used for <see cref="IncrementalGeneratorInitializationContext.CompilationProvider"/>.
/// </summary>
/// <seealso cref="IncrementalGeneratorInitializationContext.CompilationProvider"/>
internal interface IIncrementalGeneratorCompilationHandler
{
	/// <summary>
	/// Try to generate the source.
	/// </summary>
	/// <param name="spc">The context used for generating.</param>
	/// <param name="compilation">
	/// The <see cref="Compilation"/> instance that provides the information for the calling project.
	/// </param>
	public abstract void Output(SourceProductionContext spc, Compilation compilation);
}
