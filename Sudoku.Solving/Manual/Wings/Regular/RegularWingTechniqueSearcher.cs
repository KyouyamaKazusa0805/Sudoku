using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static System.Algorithms;
using static System.Math;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Wings.Regular
{
	/// <summary>
	/// Encapsulates a <b>regular wing</b> technique solver.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.XyWing))]
	public sealed class RegularWingTechniqueSearcher : WingTechniqueSearcher
	{
		/// <summary>
		/// The size.
		/// </summary>
		private readonly int _size;


		/// <summary>
		/// Initializes an instance with the specified size.
		/// </summary>
		/// <param name="size">The size.</param>
		public RegularWingTechniqueSearcher(int size) => _size = size;


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 42;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			// Iterate on the size.
			// Note that the greatest size is determined by two factors: the size that you specified
			// and the number of bi-value cells in the grid.
			for (int size = 3, count = Min(_size, BivalueMap.Count); size <= count; size++)
			{
				// Iterate on each pivot cell.
				foreach (int pivot in EmptyMap)
				{
					short mask = grid.GetCandidateMask(pivot);
					int candsCount = mask.CountSet();
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
					foreach (int[] cells in map.ToArray().GetCombinations(size - 1))
					{
						// Check duplicate.
						// If two cells contain same candidates, the wing cannot be formed.
						bool flag = false;
						for (int i = 0, length = cells.Length; i < length - 1; i++)
						{
							for (int j = i + 1; j < length; j++)
							{
								if (grid.GetMask(cells[i]) == grid.GetMask(cells[j]))
								{
									flag = true;
									goto Label_Determine;
								}
							}
						}

					Label_Determine:
						if (flag)
						{
							continue;
						}

						short union = mask, inter = (short)(Grid.MaxCandidatesMask & mask);
						foreach (int cell in cells)
						{
							short m = grid.GetCandidateMask(cell);
							union |= m;
							inter &= m;
						}

						if (union.CountSet() != size || inter != 0 && !inter.IsPowerOfTwo())
						{
							continue;
						}

						// Get the Z digit (The removing value).
						bool isIncomplete = inter == 0;
						short interWithoutPivot = (short)(union & ~grid.GetCandidateMask(pivot));
						short maskToCheck = isIncomplete ? interWithoutPivot : inter;
						if (!maskToCheck.IsPowerOfTwo())
						{
							continue;
						}

						// The pattern should be "az, bz, cz, dz, ... , abcd(z)".
						int zDigit = maskToCheck.FindFirstSet();
						var cellsMap = new GridMap(cells);
						if (((cellsMap + pivot) & CandMaps[zDigit]).Count != (isIncomplete ? size - 1 : size))
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
							conclusions.Add(new Conclusion(Elimination, cell, zDigit));
						}

						// Gather highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add((digit == zDigit ? 1 : 0, cell * 9 + digit));
							}
						}
						foreach (int digit in grid.GetCandidates(pivot))
						{
							candidateOffsets.Add((digit == zDigit ? 1 : 0, pivot * 9 + digit));
						}

						accumulator.Add(
							new RegularWingTechniqueInfo(
								conclusions,
								views: new[] { new View(candidateOffsets) },
								pivot,
								pivotCandidatesCount: mask.CountSet(),
								digitsMask: union,
								cellOffsets: cells));
					}
				}
			}
		}
	}
}
