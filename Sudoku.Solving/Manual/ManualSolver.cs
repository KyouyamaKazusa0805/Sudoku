using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.Data.Meta;
using Sudoku.Solving.Manual.Fishes.Basic;
using Sudoku.Solving.Manual.Intersections;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Solving.Manual.Subsets;

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
			// Initialization.
			var cloneation = grid.Clone();
			var steps = new List<TechniqueInfo>();

			// Enable all step finders.
			var stepFinders = new List<StepFinder>()
			{
				new SingleStepFinder(EnableFullHouse, EnableLastDigit),
				new IntersectionStepFinder(),
				new SubsetStepFinder(),
				//new BasicFishStepFinder(),
				new NormalFishStepFinder(),
			};

			// Start time recording and solving.
			var stopwatch = new Stopwatch();
			stopwatch.Start();
		Label_StartSolving:
			int digit = 0;
			var ittoRyuInfo = (TechniqueInfo?)null;
			for (int i = 0; i < stepFinders.Count; i++)
			{
				var stepFinder = stepFinders[i];
				var infos = stepFinder.TakeAll(cloneation);
				if (IttoRyuWhenPossible && i == 0) // 'i == 0' stands for single step finder.
				{
					// TODO: To check whether the puzzle has that digit.
					// TODO: _ If the digit does not exist, we should skip this digit
					// TODO: _ and find the next digit, until the conclusion found.
					foreach (var info in infos)
					{
						var conclusion = info.Conclusions[0];
						int curDigit = conclusion.Digit;
						if (conclusion.Type == ConclusionType.Assignment
							&& (curDigit == (digit + 1) % 9 || curDigit == digit))
						{
							ittoRyuInfo = info;
							if (curDigit == (digit + 1) % 9)
							{
								digit = (digit + 1) % 9;
							}

							break;
						}
					}
				}

				var selection = OptimizedApplyingOrder
					? infos.GetElementByMinSelector(info => info.Difficulty)
					: infos.FirstOrDefault();
				var step = IttoRyuWhenPossible
					? ittoRyuInfo is null
						? selection
						: ittoRyuInfo
					: selection;

				if (step is null)
				{
					// If current step cannot find any steps,
					// we will turn to the next step finder to
					// continue solving puzzle.
					continue;
				}
				else
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
							initialGrid: grid,
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
			}

			// All solver cannot finish the puzzle...
			// :(
			if (stopwatch.IsRunning)
			{
				stopwatch.Stop();
			}

			return new AnalysisResult(
				initialGrid: grid,
				solverName: SolverName,
				hasSolved: false,
				solution: null,
				elapsedTime: stopwatch.Elapsed,
				solvingList: steps,
				additional: null);
		}
	}
}
