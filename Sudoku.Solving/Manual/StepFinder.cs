using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

			// Note that the parameter is unnecessary to pass.
			var searchers = _solver.GetHodokuModeSearchers();

			TechniqueSearcher.InitializeMaps(grid);
			var bag = new List<TechniqueInfo>();
			var progressResult = new TechniqueProgressResult(
				searchers.Length, countryCode == CountryCode.Default ? CountryCode.EnUs : countryCode);
			foreach (var searcher in searchers)
			{
				if (searcher.GetType().GetCustomAttribute<OnlyEnableInAnalysisAttribute>() is not null)
				{
					// This searcher is only used in analysis. Therefore, the searcher is disabled here.
					continue;
				}

				var props = g(searcher);
				if (props is { IsEnabled: false, DisabledReason: not DisabledReason.HighAllocation })
				{
					continue;
				}

				if ((sukaku, searcher) is (true, UniquenessTechniqueSearcher))
				{
					// Sukaku mode can't use them.
					// In fact, sukaku can use uniqueness tests, but this will make the project
					// a large modification.
					continue;
				}

				if (progress is not null)
				{
					progressResult.CurrentTechnique = Resources.GetValue($"Progress{searcher.DisplayName}");
					progressResult.CurrentIndex++;
					progress.Report(progressResult);
				}

				searcher.GetAll(bag, grid);
			}

			// Group them up.
			if (progress is not null)
			{
				progressResult.CurrentTechnique = Resources.GetValue("Summary");
				progressResult.CurrentIndex++;
				progress.Report(progressResult);
			}

			// Return the result.
			return from step in bag.Distinct() group step by step.Name;

			static TechniqueProperties g(TechniqueSearcher searcher) =>
				(TechniqueProperties)
					searcher
					.GetType()
					.GetProperty("Properties", BindingFlags.Public | BindingFlags.Static)!
					.GetValue(null)!;
		}
	}
}
