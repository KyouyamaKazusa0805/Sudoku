namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents a flag that describes for a single technique usage or type kind.
/// </summary>
public enum SingleTechniqueFlag
{
	/// <summary>
	/// Represents none of technique used.
	/// </summary>
	None = 0,

	/// <summary>
	/// Represents full house.
	/// </summary>
	FullHouse = 10,

	/// <summary>
	/// Represents last digit.
	/// </summary>
	LastDigit = 20,

	/// <summary>
	/// Represents hidden single.
	/// </summary>
	HiddenSingle = 30,

	/// <summary>
	/// Represents hidden single in block.
	/// </summary>
	HiddenSingleBlock,

	/// <summary>
	/// Represents hidden single in row.
	/// </summary>
	HiddenSingleRow,

	/// <summary>
	/// Represents hidden single in column.
	/// </summary>
	HiddenSingleColumn,

	/// <summary>
	/// Represents naked single.
	/// </summary>
	NakedSingle = 40
}
