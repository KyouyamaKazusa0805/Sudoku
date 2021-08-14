namespace Sudoku.Solving.Manual;

/// <summary>
/// Indicates the rarity of the technique.
/// </summary>
[Closed]
public enum Rarity : byte
{
	/// <summary>
	/// Indicates the technique appears very commonly, such as <b>Hidden Single</b>.
	/// </summary>
	VeryCommon,

	/// <summary>
	/// Indicates the rarity is common.
	/// </summary>
	Common,

	/// <summary>
	/// Indicates the rarity is seldom.
	/// </summary>
	Seldom,

	/// <summary>
	/// Indicates the technique only appears in the special puzzles, which are designed on purpose.
	/// </summary>
	OnlyForSpecialPuzzles
}
