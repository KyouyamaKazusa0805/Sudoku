namespace Sudoku.Generating;

/// <summary>
/// Represents a generator type that produces a complex data type <typeparamref name="TResult"/>,
/// encapsulating the details of the result.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public interface IGenerator<out TResult> : IPuzzleGenerator where TResult : PuzzleBase
{
	/// <summary>
	/// Generates a puzzle and return an instance of type <typeparamref name="TResult"/> indicating the result.
	/// </summary>
	/// <param name="progress">An <see cref="IProgress{T}"/> instance that is used for reporting the state.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>The result returned.</returns>
	public new abstract TResult Generate(IProgress<GeneratorProgress>? progress = null, CancellationToken cancellationToken = default);

	/// <inheritdoc/>
	Grid IPuzzleGenerator.Generate(IProgress<GeneratorProgress>? progress, CancellationToken cancellationToken)
		=> Generate(progress, cancellationToken) is { Success: true, Puzzle: var result } ? result : Grid.Undefined;
}
