using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Models;
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
using TechniquesGroupedByName = System.Linq.IGrouping<string, Sudoku.Solving.TechniqueInfo>;

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
		private readonly Settings _settings;


		/// <summary>
		/// Initializes an instance with the specified settings.
		/// </summary>
		/// <param name="settings">The settings.</param>
		public StepFinder(Settings settings) => _settings = settings;


		/// <summary>
		/// Search for all possible steps in a grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="progress">The progress.</param>
		/// <param name="globalizationString">The globalization string.</param>
		public IEnumerable<TechniquesGroupedByName> Search(
			Grid grid, IProgress<IProgressResult>? progress, string? globalizationString)
		{
			if (grid.HasSolved || !grid.IsValid(out _, out bool? sukaku))
			{
				return Array.Empty<TechniquesGroupedByName>();
			}

			var solver = _settings.MainManualSolver;
			var searchers = new TechniqueSearcher[]
			{
				new SingleTechniqueSearcher(solver.EnableFullHouse, solver.EnableLastDigit),
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
				new ErIntersectionPairTechniqueSearcher(),
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
					_settings.MainManualSolver.HobiwanFishMaximumSize,
					_settings.MainManualSolver.HobiwanFishMaximumExofinsCount,
					_settings.MainManualSolver.HobiwanFishMaximumEndofinsCount,
					_settings.MainManualSolver.HobiwanFishCheckTemplates),
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
			var progressResult = new TechniqueProgressResult(searchers.Length, globalizationString ?? "en-us");
			foreach (var searcher in searchers)
			{
				if (!searcher.SearcherProperties!.IsEnabled)
				{
					continue;
				}

				if ((sukaku, searcher) is (true, UniquenessTechniqueSearcher))
				{
					// Sukaku mode cannot use them.
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
			return from Step in bag.Distinct() group Step by Step.Name;
		}
	}
}
