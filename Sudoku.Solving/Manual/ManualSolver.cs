using System;
using System.Collections.Generic;
using Sudoku.ComponentModel;
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


		/// <summary>
		/// Indicates the list that used as a cache.
		/// </summary>
		private static List<TechniqueInfo> TempList => new List<TechniqueInfo>();


		/// <inheritdoc/>
		public override AnalysisResult Solve(IReadOnlyGrid grid) => Solve(grid, null);

		/// <summary>
		/// To solve the puzzle.
		/// </summary>
		/// <param name="grid">The puzzle.</param>
		/// <param name="progress">The progress instance to report the state.</param>
		/// <returns>The analysis result.</returns>
		public AnalysisResult Solve(IReadOnlyGrid grid, IProgress<IProgressResult>? progress)
		{
			if (grid.IsValid(out var solution, out bool? sukaku))
			{
				// Solve the puzzle.
				int emptyCellsCount = grid.EmptyCellsCount;
				int candsCount = grid.CandidatesCount;
				try
				{
					GridProgressResult defaultValue = default;
					var progressResult = new GridProgressResult(candsCount, emptyCellsCount, candsCount);
					ref var paramProgressResult = ref progress is null ? ref defaultValue : ref progressResult;

					progress?.Report(progressResult);

					return AnalyzeDifficultyStrictly
						? SolveWithStrictDifficultyRating(
							grid, grid.Clone(), TempList, solution, sukaku.Value,
							ref paramProgressResult, progress)
						: SolveNaively(
							grid, grid.Clone(), TempList, solution, sukaku.Value,
							ref paramProgressResult, progress);
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
