namespace Sudoku.Solving.Manual;

/// <summary>
/// Indicates the rarity of the technique.
/// </summary>
[Closed]
public enum Rarity : byte
{
	/// <summary>
	/// Indicates the technique always appears, such as <b>Hidden Single</b>.
	/// </summary>
	Always,

	/// <summary>
	/// Indicates the rarity is often appears.
	/// </summary>
	Often,

	/// <summary>
	/// Indicates the rarity is sometimes appears.
	/// </summary>
	Sometimes,

	/// <summary>
	/// Indicates the rarity is seldom appears.
	/// </summary>
	Seldom,

	/// <summary>
	/// Indicates the rarity is hardly ever appears.
	/// </summary>
	HardlyEver,

	/// <summary>
	/// Indicates the technique only appears in the special puzzles designed on purpose.
	/// </summary>
	OnlyForSpecialPuzzles,

	/// <summary>
	/// Indicates the technique can't appear because the technique will be replaced with another technique.
	/// </summary>
	ReplacedByOtherTechniques
}
