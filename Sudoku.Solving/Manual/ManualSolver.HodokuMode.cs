using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.Data;
using Sudoku.Extensions;
using Sudoku.Models;
using Sudoku.Runtime;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Manual.Symmetry;
using Sudoku.Solving.Manual.Uniqueness;

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
		/// In the default case, this parameter is <see langword="default"/>(<see cref="GridProgressResult"/>) is okay.
		/// </param>
		/// <param name="progress">
		/// The progress used for report the current state. If we don't need, the value should
		/// be assigned <see langword="null"/>.
		/// </param>
		/// <returns>The analysis result.</returns>
		/// <exception cref="WrongHandlingException">
		/// Throws when the solver can't solved due to wrong handling.
		/// </exception>
		/// <seealso cref="GridProgressResult"/>
		private AnalysisResult SolveNaively(
			in SudokuGrid grid, ref SudokuGrid cloneation, List<TechniqueInfo> steps, in SudokuGrid solution,
			bool sukaku, ref GridProgressResult progressResult, IProgress<IProgressResult>? progress)
		{
			// Check symmetry first.
			var stepGrids = new List<SudokuGrid>();
			if (!sukaku && CheckGurthSymmetricalPlacement)
			{
				var symmetrySearcher = new GspTechniqueSearcher();
				var tempStep = symmetrySearcher.GetOne(cloneation);
				if (tempStep is not null)
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
						throw new WrongHandlingException(grid);
					}
				}
			}

		Searching:
			// Start searching.
			var searchers = this.GetHodokuModeSearchers(solution);

			if (UseCalculationPriority)
			{
				static int cmp(in TechniqueSearcher a, in TechniqueSearcher b)
				{
					int l = TechniqueProperties.GetPropertiesFrom(a)!.Priority;
					int r = TechniqueProperties.GetPropertiesFrom(b)!.Priority;
					return l > r ? 1 : l < r ? -1 : 0;
				}

				unsafe
				{
					searchers.Sort(&cmp);
				}
			}

			var bag = new List<TechniqueInfo>();
			var stopwatch = new Stopwatch();
			stopwatch.Start();

		Restart:
			TechniqueSearcher.InitializeMaps(cloneation);
			for (int i = 0, length = searchers.Length; i < length; i++)
			{
				var searcher = searchers[i];
				if ((sukaku, searcher) is (true, UniquenessTechniqueSearcher))
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
					unsafe
					{
						if (!CheckConclusionValidityAfterSearched || bag.All(&InternalChecking, solution))
						{
							foreach (var step in bag)
							{
								if (
									RecordTechnique(
										steps, step, grid, ref cloneation, stopwatch, stepGrids, out var result))
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
							TechniqueInfo? wrongStep = null;
							foreach (var step in bag)
							{
								if (!CheckConclusionsValidity(solution, step.Conclusions))
								{
									wrongStep = step;
									break;
								}
							}

							string stepGridStr = cloneation.ToString("#");
							throw new WrongHandlingException(
								grid,
								$"The specified step is wrong: {wrongStep}, {Environment.NewLine}" +
								$"the current grid: {stepGridStr}.");
						}
					}
				}
				else
				{
					TechniqueInfo? step;
					unsafe
					{
						step = OptimizedApplyingOrder
						? bag.GetElementByMinSelector<TechniqueInfo, decimal>(&InternalSelector)
						: bag.FirstOrDefault();
					}

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
							RecordTechnique(
								steps, step, grid, ref cloneation, stopwatch, stepGrids, out var result))
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

					string stepGridStr = cloneation.ToString("#");
					throw new WrongHandlingException(
						grid,
						$"The specified step is wrong: {step}, {Environment.NewLine}" +
						$"the current grid: {stepGridStr}.");
				}
			}

			// All solver can't finish the puzzle...
			// :(
			stopwatch.Stop();
			return new(
				solverName: SolverName,
				puzzle: grid,
				solution: null,
				hasSolved: false,
				elapsedTime: stopwatch.Elapsed,
				steps,
				stepGrids);
		}
	}
}
