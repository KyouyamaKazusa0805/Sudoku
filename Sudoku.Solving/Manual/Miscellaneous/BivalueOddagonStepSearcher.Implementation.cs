using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;

namespace Sudoku.Solving.Manual.Miscellaneous
{
	partial class BivalueOddagonStepSearcher
	{
		/// <summary>
		/// Check type 1.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="d1">The digit 1.</param>
		/// <param name="d2">The digit 2.</param>
		/// <param name="loop">(<see langword="in"/> parameter) The loop.</param>
		/// <param name="links">The links.</param>
		/// <param name="extraCellsMap">(<see langword="in"/> parameter) The extra cells map.</param>
		partial void CheckType1(
			IList<BivalueOddagonStepInfo> accumulator, in SudokuGrid grid, int d1, int d2, in Cells loop,
			IReadOnlyList<Link> links, in Cells extraCellsMap)
		{
			int extraCell = extraCellsMap[0];
			var conclusions = new List<Conclusion>();
			if (grid.Exists(extraCell, d1) is true)
			{
				conclusions.Add(new(ConclusionType.Elimination, extraCell, d1));
			}
			if (grid.Exists(extraCell, d2) is true)
			{
				conclusions.Add(new(ConclusionType.Elimination, extraCell, d2));
			}
			if (conclusions.Count != 0)
			{
				return;
			}

			var candidateOffsets = new List<DrawingInfo>();
			foreach (int cell in loop - extraCell)
			{
				candidateOffsets.Add(new(0, cell * 9 + d1));
				candidateOffsets.Add(new(0, cell * 9 + d2));
			}

			accumulator.AddIfDoesNotContain(
				new BivalueOddagonType1StepInfo(
					conclusions,
					new View[] { new() { Candidates = candidateOffsets, Links = links } },
					loop,
					d1,
					d2,
					extraCell));
		}
	}
}
