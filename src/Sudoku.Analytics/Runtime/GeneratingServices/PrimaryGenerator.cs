namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator that supports generating for puzzles that can be solved by only using Direct Single techniques.
/// </summary>
public abstract class PrimaryGenerator : TechniqueGenerator, IAlignedJustOneCellGenerator, IPrimaryGenerator
{
	/// <summary>
	/// Indicates center houses.
	/// </summary>
	protected static readonly House[] CenterHouses = [4, 12, 13, 14, 21, 22, 23];

	/// <summary>
	/// Indicates center houses, strictly.
	/// </summary>
	protected static readonly House[] StrictCenterHouses = [4, 13, 22];

	/// <summary>
	/// Represents a seed array for cells that can be used in core methods.
	/// </summary>
	protected static readonly Cell[] CellSeed = Enumerable.Range(0, 81).ToArray();

	/// <summary>
	/// Represents a seed array for houses that can be used in core methods.
	/// </summary>
	protected static readonly House[] HouseSeed = Enumerable.Range(0, 27).ToArray();

	/// <summary>
	/// Represents a seed array for digits that can be used in core methods.
	/// </summary>
	protected static readonly Digit[] DigitSeed = Enumerable.Range(0, 9).ToArray();

	/// <summary>
	/// Represents an analyzer that will be used in generating phased puzzles.
	/// </summary>
	protected static readonly Analyzer SingleAnalyzer = Analyzer.Default
		.WithStepSearchers(new SingleStepSearcher { EnableFullHouse = true, HiddenSinglesInBlockFirst = true })
		.WithUserDefinedOptions(new() { DistinctDirectMode = true, IsDirectMode = true });


	/// <inheritdoc/>
	public ConclusionCellAlignment Alignment { get; set; }

	/// <inheritdoc/>
	public abstract TechniqueSet SupportedTechniques { get; }


	/// <summary>
	/// Try to shuffle the sequence.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="values">The values to be shuffled.</param>
	protected static void ShuffleSequence<T>(T[] values)
	{
		Rng.Shuffle(values);
		Rng.Shuffle(values);
		Rng.Shuffle(values);
	}

	/// <summary>
	/// Randomly select a <see cref="SingleSubtype"/> instance.
	/// </summary>
	/// <param name="house">The house selected.</param>
	/// <param name="match">The extra match method.</param>
	/// <returns>The subtype selected.</returns>
	protected static SingleSubtype RandomlySelectSubtype(House house, Func<SingleSubtype, bool> match)
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

	/// <summary>
	/// Checks for the block position of the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The block position.</returns>
	protected static Digit BlockPositionOf(Cell cell)
	{
		var block = cell.ToHouse(HouseType.Block);
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
	protected static House RandomlySelectHouse(ConclusionCellAlignment alignment)
		=> alignment switch
		{
			ConclusionCellAlignment.NotLimited => Rng.NextHouse(),
			ConclusionCellAlignment.CenterHouse => 9 * Rng.Next(0, 3) + 4,
			ConclusionCellAlignment.CenterBlock => CenterHouses[Rng.Next(0, CenterHouses.Length)],
			_ => StrictCenterHouses[Rng.Next(0, StrictCenterHouses.Length)]
		};


	/// <inheritdoc/>
	public abstract Grid GeneratePrimary(CancellationToken cancellationToken = default);

	/// <inheritdoc/>
	public abstract Grid GenerateJustOneCell(out Step? step, CancellationToken cancellationToken = default);

	/// <inheritdoc/>
	public abstract Grid GenerateJustOneCell(out Grid phasedGrid, out Step? step, CancellationToken cancellationToken = default);
}
