using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual;
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
using Sudoku.Solving.Manual.Uniqueness.Bugs;
using Sudoku.Solving.Manual.Uniqueness.Extended;
using Sudoku.Solving.Manual.Uniqueness.Loops;
using Sudoku.Solving.Manual.Uniqueness.Polygons;
using Sudoku.Solving.Manual.Uniqueness.Qiu;
using Sudoku.Solving.Manual.Uniqueness.Rects;
using Sudoku.Solving.Manual.Uniqueness.Square;
using Sudoku.Solving.Manual.Wings.Irregular;
using Sudoku.Solving.Manual.Wings.Regular;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ManualSolver"/>.
	/// </summary>
	/// <seealso cref="ManualSolver"/>
	public static class ManualSolverEx
	{
		/// <summary>
		/// Get the searchers to enumerate on Sudoku Explainer mode.
		/// </summary>
		/// <param name="this">(<see langword="this"/> paraemeter) The manual solver.</param>
		/// <param name="solution">
		/// (<see langword="in"/> paraemeter) The solution for a sudoku grid.
		/// This parameter is necessary because some technique searchers will use this value,
		/// such as <see cref="BruteForceTechniqueSearcher"/>.
		/// </param>
		/// <returns>The result.</returns>
		public static TechniqueSearcher[][] GetSeModeSearchers(
			this ManualSolver @this, in SudokuGrid? solution = null)
		{
			var list = @this.GetHodokuModeSearchers(solution);
			var dic = new Dictionary<int, IList<TechniqueSearcher>>();
			foreach (var searcher in list)
			{
				int level = TechniqueProperties.GetPropertiesFrom(searcher)!.DisplayLevel;
				if (dic.TryGetValue(level, out var l))
				{
					l.Add(searcher);
				}
				else
				{
					dic.Add(level, new List<TechniqueSearcher> { searcher });
				}
			}

			return dic.ToArray<int, IList<TechniqueSearcher>, TechniqueSearcher>();
		}

		/// <summary>
		/// Get the searchers to enumerate on Hodoku mode.
		/// </summary>
		/// <param name="this">(<see langword="this"/> paraemeter) The manual solver.</param>
		/// <param name="solution">
		/// (<see langword="in"/> paraemeter) The solution for a sudoku grid.
		/// This parameter is necessary because some technique searchers will use this value,
		/// such as <see cref="BruteForceTechniqueSearcher"/>. The default value is <see langword="null"/>.
		/// </param>
		/// <returns>The result.</returns>
		public static TechniqueSearcher[] GetHodokuModeSearchers(
			this ManualSolver @this, in SudokuGrid? solution = null)
		{
			var result = new List<TechniqueSearcher>
			{
				new SingleTechniqueSearcher(@this.EnableFullHouse, @this.EnableLastDigit, @this.ShowDirectLines),
				new LcTechniqueSearcher(),
				new SubsetTechniqueSearcher(),
				new NormalFishTechniqueSearcher(),
				new RegularWingTechniqueSearcher(@this.CheckRegularWingSize),
				new IrregularWingTechniqueSearcher(),
				new TwoStrongLinksTechniqueSearcher(),
				new UrTechniqueSearcher(@this.CheckIncompleteUniquenessPatterns, @this.SearchExtendedUniqueRectangles),
				new XrTechniqueSearcher(),
				new UlTechniqueSearcher(),
				new EmptyRectangleTechniqueSearcher(),
				new AlcTechniqueSearcher(@this.CheckAlmostLockedQuadruple),
				new SdcTechniqueSearcher(@this.AllowOverlappingAlses, @this.AlsHighlightRegionInsteadOfCell, @this.AllowAlsCycles),
				new BdpTechniqueSearcher(),
				new QdpTechniqueSearcher(),
				new UsTechniqueSearcher(),
				new GuardianTechniqueSearcher(),
				new BugTechniqueSearcher(@this.UseExtendedBugSearcher),
				new EripTechniqueSearcher(),
				new AicTechniqueSearcher(),
				new AlsXzTechniqueSearcher(@this.AllowOverlappingAlses, @this.AlsHighlightRegionInsteadOfCell, @this.AllowAlsCycles),
				new AlsXyWingTechniqueSearcher(@this.AllowOverlappingAlses, @this.AlsHighlightRegionInsteadOfCell, @this.AllowAlsCycles),
				new AlsWWingTechniqueSearcher(@this.AllowOverlappingAlses, @this.AlsHighlightRegionInsteadOfCell, @this.AllowAlsCycles),
				new DeathBlossomTechniqueSearcher(@this.AllowOverlappingAlses, @this.AlsHighlightRegionInsteadOfCell, @this.MaxPetalsOfDeathBlossom),
				new FcTechniqueSearcher(nishio: true, multiple: false, dynamic: true),
				new FcTechniqueSearcher(nishio: false, multiple: true, dynamic: false),
				new FcTechniqueSearcher(nishio: false, multiple: true, dynamic: true),
				new BugMultipleWithFcTechniqueSearcher(),
				new HobiwanFishTechniqueSearcher(@this.HobiwanFishMaximumSize, @this.HobiwanFishMaximumExofinsCount, @this.HobiwanFishMaximumEndofinsCount, @this.HobiwanFishCheckTemplates),
				new JeTechniqueSearcher(@this.CheckAdvancedInExocet),
				new SeTechniqueSearcher(@this.CheckAdvancedInExocet),
				new SkLoopTechniqueSearcher(),
				new AlsNetTechniqueSearcher(),
				new PomTechniqueSearcher(),
				new BowmanBingoTechniqueSearcher(@this.BowmanBingoMaximumLength),
				new TemplateTechniqueSearcher(@this.OnlyRecordTemplateDelete)
			};

			if (solution.HasValue)
			{
				result.Add(new BruteForceTechniqueSearcher(solution.Value));
			}

			return result.ToArray();
		}
	}
}
