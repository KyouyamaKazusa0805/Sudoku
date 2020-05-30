using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Encapsulates an empty rectangle technique searcher.
	/// </summary>
	[TechniqueDisplay("Empty Rectangle")]
	public sealed class EmptyRectangleTechniqueSearcher : SdpTechniqueSearcher
	{
		/// <summary>
		/// Indicates all regions iterating on the specified block
		/// forming an empty rectangle.
		/// </summary>
		private static readonly int[,] LinkIds =
		{
			{ 12, 13, 14, 15, 16, 17, 21, 22, 23, 24, 25, 26 },
			{ 12, 13, 14, 15, 16, 17, 18, 19, 20, 24, 25, 26 },
			{ 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 },
			{ 9, 10, 11, 15, 16, 17, 21, 22, 23, 24, 25, 26 },
			{ 9, 10, 11, 15, 16, 17, 18, 19, 20, 24, 25, 26 },
			{ 9, 10, 11, 15, 16, 17, 18, 19, 20, 21, 22, 23 },
			{ 9, 10, 11, 12, 13, 14, 21, 22, 23, 24, 25, 26 },
			{ 9, 10, 11, 12, 13, 14, 18, 19, 20, 24, 25, 26 },
			{ 9, 10, 11, 12, 13, 14, 18, 19, 20, 21, 22, 23 }
		};


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 46;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				for (int block = 0; block < 9; block++)
				{
					// Check the empty rectangle occupies more than 2 cells.
					// and the structure forms an empty rectangle.
					var erMap = CandMaps[digit] & RegionMaps[block];
					if (erMap.Count < 2 || !IsEmptyRectangle(erMap, block, out int row, out int column))
					{
						continue;
					}

					// Search for conjugate pair.
					for (int i = 0; i < 12; i++)
					{
						var linkMap = CandMaps[digit] & RegionMaps[LinkIds[block, i]];
						if (linkMap.Count != 2)
						{
							continue;
						}

						if (linkMap.BlockMask.IsPowerOfTwo()
							|| i < 6 && (linkMap & RegionMaps[column]).IsEmpty
							|| i >= 6 && (linkMap & RegionMaps[row]).IsEmpty)
						{
							continue;
						}

						int[] t = (linkMap - (i < 6 ? RegionMaps[column] : RegionMaps[row])).ToArray();
						int elimRegion = i < 6 ? t[0] % 9 + 18 : t[0] / 9 + 9;
						var elimCellMap = i < 6
							? CandMaps[digit] & RegionMaps[elimRegion] & RegionMaps[row]
							: CandMaps[digit] & RegionMaps[elimRegion] & RegionMaps[column];

						if (elimCellMap.IsEmpty)
						{
							continue;
						}

						int elimCell = elimCellMap.SetAt(0);
						if (!(grid.Exists(elimCell, digit) is true))
						{
							continue;
						}

						// Record all highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						var cpCells = new List<int>(2);
						foreach (int cell in RegionMaps[block] & CandMaps[digit])
						{
							candidateOffsets.Add((1, cell * 9 + digit));
						}
						foreach (int cell in linkMap)
						{
							candidateOffsets.Add((0, cell * 9 + digit));
							cpCells.Add(cell);
						}

						// Empty rectangle.
						accumulator.Add(
							new EmptyRectangleTechniqueInfo(
								conclusions: new[] { new Conclusion(Elimination, elimCell, digit) },
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: new[] { (0, block) },
										links: null)
								},
								digit,
								block,
								conjugatePair: new ConjugatePair(cpCells[0], cpCells[1], digit)));
					}
				}
			}
		}

		/// <summary>
		/// Check whether the cells form an empty cell.
		/// </summary>
		/// <param name="blockMap">The empty cell grid map.</param>
		/// <param name="block">The block.</param>
		/// <param name="row">(<see langword="out"/> parameter) The row.</param>
		/// <param name="column">(<see langword="out"/> parameter) The column.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		private bool IsEmptyRectangle(GridMap blockMap, int block, out int row, out int column)
		{
			int r = block / 3 * 3 + 9;
			int c = block % 3 * 3 + 18;
			for (int i = r, count = 0; i < r + 3; i++)
			{
				if ((blockMap & RegionMaps[i]).IsNotEmpty || ++count <= 1)
				{
					continue;
				}

				row = column = -1;
				return false;
			}

			for (int i = c, count = 0; i < c + 3; i++)
			{
				if ((blockMap & RegionMaps[i]).IsNotEmpty || ++count <= 1)
				{
					continue;
				}

				row = column = -1;
				return false;
			}

			for (int i = r; i < r + 3; i++)
			{
				for (int j = c; j < c + 3; j++)
				{
					if ((blockMap - (RegionMaps[i] | RegionMaps[j])).IsNotEmpty)
					{
						continue;
					}

					(row, column) = (i, j);
					return true;
				}
			}

			row = column = -1;
			return false;
		}
	}
}
