using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sudoku.Data;
using Sudoku.Globalization;
using Sudoku.Models;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Manual.Alses;
using Sudoku.Solving.Manual.Alses.Basic;
using Sudoku.Solving.Manual.Alses.Mslses;
using Sudoku.Solving.Manual.Chaining;
using Sudoku.Solving.Manual.Exocets;
using Sudoku.Solving.Manual.Fishes;
using Sudoku.Solving.Manual.Intersections;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Solving.Manual.Sdps;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Solving.Manual.Subsets;
using Sudoku.Solving.Manual.Uniqueness;
using Sudoku.Solving.Manual.Uniqueness.Bugs;
using Sudoku.Solving.Manual.Uniqueness.Extended;
using Sudoku.Solving.Manual.Uniqueness.Loops;
using Sudoku.Solving.Manual.Uniqueness.Polygons;
using Sudoku.Solving.Manual.Uniqueness.Qiu;
using Sudoku.Solving.Manual.Uniqueness.Rects;
using Sudoku.Solving.Manual.Uniqueness.Square;
using Sudoku.Solving.Manual.Wings.Irregular;
using Sudoku.Solving.Manual.Wings.Regular;
using Sudoku.Windows;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides a step finder.
	/// </summary>
	public sealed class StepFinder
	{
		/// <summary>
		/// The settings used.
		/// </summary>
		private readonly WindowsSettings _settings;


		/// <summary>
		/// Initializes an instance with the specified settings.
		/// </summary>
		/// <param name="settings">The settings.</param>
		public StepFinder(WindowsSettings settings) => _settings = settings;


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

			var solver = _settings.MainManualSolver;
			var searchers = new TechniqueSearcher[]
			{
				new SingleTechniqueSearcher(solver.EnableFullHouse, solver.EnableLastDigit, solver.ShowDirectLines),
				new LcTechniqueSearcher(),
				new SubsetTechniqueSearcher(),
				new NormalFishTechniqueSearcher(),
				new RegularWingTechniqueSearcher(solver.CheckRegularWingSize),
				new IrregularWingTechniqueSearcher(),
				new TwoStrongLinksTechniqueSearcher(),
				new UrTechniqueSearcher(solver.CheckIncompleteUniquenessPatterns, solver.SearchExtendedUniqueRectangles),
				new XrTechniqueSearcher(),
				new UlTechniqueSearcher(),
				new EmptyRectangleTechniqueSearcher(),
				new AlcTechniqueSearcher(solver.CheckAlmostLockedQuadruple),
				new SdcTechniqueSearcher(
					solver.AllowOverlappingAlses, solver.AlsHighlightRegionInsteadOfCell, solver.AllowAlsCycles),
				new Sdc3dTechniqueSearcher(
					solver.AllowOverlappingAlses, solver.AlsHighlightRegionInsteadOfCell, solver.AllowAlsCycles),
				new BdpTechniqueSearcher(),
				new QdpTechniqueSearcher(),
				new UsTechniqueSearcher(),
				new GuardianTechniqueSearcher(),
				new BugTechniqueSearcher(solver.UseExtendedBugSearcher),
				new EripTechniqueSearcher(),
				new AicTechniqueSearcher(),
				new AlsXzTechniqueSearcher(
					solver.AllowOverlappingAlses, solver.AlsHighlightRegionInsteadOfCell, solver.AllowAlsCycles),
				new AlsXyWingTechniqueSearcher(
					solver.AllowOverlappingAlses, solver.AlsHighlightRegionInsteadOfCell, solver.AllowAlsCycles),
				new AlsWWingTechniqueSearcher(
					solver.AllowOverlappingAlses, solver.AlsHighlightRegionInsteadOfCell, solver.AllowAlsCycles),
				new DeathBlossomTechniqueSearcher(
					solver.AllowOverlappingAlses, solver.AlsHighlightRegionInsteadOfCell, solver.MaxPetalsOfDeathBlossom),
				new HobiwanFishTechniqueSearcher(
					solver.HobiwanFishMaximumSize, solver.HobiwanFishMaximumExofinsCount,
					solver.HobiwanFishMaximumEndofinsCount, solver.HobiwanFishCheckTemplates),
				new FcTechniqueSearcher(),
				new BowmanBingoTechniqueSearcher(solver.BowmanBingoMaximumLength),
				new PomTechniqueSearcher(),
				new JuniorExocetTechniqueSearcher(solver.CheckAdvancedInExocet),
				new SeniorExocetTechniqueSearcher(solver.CheckAdvancedInExocet),
				new SkLoopTechniqueSearcher(),
				new AlsNetTechniqueSearcher(),
			};

			TechniqueSearcher.InitializeMaps(grid);
			var bag = new List<TechniqueInfo>();
			var progressResult = new TechniqueProgressResult(
				searchers.Length, countryCode == CountryCode.Default ? CountryCode.EnUs : countryCode);
			foreach (var searcher in searchers)
			{
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
