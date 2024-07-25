namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator that can generate a valid puzzle.
/// </summary>
public interface IUniqueGenerator : ITechniqueGenerator, IGenerator<Grid>
{
	/// <summary>
	/// Indicates the analyzer used.
	/// </summary>
	public static abstract ref readonly Analyzer Analyzer { get; }


	/// <summary>
	/// Generates a puzzle and return a <see cref="Grid"/> instance;
	/// using <paramref name="cancellationToken"/> to cancel the operation.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token instance that can cancel the current operation.</param>
	/// <returns>The result <see cref="Grid"/> instance generated.</returns>
	public abstract Grid GenerateUnique(CancellationToken cancellationToken = default);

	/// <inheritdoc/>
	Grid IGenerator<Grid>.Generate(IProgress<GeneratorProgress>? progress, CancellationToken cancellationToken)
		=> GenerateUnique(cancellationToken);
}
