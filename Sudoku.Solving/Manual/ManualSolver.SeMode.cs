using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.ComponentModel;
using Sudoku.Data;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual.Uniqueness;

namespace Sudoku.Solving.Manual
{
	partial class ManualSolver
	{
		/// <summary>
		/// Solve the puzzle with <see cref="AnalyzeDifficultyStrictly"/> option.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="cloneation">The cloneation grid to calculate.</param>
		/// <param name="steps">All steps found.</param>
		/// <param name="solution">The solution.</param>
		/// <param name="sukaku">Indicates whether the current mode is sukaku mode.</param>
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
		private AnalysisResult SolveWithStrictDifficultyRating(
			IReadOnlyGrid grid, Grid cloneation, List<TechniqueInfo> steps, IReadOnlyGrid solution, bool sukaku,
			ref GridProgressResult progressResult, IProgress<IProgressResult>? progress)
		{
			var searchers = GetSearchersSeMode(solution);
			var stepGrids = new Bag<IReadOnlyGrid>();
			var bag = new Bag<TechniqueInfo>();
			var stopwatch = new Stopwatch();
			stopwatch.Start();

		Label_Restart:
			TechniqueSearcher.InitializeMaps(cloneation);
			for (int i = 0, length = searchers.Length; i < length; i++)
			{
				var searcherListGroup = searchers[i];
				foreach (var searcher in searcherListGroup)
				{
					if (sukaku is true && searcher is UniquenessTechniqueSearcher)
					{
						// Sukaku mode cannot use them.
						// In fact, sukaku can use uniqueness tests, however the program should
						// produce a large modification.
						continue;
					}

					if (!(bool)searcher.GetType().GetProperty("IsEnabled")!.GetValue(null)!)
					{
						// Skip the technique when the static property 'IsEnabled' is set false.
						continue;
					}

					if (EnableGarbageCollectionForcedly
						&& searcher.HasMarked<TechniqueSearcher, HighAllocationAttribute>(out _))
					{
						GC.Collect();
					}

					searcher.GetAll(bag, cloneation);
				}
				if (bag.Count == 0)
				{
					continue;
				}

				if (FastSearch)
				{
					decimal minDiff = bag.Min(info => info.Difficulty);
					var selection = from info in bag where info.Difficulty == minDiff select info;
					if (selection.None())
					{
						continue;
					}

					if (!CheckConclusionValidityAfterSearched
						|| selection.All(info => CheckConclusionsValidity(solution, info.Conclusions)))
					{
						foreach (var step in selection)
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

						if (!(progress is null))
						{
							ReportProgress(cloneation, progress, ref progressResult);
						}

						goto Label_Restart;
					}
					else
					{
						TechniqueInfo? wrongStep = null;
						foreach (var step in selection)
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
					var step = bag.GetElementByMinSelector(info => info.Difficulty);
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
							// The puzzle has been solved.
							// :)
							stopwatch.Stop();
							return result;
						}
						else
						{
							// The puzzle has not been finished,
							// we should turn to the first step finder
							// to continue solving puzzle.
							bag.Clear();

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
