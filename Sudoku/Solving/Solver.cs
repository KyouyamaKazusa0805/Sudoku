using System.Threading.Tasks;
using Sudoku.Data.Meta;

namespace Sudoku.Solving
{
	public abstract class Solver
	{
		public abstract string SolverName { get; }


		public abstract AnalysisResult Solve(Grid grid);

		public virtual async Task<AnalysisResult> SolveAsync(Grid grid, bool continueOnCapturedContext) =>
			await Task.Run(() => Solve(grid)).ConfigureAwait(continueOnCapturedContext);
	}
}
