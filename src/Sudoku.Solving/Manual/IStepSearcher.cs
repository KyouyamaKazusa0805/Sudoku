#if false

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
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		static abstract TechniqueProperties Properties { get; }


		/// <summary>
		/// 
		/// </summary>
		/// <param name="accumulator"></param>
		/// <param name="grid"></param>
		void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid);
	}
}

#endif