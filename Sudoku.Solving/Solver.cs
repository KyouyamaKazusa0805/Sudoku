using System.Threading.Tasks;
using Sudoku.Data.Meta;

namespace Sudoku.Solving
{
	/// <summary>
	/// Indicates an instance used for solving a sudoku puzzle.
	/// </summary>
	public abstract class Solver
	{
		/// <summary>
		/// The name of this solver.
		/// </summary>
		public abstract string SolverName { get; }


		/// <summary>
		/// Solves the specified puzzle.
		/// </summary>
		/// <param name="grid">The puzzle.</param>
		/// <returns>
		/// An <see cref="AnalysisResult"/> displaying all information of solving.
		/// </returns>
		public abstract AnalysisResult Solve(IReadOnlyGrid grid);

		/// <summary>
		/// Solves the specified puzzle asynchronizedly.
		/// </summary>
		/// <param name="grid">The puzzle.</param>
		/// <param name="continueOnCapturedContext">
		/// <see langword="true"/> to attempt to marshal the continuation back to
		/// the original context captured; otherwise, <see langword="false"/>.
		/// </param>
		/// <returns>The solving task.</returns>
		public virtual async Task<AnalysisResult> SolveAsync(
			IReadOnlyGrid grid, bool continueOnCapturedContext) =>
			await Task.Run(() => Solve(grid)).ConfigureAwait(continueOnCapturedContext);
	}
}
