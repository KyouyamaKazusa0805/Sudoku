using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sudoku.Data;
using Sudoku.Models;
using Sudoku.Runtime;
using Sudoku.Solving.Checking;
using Sudoku.Windows;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides a solver that use logical methods to solve a specified sudoku puzzle.
	/// </summary>
	[Serializable]
	public sealed partial class ManualSolver : Solver
	{
		/// <inheritdoc/>
		public override string SolverName => Resources.GetValue("Manual");


		/// <summary>
		/// Indicates the list that used as a cache.
		/// </summary>
		private static List<TechniqueInfo> TempList => new();


		/// <inheritdoc/>
		public override AnalysisResult Solve(in SudokuGrid grid) => Solve(grid, null);

		/// <summary>
		/// To solve the specified puzzle in asynchronous way.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="progress">The progress.</param>
		/// <param name="globalizationString">The globalization string.</param>
		/// <returns>The task of the execution.</returns>
		public async Task<AnalysisResult> SolveAsync(
			SudokuGrid grid, IProgress<IProgressResult>? progress, string? globalizationString = null) =>
			await Task.Run(() => Solve(grid, progress, globalizationString));

		/// <summary>
		/// To solve the puzzle.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The puzzle.</param>
		/// <param name="progress">The progress instance to report the state.</param>
		/// <param name="globalizationString">
		/// The globalization string. The default value is <see langword="null"/>.
		/// </param>
		/// <returns>The analysis result.</returns>
		/*skiplocalsinit*/
		public AnalysisResult Solve(
			in SudokuGrid grid, IProgress<IProgressResult>? progress, string? globalizationString = null)
		{
			if (grid.IsValid(out var solution, out bool? sukaku))
			{
				// Solve the puzzle.
				int emptyCellsCount = grid.EmptiesCount;
				int candsCount = grid.CandidatesCount;
				try
				{
#pragma warning disable CS0612
					GridProgressResult defaultValue = default;
					var defaultPr = new GridProgressResult(candsCount, emptyCellsCount, candsCount, globalizationString);
					ref var pr = ref progress is null ? ref defaultValue : ref defaultPr;
					progress?.Report(defaultPr);

					var copied = grid;
					return AnalyzeDifficultyStrictly
						? SolveSeMode(grid, ref copied, TempList, solution, sukaku.Value, ref pr, progress)
						: SolveNaively(grid, ref copied, TempList, solution, sukaku.Value, ref pr, progress);
#pragma warning restore CS0612
				}
				catch (WrongHandlingException ex)
				{
					return new(
						solverName: SolverName,
						puzzle: grid,
						solution: null,
						hasSolved: false,
						elapsedTime: TimeSpan.Zero,
						additional: ex.Message);
				}
			}
			else
			{
				return new(
					solverName: SolverName,
					puzzle: grid,
					solution: null,
					hasSolved: false,
					elapsedTime: TimeSpan.Zero,
					additional: "The puzzle doesn't have a unique solution (multiple solutions or no solution).");
			}
		}
	}
}
