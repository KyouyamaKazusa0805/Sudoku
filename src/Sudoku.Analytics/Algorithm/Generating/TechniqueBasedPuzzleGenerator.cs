namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a generator type that generates puzzles, relating to a kind of technique.
/// </summary>
public abstract class TechniqueBasedPuzzleGenerator
{
	/// <summary>
	/// Indicates the supported sudoku puzzle types.
	/// </summary>
	public abstract SudokuType SupportedPuzzleTypes { get; }

	/// <summary>
	/// Indicates the supported techniques.
	/// </summary>
	public abstract TechniqueSet SupportedTechniques { get; }


	/// <summary>
	/// Indicates the random number generator.
	/// </summary>
	private protected static Random Rng => Random.Shared;


	/// <summary>
	/// Generates a puzzle that has multiple solutions, with only one cell has only one possibility to be filled
	/// that can be solved in logic.
	/// </summary>
	/// <inheritdoc cref="GenerateUnique(out Grid, CancellationToken)"/>
	public abstract GenerationResult GenerateJustOneCell(out Grid result, CancellationToken cancellationToken = default);

	/// <summary>
	/// Generates a puzzle that has a unique solution, with a must that contains the specified technique appeared in the puzzle.
	/// </summary>
	/// <param name="result">
	/// The puzzle returned. The argument becomes valid if and only if the return value is <see cref="GenerationResult.Success"/>.
	/// </param>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>
	/// A <see cref="GenerationResult"/> enumeration field describing whether the generation is failed, and why failed.
	/// </returns>
	/// <seealso cref="Grid.Undefined"/>
	/// <seealso cref="GenerationResult.Success"/>
	public abstract GenerationResult GenerateUnique(out Grid result, CancellationToken cancellationToken = default);
}
