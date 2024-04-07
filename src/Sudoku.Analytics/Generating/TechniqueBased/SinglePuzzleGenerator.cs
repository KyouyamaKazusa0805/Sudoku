namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents a puzzle generator that generates for puzzles using single techniques.
/// </summary>
/// <typeparam name="TStep">The type of the step supported.</typeparam>
public abstract class SinglePuzzleGenerator<TStep> : TechniqueBasedPuzzleGenerator, IGenerator<PhasedJustOneCellPuzzle>
	where TStep : SingleStep
{
	/// <summary>
	/// Indicates center houses.
	/// </summary>
	private protected static readonly House[] CenterHouses = [4, 12, 13, 14, 21, 22, 23];

	/// <summary>
	/// Indicates center houses, strictly.
	/// </summary>
	private protected static readonly House[] StrictCenterHouses = [4, 13, 22];

	/// <summary>
	/// Indicates the analyzer. This field can only be called inside method <see cref="GenerateJustOneCellPhased"/>.
	/// </summary>
	/// <seealso cref="GenerateJustOneCellPhased"/>
	private protected static readonly Analyzer SingleAnalyzer = Analyzers.Default
		.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true, HiddenSinglesInBlockFirst = true })
		.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true });


	/// <inheritdoc/>
	public override SudokuType SupportedTypes => SudokuType.JustOneCell;


	/// <summary>
	/// Generates a puzzle that is a just-one-cell, but is created from a normal puzzle that contains a unique solution.
	/// </summary>
	/// <param name="subtype">Indicates the subtype to be checked.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current operation.</param>
	/// <returns>A <see cref="PhasedJustOneCellPuzzle"/> instance to describe the result.</returns>
	/// <seealso cref="PhasedJustOneCellPuzzle"/>
	public abstract PhasedJustOneCellPuzzle GenerateJustOneCellPhased(SingleSubtype subtype = SingleSubtype.None, CancellationToken cancellationToken = default);

	/// <summary>
	/// Append interfering digits from the puzzle unfixed. The grid is generated in method
	/// <see cref="GenerateJustOneCellPhased(SingleSubtype, CancellationToken)"/>.
	/// </summary>
	/// <param name="puzzle">An unfixed puzzle to be operated.</param>
	/// <param name="baseGrid">The solution to be referenced.</param>
	/// <param name="targetCell">The target cell to avoid.</param>
	/// <param name="interferingCells">The cells that are filled with interfering digits.</param>
	/// <returns>
	/// A <see cref="GeneratingFailedReason"/> instance desribing the reason why this method failed to operate.
	/// </returns>
	/// <seealso cref="GenerateJustOneCellPhased(SingleSubtype, CancellationToken)"/>
	/// <seealso cref="GeneratingFailedReason"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected GeneratingFailedReason AppendInterferingDigitsBaseGrid(scoped ref Grid puzzle, scoped ref readonly Grid baseGrid, Cell targetCell, out CellMap interferingCells)
		=> AppendInterferingDigitsCore(ref puzzle, baseGrid.FixedGrid.SolutionGrid, targetCell, out interferingCells);

	/// <inheritdoc/>
	PhasedJustOneCellPuzzle IGenerator<PhasedJustOneCellPuzzle>.Generate(IProgress<GeneratorProgress>? progress, CancellationToken cancellationToken)
		=> GenerateJustOneCellPhased(cancellationToken: cancellationToken);


	/// <summary>
	/// Checks for the block position of the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The block position.</returns>
	private protected static Digit BlockPositionOf(Cell cell)
	{
		var block = cell.ToHouseIndex(HouseType.Block);
		var i = 0;
		foreach (var c in HousesCells[block])
		{
			if (c == cell)
			{
				return i;
			}
			i++;
		}
		return -1;
	}

	/// <summary>
	/// Randomly select a house, obeying the specified grid alignment rule.
	/// </summary>
	/// <param name="alignment">Indicates the grid alignment value to be used.</param>
	/// <returns>The house index.</returns>
	private protected static House RandomlySelectHouse(ConlusionCellAlignment alignment)
		=> alignment switch
		{
			ConlusionCellAlignment.NotLimited => Rng.Next(0, 27),
			ConlusionCellAlignment.CenterHouse => 9 * Rng.Next(0, 3) + 4,
			ConlusionCellAlignment.CenterBlock => CenterHouses[Rng.Next(0, CenterHouses.Length)],
			_ => StrictCenterHouses[Rng.Next(0, StrictCenterHouses.Length)]
		};

	/// <summary>
	/// Randomly select a <see cref="SingleSubtype"/> instance.
	/// </summary>
	/// <param name="house">The house selected.</param>
	/// <param name="match">The extra match method.</param>
	/// <returns>The subtype selected.</returns>
	private protected static SingleSubtype RandomlySelectSubtype(House house, Func<SingleSubtype, bool> match)
	{
		var prefixMustBe = house switch { < 9 => "BlockHiddenSingle", < 18 => "RowHiddenSingle", _ => "ColumnHiddenSingle" };
		var range = Enum.GetValues<SingleSubtype>();
		SingleSubtype subtype;
		do
		{
			subtype = range[Rng.Next(0, range.Length)];
		} while (!match(subtype) || subtype is SingleSubtype.None or SingleSubtype.Unknown || !subtype.ToString().StartsWith(prefixMustBe) || subtype.IsUnnecessary());
		return subtype;
	}
}
