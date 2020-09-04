using System;
using Sudoku.Data;

namespace Sudoku.Solving
{
	/// <summary>
	/// Indicates an instance used for solving a sudoku puzzle.
	/// </summary>
	[Serializable]
	public abstract class Solver
	{
		/// <summary>
		/// Indicates the name of this solver.
		/// </summary>
		public abstract string SolverName { get; }


		/// <summary>
		/// To solve the specified puzzle.
		/// </summary>
		/// <param name="grid">The puzzle.</param>
		/// <returns>
		/// An <see cref="AnalysisResult"/> displaying all information of solving.
		/// </returns>
		public abstract AnalysisResult Solve(Grid grid);
	}
}
