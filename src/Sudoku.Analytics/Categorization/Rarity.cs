﻿namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Indicates the rarity of the technique.
/// </summary>
[Flags]
public enum Rarity : byte
{
	/// <summary>
	/// Indicates the technique can't be confirmed for its rarity.
	/// </summary>
	Unknown = 0,

	/// <summary>
	/// Indicates the technique always appears, such as <b>Hidden Single</b>.
	/// </summary>
	Always = 1,

	/// <summary>
	/// Indicates the rarity is often appears.
	/// </summary>
	Often = 1 << 1,

	/// <summary>
	/// Indicates the rarity is sometimes appears.
	/// </summary>
	Sometimes = 1 << 2,

	/// <summary>
	/// Indicates the rarity is seldom appears.
	/// </summary>
	Seldom = 1 << 3,

	/// <summary>
	/// Indicates the rarity is hardly ever appears.
	/// </summary>
	HardlyEver = 1 << 4,

	/// <summary>
	/// Indicates the technique only appears in the special puzzles designed on purpose.
	/// </summary>
	OnlyForSpecialPuzzles = 1 << 5,

	/// <summary>
	/// Indicates the technique can't appear because the technique will be replaced with another technique.
	/// </summary>
	ReplacedByOtherTechniques = 1 << 6
}
