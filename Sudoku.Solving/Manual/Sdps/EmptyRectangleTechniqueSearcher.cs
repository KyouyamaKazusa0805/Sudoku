using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Encapsulates an empty rectangle technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.EmptyRectangle))]
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


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(46);


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, Grid grid)
		{
			for (int digit = 0; digit < 9; digit++)
			{
				for (int block = 0; block < 9; block++)
				{
					// Check the empty rectangle occupies more than 2 cells.
					// and the structure forms an empty rectangle.
					var erMap = CandMaps[digit] & RegionMaps[block];
					if (erMap.Count < 2 || !erMap.IsEmptyRectangle(block, out int row, out int column))
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

						int elimCell = elimCellMap.First;
						if (grid.Exists(elimCell, digit) is not true)
						{
							continue;
						}

						// Record all highlight candidates.
						var candidateOffsets = new List<DrawingInfo>();
						var cpCells = new List<int>(2);
						foreach (int cell in RegionMaps[block] & CandMaps[digit])
						{
							candidateOffsets.Add(new(1, cell * 9 + digit));
						}
						foreach (int cell in linkMap)
						{
							candidateOffsets.Add(new(0, cell * 9 + digit));
							cpCells.Add(cell);
						}

						// Empty rectangle.
						accumulator.Add(
							new EmptyRectangleTechniqueInfo(
								new Conclusion[] { new(Elimination, elimCell, digit) },
								new View[] { new(null, candidateOffsets, new DrawingInfo[] { new(0, block) }, null) },
								digit,
								block,
								new(cpCells[0], cpCells[1], digit)));
					}
				}
			}
		}
	}
}
