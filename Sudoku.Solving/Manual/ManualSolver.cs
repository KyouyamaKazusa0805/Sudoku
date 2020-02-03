using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.Data.Meta;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Manual.Chaining;
using Sudoku.Solving.Manual.Fishes.Basic;
using Sudoku.Solving.Manual.Intersections;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Solving.Manual.Subsets;
using Sudoku.Solving.Manual.Uniqueness.Rectangles;
using Sudoku.Solving.Manual.Wings.Irregular;
using Sudoku.Solving.Manual.Wings.Regular;

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
				return CheckMinimumDifficultyStrictly
					? SolveWithStrictDifficultyRating(grid, grid.Clone(), new List<TechniqueInfo>(), solution)
					: SolveNaively(grid, grid.Clone(), new List<TechniqueInfo>(), solution);
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
		/// Solve the puzzle with <see cref="CheckMinimumDifficultyStrictly"/> option.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="cloneation">The cloneation grid to calculate.</param>
		/// <param name="steps">All steps found.</param>
		/// <param name="solution">The solution.</param>
		/// <returns>The analysis result.</returns>
		private AnalysisResult SolveWithStrictDifficultyRating(
			Grid grid, Grid cloneation, List<TechniqueInfo> steps, Grid solution)
		{
			var searchers = EnableBruteForce
				? new TechniqueSearcher[][]
				{
					new[] { new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit) },
					new[] { new IntersectionTechniqueSearcher() },
					new TechniqueSearcher[]
					{
						new SubsetTechniqueSearcher(),
						new NormalFishTechniqueSearcher(),
						new RegularWingTechniqueSearcher(CheckRegularWingSize),
						new IrregularWingTechniqueSearcher(),
						new UniqueRectangleTechniqueSearcher(CheckIncompletedUniquenessPatterns),
						new TwoStrongLinksTechniqueSearcher(),
						new AlmostLockedCandidatesTechniqueSearcher(),
					},
					new[] { new BruteForceTechniqueSearcher(solution) }
				}
				: new TechniqueSearcher[][] // Does not have brute force.
				{
					new[] { new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit) },
					new[] { new IntersectionTechniqueSearcher() },
					new TechniqueSearcher[]
					{
						new SubsetTechniqueSearcher(),
						new NormalFishTechniqueSearcher(),
						new RegularWingTechniqueSearcher(CheckRegularWingSize),
						new IrregularWingTechniqueSearcher(),
						new UniqueRectangleTechniqueSearcher(CheckIncompletedUniquenessPatterns),
						new TwoStrongLinksTechniqueSearcher(),
						new AlmostLockedCandidatesTechniqueSearcher(),
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
		/// <returns>The analysis result.</returns>
		private AnalysisResult SolveNaively(
			Grid grid, Grid cloneation, List<TechniqueInfo> steps, Grid solution)
		{
			var searchers = EnableBruteForce
				? new TechniqueSearcher[]
				{
					new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit),
					new IntersectionTechniqueSearcher(),
					new SubsetTechniqueSearcher(),
					new NormalFishTechniqueSearcher(),
					new RegularWingTechniqueSearcher(CheckRegularWingSize),
					new IrregularWingTechniqueSearcher(),
					new UniqueRectangleTechniqueSearcher(CheckIncompletedUniquenessPatterns),
					new TwoStrongLinksTechniqueSearcher(),
					new AlmostLockedCandidatesTechniqueSearcher(),
					new BruteForceTechniqueSearcher(solution),
				}
				: new TechniqueSearcher[] // Does not have brute force.
				{
					new SingleTechniqueSearcher(EnableFullHouse, EnableLastDigit),
					new IntersectionTechniqueSearcher(),
					new SubsetTechniqueSearcher(),
					new NormalFishTechniqueSearcher(),
					new RegularWingTechniqueSearcher(CheckRegularWingSize),
					new IrregularWingTechniqueSearcher(),
					new UniqueRectangleTechniqueSearcher(CheckIncompletedUniquenessPatterns),
					new TwoStrongLinksTechniqueSearcher(),
					new AlmostLockedCandidatesTechniqueSearcher(),
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
	}
}
