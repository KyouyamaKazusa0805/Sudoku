using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.ComponentModel;
using Sudoku.Data;
using Sudoku.Extensions;
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
using Sudoku.Solving.Manual.Uniqueness.Rects;
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
		public IEnumerable<IGrouping<string, TechniqueInfo>> Search(
			IReadOnlyGrid grid, IProgress<IProgressResult>? progress)
		{
			if (grid.HasSolved || !grid.IsValid(out _, out bool? sukaku))
			{
				return Array.Empty<IGrouping<string, TechniqueInfo>>();
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
				new UrTechniqueSearcher(solver.CheckIncompletedUniquenessPatterns, solver.SearchExtendedUniqueRectangles),
				new XrTechniqueSearcher(),
				new UlTechniqueSearcher(),
				new EmptyRectangleTechniqueSearcher(),
				new AlcTechniqueSearcher(solver.CheckAlmostLockedQuadruple),
				new SdcTechniqueSearcher(
					solver.AllowOverlappingAlses, solver.AlsHighlightRegionInsteadOfCell, solver.AllowAlsCycles),
				new BdpTechniqueSearcher(),
				new BugTechniqueSearcher(solver.UseExtendedBugSearcher),
				new ErIntersectionPairTechniqueSearcher(),
				new AlsXzTechniqueSearcher(
					solver.AllowOverlappingAlses, solver.AlsHighlightRegionInsteadOfCell, solver.AllowAlsCycles),
				new AlsXyWingTechniqueSearcher(
					solver.AllowOverlappingAlses, solver.AlsHighlightRegionInsteadOfCell, solver.AllowAlsCycles),
				new AlsWWingTechniqueSearcher(
					solver.AllowOverlappingAlses, solver.AlsHighlightRegionInsteadOfCell, solver.AllowAlsCycles),
				new DeathBlossomTechniqueSearcher(
					solver.AllowOverlappingAlses, solver.AlsHighlightRegionInsteadOfCell, solver.MaxPetalsOfDeathBlossom),
				//new GroupedAicTechniqueSearcher(
				//	true, false, false, _settings.MainManualSolver.AicMaximumLength,
				//	_settings.MainManualSolver.ReductDifferentPathAic,
				//	_settings.MainManualSolver.OnlySaveShortestPathAic,
				//	_settings.MainManualSolver.CheckHeadCollision,
				//	_settings.MainManualSolver.CheckContinuousNiceLoop),
				//new GroupedAicTechniqueSearcher(
				//	false, true, false, _settings.MainManualSolver.AicMaximumLength,
				//	_settings.MainManualSolver.ReductDifferentPathAic,
				//	_settings.MainManualSolver.OnlySaveShortestPathAic,
				//	_settings.MainManualSolver.CheckHeadCollision,
				//	_settings.MainManualSolver.CheckContinuousNiceLoop),
				//new GroupedAicTechniqueSearcher(
				//	false, false, true, _settings.MainManualSolver.AicMaximumLength,
				//	_settings.MainManualSolver.ReductDifferentPathAic,
				//	_settings.MainManualSolver.OnlySaveShortestPathAic,
				//	_settings.MainManualSolver.CheckHeadCollision,
				//	_settings.MainManualSolver.CheckContinuousNiceLoop),
				//new HobiwanFishTechniqueSearcher(
				//	_settings.MainManualSolver.HobiwanFishMaximumSize,
				//	_settings.MainManualSolver.HobiwanFishMaximumExofinsCount,
				//	_settings.MainManualSolver.HobiwanFishMaximumEndofinsCount,
				//	_settings.MainManualSolver.HobiwanFishCheckTemplates),
				new BowmanBingoTechniqueSearcher(solver.BowmanBingoMaximumLength),
				new PomTechniqueSearcher(),
				new CccTechniqueSearcher(),
				new JuniorExocetTechniqueSearcher(solver.CheckAdvancedInExocet),
				new SeniorExocetTechniqueSearcher(solver.CheckAdvancedInExocet),
				new SkLoopTechniqueSearcher(),
				new AlsNetTechniqueSearcher(),
			};

			TechniqueSearcher.InitializeMaps(grid);
			var bag = new Bag<TechniqueInfo>();
			var progressResult = new TechniqueProgressResult(searchers.Length);
			foreach (var searcher in searchers)
			{
				if (sukaku is true && searcher is UniquenessTechniqueSearcher)
				{
					// Sukaku mode cannot use them.
					// In fact, sukaku can use uniqueness tests, however the program should
					// produce a large modification.
					continue;
				}

				if (!(progress is null))
				{
					_ = searcher.HasMarked<TechniqueSearcher, TechniqueDisplayAttribute>(out var attributes);
					progressResult.CurrentTechnique = attributes.First().DisplayName;
					progress.Report(progressResult);
				}

				searcher.GetAll(bag, grid);
			}

			// Group them up.
			if (!(progress is null))
			{
				progressResult.CurrentTechnique = "Summary...";
				progress.Report(progressResult);
			}

			// Return the result.
			return from step in bag group step by step.Name;
		}
	}
}
