using System.Collections.Generic;
using System.Diagnostics;
using Sudoku.Data.Meta;
using Sudoku.Linq;
using Sudoku.Solving.Locked;
using Sudoku.Solving.Singles;
using Sudoku.Solving.Subsets;

namespace Sudoku.Solving
{
	public sealed partial class ManualSolver : Solver
	{
		/// <summary>
		/// Specify whether the solver will optimize techniques applying order or not.
		/// </summary>
		public bool OptimizedApplyingOrder { get; set; }

		/// <summary>
		/// The name of this solver.
		/// </summary>
		public override string Name => "Manual";


		/// <summary>
		/// Solve the puzzle.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="analysisInfo">The analysis information.</param>
		/// <returns>The result.</returns>
		public override Grid? Solve(Grid grid, out AnalysisInfo analysisInfo)
		{
			// Initialization.
			var cloneation = grid.Clone();
			var steps = new List<TechniqueInfo>();

			// Enable all step finders.
			var stepFinders = new List<StepFinder>()
			{
				new SingleStepFinder(),
				new LockedStepFinder(),
				new SubsetStepFinder()
			};

			// Start time recording and solving.
			var stopwatch = new Stopwatch();
			stopwatch.Start();
		Start:
			foreach (var stepFinder in stepFinders)
			{
				var step = OptimizedApplyingOrder
					? stepFinder.TakeAll(cloneation).Min(info => info.Difficulty)
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

					if (cloneation.IsSolved)
					{
						// The puzzle has been solved.
						// :)
						stopwatch.Stop();

						analysisInfo = new(Name, steps, stopwatch.Elapsed, true);
						return cloneation;
					}
					else
					{
						// The puzzle has not been finished,
						// we should turn to the first step finder
						// to continue solving puzzle.
						goto Start;
					}
				}
			}

			// All solver cannot finish the puzzle...
			// :(
			//throw new CannotBeSolvedException(grid);
			analysisInfo = new(Name, steps, stopwatch.Elapsed, false);
			return null;
		}
	}
}
