using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Solving.Manual.Singles;
using static Sudoku.Data.CellStatus;
using static Sudoku.Solving.ConclusionType;

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
		public override string SolverName => "Manual (Light)";


		/// <inheritdoc/>
		public override AnalysisResult Solve(IReadOnlyGrid grid)
		{
			var cloneation = grid.Clone();

			var steps = new List<TechniqueInfo>();
			var searcher = new SingleTechniqueSearcher(false, false);
			var bag = new Bag<TechniqueInfo>();
			while (!cloneation.HasSolved)
			{
				searcher.AccumulateAll(bag, cloneation);
				if (bag.Count == 0)
				{
					break;
				}

				foreach (var step in bag)
				{
					if (RecordTechnique(steps, step, grid, cloneation, out var result))
					{
						return result;
					}
				}

				bag.Clear();
			}

			return new AnalysisResult(
				puzzle: grid,
				solverName: SolverName,
				hasSolved: false,
				solution: null,
				elapsedTime: TimeSpan.Zero,
				solvingList: steps,
				additional: null,
				stepGrids: null);
		}

		/// <summary>
		/// To record the current technique step.
		/// </summary>
		/// <param name="steps">The steps have been found.</param>
		/// <param name="step">The current step.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="cloneation">The cloneation of the grid.</param>
		/// <param name="result">The result.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		private bool RecordTechnique(
			List<TechniqueInfo> steps, TechniqueInfo step, IReadOnlyGrid grid, Grid cloneation,
			[NotNullWhen(true)] out AnalysisResult? result)
		{
			bool needAdd = false;
			foreach (var conclusion in step.Conclusions)
			{
				switch (conclusion.ConclusionType)
				{
					case Assignment when cloneation.GetCellStatus(conclusion.CellOffset) == Empty:
					case Elimination when cloneation.Exists(conclusion.CellOffset, conclusion.Digit) is true:
					{
						needAdd = true;

						goto Label_Checking;
					}
				}
			}

		Label_Checking:
			if (needAdd)
			{
				step.ApplyTo(cloneation);
				steps.Add(step);

				if (cloneation.HasSolved)
				{
					result = new AnalysisResult(
						puzzle: grid,
						solverName: SolverName,
						hasSolved: true,
						solution: cloneation,
						elapsedTime: TimeSpan.Zero,
						solvingList: steps,
						additional: null,
						stepGrids: null);
					return true;
				}
			}

			result = null;
			return false;
		}
	}
}
