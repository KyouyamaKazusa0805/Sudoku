using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sudoku.Data.Meta;
using Sudoku.Solving.Manual.Singles;

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
				new SinglesStepFinder(EnableFullHouse, EnableLastDigit)
			};

			// Start time recording and solving.
			var stopwatch = new Stopwatch();
			stopwatch.Start();
		Label_StartSolving:
			foreach (var stepFinder in stepFinders)
			{
				var step = OptimizedApplyingOrder
					? stepFinder.TakeAll(cloneation).GetElementByMinSelector(info => info.Difficulty)
					: stepFinder.TakeOne(cloneation);

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
				solverName: SolverName,
				hasSolved: false,
				solution: null,
				elapsedTime: stopwatch.Elapsed,
				solvingList: steps,
				additional: null);
		}
	}
}
