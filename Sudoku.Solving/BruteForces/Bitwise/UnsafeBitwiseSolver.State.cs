namespace Sudoku.Solving.BruteForces.Bitwise
{
	public sealed partial class UnsafeBitwiseSolver
	{
		private unsafe struct State
		{
			/// <summary>
			/// Pencil marks in bands by digit.
			/// </summary>
			public fixed long bands[27];

			/// <summary>
			/// Value of bands last time it was calculated.
			/// </summary>
			public fixed long prevBands[27];

			/// <summary>
			/// Bit vector of unsolved cells.
			/// </summary>
			public fixed long unsolvedCells[3];

			/// <summary>
			/// Bit vector of unsolved rows - three bits per band.
			/// </summary>
			public fixed long unsolvedRows[3];

			/// <summary>
			/// Bit vector of cells with exactly two pencil marks.
			/// </summary>
			public fixed long pairs[3];
		}
	}
}
