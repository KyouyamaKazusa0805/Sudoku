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
	public sealed partial class ManualSolver : Solver
	{
		public override string SolverName => "Manual";


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
				new BasicFishStepFinder()
			};

			// Start time recording and solving.
			int digit = 0;
			decimal min = decimal.MaxValue;
			TechniqueInfo? minInfo = null, firstInfo = null, ittoRyuInfo = null;
			var stopwatch = new Stopwatch();
			stopwatch.Start();
		Label_StartSolving:
			foreach (var stepFinder in stepFinders)
			{
				var infos = stepFinder.TakeAll(cloneation);
				foreach (var info in infos)
				{
					if (!OptimizedApplyingOrder)
					{
						firstInfo = info;
						break;
					}
					if (OptimizedApplyingOrder && info.Difficulty < min)
					{
						min = info.Difficulty;
						minInfo = info;
					}

					var conclusion = info.Conclusions[0];
					int curDigit = conclusion.Digit;
					if (conclusion.Type == ConclusionType.Assignment
						&& (curDigit == (digit + 1) % 9 || curDigit == digit))
					{
						ittoRyuInfo = info;
					}
				}

				var step = IttoRyuWhenPossible
					? ittoRyuInfo is null ? firstInfo : ittoRyuInfo
					: OptimizedApplyingOrder ? minInfo : firstInfo;

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
