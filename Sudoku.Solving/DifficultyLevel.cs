using System;

namespace Sudoku.Solving
{
	/// <summary>
	/// Provides a difficulty kind for a puzzle.
	/// </summary>
	public enum DifficultyLevel : short
	{
		/// <summary>
		/// Indicates the difficulty level is unknown.
		/// </summary>
		Unknown,

		/// <summary>
		/// Indicates the difficulty is very easy.
		/// </summary>
		[Obsolete("This field will be removed in future.", true)]
		VeryEasy,

		/// <summary>
		/// Indicates the difficulty is easy.
		/// </summary>
		Easy,

		/// <summary>
		/// Indicates the difficulty is moderate.
		/// </summary>
		Moderate,

		/// <summary>
		/// Indicates the difficulty is advanced.
		/// </summary>
		[Obsolete("This field will be removed in future.", true)]
		Advanced,

		/// <summary>
		/// Indicates the difficulty is hard.
		/// </summary>
		Hard,

		/// <summary>
		/// Indicates the difficulty is very hard.
		/// </summary>
		[Obsolete("This field will be removed in future.", true)]
		VeryHard,

		/// <summary>
		/// Indicates the difficulty is fiendish.
		/// </summary>
		Fiendish,

		/// <summary>
		/// Indicates the difficulty is diabolical.
		/// </summary>
		Diabolical,

		/// <summary>
		/// Indicates the difficulty is crazy.
		/// </summary>
		[Obsolete("This field will be removed in future.", true)]
		Crazy,

		/// <summary>
		/// Indicates the difficulty is nightmare.
		/// </summary>
		Nightmare,

		/// <summary>
		/// Indicates the difficulty is beyond nightmare.
		/// </summary>
		[Obsolete("This field will be removed in future.", true)]
		BeyondNightmare,

		/// <summary>
		/// Indicates the puzzle cannot be solved
		/// unless using last resort methods.
		/// </summary>
		LastResort,
	}
}
