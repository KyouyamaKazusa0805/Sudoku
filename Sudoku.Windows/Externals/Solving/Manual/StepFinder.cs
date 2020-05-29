using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sudoku.Data;
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
		public async Task<IEnumerable<IGrouping<string, TechniqueInfo>>> SearchAsync(IReadOnlyGrid grid)
		{
			if (grid.HasSolved || !grid.IsValid(out _, out bool? sukaku))
			{
				return Array.Empty<IGrouping<string, TechniqueInfo>>();
			}

			var searchers = new TechniqueSearcher[]
			{
				new SingleTechniqueSearcher(
					_settings.MainManualSolver.EnableFullHouse, _settings.MainManualSolver.EnableLastDigit),
				new LcTechniqueSearcher(),
				new SubsetTechniqueSearcher(),
				new NormalFishTechniqueSearcher(),
				new RegularWingTechniqueSearcher(_settings.MainManualSolver.CheckRegularWingSize),
				new IrregularWingTechniqueSearcher(),
				new TwoStrongLinksTechniqueSearcher(),
				new UrTechniqueSearcher(
					_settings.MainManualSolver.CheckUncompletedUniquenessPatterns,
					_settings.MainManualSolver.SearchExtendedUniqueRectangles),
				new XrTechniqueSearcher(),
				new UlTechniqueSearcher(),
				new EmptyRectangleTechniqueSearcher(),
				new AlcTechniqueSearcher(_settings.MainManualSolver.CheckAlmostLockedQuadruple),
				new SdcTechniqueSearcher(
					_settings.MainManualSolver.AllowOverlappingAlses,
					_settings.MainManualSolver.AlsHighlightRegionInsteadOfCell,
					_settings.MainManualSolver.AllowAlsCycles),
				new BdpTechniqueSearcher(),
				new BugTechniqueSearcher(_settings.MainManualSolver.UseExtendedBugSearcher),
				new ErIntersectionPairTechniqueSearcher(),
				new AlsXzTechniqueSearcher(
					_settings.MainManualSolver.AllowOverlappingAlses,
					_settings.MainManualSolver.AlsHighlightRegionInsteadOfCell,
					_settings.MainManualSolver.AllowAlsCycles),
				new AlsXyWingTechniqueSearcher(
					_settings.MainManualSolver.AllowOverlappingAlses,
					_settings.MainManualSolver.AlsHighlightRegionInsteadOfCell,
					_settings.MainManualSolver.AllowAlsCycles),
				new AlsWWingTechniqueSearcher(
					_settings.MainManualSolver.AllowOverlappingAlses,
					_settings.MainManualSolver.AlsHighlightRegionInsteadOfCell,
					_settings.MainManualSolver.AllowAlsCycles),
				new DeathBlossomTechniqueSearcher(
					_settings.MainManualSolver.AllowOverlappingAlses,
					_settings.MainManualSolver.AlsHighlightRegionInsteadOfCell,
					_settings.MainManualSolver.MaxPetalsOfDeathBlossom),
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
				new BowmanBingoTechniqueSearcher(_settings.MainManualSolver.BowmanBingoMaximumLength),
				new PomTechniqueSearcher(),
				new CccTechniqueSearcher(),
				new JuniorExocetTechniqueSearcher(_settings.MainManualSolver.CheckAdvancedInExocet),
				new SeniorExocetTechniqueSearcher(_settings.MainManualSolver.CheckAdvancedInExocet),
				new SkLoopTechniqueSearcher(),
				new AlsNetTechniqueSearcher(),
			};

			TechniqueSearcher.InitializeMaps(grid);
			var bag = new Bag<TechniqueInfo>();
			foreach (var searcher in searchers)
			{
				if (sukaku is true && searcher is UniquenessTechniqueSearcher)
				{
					// Sukaku mode cannot use them.
					// In fact, sukaku can use uniqueness tests, however the program should
					// produce a large modification.
					continue;
				}

				await Task.Run(() => searcher.GetAll(bag, grid));
			}

			return await Task.Run(() => from step in bag group step by step.Name);
		}
	}
}
