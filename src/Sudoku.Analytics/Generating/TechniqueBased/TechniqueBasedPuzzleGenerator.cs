namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents a generator type that generates puzzles, relating to a kind of technique.
/// </summary>
public abstract class TechniqueBasedPuzzleGenerator : ICultureFormattable, IPuzzleGenerator
{
	/// <summary>
	/// Represents a seed array for cells that can be used in core methods.
	/// </summary>
	private protected static readonly Cell[] CellSeed = Enumerable.Range(0, 81).ToArray();

	/// <summary>
	/// Represents a seed array for houses that can be used in core methods.
	/// </summary>
	private protected static readonly House[] HouseSeed = Enumerable.Range(0, 27).ToArray();

	/// <summary>
	/// Represents a seed array for digits that can be used in core methods.
	/// </summary>
	private protected static readonly Digit[] DigitSeed = Enumerable.Range(0, 9).ToArray();


	/// <summary>
	/// Indicates the supported sudoku puzzle types.
	/// </summary>
	public abstract SudokuType SupportedTypes { get; }

	/// <summary>
	/// Indicates the supported techniques.
	/// </summary>
	public abstract TechniqueSet SupportedTechniques { get; }


	/// <summary>
	/// Indicates the random number generator.
	/// </summary>
	private protected static Random Rng => Random.Shared;


	/// <inheritdoc/>
	public sealed override string ToString() => ToString(null);

	/// <inheritdoc/>
	public string ToString(CultureInfo? culture = null) => SupportedTechniques.ToString(culture);

	/// <summary>
	/// Generates a puzzle that has multiple solutions, with only one cell has only one possibility to be filled
	/// that can be solved in logic.
	/// </summary>
	/// <returns>A type that encapsulates the result detail.</returns>
	public abstract JustOneCellPuzzle GenerateJustOneCell();

	/// <summary>
	/// Generates a puzzle that has a unique solution, with a must that contains the specified technique appeared in the puzzle.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>A type that encapsulates the result detail.</returns>
	public virtual FullPuzzle GenerateUnique(CancellationToken cancellationToken = default)
		=> new FullPuzzleFailed(GeneratingFailedReason.Unnecessary);

	/// <inheritdoc/>
	Grid IPuzzleGenerator.Generate(IProgress<GeneratorProgress>? progress, CancellationToken cancellationToken)
	{
		var result = GenerateUnique(cancellationToken);
		return result.Result != GeneratingFailedReason.None ? Grid.Undefined : result.Puzzle;
	}


	/// <summary>
	/// Try to shuffle the sequence for 3 times.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="values">The values to be shuffled.</param>
	protected static void ShuffleSequence<T>(T[] values)
	{
		for (var i = 0; i < 3; i++)
		{
			Rng.Shuffle(values);
		}
	}
}
