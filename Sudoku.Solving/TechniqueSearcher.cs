using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Sudoku.Data.Meta;

namespace Sudoku.Solving
{
	/// <summary>
	/// Encapsulates a step finder that used in solving in
	/// <see cref="Manual.ManualSolver"/>.
	/// </summary>
	public abstract class TechniqueSearcher
	{
		/// <summary>
		/// Take a step after searched all solving steps.
		/// </summary>
		/// <param name="grid">The grid to search steps.</param>
		/// <returns>A technique information.</returns>
		public TechniqueInfo? TakeOne(Grid grid) => TakeAll(grid).FirstOrDefault();

		/// <summary>
		/// Take the specified number of steps.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="count">The number of steps you want to take.</param>
		/// <returns>The specified number of technique information instances.</returns>
		public IReadOnlyList<TechniqueInfo> Take(Grid grid, int count)
		{
			Contract.Assume(count >= 1);

			// 'Take' method will never throw exceptions when
			// count is greater than the step count of the list.
			return TakeAll(grid).Take(count).ToList();
		}

		/// <summary>
		/// Take all technique steps after searched.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The list of all steps found.</returns>
		public abstract IReadOnlyList<TechniqueInfo> TakeAll(Grid grid);
	}
}
