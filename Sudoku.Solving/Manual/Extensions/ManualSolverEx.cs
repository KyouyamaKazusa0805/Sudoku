using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Solving.Manual.Alses;
using Sudoku.Solving.Manual.Alses.Basic;
using Sudoku.Solving.Manual.Alses.Mslses;
using Sudoku.Solving.Manual.Chaining;
using Sudoku.Solving.Manual.Exocets;
using Sudoku.Solving.Manual.Fishes;
using Sudoku.Solving.Manual.Intersections;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Solving.Manual.RankTheory;
using Sudoku.Solving.Manual.Sdps;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Solving.Manual.Subsets;
using Sudoku.Solving.Manual.Uniqueness.Bugs;
using Sudoku.Solving.Manual.Uniqueness.Extended;
using Sudoku.Solving.Manual.Uniqueness.Loops;
using Sudoku.Solving.Manual.Uniqueness.Polygons;
using Sudoku.Solving.Manual.Uniqueness.Qiu;
using Sudoku.Solving.Manual.Uniqueness.Rects;
using Sudoku.Solving.Manual.Uniqueness.Reversal;
using Sudoku.Solving.Manual.Uniqueness.Square;
using Sudoku.Solving.Manual.Wings.Irregular;
using Sudoku.Solving.Manual.Wings.Regular;

namespace Sudoku.Solving.Manual.Extensions
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
		/// such as <see cref="BfStepSearcher"/>.
		/// </param>
		/// <returns>The result.</returns>
		public static StepSearcher[][] GetSeModeSearchers(this ManualSolver @this, in SudokuGrid? solution)
		{
			var list = @this.GetHodokuModeSearchers(solution);
			var dic = new Dictionary<int, IList<StepSearcher>>();
			foreach (var searcher in list)
			{
				int level = TechniqueProperties.GetPropertiesFrom(searcher)!.DisplayLevel;
				if (dic.TryGetValue(level, out var l))
				{
					l.Add(searcher);
				}
				else
				{
					dic.Add(level, new List<StepSearcher> { searcher });
				}
			}

			return dic.ToArray<int, IList<StepSearcher>, StepSearcher>();
		}

		/// <summary>
		/// Get the searchers to enumerate on Hodoku mode.
		/// </summary>
		/// <param name="this">(<see langword="this"/> paraemeter) The manual solver.</param>
		/// <param name="solution">
		/// (<see langword="in"/> paraemeter) The solution for a sudoku grid.
		/// This parameter is necessary because some technique searchers will use this value,
		/// such as <see cref="BfStepSearcher"/>. The default value is <see langword="null"/>.
		/// </param>
		/// <returns>The result.</returns>
		public static StepSearcher[] GetHodokuModeSearchers(this ManualSolver @this, in SudokuGrid? solution)
		{
			var result = new List<StepSearcher>
			{
				new SingleStepSearcher(@this.EnableFullHouse, @this.EnableLastDigit, @this.ShowDirectLines),
				new LcStepSearcher(),
				new SubsetStepSearcher(),
				new NormalFishStepSearcher(),
				new RegularWingStepSearcher(@this.CheckRegularWingSize),
				new IrregularWingStepSearcher(),
				new TwoStrongLinksStepSearcher(),
				new UrStepSearcher(@this.CheckIncompleteUniquenessPatterns, @this.SearchExtendedUniqueRectangles),
				new XrStepSearcher(),
				new UlStepSearcher(),
				new ErStepSearcher(),
				new AlcStepSearcher(@this.CheckAlmostLockedQuadruple),
				new SdcStepSearcher(),
				new Sdc3dStepSearcher(),
				new BdpStepSearcher(),
				new ReverseBugStepSearcher(),
				new QdpStepSearcher(),
				new UsStepSearcher(),
				new GuardianStepSearcher(),
				new BugStepSearcher(@this.UseExtendedBugSearcher),
				new EripStepSearcher(),
				new BivalueOddagonStepSearcher(),
				new AicStepSearcher(),
				new AlsXzStepSearcher(@this.AllowOverlappingAlses, @this.AlsHighlightRegionInsteadOfCell, @this.AllowAlsCycles),
				new AlsXyWingStepSearcher(@this.AllowOverlappingAlses, @this.AlsHighlightRegionInsteadOfCell, @this.AllowAlsCycles),
				new AlsWWingStepSearcher(@this.AllowOverlappingAlses, @this.AlsHighlightRegionInsteadOfCell, @this.AllowAlsCycles),
				//new DeathBlossomStepSearcher(@this.AllowOverlappingAlses, @this.AlsHighlightRegionInsteadOfCell, @this.MaxPetalsOfDeathBlossom),
				new ComplexFishStepSearcher(),
				new FcStepSearcher(nishio: true, multiple: false, dynamic: true),
				new FcStepSearcher(nishio: false, multiple: true, dynamic: false),
				new BugMultipleWithFcStepSearcher(),
				new FcStepSearcher(nishio: false, multiple: true, dynamic: true),
				new JeStepSearcher(@this.CheckAdvancedInExocet),
				new SeStepSearcher(@this.CheckAdvancedInExocet),
				new SkLoopStepSearcher(),
				new AlsNetStepSearcher(),
				new PomStepSearcher(),
				new BowmanBingoStepSearcher(@this.BowmanBingoMaximumLength),
				new TemplateStepSearcher(@this.OnlyRecordTemplateDelete),
#if DOUBLE_LAYERED_ASSUMPTION
				new FcPlusTechniqueSearcher(level: 1),
				new FcPlusTechniqueSearcher(level: 2),
				new FcPlusTechniqueSearcher(level: 3),
				new FcPlusTechniqueSearcher(level: 4),
				new FcPlusTechniqueSearcher(level: 5),
#endif
			};

			if (solution.HasValue)
			{
				result.Add(new BfStepSearcher(solution.Value));
			}

			return result.ToArray();
		}
	}
}
