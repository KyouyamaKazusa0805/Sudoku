using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Solving.Manual.LastResorts;

namespace Sudoku.Solving.Manual.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ManualSolver"/>.
	/// </summary>
	/// <seealso cref="ManualSolver"/>
	public static class ManualSolverEx
	{
		/// <summary>
		/// Get the searchers to enumerate on Sudoku Explainer mode.
		/// </summary>
		/// <param name="this">The manual solver.</param>
		/// <param name="solution">
		/// The solution for a sudoku grid.
		/// This parameter is necessary because some technique searchers will use this value,
		/// such as <see cref="BfStepSearcher"/>.
		/// </param>
		/// <returns>The result.</returns>
		public static StepSearcher[][] GetSeModeSearchers(this ManualSolver @this, in SudokuGrid? solution)
		{
			var list = @this.GetHodokuModeSearchers(solution);
			var dic = new Dictionary<int, IList<StepSearcher>>();
			foreach (var searcher in list)
			{
				int level = TechniqueProperties.FromSearcher(searcher)!.DisplayLevel;
				if (dic.TryGetValue(level, out var l))
				{
					l.Add(searcher);
				}
				else
				{
					dic.Add(level, new List<StepSearcher> { searcher });
				}
			}

			return dic.ToArray<int, IList<StepSearcher>, StepSearcher>();
		}

		/// <summary>
		/// Get the searchers to enumerate on Hodoku mode.
		/// </summary>
		/// <param name="this">The manual solver.</param>
		/// <param name="solution">
		/// The solution for a sudoku grid.
		/// This parameter is necessary because some technique searchers will use this value,
		/// such as <see cref="BfStepSearcher"/>. The default value is <see langword="null"/>.
		/// </param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static StepSearcher[] GetHodokuModeSearchers(this ManualSolver @this, in SudokuGrid? solution) =>
			@this.GetSearchers(solution);
	}
}
