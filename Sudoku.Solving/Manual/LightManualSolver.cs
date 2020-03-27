using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Data;
using Sudoku.Solving.Manual.Singles;
using Sudoku.Solving.Utils;

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
				if (!bag.Any())
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

		private bool RecordTechnique(
			List<TechniqueInfo> steps, TechniqueInfo step, IReadOnlyGrid grid, Grid cloneation,
			[NotNullWhen(true)] out AnalysisResult? result)
		{
			bool needAdd = false;
			foreach (var conclusion in step.Conclusions)
			{
				switch (conclusion.ConclusionType)
				{
					case ConclusionType.Assignment:
					{
						if (cloneation.GetCellStatus(conclusion.CellOffset) == CellStatus.Empty)
						{
							needAdd = true;
						}

						break;
					}
					case ConclusionType.Elimination:
					{
						if (cloneation.CandidateExists(conclusion.CellOffset, conclusion.Digit))
						{
							needAdd = true;
						}

						break;
					}
				}
			}

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
