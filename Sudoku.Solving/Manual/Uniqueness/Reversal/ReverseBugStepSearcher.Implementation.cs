using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Reversal
{
	partial class ReverseBugStepSearcher
	{
		/// <summary>
		/// Check the type 1.
		/// </summary>
		/// <param name="accumulator">The accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="d1">The digit 1 to check.</param>
		/// <param name="d2">The digit 2 to check.</param>
		/// <param name="loop">(<see langword="in"/> parameter) The cells of that loop.</param>
		/// <param name="links">The links.</param>
		/// <param name="extraCell">Indicates the extra cell.</param>
		partial void CheckType1(
			IList<ReverseBugStepInfo> accumulator, in SudokuGrid grid, int d1, int d2,
			in Cells loop, IReadOnlyList<Link> links, int extraCell)
		{
			var conclusions = new List<Conclusion>();
			if (grid.Exists(extraCell, d1) is true)
			{
				conclusions.Add(new(Elimination, extraCell, d1));
			}
			if (grid.Exists(extraCell, d2) is true)
			{
				conclusions.Add(new(Elimination, extraCell, d2));
			}
			if (conclusions.Count == 0)
			{
				return;
			}

			var cellOffsets = new List<DrawingInfo>();
			foreach (int cell in loop)
			{
				cellOffsets.Add(new(0, cell));
			}

			accumulator.AddIfDoesNotContain(
				new ReverseBugType1StepInfo(
					conclusions,
					new View[] { new() { Cells = cellOffsets, Links = links } },
					loop,
					d1,
					d2,
					extraCell,
					(grid.GetCandidates(extraCell) & (1 << d1 | 1 << d2)).FindFirstSet()));
		}
	}
}
