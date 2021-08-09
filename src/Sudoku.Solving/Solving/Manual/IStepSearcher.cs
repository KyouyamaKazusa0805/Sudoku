using System.Collections.Generic;
using Sudoku.Data;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides the basic restraint of a <see cref="StepSearcher"/>.
	/// </summary>
	/// <seealso cref="StepSearcher"/>
	public interface IStepSearcher
	{
		/// <summary>
		/// Accumulate all technique information instances into the specified accumulator.
		/// </summary>
		/// <param name="accumulator">The accumulator to store technique information.</param>
		/// <param name="grid">The grid to search for techniques.</param>
		void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid);
	}
}
