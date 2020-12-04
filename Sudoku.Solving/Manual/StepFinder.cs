using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Globalization;
using Sudoku.Models;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Manual.Uniqueness;
using Sudoku.Windows;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides a step finder.
	/// </summary>
	public sealed class StepFinder
	{
		/// <summary>
		/// Indicates the inner solver.
		/// </summary>
		private readonly ManualSolver _solver;


		/// <summary>
		/// Initializes an instance with the specified solver.
		/// </summary>
		/// <param name="solver">The solver.</param>
		/// <remarks>
		/// This solver will provide the settings of the searching operation.
		/// </remarks>
		public StepFinder(ManualSolver solver) => _solver = solver;


		/// <summary>
		/// Search for all possible steps in a grid.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="progress">The progress.</param>
		/// <param name="countryCode">The country code.</param>
		public IEnumerable<IGrouping<string, TechniqueInfo>> Search(
			in SudokuGrid grid, IProgress<IProgressResult>? progress, CountryCode countryCode)
		{
			if (grid.HasSolved || !grid.IsValid(out bool? sukaku))
			{
				return Array.Empty<IGrouping<string, TechniqueInfo>>();
			}

			bool onlyShowSameLevelTechniquesInFindAllSteps = _solver.OnlyShowSameLevelTechniquesInFindAllSteps;

			// Note that the parameter is unnecessary to pass.
			var searchers = _solver.GetHodokuModeSearchers();

			TechniqueSearcher.InitializeMaps(grid);
			int i = -1;
			var bag = new List<TechniqueInfo>();
			var progressResult = new TechniqueProgressResult(
				searchers.Length, countryCode == CountryCode.Default ? CountryCode.EnUs : countryCode);
			foreach (var searcher in searchers)
			{
				// Check whether the searcher is only used for analyzing a sudoku grid.
				// If so, the searcher will be disabled here.
				var (isEnabled, _, _, disabledReason, onlyEnableInAnalysis, level, displayLabel) =
					TechniqueProperties.GetPropertiesFrom(searcher)!;

				if (onlyEnableInAnalysis)
				{
					continue;
				}

				// Skip the searcher that is disabled.
				if (!isEnabled && disabledReason != DisabledReason.HighAllocation)
				{
					continue;
				}

				// Check the searcher.
				// Sukaku mode can't use them.
				// In fact, sukaku can use uniqueness tests, but this will make the project
				// a large modification.
				if ((sukaku, searcher) is (true, UniquenessTechniqueSearcher))
				{
					continue;
				}

				// Check the level of the searcher.
				// If a searcher contains the upper level value than the current searcher holding,
				// the searcher will be skipped to search steps.
				int currentLevel = default;
				if (onlyShowSameLevelTechniquesInFindAllSteps)
				{
					currentLevel = level;
					if (i != -1 && i != currentLevel)
					{
						continue;
					}
				}

				// Update the progress result.
				if (progress is not null)
				{
					progressResult.CurrentTechnique = Resources.GetValue($"Progress{displayLabel}");
					progressResult.CurrentIndex++;
					progress.Report(progressResult);
				}

				// Searching.
				var tempBag = new List<TechniqueInfo>();
				searcher.GetAll(tempBag, grid);

				// Gather the technique steps, and record the current level of the searcher.
				if (tempBag.Count != 0)
				{
					if (onlyShowSameLevelTechniquesInFindAllSteps)
					{
						i = currentLevel;
					}

					bag.AddRange(tempBag);
				}
			}

			// Group them up.
			if (progress is not null)
			{
				progressResult.CurrentTechnique = Resources.GetValue("Summary");
				progressResult.CurrentIndex++;
				progress.Report(progressResult);
			}

			// Return the result.
			return from step in Enumerable.Distinct(bag) group step by step.Name;
		}
	}
}
