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
using Sudoku.Solving.Manual.Uniqueness;
using static System.Reflection.BindingFlags;

namespace Sudoku.Solving.Manual
{
	partial class ManualSolver
	{
		/// <summary>
		/// Solve the puzzle with <see cref="AnalyzeDifficultyStrictly"/> option.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="cloneation">(<see langword="ref"/> parameter) The cloneation grid to calculate.</param>
		/// <param name="steps">All steps found.</param>
		/// <param name="solution">(<see langword="in"/> parameter) The solution.</param>
		/// <param name="sukaku">Indicates whether the current mode is sukaku mode.</param>
		/// <param name="progressResult">
		/// (<see langword="ref"/> parameter)
		/// The progress result. This parameter is used for modify the state of UI controls.
		/// The current argument won't be used until <paramref name="progress"/> isn't <see langword="null"/>.
		/// In the default case, this parameter is
		/// <see langword="default"/>(<see cref="GridProgressResult"/>) is okay.
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
		private AnalysisResult SolveSeMode(
			in SudokuGrid grid, ref SudokuGrid cloneation, IList<TechniqueInfo> steps, in SudokuGrid solution,
			bool sukaku, ref GridProgressResult progressResult, IProgress<IProgressResult>? progress)
		{
			var searchers =
#if I_CANT_DECIDE_WHETHER_THE_FIELD_SHOULD_BE_REMOVED__IF_THE_FIELD_IS_REMOVED__THIS_METHOD_WILL_BE_CHANGED_SYNCHRONIZEDLY
				this.GetHodokuModeSearchers(solution);
#else
				this.GetSeModeSearchers(solution);
#endif

			var stepGrids = new List<SudokuGrid>();
			var bag = new List<TechniqueInfo>();
			var stopwatch = new Stopwatch();
			stopwatch.Start();

		Restart:
			TechniqueSearcher.InitializeMaps(cloneation);
			for (int i = 0, length = searchers.Length; i < length; i++)
			{
				var searcherListGroup = searchers[i];
				foreach (var searcher in searcherListGroup)
				{
					if ((sukaku, searcher) is (true, UniquenessTechniqueSearcher))
					{
						// Sukaku mode can't use them.
						// In fact, sukaku can use uniqueness tests, however the program should
						// produce a large modification.
						continue;
					}

					var props = g(searcher);
					if (props is { IsEnabled: false, DisabledReason: not DisabledReason.HighAllocation })
					{
						continue;
					}

					if (EnableGarbageCollectionForcedly
						&& props.DisabledReason.Flags(DisabledReason.HighAllocation))
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
					decimal minDiff = bag.Min(static info => info.Difficulty);
					var selection = from info in bag where info.Difficulty == minDiff select info;
					if (selection.None())
					{
						continue;
					}

					unsafe
					{
						if (!CheckConclusionValidityAfterSearched || selection.All(&InternalChecking, solution))
						{
							foreach (var step in selection)
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

							if (progress is not null)
							{
								ReportProgress(cloneation, progress, ref progressResult);
							}

							goto Restart;
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
				}
				else
				{
					TechniqueInfo? step;
					unsafe
					{
						step = bag.GetElementByMinSelector<TechniqueInfo, decimal>(&InternalSelector);
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

							if (progress is not null)
							{
								ReportProgress(cloneation, progress, ref progressResult);
							}

							goto Restart;
						}
					}
					else
					{
						throw new WrongHandlingException(grid, $"The specified step is wrong: {step}.");
					}
				}
			}

			// All solver can't finish the puzzle...
			// :(
			stopwatch.Stop();
			return new(
				SolverName,
				grid,
				null,
				false,
				stopwatch.Elapsed,
				steps!.AsReadOnlyList()!,
				stepGrids);

			static TechniqueProperties g(TechniqueSearcher searcher) =>
				(TechniqueProperties)searcher.GetType().GetProperty("Properties", Public | Static)!.GetValue(null)!;
		}
	}
}
