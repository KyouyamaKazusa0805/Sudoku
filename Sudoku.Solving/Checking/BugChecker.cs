using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Extensions;
using Sudoku.Solving.Utils;
using static Sudoku.GridProcessings;

namespace Sudoku.Solving.Checking
{
	/// <summary>
	/// Encapsulates a BUG technique checker.
	/// </summary>
	public sealed class BugChecker
	{
		/// <summary>
		/// The distribution of all empty cells.
		/// </summary>
		private readonly GridMap _emptyMap;

		/// <summary>
		/// The distribution of all bivalue cells.
		/// </summary>
		private readonly GridMap _bivalueMap;

		/// <summary>
		/// The distribution of all digits.
		/// </summary>
		private readonly GridMap[] _candMaps;


		/// <summary>
		/// Initializes an instance with the specified grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		public BugChecker(IReadOnlyGrid grid)
		{
			if (grid.IsValid(out _))
			{
				(_emptyMap, _bivalueMap, _candMaps, _) = Grid = grid;
			}
			else
			{
				throw new ArgumentException(
					"The specified grid does not have a unique solution.", nameof(grid));
			}
		}


		/// <summary>
		/// Indicates the current grid is a BUG+n pattern.
		/// </summary>
		public bool IsBugPattern => GetAllTrueCandidates().Count != 0;

		/// <summary>
		/// The grid.
		/// </summary>
		public IReadOnlyGrid Grid { get; }

		/// <summary>
		/// Indicates all true candidates (non-BUG candidates).
		/// </summary>
		public IReadOnlyList<int> TrueCandidates => GetAllTrueCandidates();

		/// <summary>
		/// Get all true candidates when the number of empty cells
		/// is below than the argument.
		/// </summary>
		/// <param name="maximumEmptyCells">The maximum number of the empty cells.</param>
		/// <returns>All true candidates.</returns>
		public IReadOnlyList<int> GetAllTrueCandidates(int maximumEmptyCells)
		{
			var allRegionsMap = GetAllRegionMaps();
			int[] array = _emptyMap.ToArray();

			// Get the number of multivalue cells.
			int multivalueCellsCount = 0;
			foreach (int value in array)
			{
				int candidatesCount = Grid.GetCandidatesReversal(value).CountSet();
				if (candidatesCount == 1 || candidatesCount > 2 && ++multivalueCellsCount > maximumEmptyCells)
				{
					return Array.Empty<int>();
				}
			}

			// Store all bivalue cells.
			var stack = new GridMap[multivalueCellsCount + 1, 9];
			if (_bivalueMap.IsNotEmpty)
			{
				int[] bivalueCells = _bivalueMap.ToArray();
				foreach (int bivalueCell in bivalueCells)
				{
					int[] digits = Grid.GetCandidatesReversal(bivalueCell).GetAllSets().ToArray();
					for (int j = 0; j < 2; j++)
					{
						int digit = digits[j];
						ref var map = ref stack[0, digit];
						map.Add(bivalueCell);

						var (r, c, b) = CellUtils.GetRegion(bivalueCell);
						var span = (Span<int>)stackalloc[] { r + 9, c + 18, b };
						for (int k = 0; k < 3; k++)
						{
							if ((map & allRegionsMap[span[k]]).Count > 2)
							{
								return Array.Empty<int>();
							}
						}
					}
				}
			}

			// Store all multivalue cells.
			short mask = default;
			short[,] pairs = new short[multivalueCellsCount, 37];
			int[] multivalueCellsMap = (_emptyMap - _bivalueMap).ToArray();
			for (int i = 0; i < multivalueCellsMap.Length; i++)
			{
				mask = Grid.GetCandidatesReversal(multivalueCellsMap[i]);
				short[] list = GetAllCombinations(mask, 2);
				pairs[i, 0] = (short)list.Length;

				for (int z = 1; z <= list.Length; z++)
				{
					pairs[i, z] = list[z - 1];
				}
			}

			var playground = (Span<int>)stackalloc[] { 0, 0, 0 };
			int pt = 1;
			int[] chosen = new int[multivalueCellsCount + 1];
			var resultMap = new GridMap[9];
			var result = new List<int>();
			do
			{
				int i;
				int ps = multivalueCellsMap[pt - 1];
				bool @continue = false;
				for (i = chosen[pt] + 1; i <= pairs[pt - 1, 0]; i++)
				{
					@continue = true;
					mask = pairs[pt - 1, i];
					for (int j = 0; j < 2; j++)
					{
						var temp = stack[pt - 1, mask.SetAt(j)];
						temp.Add(ps);
						var (r, c, b) = CellUtils.GetRegion(ps);

						// Use 'stackalloc' frequently may destroy the call stack.
						// So we should use a playground is OK.
						playground[0] = b;
						playground[1] = r + 9;
						playground[2] = c + 18;
						for (int k = 0; k < 3; k++)
						{
							if ((temp & allRegionsMap[playground[k]]).Count > 2)
							{
								@continue = false;
								break;
							}
						}

						if (!@continue) break;
					}

					if (@continue) break;
				}

				if (@continue)
				{
					for (int z = 0; z < stack.GetLength(1); z++)
					{
						stack[pt, z] = stack[pt - 1, z];
					}

					chosen[pt] = i;
					var digits = mask.GetAllSets();
					stack[pt, digits.ElementAt(0)].Add(ps);
					stack[pt, digits.ElementAt(1)].Add(ps);
					if (pt == multivalueCellsCount)
					{
						for (int k = 0; k < 9; k++)
						{
							ref var map = ref resultMap[k];
							map = _candMaps[k] - stack[pt, k];
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
		/// Get all true candidates.
		/// </summary>
		/// <returns>All true candidates.</returns>
		public IReadOnlyList<int> GetAllTrueCandidates() => GetAllTrueCandidates(20);


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
			foreach (short z in new BitCombinationGenerator(9, oneCount))
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
				foreach (int offset in RegionCells[region])
				{
					result[region].Add(offset);
				}
			}

			return result;
		}
	}
}
