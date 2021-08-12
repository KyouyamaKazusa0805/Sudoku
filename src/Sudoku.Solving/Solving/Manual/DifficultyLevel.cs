namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides a difficulty kind for a puzzle.
	/// </summary>
	[Flags]
	public enum DifficultyLevel : byte
	{
		/// <summary>
		/// Indicates the difficulty level is unknown.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// Indicates the difficulty is easy.
		/// </summary>
		Easy = 1,

		/// <summary>
		/// Indicates the difficulty is moderate.
		/// </summary>
		Moderate = 2,

		/// <summary>
		/// Indicates the difficulty is hard.
		/// </summary>
		Hard = 4,

		/// <summary>
		/// Indicates the difficulty is fiendish.
		/// </summary>
		Fiendish = 8,

		/// <summary>
		/// Indicates the difficulty is nightmare.
		/// </summary>
		Nightmare = 16,

		/// <summary>
		/// Indicates the puzzle can't be solved
		/// unless using last resort methods.
		/// </summary>
		LastResort = 32,
	}
}
