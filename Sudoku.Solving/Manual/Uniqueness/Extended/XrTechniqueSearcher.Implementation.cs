using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using System.Collections.Generic;
using System.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Uniqueness.Extended
{
	partial class XrTechniqueSearcher
	{
		/// <summary>
		/// Check type 1.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="allCellsMap">(<see langword="in"/> parameter) The map of all cells used.</param>
		/// <param name="extraCells">(<see langword="in"/> parameter) The extra cells map.</param>
		/// <param name="normalDigits">The normal digits mask.</param>
		/// <param name="extraDigit">The extra digit.</param>
		partial void CheckType1(
			IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap allCellsMap,
			in GridMap extraCells, short normalDigits, int extraDigit)
		{
			var conclusions = new List<Conclusion>();
			var candidateOffsets = new List<DrawingInfo>();
			foreach (int cell in allCellsMap)
			{
				if (cell == extraCells.First)
				{
					foreach (int digit in grid.GetCandidateMask(cell))
					{
						if (digit != extraDigit)
						{
							conclusions.Add(new(Elimination, cell, digit));
						}
					}
				}
				else
				{
					foreach (int digit in grid.GetCandidateMask(cell))
					{
						candidateOffsets.Add(new(0, cell * 9 + digit));
					}
				}
			}

			if (conclusions.Count == 0)
			{
				return;
			}

			accumulator.Add(
				new XrType1TechniqueInfo(
					conclusions,
					new View[] { new(candidateOffsets) },
					allCellsMap,
					normalDigits));
		}

		/// <summary>
		/// Check type 2.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="allCellsMap">(<see langword="in"/> parameter) The map of all cells used.</param>
		/// <param name="extraCells">(<see langword="in"/> parameter) The extra cells map.</param>
		/// <param name="normalDigits">The normal digits mask.</param>
		/// <param name="extraDigit">The extra digit.</param>
		partial void CheckType2(
			IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap allCellsMap,
			in GridMap extraCells, short normalDigits, int extraDigit)
		{
			var elimMap = extraCells.PeerIntersection & CandMaps[extraDigit];
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			var candidateOffsets = new List<DrawingInfo>();
			foreach (int cell in elimMap)
			{
				conclusions.Add(new(Elimination, cell, extraDigit));
			}

			foreach (int cell in allCellsMap)
			{
				foreach (int digit in grid.GetCandidateMask(cell))
				{
					candidateOffsets.Add(new(digit == extraDigit ? 1 : 0, cell * 9 + digit));
				}
			}

			accumulator.Add(
				new XrType2TechniqueInfo(
					conclusions,
					new View[] { new(candidateOffsets) },
					allCellsMap,
					normalDigits,
					extraDigit));
		}

		/// <summary>
		/// Check type 3.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="allCellsMap">(<see langword="in"/> parameter) The map of all cells used.</param>
		/// <param name="normalDigits">The normal digits mask.</param>
		/// <param name="extraDigits">The extra digits mask.</param>
		/// <param name="extraCellsMap">(<see langword="in"/> parameter) The map of extra cells.</param>
		partial void CheckType3Naked(
			IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap allCellsMap,
			short normalDigits, short extraDigits, in GridMap extraCellsMap)
		{
			foreach (int region in extraCellsMap.CoveredRegions)
			{
				int[] otherCells = ((RegionMaps[region] & EmptyMap) - allCellsMap).ToArray();
				for (int size = 1; size < otherCells.Length; size++)
				{
					foreach (int[] cells in otherCells.GetSubsets(size))
					{
						short mask = 0;
						foreach (int cell in cells)
						{
							mask |= grid.GetCandidateMask(cell);
						}

						if ((mask & extraDigits) != extraDigits || mask.PopCount() != size + 1)
						{
							continue;
						}

						var elimMap = (RegionMaps[region] & EmptyMap) - allCellsMap - cells;
						if (elimMap.IsEmpty)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int digit in mask)
						{
							foreach (int cell in elimMap & CandMaps[digit])
							{
								conclusions.Add(new(Elimination, cell, digit));
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<DrawingInfo>();
						foreach (int cell in allCellsMap - extraCellsMap)
						{
							foreach (int digit in grid.GetCandidateMask(cell))
							{
								candidateOffsets.Add(new(0, cell * 9 + digit));
							}
						}
						foreach (int cell in extraCellsMap)
						{
							foreach (int digit in grid.GetCandidateMask(cell))
							{
								candidateOffsets.Add(new((mask >> digit & 1) != 0 ? 1 : 0, cell * 9 + digit));
							}
						}
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidateMask(cell))
							{
								candidateOffsets.Add(new(1, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new XrType3TechniqueInfo(
								conclusions,
								new View[] { new(null, candidateOffsets, new DrawingInfo[] { new(0, region) }, null) },
								allCellsMap,
								normalDigits,
								cells,
								mask,
								region));
					}
				}
			}
		}

		/// <summary>
		/// Check type 4 and a part of type 1 that
		/// <see cref="CheckType1(IList{TechniqueInfo}, in SudokuGrid, in GridMap, in GridMap, short, int)"/>
		/// cannot be found.
		/// </summary>
		/// <param name="accumulator">The technique accumulator.</param>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <param name="allCellsMap">(<see langword="in"/> parameter) The map of all cells used.</param>
		/// <param name="normalDigits">The normal digits mask.</param>
		/// <param name="extraCellsMap">(<see langword="in"/> parameter) The map of extra cells.</param>
		partial void CheckType14(
			IList<TechniqueInfo> accumulator, in SudokuGrid grid, in GridMap allCellsMap,
			short normalDigits, in GridMap extraCellsMap)
		{
			switch (extraCellsMap.Count)
			{
				case 1:
				{
					// Type 1 found.
					// Check eliminations.
					var conclusions = new List<Conclusion>();
					int extraCell = extraCellsMap.First;
					foreach (int digit in normalDigits)
					{
						if (grid.Exists(extraCell, digit) is true)
						{
							conclusions.Add(new(Elimination, extraCell, digit));
						}
					}

					if (conclusions.Count == 0)
					{
						return;
					}

					// Record all highlight candidates.
					var candidateOffsets = new List<DrawingInfo>();
					foreach (int cell in allCellsMap)
					{
						if (cell == extraCell)
						{
							continue;
						}

						foreach (int digit in grid.GetCandidateMask(cell))
						{
							candidateOffsets.Add(new(0, cell * 9 + digit));
						}
					}

					accumulator.Add(
						new XrType1TechniqueInfo(
							conclusions,
							new View[] { new(candidateOffsets) },
							allCellsMap,
							normalDigits));

					break;
				}
				case 2:
				{
					// Type 4.
					short m1 = grid.GetCandidateMask(extraCellsMap.First);
					short m2 = grid.GetCandidateMask(extraCellsMap.SetAt(1));
					short conjugateMask = (short)(m1 & m2 & normalDigits);
					if (conjugateMask == 0)
					{
						return;
					}

					foreach (int conjugateDigit in conjugateMask)
					{
						foreach (int region in extraCellsMap.CoveredRegions)
						{
							var map = RegionMaps[region] & extraCellsMap;
							if (map != extraCellsMap || map != (CandMaps[conjugateDigit] & RegionMaps[region]))
							{
								continue;
							}

							short elimDigits = (short)(normalDigits & ~(1 << conjugateDigit));
							var conclusions = new List<Conclusion>();
							foreach (int digit in elimDigits)
							{
								foreach (int cell in extraCellsMap & CandMaps[digit])
								{
									conclusions.Add(new(Elimination, cell, digit));
								}
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							var candidateOffsets = new List<DrawingInfo>();
							foreach (int cell in allCellsMap - extraCellsMap)
							{
								foreach (int digit in grid.GetCandidateMask(cell))
								{
									candidateOffsets.Add(new(0, cell * 9 + digit));
								}
							}
							foreach (int cell in extraCellsMap)
							{
								candidateOffsets.Add(new(1, cell * 9 + conjugateDigit));
							}

							accumulator.Add(
								new XrType4TechniqueInfo(
									conclusions,
									new View[] { new(null, candidateOffsets, new DrawingInfo[] { new(0, region) }, null) },
									allCellsMap,
									normalDigits,
									new(extraCellsMap, conjugateDigit)));
						}
					}

					break;
				}
			}
		}
	}
}
