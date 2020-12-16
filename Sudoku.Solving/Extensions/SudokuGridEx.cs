using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Manual;
using Sudoku.Solving.Manual.Intersections;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Solving.Manual.Subsets;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SudokuGrid"/>.
	/// </summary>
	/// <seealso cref="SudokuGrid"/>
	public static class SudokuGridEx
	{
		/// <summary>
		/// All available SSTS searchers.
		/// </summary>
		private static readonly StepSearcher[] SstsSearchers =
		{
			new SingleStepSearcher(false, false, false),
			new LcStepSearcher(),
			new SubsetStepSearcher()
		};


		/// <summary>
		/// To clean the grid.
		/// </summary>
		/// <param name="this">(<see langword="this ref"/> parameter) The grid.</param>
		/// <remarks>
		/// "To clean a grid" means we process this grid to fill with singles that is found
		/// in <see cref="SingleStepSearcher"/>, and remove eliminations that is found
		/// in <see cref="LcStepSearcher"/> and <see cref="SubsetStepSearcher"/>.
		/// The process won't stop until the puzzle cannot use these techniques.
		/// </remarks>
		/// <seealso cref="SingleStepSearcher"/>
		/// <seealso cref="LcStepSearcher"/>
		/// <seealso cref="SubsetStepSearcher"/>
		public static void Clean(this ref SudokuGrid @this)
		{
			var steps = new List<StepInfo>();

			while (true)
			{
				steps.Clear();
				StepSearcher.InitializeMaps(@this);
				for (int i = 0, length = SstsSearchers.Length; i < length; i++)
				{
					var searcher = SstsSearchers[i];
					searcher.GetAll(steps, @this);
					if (steps.Count == 0)
					{
						continue;
					}

					foreach (var step in steps)
					{
						step.ApplyTo(ref @this);
					}
				}
			}
		}
	}
}
