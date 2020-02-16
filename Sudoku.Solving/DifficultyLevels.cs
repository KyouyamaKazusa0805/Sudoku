namespace Sudoku.Solving
{
	/// <summary>
	/// Provides a difficulty kind for a puzzle.
	/// </summary>
	public enum DifficultyLevels : short
	{
		/// <summary>
		/// Indicates the difficulty level is unknown.
		/// </summary>
		Unknown,
		/// <summary>
		/// Indicates the difficulty is very easy.
		/// </summary>
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
		Advanced,
		/// <summary>
		/// Indicates the difficulty is hard.
		/// </summary>
		Hard,
		/// <summary>
		/// Indicates the difficulty is very hard.
		/// </summary>
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
		Crazy,
		/// <summary>
		/// Indicates the difficulty is nightmare.
		/// </summary>
		Nightmare,
		/// <summary>
		/// Indicates the difficulty is beyond nightmare.
		/// </summary>
		BeyondNightmare,
		/// <summary>
		/// Indicates the puzzle cannot be solved
		/// unless using last resort methods.
		/// </summary>
		LastResort,
	}
}
