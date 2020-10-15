namespace Sudoku.Data
{
	unsafe partial struct SudokuGrid
	{
		/// <summary>
		/// Indicates the event triggered when the value is changed.
		/// </summary>
		public static delegate* managed<ref SudokuGrid, in ValueChangedArgs, void> ValueChanged { get; }

		/// <summary>
		/// Indicates the event triggered when should recompute candidates.
		/// </summary>
		public static delegate* managed<ref SudokuGrid, void> RecomputeCandidates { get; }
	}
}
