namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents a generator type that generates puzzles, relating to a kind of technique.
/// </summary>
public abstract class TechniqueBasedPuzzleGenerator :
	IFormattable,
	IGenerator<Grid>,
	IGenerator<FullPuzzle>,
	IGenerator<JustOneCellPuzzle>
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
	/// Represents the solver.
	/// </summary>
	private protected static readonly BitwiseSolver Solver = new();


	/// <summary>
	/// Indicates the percentage of interfering digits that can be inserted into a just-one-cell puzzle,
	/// in order to interfere the user to find the answer of the puzzle.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Although the value is of type <see cref="double"/>, only 2 digits after decimal point is used,
	/// i.e. the precision of this value is 0.01.
	/// </para>
	/// <para>
	/// In addition, the value must be greater than 0. The greater the value will be, the more interfering digits will be produced.
	/// The value is 0 by default.
	/// </para>
	/// <para>This property will be used in method <see cref="GenerateJustOneCell"/> only.</para>
	/// <para><b>
	/// Please note that the greater the value will be, the more unstable the puzzle will be.
	/// This means, if the value is larger than expectation, the puzzle may not guarantee uniqueness on answer.
	/// </b></para>
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

	/// <inheritdoc cref="IFormattable.ToString(string?, IFormatProvider?)"/>
	public string ToString(IFormatProvider? formatProvider) => SupportedTechniques.ToString(formatProvider as CultureInfo);

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

	/// <summary>
	/// Append interfering digits from the puzzle unfixed. The grid is generated in method <see cref="GenerateJustOneCell"/>.
	/// </summary>
	/// <param name="puzzle">An unfixed puzzle to be operated.</param>
	/// <param name="targetCell">The target cell to avoid.</param>
	/// <param name="interferingCells">The cells that are filled with interfering digits.</param>
	/// <returns>
	/// A <see cref="GeneratingFailedReason"/> instance describing the reason why this method failed to operate.
	/// </returns>
	/// <seealso cref="GenerateJustOneCell"/>
	/// <seealso cref="GeneratingFailedReason"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected unsafe GeneratingFailedReason AppendInterferingDigitsNoBaseGrid(ref Grid puzzle, Cell targetCell, out CellMap interferingCells)
	{
		// Try to get an answer of the puzzle.
		// I know the puzzle currently has multiple solutions, but we should fix one solution,
		// in order to fill digits.
		// Please note that the puzzle is not fixed now. We should find a solution using such modifiable digits.
		const int length = 82;
		var solutionBuffer = stackalloc char[length];
		new BitwiseSolver().SolveString(puzzle.ToString("!"), solutionBuffer, 1);
		var solution = Grid.Parse(new ReadOnlySpan<char>(solutionBuffer, length));
		if (solution.IsUndefined)
		{
			interferingCells = [];
			return GeneratingFailedReason.InvalidData;
		}

		return AppendInterferingDigitsCore(ref puzzle, in solution, targetCell, [], out interferingCells);
	}

	/// <summary>
	/// Append interfering digits from the puzzle unfixed.
	/// </summary>
	/// <param name="puzzle">An unfixed puzzle to be operated.</param>
	/// <param name="solution">Indicates the solution grid to the <paramref name="puzzle"/>.</param>
	/// <param name="targetCell">The target cell to avoid.</param>
	/// <param name="interferingCells">The cells that are filled with interfering digits.</param>
	/// <param name="excludedCells">Indicates the excluded cells.</param>
	/// <returns>
	/// A <see cref="GeneratingFailedReason"/> instance describing the reason why this method failed to operate.
	/// </returns>
	/// <seealso cref="GenerateJustOneCell"/>
	/// <seealso cref="GeneratingFailedReason"/>
	protected GeneratingFailedReason AppendInterferingDigitsCore(
		ref Grid puzzle,
		ref readonly Grid solution,
		Cell targetCell,
		ref readonly CellMap excludedCells,
		out CellMap interferingCells
	)
	{
		interferingCells = [];
		if (!InterferingPercentage.NearlyEquals(0, 1E-2F))
		{
			// Shuffle cell indices.
			ShuffleSequence(CellSeed);

			// Add interfering digits.
			var givensCount = puzzle.ModifiablesCount;
			var factGivensCount = GetGivensCount(givensCount, InterferingPercentage, out var interferingDigitsCount);
			if (factGivensCount >= 81)
			{
				return GeneratingFailedReason.InvalidData;
			}

			for (var i = 0; i < interferingDigitsCount; i++)
			{
				var interferingCell = CellSeed[i];
				if (puzzle.GetState(interferingCell) != CellState.Modifiable
					&& !excludedCells.Contains(interferingCell)
					&& interferingCell != targetCell)
				{
					// Set the value onto the puzzle.
					interferingCells.Add(interferingCell);
					puzzle.SetDigit(interferingCell, solution.GetDigit(interferingCell));
				}
			}
		}

		return GeneratingFailedReason.None;
	}

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);

	/// <inheritdoc/>
	Grid IGenerator<Grid>.Generate(IProgress<GeneratorProgress>? progress, CancellationToken cancellationToken)
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

	/// <summary>
	/// Calculates the number of givens in fact.
	/// </summary>
	/// <param name="currentGivensCount">The number of givens currently in puzzle.</param>
	/// <param name="ratio">The ratio. Pass <see cref="InterferingPercentage"/> to this parameter.</param>
	/// <param name="interferingDigitsCount">The final number of interfering digits.</param>
	/// <returns>The result number of given cells.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private protected static Cell GetGivensCount(Cell currentGivensCount, double ratio, out Cell interferingDigitsCount)
	{
		var result = (Cell)Math.Round(currentGivensCount * (1 + ratio));
		interferingDigitsCount = result - currentGivensCount;
		return result;
	}
}
