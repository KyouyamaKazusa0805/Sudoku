namespace Sudoku.Data
{
	partial struct SudokuGrid
	{
		/// <summary>
		/// Indicates the event triggered when the value is changed.
		/// </summary>
		public static unsafe delegate*<ref SudokuGrid, in ValueChangedArgs, void> ValueChanged { get; }

		/// <summary>
		/// Indicates the event triggered when should re-compute candidates.
		/// </summary>
		public static unsafe delegate*<ref SudokuGrid, void> RefreshingCandidates { get; }
	}
}
