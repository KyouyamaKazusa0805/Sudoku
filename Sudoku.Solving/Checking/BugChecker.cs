using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;
using Sudoku.Solving.Utils;

namespace Sudoku.Solving.Checking
{
	/// <summary>
	/// Encapsulates a BUG technique checker.
	/// </summary>
	public sealed partial class BugChecker
	{
		/// <summary>
		/// Initializes an instance with the specified grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		public BugChecker(Grid grid) => Grid = grid;


		/// <summary>
		/// The grid.
		/// </summary>
		public Grid Grid { get; }


		/// <summary>
		/// Get all true candidates.
		/// </summary>
		/// <returns>All true candidates.</returns>
		public IReadOnlyList<int> GetAllTrueCandidates()
		{
			var emptyCellsMap = GetEmptyCellsGridMap();
			var bivalueCellsMap = GetBivalueCellsMap();
			var digitDistributions = GetAllDigitDistributionMaps();
			var allRegionsMap = GetAllRegionMaps();
			int[] array = emptyCellsMap.ToArray();

			// Get the number of multivalue cells.
			int multivalueCellsCount = 0;
			foreach (int value in array)
			{
				int candidatesCount = Grid.GetCandidatesReversal(value).CountSet();
				if (candidatesCount == 1)
				{
					return Array.Empty<int>();
				}

				if (candidatesCount > 2)
				{
					multivalueCellsCount++;
				}
			}

			var stack = new GridMap[multivalueCellsCount + 1, 9];
			int[] chosen = new int[multivalueCellsCount + 1];
			if (bivalueCellsMap.Count > 0)
			{
				int[] bivalueCells = bivalueCellsMap.ToArray();
				foreach (int bivalueCell in bivalueCells)
				{
					int[] digits = Grid.GetCandidatesReversal(bivalueCell).GetAllSets().ToArray();
					for (int j = 0; j < 2; j++)
					{
						int digit = digits[j];
						stack[0, digit][bivalueCell] = true;

						var (r, c, b) = CellUtils.GetRegion(bivalueCell);
						var span = (Span<int>)stackalloc[] { r + 9, c + 18, b };
						for (int k = 0; k < 3; k++)
						{
							if ((stack[0, digit] & allRegionsMap[span[k]]).Count > 2)
							{
								return Array.Empty<int>();
							}
						}
					}
				}
			}

			short mask = default;
			short[,] pairs = new short[multivalueCellsCount, 37];
			int[] multivalueCellsMap = (emptyCellsMap - bivalueCellsMap).ToArray();
			for (int i = 0; i < multivalueCellsMap.Length; i++)
			{
				mask = Grid.GetCandidatesReversal(multivalueCellsMap[i]);
				short[] list = GetAllCombinations(mask, 2);
				pairs[i, 0] = (short)list.Length;
				pairs[i, 1] = list[0];
			}

			int pt = 1;
			var resultMap = new GridMap[9];
			var result = new List<int>();
			do
			{
				int i;
				int ps = multivalueCellsMap[pt - 1];
				bool @continue = false;
				for (i = chosen[pt] + 1; i < pairs[pt - 1, 0] + 1; i++)
				{
					@continue = true;
					mask = pairs[pt - 1, i];
					for (int j = 0; j < 2; j++)
					{
						int digit = mask.GetAllSets().ToArray()[j];
						var temp = stack[pt - 1, digit];
						temp[ps] = true;
						var (r, c, b) = CellUtils.GetRegion(ps);
						var span = (Span<int>)stackalloc[] { r + 9, c + 18, b };
						for (int k = 0; k < 3; k++)
						{
							if ((temp & allRegionsMap[span[k]]).Count > 2)
							{
								@continue = false;
								break;
							}
						}

						if (!@continue)
						{
							break;
						}
					}

					if (@continue)
					{
						break;
					}
				}

				if (@continue)
				{
					for (int z = 0; z < 9; z++)
					{
						stack[pt, z] = stack[pt - 1, z];
					}

					chosen[pt] = i;
					int[] digits = mask.GetAllSets().ToArray();
					stack[pt, digits[0]][ps] = true;
					stack[pt, digits[1]][ps] = true;
					if (pt == multivalueCellsCount)
					{
						for (int k = 0; k < 9; k++)
						{
							ref var map = ref resultMap[k];
							map = digitDistributions[k] - stack[pt, k];
							foreach (int cell in map.Offsets)
							{
								result.Add(cell * 9 + k);
							}
						}

						return result;
					}

					pt++;
				}
				else
				{
					chosen[pt] = 0;
					pt--;
				}
			} while (pt > 0);

			return result;
		}

		/// <summary>
		/// Get the grid map instance with all bivalue cells
		/// set <see langword="true"/>.
		/// </summary>
		/// <returns>The result.</returns>
		private GridMap GetBivalueCellsMap()
		{
			var result = GridMap.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				var (status, candidates) = Grid.GetMask(cell);
				if (status == CellStatus.Empty && candidates.CountSet() == 7)
				{
					result[cell] = true;
				}
			}

			return result;
		}

		/// <summary>
		/// Get the grid map instance with all empty cells set
		/// <see langword="true"/>.
		/// </summary>
		/// <returns>The result.</returns>
		private GridMap GetEmptyCellsGridMap()
		{
			var result = GridMap.Empty;
			for (int cell = 0; cell < 81; cell++)
			{
				(var status, _) = Grid.GetMask(cell);
				if (status == CellStatus.Empty)
				{
					result[cell] = true;
				}
			}

			return result;
		}

		/// <summary>
		/// Get 9 grid maps representing all digits' distributions.
		/// </summary>
		/// <returns>All grid maps.</returns>
		private GridMap[] GetAllDigitDistributionMaps()
		{
			var result = new GridMap[9];
			for (int i = 0; i < 9; i++)
			{
				ref var map = ref result[i];
				map = new GridMap();

				for (int cell = 0; cell < 81; cell++)
				{
					if (Grid.CandidateExists(cell, i))
					{
						map[cell] = true;
					}
				}
			}

			return result;
		}


		/// <summary>
		/// Get all combinations of a specified mask.
		/// </summary>
		/// <param name="mask">The mask.</param>
		/// <param name="oneCount">
		/// The number of <see langword="true"/> bits.
		/// </param>
		/// <returns>All combinations.</returns>
		private static short[] GetAllCombinations(short mask, int oneCount)
		{
			var result = new List<short>();
			foreach (short z in new BitEnumerator(9, oneCount))
			{
				if ((mask | z) == mask)
				{
					result.Add(z);
				}
			}

			return result.ToArray();
		}

		/// <summary>
		/// Get all grid maps about all regions (all cells lie on
		/// specified region will be set <see langword="true"/>).
		/// </summary>
		/// <returns>The grid maps.</returns>
		private static GridMap[] GetAllRegionMaps()
		{
			var result = new GridMap[27];
			for (int region = 0; region < 27; region++)
			{
				foreach (int offset in GridMap.GetCellsIn(region))
				{
					result[region][offset] = true;
				}
			}

			return result;
		}
	}
}
