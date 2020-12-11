using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Constants;
using Sudoku.Data;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Encapsulates a step finder that used in solving in <see cref="ManualSolver"/>.
	/// </summary>
	/// <seealso cref="ManualSolver"/>
	public abstract partial class StepSearcher
	{
		/// <summary>
		/// Take a technique step after searched all solving steps.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid to search steps.</param>
		/// <returns>A technique information.</returns>
		public StepInfo? GetOne(in SudokuGrid grid)
		{
			var bag = new List<StepInfo>();
			GetAll(bag, grid);
			return bag.FirstOrDefault();
		}

		/// <summary>
		/// Accumulate all technique information instances into the specified accumulator.
		/// </summary>
		/// <param name="accumulator">The accumulator to store technique information.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid to search for techniques.</param>
		public abstract void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid);


		/// <summary>
		/// Initialize the maps that used later.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		public static void InitializeMaps(in SudokuGrid grid)
		{
			var (e, b, c, d, v) = grid;
			EmptyMap = e;
			BivalueMap = b;
			CandMaps = c;
			DigitMaps = d;
			ValueMaps = v;
		}
	}
}
