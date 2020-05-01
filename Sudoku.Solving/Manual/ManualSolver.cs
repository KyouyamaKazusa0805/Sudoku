using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Extensions;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Manual.Alses;
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
using Sudoku.Solving.Manual.Uniqueness.Bugs;
using Sudoku.Solving.Manual.Uniqueness.Extended;
using Sudoku.Solving.Manual.Uniqueness.Loops;
using Sudoku.Solving.Manual.Uniqueness.Polygons;
using Sudoku.Solving.Manual.Uniqueness.Rects;
using Sudoku.Solving.Manual.Wings.Irregular;
using Sudoku.Solving.Manual.Wings.Regular;
using static Sudoku.Solving.ConclusionType;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides a solver that use logical methods to solve a specified sudoku puzzle.
	/// </summary>
	[Serializable]
	public sealed partial class ManualSolver : Solver
	{
		/// <inheritdoc/>
		public override string SolverName => "Manual";


		/// <inheritdoc/>
		public override AnalysisResult Solve(IReadOnlyGrid grid)
		{
			if (grid.IsValid(out var solution))
			{
				// Solve the puzzle.
				try
				{
					return AnalyzeDifficultyStrictly
						? SolveWithStrictDifficultyRating(grid, grid.Clone(), new List<TechniqueInfo>(), solution)
						: SolveNaively(grid, grid.Clone(), new List<TechniqueInfo>(), solution);
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
						additional: ex.Message,
						stepGrids: null);
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
					additional: "The puzzle does not have a unique solution (multiple solutions or no solution).",
					stepGrids: null);
			}
		}

		/// <summary>
		/// Solve the puzzle with <see cref="AnalyzeDifficultyStrictly"/> option.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="cloneation">The cloneation grid to calculate.</param>
		/// <param name="steps">All steps found.</param>
		/// <param name="solution">The solution.</param>
		/// <returns>The analysis result.</returns>
		/// <exception cref="WrongHandlingException">
		/// Throws when the solver cannot solved due to wrong handling.
		/// </exception>
		private AnalysisResult SolveWithStrictDifficultyRating(
			IReadOnlyGrid grid, Grid cloneation, List<TechniqueInfo> steps,
			IReadOnlyGrid solution)
		{
			var searchers = new TechniqueSearcher[][]
			{
				new[] { new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit) },
				new[] { new LcTechniqueSearcher() },
				new TechniqueSearcher[]
				{
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
					new SdcTechniqueSearcher(),
					new BdpTechniqueSearcher(),
					new BugTechniqueSearcher(UseExtendedBugSearcher),
					new ErIntersectionPairTechniqueSearcher(),
					new AlsXzTechniqueSearcher(AllowOverlapAlses, AlsHighlightRegionInsteadOfCell),
					new AlsXyWingTechniqueSearcher(AllowOverlapAlses, AlsHighlightRegionInsteadOfCell),
					new AlsWWingTechniqueSearcher(AllowOverlapAlses, AlsHighlightRegionInsteadOfCell),
					new GroupedAicTechniqueSearcher(
						true, false, false, AicMaximumLength, ReductDifferentPathAic,
						OnlySaveShortestPathAic, CheckHeadCollision, CheckContinuousNiceLoop),
					new GroupedAicTechniqueSearcher(
						false, true, false, AicMaximumLength, ReductDifferentPathAic,
						OnlySaveShortestPathAic, CheckHeadCollision, CheckContinuousNiceLoop),
					new GroupedAicTechniqueSearcher(
						false, false, true, AicMaximumLength, ReductDifferentPathAic,
						OnlySaveShortestPathAic, CheckHeadCollision, CheckContinuousNiceLoop),
				},
				new TechniqueSearcher[]
				{
					new BowmanBingoTechniqueSearcher(BowmanBingoMaximumLength),
					new DeathBlossomTechniqueSearcher(
						AllowOverlapAlses, AlsHighlightRegionInsteadOfCell, MaxPetalsOfDeathBlossom),
					new HobiwanFishTechniqueSearcher(
						HobiwanFishMaximumSize, HobiwanFishMaximumExofinsCount,
						HobiwanFishMaximumEndofinsCount, HobiwanFishCheckTemplates),
					new PomTechniqueSearcher(),
					new TemplateTechniqueSearcher(OnlyRecordTemplateDelete),
				},
				new TechniqueSearcher[]
				{
					new CccTechniqueSearcher(),
					new JuniorExocetTechniqueSearcher(CheckAdvancedInExocet),
					new SeniorExocetTechniqueSearcher(CheckAdvancedInExocet),
					new SkLoopTechniqueSearcher(),
				},
				new[] { new BruteForceTechniqueSearcher(solution) }
			};

			var stepGrids = new Bag<IReadOnlyGrid>();
			var bag = new Bag<TechniqueInfo>();
			var stopwatch = new Stopwatch();
			stopwatch.Start();
		Label_Restart:
			for (int i = 0, length = searchers.Length; i < length; i++)
			{
				var searcherListGroup = searchers[i];
				foreach (var searcher in searcherListGroup)
				{
					if (!(bool)searcher.GetType().GetProperty("IsEnabled")!.GetValue(null)!)
					{
						// Skip the technique when the static property 'IsEnabled' is set false.
						continue;
					}

					if (EnableGarbageCollectionForcedly
						&& searcher.HasMarkedAttribute<HighAllocationAttribute>(false, out _))
					{
						GC.Collect();
					}

					searcher.GetAll(bag, cloneation);
				}
				if (!bag.Any())
				{
					continue;
				}

				if (FastSearch)
				{
					decimal minDiff = bag.Min(info => info.Difficulty);
					var selection = from info in bag
									where info.Difficulty == minDiff
									select info;
					if (!selection.Any())
					{
						continue;
					}

					if (CheckConclusionValidityAfterSearched
						? selection.All(info => CheckConclusionsValidity(solution, info.Conclusions))
						: true)
					{
						foreach (var step in selection)
						{
							if (RecordTechnique(
								steps, step, grid, cloneation, stopwatch, stepGrids, out var result))
							{
								stopwatch.Stop();
								return result;
							}
						}

						// The puzzle has not been finished,
						// we should turn to the first step finder
						// to continue solving puzzle.
						bag.Clear();
						goto Label_Restart;
					}
					else
					{
						var wrongStep = (TechniqueInfo?)null;
						foreach (var step in selection)
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
					var step = bag.GetElementByMinSelector(info => info.Difficulty);
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
						if (RecordTechnique(
							steps, step, grid, cloneation, stopwatch, stepGrids, out var result))
						{
							// The puzzle has been solved.
							// :)
							stopwatch.Stop();
							return result;
						}
						else
						{
							// The puzzle has not been finished,
							// we should turn to the first step finder
							// to continue solving puzzle.
							bag.Clear();
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

		/// <summary>
		/// Solve naively.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="cloneation">The cloneation grid to calculate.</param>
		/// <param name="steps">All steps found.</param>
		/// <param name="solution">The solution.</param>
		/// <returns>The analysis result.</returns>
		/// <exception cref="WrongHandlingException">
		/// Throws when the solver cannot solved due to wrong handling.
		/// </exception>
		private AnalysisResult SolveNaively(
			IReadOnlyGrid grid, Grid cloneation, List<TechniqueInfo> steps,
			IReadOnlyGrid solution)
		{
			// Check symmetry first.
			if (CheckGurthSymmetricalPlacement)
			{
				var symmetrySearcher = new GspTechniqueSearcher();
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
				new SdcTechniqueSearcher(),
				new BdpTechniqueSearcher(),
				new BugTechniqueSearcher(UseExtendedBugSearcher),
				new ErIntersectionPairTechniqueSearcher(),
				new AlsXzTechniqueSearcher(AllowOverlapAlses, AlsHighlightRegionInsteadOfCell),
				new AlsXyWingTechniqueSearcher(AllowOverlapAlses, AlsHighlightRegionInsteadOfCell),
				new AlsWWingTechniqueSearcher(AllowOverlapAlses, AlsHighlightRegionInsteadOfCell),
				new DeathBlossomTechniqueSearcher(
					AllowOverlapAlses, AlsHighlightRegionInsteadOfCell, MaxPetalsOfDeathBlossom),
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
				new BruteForceTechniqueSearcher(solution),
			};

			static T g<T>(TechniqueSearcher s, string p) => (T)s.GetType().GetProperty(p)!.GetValue(null)!;
			if (UseCalculationPriority)
			{
				Array.Sort(
					searchers,
					(a, b) => g<int>(a, "Priority").CompareTo(g<int>(b, "Priority")));
			}

			var stepGrids = new Bag<IReadOnlyGrid>();
			var bag = new Bag<TechniqueInfo>();
			var stopwatch = new Stopwatch();
			stopwatch.Start();
		Label_Restart:
			for (int i = 0, length = searchers.Length; i < length; i++)
			{
				var searcher = searchers[i];

				if (!g<bool>(searcher, "IsEnabled"))
				{
					// Skip the technique when the static property 'IsEnabled' is set false.
					continue;
				}

				searcher.GetAll(bag, cloneation);
				if (!bag.Any())
				{
					continue;
				}

				if (FastSearch)
				{
					if (CheckConclusionValidityAfterSearched
						? bag.All(info => CheckConclusionsValidity(solution, info.Conclusions))
						: true)
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
							&& searcher.HasMarkedAttribute<HighAllocationAttribute>(false, out _))
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

					if (CheckConclusionValidityAfterSearched
						? CheckConclusionsValidity(solution, step.Conclusions)
						: true)
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
								&& searcher.HasMarkedAttribute<HighAllocationAttribute>(false, out _))
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

		/// <summary>
		/// Bound with on-solving methods returns the solving state.
		/// </summary>
		/// <param name="steps">The steps.</param>
		/// <param name="step">The step.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="cloneation">The cloneation (playground).</param>
		/// <param name="stopwatch">The stopwatch.</param>
		/// <param name="stepGrids">The step grids.</param>
		/// <param name="result">(<see langword="out"/> parameter) The analysis result.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		/// <seealso cref="SolveNaively(IReadOnlyGrid, Grid, List{TechniqueInfo}, IReadOnlyGrid)"/>
		/// <seealso cref="SolveWithStrictDifficultyRating(IReadOnlyGrid, Grid, List{TechniqueInfo}, IReadOnlyGrid)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool RecordTechnique(
			List<TechniqueInfo> steps, TechniqueInfo step, IReadOnlyGrid grid,
			Grid cloneation, Stopwatch stopwatch, IBag<IReadOnlyGrid> stepGrids,
			[NotNullWhen(true)] out AnalysisResult? result)
		{
			bool needAdd = false;
			foreach (var (t, c, d) in step.Conclusions)
			{
				switch (t)
				{
					case Assignment when cloneation.GetStatus(c) == CellStatus.Empty:
					case Elimination when cloneation.Exists(c, d) is true:
					{
						needAdd = true;

						goto Label_Decide;
					}
				}
			}

		Label_Decide:
			if (needAdd)
			{
				stepGrids.Add(cloneation.Clone());
				step.ApplyTo(cloneation);
				steps.Add(step);

				if (cloneation.HasSolved)
				{
					result = new AnalysisResult(
						puzzle: grid,
						solverName: SolverName,
						hasSolved: true,
						solution: cloneation,
						elapsedTime: stopwatch.Elapsed,
						solvingList: steps,
						additional: null,
						stepGrids);
					return true;
				}
			}

			result = null;
			return false;
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
			foreach (var (t, c, d) in conclusions)
			{
				int digit = solution[c];
				switch (t)
				{
					case Assignment when digit != d:
					case Elimination when digit == d:
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
