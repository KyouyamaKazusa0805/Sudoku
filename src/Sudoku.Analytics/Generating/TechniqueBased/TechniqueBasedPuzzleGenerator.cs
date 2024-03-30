namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents a generator type that generates puzzles, relating to a kind of technique.
/// </summary>
public abstract class TechniqueBasedPuzzleGenerator :
	ICultureFormattable,
	IGenerator<FullPuzzle>,
	IGenerator<JustOneCellPuzzle>,
	IPuzzleGenerator
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
	/// Indicates the percentage of interfering digits to be inserted into a just-one-cell puzzle, interfering the puzzle and user.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The value must be greater than 0. The greater the value will be, the more interfering digits will be produced.
	/// The value is 0 by default.
	/// </para>
	/// <para>This property will be used in method <see cref="GenerateJustOneCell"/> only.</para>
	/// </remarks>
	/// <seealso cref="GenerateJustOneCell"/>
	public double InterferingPercentage { get; set; }

	/// <summary>
	/// Indicates the aligning rule controlling the case what position just-one-cell puzzles produce conclusion cells can be at.
	/// </summary>
	public ConlusionCellAlignment Alignment { get; set; }

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
		=> GenerateUnique(cancellationToken) is { Success: true, Puzzle: var result } ? result : Grid.Undefined;

	/// <inheritdoc/>
	FullPuzzle IGenerator<FullPuzzle>.Generate(IProgress<GeneratorProgress>? progress, CancellationToken cancellationToken)
		=> GenerateUnique(cancellationToken);

	/// <inheritdoc/>
	JustOneCellPuzzle IGenerator<JustOneCellPuzzle>.Generate(IProgress<GeneratorProgress>? progress, CancellationToken cancellationToken)
		=> GenerateJustOneCell();


	/// <summary>
	/// Try to shuffle the sequence for 3 times.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="values">The values to be shuffled.</param>
	private protected static void ShuffleSequence<T>(T[] values)
	{
		for (var i = 0; i < 3; i++)
		{
			Rng.Shuffle(values);
		}
	}
}
