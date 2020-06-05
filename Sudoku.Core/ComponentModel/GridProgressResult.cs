namespace Sudoku.ComponentModel
{
	/// <summary>
	/// Encapsulates a progress result used for report the current state.
	/// </summary>
	public struct GridProgressResult : IProgressResult
	{
		/// <summary>
		/// Initializes an instance with the specified current point and the total point.
		/// </summary>
		/// <param name="currentCandidatesCount">The current point.</param>
		/// <param name="currentCellsCount">The number of unsolved cells.</param>
		/// <param name="initialCandidatesCount">The number of unsolved candidates in the initial grid.</param>
		public GridProgressResult(int currentCandidatesCount, int currentCellsCount, int initialCandidatesCount) =>
			(CurrentCandidatesCount, CurrentCellsCount, InitialCandidatesCount) =
			(currentCandidatesCount, currentCellsCount, initialCandidatesCount);


		/// <summary>
		/// Indicates the number of unsolved cells.
		/// </summary>
		public int CurrentCellsCount { readonly get; set; }

		/// <summary>
		/// Indicates the number of unsolved candidates.
		/// </summary>
		public int CurrentCandidatesCount { readonly get; set; }

		/// <summary>
		/// Indicates the number of unsolved candidates in the initial grid.
		/// </summary>
		public readonly int InitialCandidatesCount { get; }

		/// <summary>
		/// Indicates the current percentage.
		/// </summary>
		public readonly double Percentage =>
			(double)(InitialCandidatesCount - CurrentCandidatesCount) / InitialCandidatesCount * 100;


		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="current">(<see langword="out"/> parameter) The number of unsolved candidates.</param>
		/// <param name="unsolvedCells">(<see langword="out"/> parameter) The number of unsolved cells.</param>
		public readonly void Deconstruct(out int current, out int unsolvedCells) =>
			(current, unsolvedCells) = (CurrentCandidatesCount, CurrentCellsCount);

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="Deconstruct"]'/>
		/// <param name="currentCandidatesCount">(<see langword="out"/> parameter) The number of unsolved candidates.</param>
		/// <param name="currentCellsCount">(<see langword="out"/> parameter) The number of unsolved cells.</param>
		/// <param name="initialCandidatesCount">
		/// (<see langword="out"/> parameter) The number of unsolved candidates in the initial grid.
		/// </param>
		public readonly void Deconstruct(
			out int currentCandidatesCount, out int currentCellsCount, out int initialCandidatesCount) =>
			(currentCandidatesCount, currentCellsCount, initialCandidatesCount) =
			(CurrentCandidatesCount, CurrentCellsCount, InitialCandidatesCount);

		/// <include file='../../GlobalDocComments.xml' path='comments/method[@name="ToString" and @paramType="__noparam"]'/>
		public override readonly string ToString() =>
			$"Unsolved cells: {CurrentCellsCount}, candidates: {CurrentCandidatesCount}";
	}
}
