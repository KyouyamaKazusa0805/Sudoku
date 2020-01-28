using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Uniqueness.Rectangles
{
	/// <summary>
	/// Encapsulates a unique rectangle technique searcher.
	/// </summary>
	public sealed partial class UniqueRectangleTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the solver should check incompleted URs.
		/// </summary>
		private readonly bool _checkIncompleted;


		/// <summary>
		/// Initializes an instance with the checking option.
		/// </summary>
		/// <param name="checkIncompletedUniquenessPatterns">
		/// Indicates whether the solver should check incompleted URs.
		/// </param>
		public UniqueRectangleTechniqueSearcher(bool checkIncompletedUniquenessPatterns) =>
			_checkIncompleted = checkIncompletedUniquenessPatterns;


		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			var result = new List<UniquenessTechniqueInfo>();

			foreach (int[] cells in TraversingTable)
			{
				// TODO: Consider the pattern below:
				// a b
				// b -ac
				// , where '-ac' means digit 'a' and 'c' can be eliminated,
				// which is a incompleted pattern.

				var bivalueCells = new List<int>();
				var extraCells = new List<int>();
				foreach (int cell in cells)
				{
					short cellMask = (short)(grid.GetMask(cell) & 511);
					if (cellMask.CountSet() == 7)
					{
						bivalueCells.Add(cell);
					}
					else
					{
						extraCells.Add(cell);
					}
				}

				short bivalueCellsMask = 0;
				foreach (int cell in bivalueCells)
				{
					bivalueCellsMask |= grid.GetMask(cell);
				}
				bivalueCellsMask &= 511;

				int z = ~bivalueCellsMask & 511;
				if (extraCells.Count == 1 && z.CountSet() == 2)
				{
					// UR type 1 found.
					var conclusions = new List<Conclusion>();
					var digits = z.GetAllSets();
					foreach (int digit in digits)
					{
						int extraCell = extraCells[0];
						if (grid.CandidateExists(extraCell, digit))
						{
							conclusions.Add(
								new Conclusion(
									ConclusionType.Elimination, extraCell * 9 + digit));
						}
					}

					int elimCount = conclusions.Count;
					if (elimCount != 0 && (
						_checkIncompleted && elimCount == 1
						|| !_checkIncompleted && elimCount == 2))
					{
						// Add all highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						foreach (int cell in bivalueCells)
						{
							foreach (int digit in digits)
							{
								if (grid.CandidateExists(cell, digit))
								{
									candidateOffsets.Add((0, cell * 9 + digit));
								}
							}
						}
						var displayCells = new List<int>();
						displayCells.AddRange(bivalueCells);
						displayCells.Add(extraCells[0]);

						result.Add(
							new UniqueRectangleTechniqueInfo(
								conclusions,
								views: new List<View>
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: null,
										linkMasks: null)
								},
								detailData: new UniqueRectangleType1DetailData(
									cells: displayCells.ToArray(),
									digits: digits.ToArray())));
					}
				}
			}

			return result;
		}
	}
}
