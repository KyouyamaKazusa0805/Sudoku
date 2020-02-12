using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Manual.SingleDigitPatterns
{
	/// <summary>
	/// Encapsulates an empty rectangle technique searcher.
	/// </summary>
	public sealed class EmptyRectangleTechniqueSearcher : SingleDigitPatternTechniqueSearcher
	{
		/// <summary>
		/// Indicates all regions iterating on the specified block
		/// forming an empty rectangle.
		/// </summary>
		private static readonly int[,] LinkIds = new int[,]
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
		/// All digit distributions.
		/// </summary>
		private GridMap[] _digitDistributions = null!;

		/// <summary>
		/// All region grid maps.
		/// </summary>
		private readonly GridMap[] _regionMaps;


		/// <summary>
		/// Initializes an instance with the specified grid.
		/// </summary>
		/// <param name="regionMaps">All region maps.</param>
		public EmptyRectangleTechniqueSearcher(GridMap[] regionMaps) => _regionMaps = regionMaps;


		/// <inheritdoc/>
		public override IReadOnlyList<TechniqueInfo> TakeAll(Grid grid)
		{
			(_, _, _digitDistributions) = grid;

			var result = new List<EmptyRectangleTechniqueInfo>();

			for (int digit = 0; digit < 9; digit++)
			{
				for (int block = 0; block < 9; block++)
				{
					// Check the empty rectangle occupies more than 2 cells.
					// and the structure forms an empty rectangle.
					var erMap = _digitDistributions[digit] & _regionMaps[block];
					if (erMap.Count < 2
						|| !IsEmptyRectangle(erMap, block, out int row, out int column))
					{
						continue;
					}

					// Search for conjugate pair.
					for (int i = 0; i < 12; i++)
					{
						var linkMap = _digitDistributions[digit] & _regionMaps[LinkIds[block, i]];
						if (linkMap.Count != 2)
						{
							continue;
						}

						if (linkMap.BlockMask.CountSet() == 1
							|| i < 6 && !(linkMap & _regionMaps[column])
							|| i >= 6 && !(linkMap & _regionMaps[row]))
						{
							continue;
						}

						int[] t = (linkMap - (i < 6 ? _regionMaps[column] : _regionMaps[row])).ToArray();
						int elimRegion = i < 6 ? t[0] % 9 + 18 : t[0] / 9 + 9;
						var elimCellMap = i < 6
							? _digitDistributions[digit] & _regionMaps[elimRegion] & _regionMaps[row]
							: _digitDistributions[digit] & _regionMaps[elimRegion] & _regionMaps[column];

						if (!elimCellMap)
						{
							continue;
						}

						int elimCell = elimCellMap.Offsets.First();
						if (!grid.CandidateExists(elimCell, digit))
						{
							continue;
						}

						// Record all highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						var cpCells = new List<int>(2);
						foreach (int cell in GridMap.GetCellsIn(block))
						{
							if (grid.CandidateExists(cell, digit))
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
						}
						foreach (int cell in linkMap.Offsets)
						{
							candidateOffsets.Add((0, cell * 9 + digit));
							cpCells.Add(cell);
						}

						// Empty rectangle.
						result.Add(
							new EmptyRectangleTechniqueInfo(
								conclusions: new[]
								{
									new Conclusion(ConclusionType.Elimination, elimCell * 9 + digit)
								},
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: new[] { (0, block) },
										linkMasks: null)
								},
								digit,
								block,
								conjugatePair: new ConjugatePair(cpCells[0], cpCells[1], digit)));
					}
				}
			}

			return result;
		}


		/// <summary>
		/// Check whether the cells form an empty cell.
		/// </summary>
		/// <param name="blockMap">The empty cell grid map.</param>
		/// <param name="block">The block.</param>
		/// <param name="row">(out parameter) The row.</param>
		/// <param name="column">(out parameter) The column.</param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		private bool IsEmptyRectangle(GridMap blockMap, int block, out int row, out int column)
		{
			int r = block / 3 * 3 + 9;
			int c = block % 3 * 3 + 18;
			for (int i = r, count = 0; i < r + 3; i++)
			{
				if (!(blockMap & _regionMaps[i]) && ++count > 1)
				{
					row = column = -1;
					return false;
				}
			}

			for (int i = c, count = 0; i < c + 3; i++)
			{
				if (!(blockMap & _regionMaps[i]) && ++count > 1)
				{
					row = column = -1;
					return false;
				}
			}

			for (int i = r; i < r + 3; i++)
			{
				for (int j = c; j < c + 3; j++)
				{
					if (!(blockMap - (_regionMaps[i] | _regionMaps[j])))
					{
						(row, column) = (i, j);
						return true;
					}
				}
			}

			row = column = -1;
			return false;
		}
	}
}
