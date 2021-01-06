using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sudoku.Data;
using Sudoku.Globalization;
using Sudoku.Models;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Manual.Uniqueness;
using Sudoku.Windows;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides a step searcher that is different with <see cref="StepSearcher"/>.
	/// </summary>
	/// <remarks>
	/// All-step searcher (<see cref="AllStepSearcher"/>) is a searcher that find all steps in a given grid,
	/// which can be a initial grid or a intermediate grid. Different with <see cref="ManualSolver"/>,
	/// this searcher only calculates for the grid in an only status.
	/// In addition, different with <see cref="StepSearcher"/>, this searcher searches all
	/// possible solving techniques.
	/// </remarks>
	/// <seealso cref="StepSearcher"/>
	/// <seealso cref="ManualSolver"/>
	public sealed class AllStepSearcher
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
		public AllStepSearcher(ManualSolver solver) => _solver = solver;


		/// <summary>
		/// Search for all possible steps in a grid.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="progress">The progress.</param>
		/// <param name="countryCode">The country code.</param>
		/// <param name="cancellationToken">The cancellation token used for cancelling an operation.</param>
		/// <returns>The result grouped by technique names.</returns>
		/// <exception cref="OperationCanceledException">Throws when the operation is cancelled.</exception>
		public IEnumerable<IGrouping<string, StepInfo>> Search(
			in SudokuGrid grid, IProgress<IProgressResult>? progress, CountryCode countryCode,
			CancellationToken? cancellationToken)
		{
			if (grid.IsSolved || !grid.IsValid(out bool? sukaku))
			{
				return Array.Empty<IGrouping<string, StepInfo>>();
			}

			bool onlyShowSameLevelTechniquesInFindAllSteps = _solver.OnlyShowSameLevelTechniquesInFindAllSteps;

			// Note that the parameter is unnecessary to pass.
			var searchers = _solver.GetHodokuModeSearchers(null);

			FastProperties.InitializeMaps(grid);
			int i = -1;
			var bag = new List<StepInfo>();
			var progressResult =
				new TechniqueProgressResult(
					searchers.Length,
					countryCode == CountryCode.Default ? CountryCode.EnUs : countryCode
				);
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
				if ((sukaku, searcher) is (true, UniquenessStepSearcher))
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

				if (cancellationToken is { IsCancellationRequested: true })
				{
					throw new OperationCanceledException();
				}

				// Update the progress result.
				if (progress is not null)
				{
					progressResult.CurrentTechnique = Resources.GetValue($"Progress{displayLabel}");
					progressResult.CurrentIndex++;
					progress.Report(progressResult);
				}

				// Searching.
				var tempBag = new List<StepInfo>();
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
			return from step in bag group step by step.Name;
		}


		/// <summary>
		/// Search for all possible steps in a grid.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="progress">The progress.</param>
		/// <param name="countryCode">The country code.</param>
		/// <param name="cancellationToken">The cancellation token used for cancelling an operation.</param>
		/// <returns>The task of that searching.</returns>
		/// <exception cref="OperationCanceledException">Throws when the operation is cancelled.</exception>
		public async Task<IEnumerable<IGrouping<string, StepInfo>>?> SearchAsync(
			SudokuGrid grid, IProgress<IProgressResult>? progress, CountryCode countryCode,
			CancellationToken? cancellationToken)
		{
			return await (cancellationToken is { } t ? Task.Run(innerSearch, t) : Task.Run(innerSearch));

			IEnumerable<IGrouping<string, StepInfo>>? innerSearch()
			{
				try
				{
					return Search(grid, progress, countryCode, cancellationToken);
				}
				catch (OperationCanceledException)
				{
					return null;
				}
			}
		}
	}
}
