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
		/// You should use the simple version of the solving method <see cref="CanSolve(in SudokuGrid)"/>.
		/// </remarks>
		/// <exception cref="NotSupportedException">Always throws.</exception>
		/// <seealso cref="CanSolve(in SudokuGrid)"/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override AnalysisResult Solve(in SudokuGrid grid) =>
			throw new NotSupportedException($"The specified method should be replaced with '{nameof(CanSolve)}'.");

		/// <summary>
		/// To check whether the specified solver can solve the puzzle.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The puzzle.</param>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether the solver
		/// solved the puzzle successfully.
		/// </returns>
		public bool CanSolve(in SudokuGrid grid)
		{
			var cloneation = grid;

			var steps = new List<TechniqueInfo>();
			var searcher = new SingleTechniqueSearcher(false, false, false);
			var bag = new List<TechniqueInfo>();
			while (!cloneation.HasSolved)
			{
				searcher.GetAll(bag, cloneation);
				if (bag.Count == 0)
				{
					break;
				}

				foreach (var step in bag)
				{
					if (SaveStep(steps, step, ref cloneation))
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
		/// <param name="cloneation">(<see langword="ref"/> parameter) The cloneation of the grid.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		private bool SaveStep(ICollection<TechniqueInfo> steps, TechniqueInfo step, ref SudokuGrid cloneation)
		{
			foreach (var (t, c, d) in step.Conclusions)
			{
				switch (t)
				{
					case Assignment when cloneation.GetStatus(c) == Empty:
					case Elimination when cloneation.Exists(c, d) is true:
					{
						step.ApplyTo(ref cloneation);
						steps.Add(step);

						return cloneation.HasSolved;
					}
				}
			}

			return false;
		}
	}
}
