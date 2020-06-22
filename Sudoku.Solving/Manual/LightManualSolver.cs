using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Windows;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides a light manual solver used for testing and checking backdoors.
	/// This solver will use mankind logic to solve a puzzle, but only
	/// <b>Hidden Single</b>s and <b>Naked Single</b>s will be used.
	/// </summary>
	public sealed class LightManualSolver : Solver
	{
		/// <inheritdoc/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string SolverName => Resources.GetValue("ManualLight");


		/// <inheritdoc/>
		/// <remarks>
		/// You should use the simple version of the solving method <see cref="CanSolve(IReadOnlyGrid)"/>.
		/// </remarks>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		/// <seealso cref="CanSolve(IReadOnlyGrid)"/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override AnalysisResult Solve(IReadOnlyGrid grid) =>
			throw new NotSupportedException($"The specified method should be replaced with '{nameof(CanSolve)}'.");

		/// <summary>
		/// To check whether the specified solver can solve the puzzle.
		/// </summary>
		/// <param name="grid">The puzzle.</param>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether the solver
		/// solved the puzzle successfully.
		/// </returns>
		public bool CanSolve(IReadOnlyGrid grid)
		{
			var cloneation = grid.Clone();

			var steps = new List<TechniqueInfo>();
			var searcher = new SingleTechniqueSearcher(false, false);
			var bag = new Bag<TechniqueInfo>();
			while (!cloneation.HasSolved)
			{
				searcher.GetAll(bag, cloneation);
				if (bag.Count == 0)
				{
					break;
				}

				foreach (var step in bag)
				{
					if (SaveTechnique(steps, step, cloneation))
					{
						return true;
					}
				}

				bag.Clear();
			}

			return false;
		}

		/// <summary>
		/// To record the current technique step.
		/// </summary>
		/// <param name="steps">The steps have been found.</param>
		/// <param name="step">The current step.</param>
		/// <param name="cloneation">The cloneation of the grid.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		private bool SaveTechnique(ICollection<TechniqueInfo> steps, TechniqueInfo step, Grid cloneation)
		{
			foreach (var conclusion in step.Conclusions)
			{
				switch (conclusion.ConclusionType)
				{
					case Assignment when cloneation.GetStatus(conclusion.CellOffset) == Empty:
					case Elimination when cloneation.Exists(conclusion.CellOffset, conclusion.Digit) is true:
					{
						step.ApplyTo(cloneation);
						steps.Add(step);

						return cloneation.HasSolved;
					}
				}
			}

			return false;
		}
	}
}
