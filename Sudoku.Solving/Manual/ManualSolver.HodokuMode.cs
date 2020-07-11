using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.ComponentModel;
using Sudoku.Data;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual.Symmetry;
using Sudoku.Solving.Manual.Uniqueness;

namespace Sudoku.Solving.Manual
{
	partial class ManualSolver
	{
		/// <summary>
		/// Solve naively.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="cloneation">The cloneation grid to calculate.</param>
		/// <param name="steps">All steps found.</param>
		/// <param name="solution">The solution.</param>
		/// <param name="sukaku">Indicates whether the current mode is sukaku.</param>
		/// <param name="progressResult">
		/// (<see langword="ref"/> parameter)
		/// The progress result. This parameter is used for modify the state of UI controls.
		/// The current argument will not be used until <paramref name="progress"/> isn't <see langword="null"/>.
		/// In the default case, this parameter is <see langword="default"/>(<see cref="GridProgressResult"/>) is okay.
		/// </param>
		/// <param name="progress">
		/// The progress used for report the current state. If we don't need, the value should
		/// be assigned <see langword="null"/>.
		/// </param>
		/// <returns>The analysis result.</returns>
		/// <exception cref="WrongHandlingException">
		/// Throws when the solver cannot solved due to wrong handling.
		/// </exception>
		/// <seealso cref="GridProgressResult"/>
		private AnalysisResult SolveNaively(
			IReadOnlyGrid grid, Grid cloneation, List<TechniqueInfo> steps, IReadOnlyGrid solution, bool sukaku,
			ref GridProgressResult progressResult, IProgress<IProgressResult>? progress)
		{
			// Check symmetry first.
			if (!sukaku && CheckGurthSymmetricalPlacement)
			{
				var symmetrySearcher = new GspTechniqueSearcher();
				var tempStep = symmetrySearcher.GetOne(cloneation);
				if (!(tempStep is null))
				{
					if (CheckConclusionsValidity(solution, tempStep.Conclusions))
					{
						tempStep.ApplyTo(cloneation);
						steps.Add(tempStep);

						if (!(progress is null))
						{
							ReportProgress(cloneation, progress, ref progressResult);
						}

						goto Label_Searching;
					}
					else
					{
						throw new WrongHandlingException(grid);
					}
				}
			}

		Label_Searching:
			// Start searching.
			var searchers = GetSearchersHodokuMode(solution);

			if (UseCalculationPriority)
			{
				searchers.Sort((a, b) => a.SearcherProperties!.Priority.CompareTo(b.SearcherProperties!.Priority));
			}

			var stepGrids = new Bag<IReadOnlyGrid>();
			var bag = new Bag<TechniqueInfo>();
			var stopwatch = new Stopwatch();
			stopwatch.Start();

		Label_Restart:
			TechniqueSearcher.InitializeMaps(cloneation);
			for (int i = 0, length = searchers.Length; i < length; i++)
			{
				var searcher = searchers[i];
				if (sukaku && searcher is UniquenessTechniqueSearcher)
				{
					continue;
				}

				var searcherAttribute = searcher.SearcherProperties!;
				if (!searcherAttribute.IsEnabled)
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
					if (!CheckConclusionValidityAfterSearched
						|| bag.All(info => CheckConclusionsValidity(solution, info.Conclusions)))
					{
						foreach (var step in bag)
						{
							if (RecordTechnique(steps, step, grid, cloneation, stopwatch, stepGrids, out var result))
							{
								stopwatch.Stop();
								return result;
							}
						}

						// The puzzle has not been finished,
						// we should turn to the first step finder
						// to continue solving puzzle.
						bag.Clear();
						if (EnableGarbageCollectionForcedly && searcher.GetType().HasMarked<HighAllocationAttribute>())
						{
							GC.Collect();
						}

						if (!(progress is null))
						{
							ReportProgress(cloneation, progress, ref progressResult);
						}

						goto Label_Restart;
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

						throw new WrongHandlingException(grid, $"The specified step is wrong: {wrongStep}.");
					}
				}
				else
				{
					var step = OptimizedApplyingOrder
						? bag.GetElementByMinSelector(info => info.Difficulty)
						: bag.FirstOrDefault();
					if (step is null)
					{
						// If current step cannot find any steps,
						// we will turn to the next step finder to
						// continue solving puzzle.
						continue;
					}

					if (!CheckConclusionValidityAfterSearched || CheckConclusionsValidity(solution, step.Conclusions))
					{
						if (RecordTechnique(steps, step, grid, cloneation, stopwatch, stepGrids, out var result))
						{
							stopwatch.Stop();
							return result;
						}
						else
						{
							// The puzzle has not been finished,
							// we should turn to the first step finder
							// to continue solving puzzle.
							bag.Clear();
							if (EnableGarbageCollectionForcedly
								&& searcher.GetType().HasMarked<HighAllocationAttribute>())
							{
								GC.Collect();
							}

							if (!(progress is null))
							{
								ReportProgress(cloneation, progress, ref progressResult);
							}

							goto Label_Restart;
						}
					}
					else
					{
						throw new WrongHandlingException(grid, $"The specified step is wrong: {step}.");
					}
				}
			}

			// All solver cannot finish the puzzle...
			// :(
			stopwatch.Stop();
			return new AnalysisResult(
				puzzle: grid,
				solverName: SolverName,
				hasSolved: false,
				solution: null,
				elapsedTime: stopwatch.Elapsed,
				solvingList: steps,
				additional: null,
				stepGrids);
		}
	}
}
