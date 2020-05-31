using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sudoku.ComponentModel;
using Sudoku.Data;
using Sudoku.Data.Extensions;
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
		/// <param name="grid">The grid.</param>
		/// <param name="cloneation">The cloneation (playground).</param>
		/// <param name="stopwatch">The stopwatch.</param>
		/// <param name="stepGrids">The step grids.</param>
		/// <param name="result">(<see langword="out"/> parameter) The analysis result.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <seealso cref="SolveNaively(IReadOnlyGrid, Grid, List{TechniqueInfo}, IReadOnlyGrid, bool, ref GridProgressResult, IProgress{GridProgressResult}?)"/>
		/// <seealso cref="SolveWithStrictDifficultyRating(IReadOnlyGrid, Grid, List{TechniqueInfo}, IReadOnlyGrid, bool, ref GridProgressResult, IProgress{GridProgressResult}?)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool RecordTechnique(
			List<TechniqueInfo> steps, TechniqueInfo step, IReadOnlyGrid grid,
			Grid cloneation, Stopwatch stopwatch, IBag<IReadOnlyGrid> stepGrids,
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

						goto Label_Determine;
					}
				}
			}

		Label_Determine:
			if (needAdd)
			{
				stepGrids.Add(cloneation.Clone());
				step.ApplyTo(cloneation);
				steps.Add(step);

				if (cloneation.HasSolved)
				{
					result = new AnalysisResult(
						puzzle: grid,
						solverName: SolverName,
						hasSolved: true,
						solution: cloneation,
						elapsedTime: stopwatch.Elapsed,
						solvingList: steps,
						additional: null,
						stepGrids);
					return true;
				}
			}

			result = null;
			return false;
		}


		/// <summary>
		/// To check the validity of all conclusions.
		/// </summary>
		/// <param name="solution">The solution.</param>
		/// <param name="conclusions">The conclusions.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		private static bool CheckConclusionsValidity(IReadOnlyGrid solution, IEnumerable<Conclusion> conclusions)
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
		/// <param name="cloneation">The cloneation grid.</param>
		/// <param name="progress">The progress reporter.</param>
		/// <param name="progressResult">(<see langword="ref"/> parameter) The progress result.</param>
		private static void ReportProgress(
			IReadOnlyGrid cloneation, IProgress<GridProgressResult> progress, ref GridProgressResult progressResult)
		{
			progressResult.CurrentCandidatesCount = cloneation.CandidatesCount;
			progressResult.CurrentCellsCount = cloneation.EmptyCellsCount;
			progress.Report(progressResult);
		}
	}
}
