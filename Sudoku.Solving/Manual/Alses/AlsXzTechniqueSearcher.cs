using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates an <b>ALS-XZ</b> technique.
	/// </summary>
	public sealed class AlsXzTechniqueSearcher : AlsTechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the ALSes can be overlapped with each other.
		/// </summary>
		private readonly bool _allowOverlapping;


		/// <summary>
		/// Initialize an instance with the specified information.
		/// </summary>
		/// <param name="allowOverlapping">
		/// Indicates whether the ALSes can be overlapped with each other.
		/// </param>
		public AlsXzTechniqueSearcher(bool allowOverlapping) =>
			_allowOverlapping = allowOverlapping;


		/// <inheritdoc/>
		public override int Priority { get; set; } = 55;


		/// <inheritdoc/>
		public override void AccumulateAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			foreach (var rcc in Rcc.GetAllRccs(grid, _allowOverlapping))
			{
				var (als1, als2, commonDigit, region) = rcc;

				// ALS-XZ found.
				// Now we should check elimination.
				// But firstly, we should check all digits appearing
				// in two ALSes.
				foreach (int elimDigit in (als1.DigitsMask | als2.DigitsMask).GetAllSets())
				{
					if (elimDigit == commonDigit)
					{
						continue;
					}

					// To check whether both ALSes contain this digit.
					// If not (either containing), continue to next iteration.
					if (((als1.DigitsMask ^ als2.DigitsMask) >> elimDigit & 1) != 0)
					{
						continue;
					}

					// Both ALSes contain the digit.
					// Now check elimination set.
					var tempList = new HashSet<int>();
					var als1RegionCells =
						from pos in als1.RelativePos
						select RegionUtils.GetCellOffset(als1.Region, pos);
					var als2RegionCells =
						from pos in als2.RelativePos
						select RegionUtils.GetCellOffset(als2.Region, pos);
					foreach (int cell in als1RegionCells)
					{
						if (!grid.CandidateExists(cell, elimDigit))
						{
							continue;
						}

						tempList.Add(cell);
					}
					foreach (int cell in als2RegionCells)
					{
						if (!grid.CandidateExists(cell, elimDigit))
						{
							continue;
						}

						tempList.Add(cell);
					}

					var elimMap = new GridMap(tempList, GridMap.InitializeOption.ProcessPeersWithoutItself);
					if (elimMap.Count == 0)
					{
						continue;
					}

					var conclusions = new List<Conclusion>();
					foreach (int cell in elimMap.Offsets)
					{
						if (grid.CandidateExists(cell, elimDigit))
						{
							conclusions.Add(new Conclusion(ConclusionType.Elimination, cell, elimDigit));
						}
					}

					if (conclusions.Count == 0)
					{
						continue;
					}

					// Add.
					var candidateOffsets = new List<(int, int)>();
					foreach (int cell in als1RegionCells)
					{
						foreach (int als1Digit in grid.GetCandidatesReversal(cell).GetAllSets())
						{
							int z = -1;
							if (als1Digit == commonDigit) z = 1;
							else if (als1Digit == elimDigit) z = 2;

							candidateOffsets.Add((z, cell * 9 + als1Digit));
						}
					}
					foreach (int cell in als2RegionCells)
					{
						foreach (int als2Digit in grid.GetCandidatesReversal(cell).GetAllSets())
						{
							int z = -2;
							if (als2Digit == commonDigit) z = 1;
							else if (als2Digit == elimDigit) z = 2;

							candidateOffsets.Add((z, cell * 9 + als2Digit));
						}
					}

					accumulator.Add(
						new AlsXzTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets,
									regionOffsets: new[] { (0, als1.Region), (0, als2.Region) },
									links: null)
							},
							rcc));
				}
			}
		}


		/// <summary>
		/// To search for all ALSes in the specified grid and the region to iterate on.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="region">The region.</param>
		/// <returns>All ALSes searched.</returns>
		private static IEnumerable<Als> GetAllAlses(IReadOnlyGrid grid, int region)
		{
			short posMask = 0;
			int i = 0;
			foreach (int cell in GridMap.GetCellsIn(region))
			{
				if (grid.GetCellStatus(cell) == CellStatus.Empty)
				{
					posMask |= (short)(1 << i);
				}

				i++;
			}
			int count = posMask.CountSet();
			if (count < 2)
			{
				yield break;
			}

			for (int size = 2; size <= count; size++)
			{
				foreach (short relativePosMask in new BitCombinationGenerator(9, size))
				{
					short digitsMask = 0;
					foreach (int cell in MaskExtensions.GetCells(region, relativePosMask))
					{
						if (grid.GetCellStatus(cell) != CellStatus.Empty)
						{
							goto Label_Continue;
						}

						digitsMask |= grid.GetCandidatesReversal(cell);
					}

					if (digitsMask.CountSet() - 1 != size)
					{
						// Not an ALS.
						continue;
					}

#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
					yield return new Als(region << 18 | relativePosMask << 9 | digitsMask);
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand

				Label_Continue:;
				}
			}
		}
	}
}
