using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Runtime;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Manual.Alses;
using Sudoku.Solving.Manual.Chaining;
using Sudoku.Solving.Manual.Fishes;
using Sudoku.Solving.Manual.Intersections;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Solving.Manual.Sdps;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Solving.Manual.Subsets;
using Sudoku.Solving.Manual.Symmetry;
using Sudoku.Solving.Manual.Uniqueness.Bugs;
using Sudoku.Solving.Manual.Uniqueness.Loops;
using Sudoku.Solving.Manual.Uniqueness.Polygons;
using Sudoku.Solving.Manual.Uniqueness.Rectangles;
using Sudoku.Solving.Manual.Wings.Irregular;
using Sudoku.Solving.Manual.Wings.Regular;
using Intersection = System.ValueTuple<int, int, Sudoku.Data.GridMap, Sudoku.Data.GridMap>;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides a solver that use logical methods to solve a specified sudoku puzzle.
	/// </summary>
	public sealed partial class ManualSolver : Solver
	{
		/// <inheritdoc/>
		public override string SolverName => "Manual";


		/// <inheritdoc/>
		public override AnalysisResult Solve(IReadOnlyGrid grid)
		{
			if (grid.IsValid(out var solution))
			{
				// Get all region maps.
				var regionMaps = new GridMap[27];
				for (int i = 0; i < 27; i++)
				{
					regionMaps[i] = GridMap.CreateInstance(i);
				}

				// Get intersection table in order to run faster in intersection technique searchers.
				var intersectionTable = new Intersection[18, 3];
				for (int i = 0; i < 18; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						int baseSet = i + 9;
						int coverSet = i < 9 ? i / 3 * 3 + j : ((i - 9) / 3 * 3 + j) * 3 % 8;
						intersectionTable[i, j] = (
							baseSet, coverSet, regionMaps[baseSet],
							regionMaps[coverSet]);
					}
				}

				// Solve the puzzle.
				try
				{
					return AnalyzeDifficultyStrictly
						? SolveWithStrictDifficultyRating(
							grid, grid.Clone(), new List<TechniqueInfo>(), solution,
							intersectionTable, regionMaps)
						: SolveNaively(
							grid, grid.Clone(), new List<TechniqueInfo>(), solution,
							intersectionTable, regionMaps);
				}
				catch (WrongHandlingException ex)
				{
					return new AnalysisResult(
						puzzle: grid,
						solverName: SolverName,
						hasSolved: false,
						solution: null,
						elapsedTime: TimeSpan.Zero,
						solvingList: null,
						additional: ex.Message);
				}
			}
			else
			{
				return new AnalysisResult(
					puzzle: grid,
					solverName: SolverName,
					hasSolved: false,
					solution: null,
					elapsedTime: TimeSpan.Zero,
					solvingList: null,
					additional: "The puzzle does not have a unique solution (multiple solutions or no solution).");
			}
		}

		/// <summary>
		/// Solve the puzzle with <see cref="AnalyzeDifficultyStrictly"/> option.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="cloneation">The cloneation grid to calculate.</param>
		/// <param name="steps">All steps found.</param>
		/// <param name="solution">The solution.</param>
		/// <param name="intersection">The intersection table.</param>
		/// <param name="regionMaps">All region maps.</param>
		/// <returns>The analysis result.</returns>
		/// <exception cref="WrongHandlingException">
		/// Throws when the solver cannot solved due to wrong handling.
		/// </exception>
		private AnalysisResult SolveWithStrictDifficultyRating(
			IReadOnlyGrid grid, Grid cloneation, List<TechniqueInfo> steps,
			IReadOnlyGrid solution, Intersection[,] intersection, GridMap[] regionMaps)
		{
			var searchers = new TechniqueSearcher[][]
			{
				new[] { new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit) },
				new[] { new LockedCandidatesTechniqueSearcher(intersection) },
				new TechniqueSearcher[]
				{
					new SubsetTechniqueSearcher(),
					new NormalFishTechniqueSearcher(),
					new RegularWingTechniqueSearcher(CheckRegularWingSize),
					new IrregularWingTechniqueSearcher(),
					new TwoStrongLinksTechniqueSearcher(),
					new UniqueRectangleTechniqueSearcher(CheckIncompletedUniquenessPatterns),
					new ExtendedRectangleTechniqueSearcher(),
					new UniqueLoopTechniqueSearcher(),
					new EmptyRectangleTechniqueSearcher(regionMaps),
					new AlmostLockedCandidatesTechniqueSearcher(intersection, CheckAlmostLockedQuadruple),
					new SueDeCoqTechniqueSearcher(regionMaps),
					new BorescoperDeadlyPatternTechniqueSearcher(),
					new BivalueUniversalGraveTechniqueSearcher(regionMaps, UseExtendedBugSearcher),
					new AlternatingInferenceChainTechniqueSearcher(true, true, AicMaximumLength),
				},
				new TechniqueSearcher[]
				{
					new HobiwanFishTechniqueSearcher(HobiwanFishMaximumSize, HobiwanFishMaximumExofinsCount, HobiwanFishMaximumEndofinsCount, HobiwanFishCheckTemplates, regionMaps),
				},
				new TechniqueSearcher[]
				{
					new BowmanBingoTechniqueSearcher(BowmanBingoMaximumLength),
					new PatternOverlayMethodTechniqueSearcher(),
					new TemplateTechniqueSearcher(OnlyRecordTemplateDelete),
				},
				new TechniqueSearcher[]
				{
					new ChuteClueCoverTechniqueSearcher(),
				},
				new[] { new BruteForceTechniqueSearcher(solution) }
			};

			var bag = new Bag<TechniqueInfo>();
			var stopwatch = new Stopwatch();
			stopwatch.Start();
		Label_StartSolving:
			for (int i = 0, length = searchers.Length; i < length; i++)
			{
				var searcherListGroup = searchers[i];
				foreach (var searcher in searcherListGroup)
				{
					// Skip all searchers marked slow running attribute.
					if (DisableSlowTechniques
						&& searcher.HasMarkedAttribute<SlowAttribute>(false, out _))
					{
						continue;
					}

					if (!EnablePatternOverlayMethod && searcher is PatternOverlayMethodTechniqueSearcher
						|| !EnableTemplate && searcher is TemplateTechniqueSearcher
						|| !EnableBruteForce && searcher is BruteForceTechniqueSearcher
						|| !EnableBowmanBingo && searcher is BowmanBingoTechniqueSearcher)
					{
						continue;
					}

					if (EnableGarbageCollectionForcedly
						&& searcher.HasMarkedAttribute<HighAllocationAttribute>(false, out _))
					{
						GC.Collect();
					}

					searcher.AccumulateAll(bag, cloneation);
				}

				var step = bag.GetElementByMinSelector(info => info.Difficulty);
				bag.Clear();

				if (step is null)
				{
					// If current step cannot find any steps,
					// we will turn to the next step finder to
					// continue solving puzzle.
					continue;
				}

				if (CheckConclusionValidityAfterSearched
					? CheckConclusionsValidity(solution, step.Conclusions)
					: true)
				{
					step.ApplyTo(cloneation);
					steps.Add(step);
					if (cloneation.HasSolved)
					{
						// The puzzle has been solved.
						// :)
						stopwatch.StopAnyway();

						return new AnalysisResult(
							puzzle: grid,
							solverName: SolverName,
							hasSolved: true,
							solution: cloneation,
							elapsedTime: stopwatch.Elapsed,
							solvingList: steps,
							additional: null);
					}
					else
					{
						// The puzzle has not been finished,
						// we should turn to the first step finder
						// to continue solving puzzle.
						goto Label_StartSolving;
					}
				}
				else
				{
					throw new WrongHandlingException(grid);
				}
			}

			// All solver cannot finish the puzzle...
			// :(
			stopwatch.StopAnyway();

			return new AnalysisResult(
				puzzle: grid,
				solverName: SolverName,
				hasSolved: false,
				solution: null,
				elapsedTime: stopwatch.Elapsed,
				solvingList: steps,
				additional: null);
		}

		/// <summary>
		/// Solve naively.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="cloneation">The cloneation grid to calculate.</param>
		/// <param name="steps">All steps found.</param>
		/// <param name="solution">The solution.</param>
		/// <param name="intersection">Intersection table.</param>
		/// <param name="regionMaps">All region maps.</param>
		/// <returns>The analysis result.</returns>
		/// <exception cref="WrongHandlingException">
		/// Throws when the solver cannot solved due to wrong handling.
		/// </exception>
		private AnalysisResult SolveNaively(
			IReadOnlyGrid grid, Grid cloneation, List<TechniqueInfo> steps,
			IReadOnlyGrid solution, Intersection[,] intersection, GridMap[] regionMaps)
		{
			// Check symmetry first.
			var symmetrySearcher = new GurthSymmetricalPlacementTechniqueSearcher();
			var tempStep = symmetrySearcher.TakeOne(cloneation);
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

		Label_Searching:
			// Start searching.
			var searchers = new TechniqueSearcher[]
			{
				new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit),
				new LockedCandidatesTechniqueSearcher(intersection),
				new SubsetTechniqueSearcher(),
				new NormalFishTechniqueSearcher(),
				new RegularWingTechniqueSearcher(CheckRegularWingSize),
				new IrregularWingTechniqueSearcher(),
				new TwoStrongLinksTechniqueSearcher(),
				new UniqueRectangleTechniqueSearcher(CheckIncompletedUniquenessPatterns),
				new ExtendedRectangleTechniqueSearcher(),
				new UniqueLoopTechniqueSearcher(),
				new EmptyRectangleTechniqueSearcher(regionMaps),
				new AlmostLockedCandidatesTechniqueSearcher(intersection, CheckAlmostLockedQuadruple),
				new SueDeCoqTechniqueSearcher(regionMaps),
				new BorescoperDeadlyPatternTechniqueSearcher(),
				new BivalueUniversalGraveTechniqueSearcher(regionMaps, UseExtendedBugSearcher),
				new AlternatingInferenceChainTechniqueSearcher(true, true, AicMaximumLength),
				new HobiwanFishTechniqueSearcher(HobiwanFishMaximumSize, HobiwanFishMaximumExofinsCount, HobiwanFishMaximumEndofinsCount, HobiwanFishCheckTemplates, regionMaps),
				new BowmanBingoTechniqueSearcher(BowmanBingoMaximumLength),
				new PatternOverlayMethodTechniqueSearcher(),
				new TemplateTechniqueSearcher(OnlyRecordTemplateDelete),
				new ChuteClueCoverTechniqueSearcher(),
				new BruteForceTechniqueSearcher(solution),
			};
			if (UseCalculationPriority)
			{
				Array.Sort(searchers, (a, b) => a.Priority.CompareTo(b.Priority));
			}

			var bag = new Bag<TechniqueInfo>();
			var stopwatch = new Stopwatch();
			stopwatch.Start();
		Label_StartSolving:
			for (int i = 0, length = searchers.Length; i < length; i++)
			{
				var searcher = searchers[i];

				// Skip all searchers marked slow running attribute.
				if (DisableSlowTechniques
					&& searcher.HasMarkedAttribute<SlowAttribute>(false, out _))
				{
					continue;
				}

				if (!EnablePatternOverlayMethod && searcher is PatternOverlayMethodTechniqueSearcher
					|| !EnableTemplate && searcher is TemplateTechniqueSearcher
					|| !EnableBruteForce && searcher is BruteForceTechniqueSearcher
					|| !EnableBowmanBingo && searcher is BowmanBingoTechniqueSearcher)
				{
					continue;
				}

				searcher.AccumulateAll(bag, cloneation);
				var step = OptimizedApplyingOrder
					? bag.GetElementByMinSelector(info => info.Difficulty)
					: bag.FirstOrDefault();
				bag.Clear();

				if (EnableGarbageCollectionForcedly
					&& searcher.HasMarkedAttribute<HighAllocationAttribute>(false, out _))
				{
					GC.Collect();
				}

				if (step is null)
				{
					// If current step cannot find any steps,
					// we will turn to the next step finder to
					// continue solving puzzle.
					continue;
				}

				if (CheckConclusionValidityAfterSearched
					? CheckConclusionsValidity(solution, step.Conclusions)
					: true)
				{
					step.ApplyTo(cloneation);
					steps.Add(step);
					if (cloneation.HasSolved)
					{
						// The puzzle has been solved.
						// :)
						stopwatch.StopAnyway();

						return new AnalysisResult(
							puzzle: grid,
							solverName: SolverName,
							hasSolved: true,
							solution: cloneation,
							elapsedTime: stopwatch.Elapsed,
							solvingList: steps,
							additional: null);
					}
					else
					{
						// The puzzle has not been finished,
						// we should turn to the first step finder
						// to continue solving puzzle.
						goto Label_StartSolving;
					}
				}
				else
				{
					throw new WrongHandlingException(grid);
				}
			}

			// All solver cannot finish the puzzle...
			// :(
			stopwatch.StopAnyway();

			return new AnalysisResult(
				puzzle: grid,
				solverName: SolverName,
				hasSolved: false,
				solution: null,
				elapsedTime: stopwatch.Elapsed,
				solvingList: steps,
				additional: null);
		}


		/// <summary>
		/// To check the validity of all conclusions.
		/// </summary>
		/// <param name="solution">The solution.</param>
		/// <param name="conclusions">The conclusions.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		private static bool CheckConclusionsValidity(
			IReadOnlyGrid solution, IEnumerable<Conclusion> conclusions)
		{
			foreach (var conclusion in conclusions)
			{
				int digit = solution[conclusion.CellOffset];
				switch (conclusion.ConclusionType)
				{
					case ConclusionType.Assignment when digit != conclusion.Digit:
					{
						return false;
					}
					case ConclusionType.Elimination when digit == conclusion.Digit:
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
