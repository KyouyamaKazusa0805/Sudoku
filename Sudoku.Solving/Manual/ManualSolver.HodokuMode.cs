using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Extensions;
using System.Linq;
using System.Threading;
using Sudoku.Data;
using Sudoku.Models;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Manual.Symmetry;
using Sudoku.Solving.Manual.Uniqueness;
using static Sudoku.Solving.Manual.FastProperties;

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

						goto Searching;
					}
					else
					{
						throw new SudokuHandlingException<SudokuGrid, string>(
							errorCode: 201,
							grid,
							tempStep.ToString());
					}
				}
			}

		Searching:
			// Start searching.
			var searchers = this.GetHodokuModeSearchers(solution);
			if (UseCalculationPriority)
			{
				static int cmp(in StepSearcher a, in StepSearcher b)
				{
					int l = TechniqueProperties.GetPropertiesFrom(a)!.Priority;
					int r = TechniqueProperties.GetPropertiesFrom(b)!.Priority;
					return l > r ? 1 : l < r ? -1 : 0;
				}

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
				if ((sukaku, searcher) is (true, UniquenessStepSearcher))
				{
					continue;
				}

				var props = TechniqueProperties.GetPropertiesFrom(searcher)!;
				if (props is { IsEnabled: false, DisabledReason: not DisabledReason.HighAllocation })
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
					bool allConclusionsAreVaild = true;
					foreach (var element in bag)
					{
						if (!CheckConclusionsValidity(solution, element.Conclusions))
						{
							allConclusionsAreVaild = false;
							break;
						}
					}

					if (!CheckConclusionValidityAfterSearched || allConclusionsAreVaild)
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
						if (EnableGarbageCollectionForcedly
							&& props.DisabledReason.Flags(DisabledReason.HighAllocation))
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
						StepInfo? wrongStep = null;
						foreach (var step in bag)
						{
							if (!CheckConclusionsValidity(solution, step.Conclusions))
							{
								wrongStep = step;
								break;
							}
						}

						throw new SudokuHandlingException<SudokuGrid, string>(
							errorCode: 201,
							grid,
							wrongStep!.ToString());
					}
				}
				else
				{
					var step = OptimizedApplyingOrder
						? (from info in bag orderby info.Difficulty select info).FirstOrDefault()
						: bag.FirstOrDefault();

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
						if (EnableGarbageCollectionForcedly
							&& props.DisabledReason.Flags(DisabledReason.HighAllocation))
						{
							GC.Collect();
						}

						if (progress is not null)
						{
							ReportProgress(cloneation, progress, ref progressResult);
						}

						goto Restart;
					}

					throw new SudokuHandlingException<SudokuGrid, string>(
						errorCode: 201,
						grid,
						step.ToString());
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
		}
	}
}
