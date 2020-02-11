using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.Data.Meta;
using Sudoku.Runtime;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Manual.Fishes.Basic;
using Sudoku.Solving.Manual.Intersections;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Solving.Manual.SingleDigitPatterns;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Solving.Manual.Subsets;
using Sudoku.Solving.Manual.Uniqueness.Bugs;
using Sudoku.Solving.Manual.Uniqueness.Rectangles;
using Sudoku.Solving.Manual.Wings.Irregular;
using Sudoku.Solving.Manual.Wings.Regular;
using Intersection = System.ValueTuple<int, int, Sudoku.Data.Meta.GridMap, Sudoku.Data.Meta.GridMap>;

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
		public override AnalysisResult Solve(Grid grid)
		{
			if (grid.IsUnique(out var solution))
			{
				// Get intersection table in order to run faster in intersection technique searchers.
				var intersectionTable = new Intersection[18, 3];
				for (int i = 0; i < 18; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						int baseSet = i + 9;
						int coverSet = i < 9 ? i / 3 * 3 + j : ((i - 9) / 3 * 3 + j) * 3 % 8;
						intersectionTable[i, j] = (
							baseSet, coverSet, GridMap.CreateInstance(baseSet),
							GridMap.CreateInstance(coverSet));
					}
				}

				// Get all region maps.
				var regionMaps = new GridMap[27];
				for (int i = 0; i < 27; i++)
				{
					regionMaps[i] = GridMap.CreateInstance(i);
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
			Grid grid, Grid cloneation, List<TechniqueInfo> steps,
			Grid solution, Intersection[,] intersection, GridMap[] regionMaps)
		{
			var searchers = EnableBruteForce
				? new TechniqueSearcher[][]
				{
					new[] { new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit) },
					new[] { new LockedCandidatesTechniqueSearcher(intersection) },
					new TechniqueSearcher[]
					{
						new SubsetTechniqueSearcher(),
						new NormalFishTechniqueSearcher(),
						new RegularWingTechniqueSearcher(CheckRegularWingSize),
						new IrregularWingTechniqueSearcher(),
						new UniqueRectangleTechniqueSearcher(CheckIncompletedUniquenessPatterns),
						new TwoStrongLinksTechniqueSearcher(),
						new EmptyRectangleTechniqueSearcher(regionMaps),
						new AlmostLockedCandidatesTechniqueSearcher(intersection),
						new BivalueUniversalGraveTechniqueSearcher(regionMaps, UseExtendedBugSearcher),
					},
					new[] { new BruteForceTechniqueSearcher(solution) }
				}
				: new TechniqueSearcher[][] // Does not have brute force.
				{
					new[] { new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit) },
					new[] { new LockedCandidatesTechniqueSearcher(intersection) },
					new TechniqueSearcher[]
					{
						new SubsetTechniqueSearcher(),
						new NormalFishTechniqueSearcher(),
						new RegularWingTechniqueSearcher(CheckRegularWingSize),
						new IrregularWingTechniqueSearcher(),
						new UniqueRectangleTechniqueSearcher(CheckIncompletedUniquenessPatterns),
						new TwoStrongLinksTechniqueSearcher(),
						new EmptyRectangleTechniqueSearcher(regionMaps),
						new AlmostLockedCandidatesTechniqueSearcher(intersection),
						new BivalueUniversalGraveTechniqueSearcher(regionMaps, UseExtendedBugSearcher),
					}
				};

			var stopwatch = new Stopwatch();
			stopwatch.Start();
		Label_StartSolving:
			foreach (var searcherListGroup in searchers)
			{
				var collection = new List<TechniqueInfo>();
				foreach (var searcher in searcherListGroup)
				{
					collection.AddRange(searcher.TakeAll(cloneation));
				}

				var step = collection.GetElementByMinSelector(info => info.Difficulty);
				if (step is null)
				{
					// If current step cannot find any steps,
					// we will turn to the next step finder to
					// continue solving puzzle.
					continue;
				}

				if (CheckEliminations(solution, step.Conclusions))
				{
					step.ApplyTo(cloneation);
					steps.Add(step);
					if (cloneation.HasSolved)
					{
						// The puzzle has been solved.
						// :)
						if (stopwatch.IsRunning)
						{
							stopwatch.Stop();
						}

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
			if (stopwatch.IsRunning)
			{
				stopwatch.Stop();
			}

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
			Grid grid, Grid cloneation, List<TechniqueInfo> steps,
			Grid solution, Intersection[,] intersection, GridMap[] regionMaps)
		{
			var searchers = EnableBruteForce
				? new TechniqueSearcher[]
				{
					new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit),
					new LockedCandidatesTechniqueSearcher(intersection),
					new SubsetTechniqueSearcher(),
					new NormalFishTechniqueSearcher(),
					new RegularWingTechniqueSearcher(CheckRegularWingSize),
					new IrregularWingTechniqueSearcher(),
					new UniqueRectangleTechniqueSearcher(CheckIncompletedUniquenessPatterns),
					new TwoStrongLinksTechniqueSearcher(),
					new EmptyRectangleTechniqueSearcher(regionMaps),
					new AlmostLockedCandidatesTechniqueSearcher(intersection),
					new BivalueUniversalGraveTechniqueSearcher(regionMaps, UseExtendedBugSearcher),
					new BruteForceTechniqueSearcher(solution),
				}
				: new TechniqueSearcher[] // Does not have brute force.
				{
					new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit),
					new LockedCandidatesTechniqueSearcher(intersection),
					new SubsetTechniqueSearcher(),
					new NormalFishTechniqueSearcher(),
					new RegularWingTechniqueSearcher(CheckRegularWingSize),
					new IrregularWingTechniqueSearcher(),
					new UniqueRectangleTechniqueSearcher(CheckIncompletedUniquenessPatterns),
					new TwoStrongLinksTechniqueSearcher(),
					new EmptyRectangleTechniqueSearcher(regionMaps),
					new AlmostLockedCandidatesTechniqueSearcher(intersection),
					new BivalueUniversalGraveTechniqueSearcher(regionMaps, UseExtendedBugSearcher),
				};

			var stopwatch = new Stopwatch();
			stopwatch.Start();
		Label_StartSolving:
			for (int i = 0, length = searchers.Length; i < length; i++)
			{
				var searcher = searchers[i];
				var infos = searcher.TakeAll(cloneation);
				var step = OptimizedApplyingOrder
					? infos.GetElementByMinSelector(info => info.Difficulty)
					: infos.FirstOrDefault();

				if (step is null)
				{
					// If current step cannot find any steps,
					// we will turn to the next step finder to
					// continue solving puzzle.
					continue;
				}

				if (CheckEliminations(solution, step.Conclusions))
				{
					step.ApplyTo(cloneation);
					steps.Add(step);
					if (cloneation.HasSolved)
					{
						// The puzzle has been solved.
						// :)
						if (stopwatch.IsRunning)
						{
							stopwatch.Stop();
						}

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
			if (stopwatch.IsRunning)
			{
				stopwatch.Stop();
			}

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
		private static bool CheckEliminations(Grid solution, IEnumerable<Conclusion> conclusions)
		{
			foreach (var conclusion in conclusions)
			{
				int digit = solution[conclusion.CellOffset];
				switch (conclusion.Type)
				{
					case ConclusionType.Assignment:
					{
						if (digit != conclusion.Digit)
						{
							return false;
						}
						break;
					}
					case ConclusionType.Elimination:
					{
						if (digit == conclusion.Digit)
						{
							return false;
						}
						break;
					}
					default:
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
