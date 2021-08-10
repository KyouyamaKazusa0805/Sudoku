using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Indicates the rarity of the technique.
	/// </summary>
	[Closed]
	public enum Rarity : byte
	{
		/// <summary>
		/// Indicates the technique appears in all sudoku puzzles, such as <b>Hidden Single</b>.
		/// </summary>
		Default,

		/// <summary>
		/// Indicates the rarity is common.
		/// </summary>
		Common,

		/// <summary>
		/// Indicates the rarity is rarely.
		/// </summary>
		Rarely
	}
}
