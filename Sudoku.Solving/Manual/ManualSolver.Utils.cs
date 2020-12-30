using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Models;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual
{
	partial class ManualSolver
	{
		/// <summary>
		/// Bound with on-solving methods returns the solving state.
		/// </summary>
		/// <param name="steps">The steps.</param>
		/// <param name="step">The step.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="cloneation">(<see langword="ref"/> parameter) The cloneation (playground).</param>
		/// <param name="stopwatch">The stopwatch.</param>
		/// <param name="stepGrids">The step grids.</param>
		/// <param name="result">(<see langword="out"/> parameter) The analysis result.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool RecordStep(
			IList<StepInfo> steps, StepInfo step, in SudokuGrid grid,
			ref SudokuGrid cloneation, Stopwatch stopwatch, IList<SudokuGrid> stepGrids,
			[NotNullWhen(true)] out AnalysisResult? result)
		{
			bool needAdd = false;
			foreach (var (t, c, d) in step.Conclusions)
			{
				switch (t)
				{
					case Assignment when cloneation.GetStatus(c) == CellStatus.Empty:
					case Elimination when cloneation.Exists(c, d) is true:
					{
						needAdd = true;

						goto FinalCheck;
					}
				}
			}

		FinalCheck:
			if (needAdd)
			{
				stepGrids.Add(cloneation);
				step.ApplyTo(ref cloneation);
				steps.Add(step);

				if (cloneation.IsSolved)
				{
					result = new(SolverName, grid, true, stopwatch.Elapsed)
					{
						Steps = steps.AsReadOnlyList(),
						StepGrids = stepGrids.AsReadOnlyList(),
					};
					return true;
				}
			}

			result = null;
			return false;
		}


		/// <summary>
		/// To check the validity of all conclusions.
		/// </summary>
		/// <param name="solution">(<see langword="in"/> parameter) The solution.</param>
		/// <param name="conclusions">The conclusions.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		private static bool CheckConclusionsValidity(in SudokuGrid solution, IEnumerable<Conclusion> conclusions)
		{
			foreach (var (t, c, d) in conclusions)
			{
				int digit = solution[c];
				switch (t)
				{
					case Assignment when digit != d:
					case Elimination when digit == d:
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// To report the progress.
		/// </summary>
		/// <param name="cloneation">(<see langword="in"/> parameter) The cloneation grid.</param>
		/// <param name="progress">The progress reporter.</param>
		/// <param name="progressResult">(<see langword="ref"/> parameter) The progress result.</param>
		private static void ReportProgress(
			in SudokuGrid cloneation, IProgress<IProgressResult> progress, ref GridProgressResult progressResult)
		{
			progressResult.CurrentCandidatesCount = cloneation.CandidatesCount;
			progressResult.CurrentCellsCount = cloneation.EmptiesCount;
			progress.Report(progressResult);
		}
	}
}
