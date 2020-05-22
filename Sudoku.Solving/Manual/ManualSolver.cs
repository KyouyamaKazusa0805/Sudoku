using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Checking;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides a solver that use logical methods to solve a specified sudoku puzzle.
	/// </summary>
	[Serializable]
	public sealed partial class ManualSolver : Solver
	{
		/// <inheritdoc/>
		public override string SolverName => "Manual";


		/// <inheritdoc/>
		public override AnalysisResult Solve(IReadOnlyGrid grid)
		{
			if (grid.IsValid(out var solution, out bool? sukaku))
			{
				// Solve the puzzle.
				try
				{
					return AnalyzeDifficultyStrictly
						? SolveWithStrictDifficultyRating(
							grid, grid.Clone(), new List<TechniqueInfo>(), solution, sukaku.Value)
						: SolveNaively(grid, grid.Clone(), new List<TechniqueInfo>(), solution, sukaku.Value);
				}
				catch (WrongHandlingException ex)
				{
					return new AnalysisResult(
						puzzle: grid,
						solverName: SolverName,
						hasSolved: false,
						solution: null,
						elapsedTime: TimeSpan.Zero,
						solvingList: null,
						additional: ex.Message,
						stepGrids: null);
				}
			}
			else
			{
				return new AnalysisResult(
					puzzle: grid,
					solverName: SolverName,
					hasSolved: false,
					solution: null,
					elapsedTime: TimeSpan.Zero,
					solvingList: null,
					additional: "The puzzle does not have a unique solution (multiple solutions or no solution).",
					stepGrids: null);
			}
		}
	}
}
