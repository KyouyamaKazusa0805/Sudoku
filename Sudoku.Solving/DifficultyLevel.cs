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
		/// Indicates the difficulty is easy.
		/// </summary>
		Easy,

		/// <summary>
		/// Indicates the difficulty is moderate.
		/// </summary>
		Moderate,

		/// <summary>
		/// Indicates the difficulty is hard.
		/// </summary>
		Hard,

		/// <summary>
		/// Indicates the difficulty is fiendish.
		/// </summary>
		Fiendish,

		/// <summary>
		/// Indicates the difficulty is nightmare.
		/// </summary>
		Nightmare,

		/// <summary>
		/// Indicates the puzzle can't be solved
		/// unless using last resort methods.
		/// </summary>
		LastResort,
	}
}
