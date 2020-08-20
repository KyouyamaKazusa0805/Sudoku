using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Literals;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Tools;

namespace Sudoku.Solving.Locked
{
	/// <summary>
	/// Represents a intersection technique step finder.
	/// Technique contains:
	/// <list type="bullet">
	/// <item><term>Pointing</term><description>2.6</description></item>
	/// <item><term>Claiming</term><description>2.8</description></item>
	/// </list>
	/// </summary>
	public sealed class LockedStepFinder : StepFinder
	{
		public override IEnumerable<TechniqueInfo> TakeAll(Grid grid)
		{
			// Iterate on locked steps.
			return from baseSet in Values.RegionRange
				   from coverSet in baseSet.CrossRegions
				   from digit in Values.DigitRange
				   let cardinality = grid.GetDigitPositionsOf(baseSet, digit).Cardinality
				   where cardinality >= 2 && cardinality <= 3
				   let cellsHavingDigit = grid.GetAllInfosWhenHavingDigit(baseSet, digit).Select(i => i.Cell)
				   let coverCells = coverSet.Cells
				   where cellsHavingDigit.All(c => coverCells.Contains(c))
				   let conclusion = GetConclusion(grid, coverSet, digit, cellsHavingDigit)
				   where conclusion.Any()
				   let view = GetView(grid, baseSet, coverSet, digit, coverCells)
				   select GetLockedInfo(digit, baseSet, coverSet, conclusion, view);
		}


		private static LockedInfo GetLockedInfo(
			int digit, Region baseSet, Region coverSet, Conclusion conclusion, View view) =>
			new(conclusion, new List<View> { view }, digit, baseSet, coverSet);

		private static Conclusion GetConclusion(
			Grid grid, Region coverSet, int digit, IEnumerable<Cell> cellsHavingDigit) =>
			new(
				conclusionType: ConclusionType.Elimination,
				candidates:
					from info in grid[coverSet]
					where !cellsHavingDigit.Contains(info.Cell) && !info.IsValueCell && info.Contains(digit)
					select new Candidate(info.Cell, digit));

		private static View GetView(
			Grid grid, Region baseSet, Region coverSet, int digit, IEnumerable<Cell> coverCells) =>
			new(
				cells: null,
				candidates:
					from cell in baseSet.Cells
					let info = grid[cell]
					where coverCells.Contains(cell) && !info.IsValueCell && info[digit]
					select ((Id)0, new Candidate(cell, digit)),
				regions: new List<(Id, Region)> { (0, baseSet), (1, coverSet) },
				inferences: null);
	}
}
