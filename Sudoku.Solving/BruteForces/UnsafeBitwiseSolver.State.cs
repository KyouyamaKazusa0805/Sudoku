namespace Sudoku.Solving.BruteForces
{
	partial class UnsafeBitwiseSolver
	{
		/// <summary>
		/// To describe a state for a current grid using binary values.
		/// </summary>
		private unsafe struct State
		{
			/// <summary>
			/// Pencil marks in bands by digit.
			/// </summary>
			public fixed uint Bands[3 * 9];

			/// <summary>
			/// Value of bands last time it was calculated.
			/// </summary>
			public fixed uint PrevBands[3 * 9];

			/// <summary>
			/// Bit vector of unsolved cells.
			/// </summary>
			public fixed uint UnsolvedCells[3];

			/// <summary>
			/// Bit vector of unsolved rows - three bits per band.
			/// </summary>
			public fixed uint UnsolvedRows[3];

			/// <summary>
			/// Bit vector of cells with exactly two pencil marks.
			/// </summary>
			public fixed uint Pairs[3];
		}
	}
}
