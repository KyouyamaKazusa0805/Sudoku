using System;
using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using static Sudoku.Constants.Processings;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Wings.Regular
{
	/// <summary>
	/// Encapsulates a <b>regular wing</b> technique solver.
	/// </summary>
	public sealed class RegularWingStepSearcher : WingStepSearcher
	{
		/// <summary>
		/// The size.
		/// </summary>
		private readonly int _size;


		/// <summary>
		/// Initializes an instance with the specified size.
		/// </summary>
		/// <param name="size">The size.</param>
		public RegularWingStepSearcher(int size) => _size = size;


		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(6, nameof(TechniqueCode.XyWing))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			// Iterate on the size.
			// Note that the greatest size is determined by two factors: the size that you specified
			// and the number of bi-value cells in the grid.
			for (int size = 3, count = Math.Min(_size, BivalueMap.Count); size <= count; size++)
			{
				// Iterate on each pivot cell.
				foreach (int pivot in EmptyMap)
				{
					short mask = grid.GetCandidates(pivot);
					int candsCount = mask.PopCount();
					if (candsCount != size && candsCount != size - 1)
					{
						// Candidates are not enough.
						continue;
					}

					var map = PeerMaps[pivot] & BivalueMap;
					if (map.Count < size - 1)
					{
						// Bivalue cells are not enough.
						continue;
					}

					// Iterate on each cell combination.
					foreach (int[] cells in map.ToArray().GetSubsets(size - 1))
					{
						// Check duplicate.
						// If two cells contain same candidates, the wing can't be formed.
						bool flag = false;
						for (int i = 0, length = cells.Length; i < length - 1; i++)
						{
							for (int j = i + 1; j < length; j++)
							{
								if (grid.GetMask(cells[i]) == grid.GetMask(cells[j]))
								{
									flag = true;
									goto Determine;
								}
							}
						}

					Determine:
						if (flag)
						{
							continue;
						}

						short union = mask, inter = (short)(SudokuGrid.MaxCandidatesMask & mask);
						foreach (int cell in cells)
						{
							short m = grid.GetCandidates(cell);
							union |= m;
							inter &= m;
						}

						if (union.PopCount() != size || inter != 0 && !inter.IsPowerOfTwo())
						{
							continue;
						}

						// Get the Z digit (The removing value).
						bool isIncomplete = inter == 0;
						short interWithoutPivot = (short)(union & ~grid.GetCandidates(pivot));
						short maskToCheck = isIncomplete ? interWithoutPivot : inter;
						if (!maskToCheck.IsPowerOfTwo())
						{
							continue;
						}

						// The pattern should be "az, bz, cz, dz, ... , abcd(z)".
						int zDigit = maskToCheck.FindFirstSet();
						var cellsMap = new Cells(cells);
						if ((cellsMap + pivot & CandMaps[zDigit]).Count != (isIncomplete ? size - 1 : size))
						{
							continue;
						}

						// Check elimination map.
						var elimMap = cellsMap.PeerIntersection;
						if (!isIncomplete)
						{
							elimMap &= PeerMaps[pivot];
						}
						elimMap &= CandMaps[zDigit];
						if (elimMap.IsEmpty)
						{
							continue;
						}

						// Gather the eliminations.
						var conclusions = new List<Conclusion>();
						foreach (int cell in elimMap)
						{
							conclusions.Add(new(ConclusionType.Elimination, cell, zDigit));
						}

						// Gather highlight candidates.
						var candidateOffsets = new List<DrawingInfo>();
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add(new(digit == zDigit ? 1 : 0, cell * 9 + digit));
							}
						}
						foreach (int digit in grid.GetCandidates(pivot))
						{
							candidateOffsets.Add(new(digit == zDigit ? 1 : 0, pivot * 9 + digit));
						}

						accumulator.Add(
							new RegularWingStepInfo(
								conclusions,
								new View[] { new() { Candidates = candidateOffsets } },
								pivot,
								mask.PopCount(),
								union,
								cells));
					}
				}
			}
		}
	}
}
