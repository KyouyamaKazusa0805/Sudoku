using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Sudoku.Data;
using Sudoku.Models;
using Sudoku.Runtime;
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
			Grid grid, Grid cloneation, List<TechniqueInfo> steps, Grid solution, bool sukaku,
			ref GridProgressResult progressResult, IProgress<IProgressResult>? progress)
		{
			// Check symmetry first.
			var stepGrids = new List<Grid>();
			if (!sukaku && CheckGurthSymmetricalPlacement)
			{
				var symmetrySearcher = new GspTechniqueSearcher();
				var tempStep = symmetrySearcher.GetOne(cloneation);
				if (tempStep is not null)
				{
					if (CheckConclusionsValidity(solution, tempStep.Conclusions))
					{
						stepGrids.Add(cloneation.Clone());
						tempStep.ApplyTo(cloneation);
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
			var searchers = GetSearchersHodokuMode(solution);

			if (UseCalculationPriority)
			{
				Array.Sort(
					searchers,
					/*static*/ (a, b) => a.SearcherProperties!.Priority.CompareTo(b.SearcherProperties!.Priority));
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

				if (!searcher.SearcherProperties!.IsEnabled)
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
						if (EnableGarbageCollectionForcedly
							&& searcher.GetType().GetCustomAttribute<HighAllocationAttribute>() is not null)
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

						// The puzzle has not been finished,
						// we should turn to the first step finder
						// to continue solving puzzle.
						bag.Clear();
						if (EnableGarbageCollectionForcedly
							&& searcher.GetType().GetCustomAttribute<HighAllocationAttribute>() is not null)
						{
							GC.Collect();
						}

						if (progress is not null)
						{
							ReportProgress(cloneation, progress, ref progressResult);
						}

						goto Restart;
					}

					throw new WrongHandlingException(grid, $"The specified step is wrong: {step}.");
				}
			}

			// All solver cannot finish the puzzle...
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
