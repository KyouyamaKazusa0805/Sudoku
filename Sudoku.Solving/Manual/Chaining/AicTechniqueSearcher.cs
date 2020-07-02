using System.Collections.Generic;
using Sudoku.Data;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates an <b>alternating inference chain</b> (AIC) technique searcher.
	/// </summary>
	/// <remarks>
	/// I want to use BFS (breadth-first searching) to search for chains, which can avoid
	/// the redundant backtracking.
	/// </remarks>
	public sealed class AicTechniqueSearcher : ChainingTechniqueSearcher
	{
		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var tempAccumulator = new Bag<ChainingTechniqueInfo>();
			GetAll(tempAccumulator, grid, true, false);
			GetAll(tempAccumulator, grid, false, true);
			GetAll(tempAccumulator, grid, true, true);

			//tempAccumulator.Sort((i1, i2) =>
			//{
			//
			//});

			accumulator.AddRange(tempAccumulator);
		}


		/// <summary>
		/// Search for chains of each type.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">Thr grid.</param>
		/// <param name="xEnabled">
		/// Indicates whether the strong 
		/// </param>
		/// <param name="yEnabled"></param>
		private void GetAll(IBag<ChainingTechniqueInfo> accumulator, IReadOnlyGrid grid, bool xEnabled, bool yEnabled)
		{
		}
	}
}
