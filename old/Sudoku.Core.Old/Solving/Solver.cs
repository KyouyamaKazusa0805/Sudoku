using Sudoku.Data.Meta;

namespace Sudoku.Solving
{
	public abstract class Solver
	{
		public abstract string Name { get; }

		public abstract Grid? Solve(Grid grid, out AnalysisInfo analysisInfo);
	}
}
