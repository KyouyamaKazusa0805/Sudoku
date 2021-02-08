using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Sudoku.Data;
using Sudoku.Models;
using Sudoku.Solving.Manual.Extensions;
using Sudoku.Solving.Manual.Symmetry;
using Sudoku.Solving.Manual.Uniqueness;
using static Sudoku.Solving.Manual.FastProperties;
// ReSharper disable All

namespace Sudoku.Solving.Manual
{
	partial class ManualSolver
	{
		/// <summary>
		/// Solve naively.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="cloneation">(<see langword="ref"/> parameter) The cloneation grid to calculate.</param>
		/// <param name="steps">All steps found.</param>
		/// <param name="solution">(<see langword="in"/> parameter) The solution.</param>
		/// <param name="sukaku">Indicates whether the current mode is sukaku.</param>
		/// <param name="progressResult">
		/// (<see langword="ref"/> parameter)
		/// The progress result. This parameter is used for modify the state of UI controls.
		/// The current argument won't be used until <paramref name="progress"/> isn't <see langword="null"/>.
		/// In the default case, this parameter being
		/// <see langword="default"/>(<see cref="GridProgressResult"/>) is okay.
		/// </param>
		/// <param name="progress">
		/// The progress used for report the current state. If we don't need, the value should
		/// be assigned <see langword="null"/>.
		/// </param>
		/// <param name="cancellationToken">The cancellation token that is used to cancel the operation.</param>
		/// <returns>The analysis result.</returns>
		/// <exception cref="SudokuHandlingException">
		/// Throws when the solver can't solved due to wrong handling.
		/// </exception>
		/// <seealso cref="GridProgressResult"/>
		private unsafe AnalysisResult SolveNaively(
			in SudokuGrid grid, ref SudokuGrid cloneation, List<StepInfo> steps, in SudokuGrid solution,
			bool sukaku, ref GridProgressResult progressResult, IProgress<IProgressResult>? progress,
			CancellationToken? cancellationToken)
		{
			// Check symmetry first.
			var stepGrids = new List<SudokuGrid>();
			if (!sukaku && CheckGurthSymmetricalPlacement)
			{
				var symmetrySearcher =
#if I_WANT_TO_DISABLE_THIS__BECAUSE_OF_TOO_SLOW
					new Gsp2StepSearcher();
#else
					new GspStepSearcher();
#endif
				if (symmetrySearcher.GetOne(cloneation) is { } tempStep)
				{
					if (CheckConclusionsValidity(solution, tempStep.Conclusions))
					{
						stepGrids.Add(cloneation);
						tempStep.ApplyTo(ref cloneation);
						steps.Add(tempStep);

						if (progress is not null)
						{
							ReportProgress(cloneation, progress, ref progressResult);
						}
					}
					else
					{
						throw new SudokuHandlingException<SudokuGrid, StepInfo>(
							errorCode: 201,
							grid,
							tempStep);
					}
				}
			}

		// Start searching.
		var searchers = this.GetHodokuModeSearchers(solution);
			if (UseCalculationPriority)
			{
				searchers.Sort(&cmp);
			}

			var bag = new List<StepInfo>();
			var stopwatch = new Stopwatch();
			stopwatch.Start();

		Restart:
			InitializeMaps(cloneation);
			for (int i = 0, length = searchers.Length; i < length; i++)
			{
				var searcher = searchers[i];
				if (sukaku && searcher is UniquenessStepSearcher)
				{
					continue;
				}

				var (
					isEnabled, _, _, _, _, onlyEnabledInFastMode, _, _
				) = TechniqueProperties.FromSearcher(searcher)!;
				if (!isEnabled)
				{
					continue;
				}

				searcher.GetAll(bag, cloneation);
				if (bag.Count == 0)
				{
					continue;
				}

				if (FastSearch)
				{
					bool allConclusionsAreValid = true;
					foreach (var element in bag)
					{
						if (!CheckConclusionsValidity(solution, element.Conclusions))
						{
							allConclusionsAreValid = false;
							break;
						}
					}

					if (!CheckConclusionValidityAfterSearched || allConclusionsAreValid)
					{
						foreach (var step in bag)
						{
							if (
								RecordStep(
									steps, step, grid, ref cloneation, stopwatch, stepGrids, out var result,
									cancellationToken))
							{
								stopwatch.Stop();
								return result;
							}
						}

						// The puzzle has not been finished,
						// we should turn to the first step finder
						// to continue solving puzzle.
						bag.Clear();
						if (EnableGarbageCollectionForcedly)
						{
							GC.Collect();
						}

						if (progress is not null)
						{
							ReportProgress(cloneation, progress, ref progressResult);
						}

						goto Restart;
					}
					else
					{
						var solutionCopied = solution;
						throw new SudokuHandlingException<SudokuGrid, StepInfo>(
							errorCode: 201,
							grid,
							bag.First(first));

						bool first(StepInfo step) => !CheckConclusionsValidity(solutionCopied, step.Conclusions);
					}
				}
				else if (!onlyEnabledInFastMode)
				{
					// If the searcher is only used in the fast mode, just skip it.

					var step =
					(
						OptimizedApplyingOrder
						? from info in bag orderby info.Difficulty select info
						: bag.AsEnumerable()
					).FirstOrDefault();

					if (step is null)
					{
						// If current step can't find any steps,
						// we will turn to the next step finder to
						// continue solving puzzle.
						continue;
					}

					if (!CheckConclusionValidityAfterSearched
						|| CheckConclusionsValidity(solution, step.Conclusions))
					{
						if (
							RecordStep(
								steps, step, grid, ref cloneation, stopwatch, stepGrids, out var result,
								cancellationToken))
						{
							stopwatch.Stop();
							return result;
						}

						// The puzzle has not been finished,
						// we should turn to the first step finder
						// to continue solving puzzle.
						bag.Clear();
						if (EnableGarbageCollectionForcedly)
						{
							GC.Collect();
						}

						if (progress is not null)
						{
							ReportProgress(cloneation, progress, ref progressResult);
						}

						goto Restart;
					}

					throw new SudokuHandlingException<SudokuGrid, StepInfo>(errorCode: 201, grid, step);
				}
			}

			// All solver can't finish the puzzle...
			// :(
			stopwatch.Stop();
			return new(SolverName, grid, false, stopwatch.Elapsed)
			{
				Steps = steps,
				StepGrids = stepGrids,
			};

			static int cmp(in StepSearcher a, in StepSearcher b) =>
				TechniqueProperties.FromSearcher(a)!.Priority - TechniqueProperties.FromSearcher(b)!.Priority;
		}
	}
}
