namespace Sudoku.Generating.TechniqueBased;

/// <summary>
/// Represents a puzzle generator that generates for puzzles using single techniques.
/// </summary>
/// <typeparam name="TStep">The type of the step supported.</typeparam>
public abstract class SinglePuzzleGenerator<TStep> : TechniqueBasedPuzzleGenerator where TStep : SingleStep
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
	/// Indicates whether the generator will also create for interferer digits.
	/// </summary>
	public bool HasInterfererDigits { get; set; }

	/// <inheritdoc cref="GridAlignment"/>
	public GridAlignment Alignment { get; set; }

	/// <inheritdoc/>
	public override SudokuType SupportedTypes => SudokuType.JustOneCell;


	/// <summary>
	/// Randomly select a house, obeying the rule <see cref="Alignment"/>.
	/// </summary>
	/// <returns>The house index.</returns>
	/// <seealso cref="Alignment"/>
	private protected House RandomlySelectHouse()
		=> Alignment switch
		{
			GridAlignment.NotLimited => Rng.Next(0, 27),
			GridAlignment.CenterHouses => 9 * Rng.Next(0, 3) + 4,
			GridAlignment.CenterBlock => CenterHouses[Rng.Next(0, CenterHouses.Length)],
			_ => StrictCenterHouses[Rng.Next(0, StrictCenterHouses.Length)]
		};


	/// <summary>
	/// Checks for the block position of the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The block position.</returns>
	private protected static int BlockPositionOf(Cell cell)
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
	/// Randomly select a <see cref="SingleTechniqueSubtype"/> instance.
	/// </summary>
	/// <param name="house">The house selected.</param>
	/// <param name="match">The extra match method.</param>
	/// <returns>The subtype selected.</returns>
	private protected static SingleTechniqueSubtype RandomlySelectSubtype(House house, Func<SingleTechniqueSubtype, bool> match)
	{
		var range = Enum.GetValues<SingleTechniqueSubtype>()[house switch { < 9 => 4..13, < 18 => 14..28, _ => 29..43 }];
		SingleTechniqueSubtype subtype;
		do
		{
			subtype = range[Rng.Next(0, range.Length)];
		} while (subtype.IsUnnecessary() || !match(subtype));
		return subtype;
	}
}
