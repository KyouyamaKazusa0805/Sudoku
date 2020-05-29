using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.Data;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
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
using Sudoku.Solving.Manual.Symmetry;
using Sudoku.Solving.Manual.Uniqueness;
using Sudoku.Solving.Manual.Uniqueness.Bugs;
using Sudoku.Solving.Manual.Uniqueness.Extended;
using Sudoku.Solving.Manual.Uniqueness.Loops;
using Sudoku.Solving.Manual.Uniqueness.Polygons;
using Sudoku.Solving.Manual.Uniqueness.Rects;
using Sudoku.Solving.Manual.Wings.Irregular;
using Sudoku.Solving.Manual.Wings.Regular;

namespace Sudoku.Solving.Manual
{
	partial class ManualSolver
	{
		/// <summary>
		/// Solve naively.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="cloneation">The cloneation grid to calculate.</param>
		/// <param name="steps">All steps found.</param>
		/// <param name="solution">The solution.</param>
		/// <param name="sukaku">Indicates whether the current mode is sukaku.</param>
		/// <returns>The analysis result.</returns>
		/// <exception cref="WrongHandlingException">
		/// Throws when the solver cannot solved due to wrong handling.
		/// </exception>
		private AnalysisResult SolveNaively(
			IReadOnlyGrid grid, Grid cloneation, List<TechniqueInfo> steps, IReadOnlyGrid solution, bool sukaku)
		{
			// Check symmetry first.
			if (!sukaku && CheckGurthSymmetricalPlacement)
			{
				var symmetrySearcher = new GspTechniqueSearcher();
				var tempStep = symmetrySearcher.GetOne(cloneation);
				if (!(tempStep is null))
				{
					if (CheckConclusionsValidity(solution, tempStep.Conclusions))
					{
						tempStep.ApplyTo(cloneation);
						steps.Add(tempStep);
						goto Label_Searching;
					}
					else
					{
						throw new WrongHandlingException(grid);
					}
				}
			}

		Label_Searching:
			// Start searching.
			var searchers = new TechniqueSearcher[]
			{
				new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit),
				new LcTechniqueSearcher(),
				new SubsetTechniqueSearcher(),
				new NormalFishTechniqueSearcher(),
				new RegularWingTechniqueSearcher(CheckRegularWingSize),
				new IrregularWingTechniqueSearcher(),
				new TwoStrongLinksTechniqueSearcher(),
				new UrTechniqueSearcher(CheckIncompletedUniquenessPatterns, SearchExtendedUniqueRectangles),
				new XrTechniqueSearcher(),
				new UlTechniqueSearcher(),
				new EmptyRectangleTechniqueSearcher(),
				new AlcTechniqueSearcher(CheckAlmostLockedQuadruple),
				new SdcTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				new BdpTechniqueSearcher(),
				new BugTechniqueSearcher(UseExtendedBugSearcher),
				new ErIntersectionPairTechniqueSearcher(),
				new AlsXzTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				new AlsXyWingTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				new AlsWWingTechniqueSearcher(AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, AllowAlsCycles),
				new DeathBlossomTechniqueSearcher(
					AllowOverlappingAlses, AlsHighlightRegionInsteadOfCell, MaxPetalsOfDeathBlossom),
				new GroupedAicTechniqueSearcher(
					true, false, false, AicMaximumLength, ReductDifferentPathAic,
					OnlySaveShortestPathAic, CheckHeadCollision, CheckContinuousNiceLoop),
				new GroupedAicTechniqueSearcher(
					false, true, false, AicMaximumLength, ReductDifferentPathAic,
					OnlySaveShortestPathAic, CheckHeadCollision, CheckContinuousNiceLoop),
				new GroupedAicTechniqueSearcher(
					false, false, true, AicMaximumLength, ReductDifferentPathAic,
					OnlySaveShortestPathAic, CheckHeadCollision, CheckContinuousNiceLoop),
				new HobiwanFishTechniqueSearcher(
					HobiwanFishMaximumSize, HobiwanFishMaximumExofinsCount,
					HobiwanFishMaximumEndofinsCount, HobiwanFishCheckTemplates),
				new JuniorExocetTechniqueSearcher(CheckAdvancedInExocet),
				new SeniorExocetTechniqueSearcher(CheckAdvancedInExocet),
				new SkLoopTechniqueSearcher(),
				new PomTechniqueSearcher(),
				new BowmanBingoTechniqueSearcher(BowmanBingoMaximumLength),
				new TemplateTechniqueSearcher(OnlyRecordTemplateDelete),
				new CccTechniqueSearcher(),
				new AlsNetTechniqueSearcher(),
				new BruteForceTechniqueSearcher(solution),
			};

			static T g<T>(TechniqueSearcher s, string p) => (T)s.GetType().GetProperty(p)!.GetValue(null)!;
			if (UseCalculationPriority)
			{
				Array.Sort(searchers, (a, b) => g<int>(a, "Priority").CompareTo(g<int>(b, "Priority")));
			}

			var stepGrids = new Bag<IReadOnlyGrid>();
			var bag = new Bag<TechniqueInfo>();
			var stopwatch = new Stopwatch();
			stopwatch.Start();

		Label_Restart:
			TechniqueSearcher.InitializeMaps(cloneation);
			for (int i = 0, length = searchers.Length; i < length; i++)
			{
				var searcher = searchers[i];

				if (sukaku is true && searcher is UniquenessTechniqueSearcher || !g<bool>(searcher, "IsEnabled"))
				{
					continue;
				}

				searcher.GetAll(bag, cloneation);
				if (bag.None())
				{
					continue;
				}

				if (FastSearch)
				{
					if (!CheckConclusionValidityAfterSearched
						|| bag.All(info => CheckConclusionsValidity(solution, info.Conclusions)))
					{
						foreach (var step in bag)
						{
							if (RecordTechnique(steps, step, grid, cloneation, stopwatch, stepGrids, out var result))
							{
								stopwatch.Stop();
								return result;
							}
						}

						// The puzzle has not been finished,
						// we should turn to the first step finder
						// to continue solving puzzle.
						bag.Clear();
						if (EnableGarbageCollectionForcedly
							&& searcher.HasMarked<TechniqueSearcher, HighAllocationAttribute>(out _))
						{
							GC.Collect();
						}

						goto Label_Restart;
					}
					else
					{
						TechniqueInfo? wrongStep = null;
						foreach (var step in bag)
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
				else
				{
					var step = OptimizedApplyingOrder
						? bag.GetElementByMinSelector(info => info.Difficulty)
						: bag.FirstOrDefault();
					if (step is null)
					{
						// If current step cannot find any steps,
						// we will turn to the next step finder to
						// continue solving puzzle.
						continue;
					}

					if (!CheckConclusionValidityAfterSearched || CheckConclusionsValidity(solution, step.Conclusions))
					{
						if (RecordTechnique(steps, step, grid, cloneation, stopwatch, stepGrids, out var result))
						{
							stopwatch.Stop();
							return result;
						}
						else
						{
							// The puzzle has not been finished,
							// we should turn to the first step finder
							// to continue solving puzzle.
							bag.Clear();
							if (EnableGarbageCollectionForcedly
								&& searcher.HasMarked<TechniqueSearcher, HighAllocationAttribute>(out _))
							{
								GC.Collect();
							}

							goto Label_Restart;
						}
					}
					else
					{
						throw new WrongHandlingException(grid, $"The specified step is wrong: {step}.");
					}
				}
			}

			// All solver cannot finish the puzzle...
			// :(
			stopwatch.Stop();
			return new AnalysisResult(
				puzzle: grid,
				solverName: SolverName,
				hasSolved: false,
				solution: null,
				elapsedTime: stopwatch.Elapsed,
				solvingList: steps,
				additional: null,
				stepGrids);
		}
	}
}
