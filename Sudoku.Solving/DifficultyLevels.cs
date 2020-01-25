using System;

namespace Sudoku.Solving
{
	/// <summary>
	/// Provides a difficulty kind for a puzzle.
	/// </summary>
	[Flags]
	public enum DifficultyLevels : short
	{
		/// <summary>
		/// Indicates the difficulty level is unknown.
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// Indicates the difficulty is very easy.
		/// </summary>
		VeryEasy = 1,
		/// <summary>
		/// Indicates the difficulty is easy.
		/// </summary>
		Easy = 2,
		/// <summary>
		/// Indicates the difficulty is moderate.
		/// </summary>
		Moderate = 4,
		/// <summary>
		/// Indicates the difficulty is advanced.
		/// </summary>
		Advanced = 8,
		/// <summary>
		/// Indicates the difficulty is hard.
		/// </summary>
		Hard = 16,
		/// <summary>
		/// Indicates the difficulty is very hard.
		/// </summary>
		VeryHard = 32,
		/// <summary>
		/// Indicates the difficulty is fiendish.
		/// </summary>
		Fiendish = 64,
		/// <summary>
		/// Indicates the difficulty is diabolical.
		/// </summary>
		Diabolical = 128,
		/// <summary>
		/// Indicates the difficulty is crazy.
		/// </summary>
		Crazy = 256,
		/// <summary>
		/// Indicates the difficulty is nightmare.
		/// </summary>
		Nightmare = 512,
		/// <summary>
		/// Indicates the difficulty is beyond nightmare.
		/// </summary>
		BeyondNightmare = 1024,
		/// <summary>
		/// Indicates the puzzle cannot be solved
		/// unless using last resort methods.
		/// </summary>
		LastResort = 2048,
	}
}
