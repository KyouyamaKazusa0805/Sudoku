using Sudoku.Solving.Manual.Intersections;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Solving.Manual.Subsets;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides extension methods to be used while solving.
	/// </summary>
	public static class Solvings
	{
		/// <summary>
		/// To clean the grid.
		/// </summary>
		/// <param name="this">The grid.</param>
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
			var sstsSearchers = ManualSolver.GetSstsSearchers();

			var steps = new List<StepInfo>();

		Again:
			steps.Clear();
			InitializeMaps(@this);
			for (int i = 0, length = sstsSearchers.Length; i < length; i++)
			{
				var searcher = sstsSearchers[i];
				searcher.GetAll(steps, @this);
				if (steps.Count == 0)
				{
					continue;
				}

				foreach (var step in steps)
				{
					step.ApplyTo(ref @this);
				}

				goto Again;
			}
		}

		/// <summary>
		/// Checks whether the next step is "single" technique after the specified candidate is removed.
		/// The "single" techniques are:
		/// <list type="bullet">
		/// <item><see cref="Technique.HiddenSingleBlock"/></item>
		/// <item><see cref="Technique.HiddenSingleRow"/></item>
		/// <item><see cref="Technique.HiddenSingleColumn"/></item>
		/// <item><see cref="Technique.NakedSingle"/></item>
		/// </list>
		/// Full houses and last digits may not be found in this method due to the limitation of the algorithm.
		/// </summary>
		/// <param name="this">The grid to check.</param>
		/// <param name="candidateRemoved">The candidate removed.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool IsSingleWhenRemoved(this in SudokuGrid @this, int candidateRemoved)
		{
			int cellRemoved = candidateRemoved / 9, digitRemoved = candidateRemoved % 9;

			// Hidden single check.
			for (int digit = 0; digit < 9; digit++)
			{
				for (int region = 0; region < 27; region++)
				{
					int count = 0;
					bool flag = true;
					foreach (int cell in RegionMaps[region])
					{
						if ((cellRemoved != cell || digitRemoved != digit)
							&& @this.Exists(cell, digit) is true
							&& ++count > 1)
						{
							flag = false;
							break;
						}
					}
					if (!flag || count == 0)
					{
						continue;
					}

					return true;
				}
			}

			// Naked single check.
			for (int cell = 0; cell < 81; cell++)
			{
				if (@this.GetStatus(cell) != CellStatus.Empty)
				{
					continue;
				}

				short mask = cell != cellRemoved
					? @this.GetCandidates(cell)
					: (short)(@this.GetCandidates(cell) & ~(1 << digitRemoved));
				if (mask == 0 || (mask & mask - 1) != 0)
				{
					continue;
				}

				return true;
			}

			return false;
		}
	}
}
