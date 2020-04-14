using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates an <b>almost locked sets XZ rule</b> or
	/// <b>extended subset principle</b> technique.
	/// </summary>
	[TechniqueDisplay("Almost Locked Sets XZ Rule")]
	public sealed class AlsXzTechniqueSearcher : AlsTechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the ALSes can be overlapped with each other.
		/// </summary>
		private readonly bool _allowOverlapping;

		/// <summary>
		/// Indicates whether the ALSes shows their region rather than cells.
		/// </summary>
		private readonly bool _alsShowRegions;


		/// <summary>
		/// Initialize an instance with the specified information.
		/// </summary>
		/// <param name="allowOverlapping">
		/// Indicates whether the ALSes can be overlapped with each other.
		/// </param>
		/// <param name="alsShowRegions">
		/// Indicates whether all ALSes shows their regions rather than cells.
		/// </param>
		public AlsXzTechniqueSearcher(bool allowOverlapping, bool alsShowRegions) =>
			(_allowOverlapping, _alsShowRegions) = (allowOverlapping, alsShowRegions);


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 55;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			foreach (var rcc in Rcc.GetAllRccs(grid, _allowOverlapping))
			{
				var (
					(region1, _, digitMask1, _, _, map1),
					(region2, _, digitMask2, _, _, map2),
					commonDigit, commonRegion) = rcc;

				// ALS-XZ found.
				// Now we should check elimination.
				// But firstly, we should check all digits appearing
				// in two ALSes.
				foreach (int elimDigit in (digitMask1 | digitMask2).GetAllSets())
				{
					// Check the current digit to iterate ('elimDigit') is same as
					// common digit. If so, the digit is always not the elimination.
					if (elimDigit == commonDigit)
					{
						continue;
					}

					// To check whether both ALSes contain this digit.
					// If not (either containing), continue to next iteration.
					if (((digitMask1 ^ digitMask2) >> elimDigit & 1) != 0)
					{
						continue;
					}

					// Both ALSes contain the digit.
					// Now check elimination set.
					var tempList = new HashSet<int>();
					foreach (int cell in map1.Offsets)
					{
						if (!(grid.Exists(cell, elimDigit) is true))
						{
							continue;
						}

						tempList.Add(cell);
					}
					foreach (int cell in map2.Offsets)
					{
						if (!(grid.Exists(cell, elimDigit) is true))
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
						if (!(grid.Exists(cell, elimDigit) is true))
						{
							continue;
						}

						conclusions.Add(new Conclusion(ConclusionType.Elimination, cell, elimDigit));
					}

					if (conclusions.Count == 0)
					{
						continue;
					}

					// Record highlight cells.
					var cellOffsets = new List<(int, int)>();
					cellOffsets.AddRange(from cell in map1.Offsets select (0, cell));
					cellOffsets.AddRange(from cell in map2.Offsets select (1, cell));

					// Record highlight candidates.
					var candidateOffsets = new List<(int, int)>();
					bool isEsp = map1.Count == 1 || map2.Count == 1;
					if (isEsp)
					{
						// Extended Subset principle.
						foreach (int cell in (map1 | map2).Offsets)
						{
							foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
							{
								candidateOffsets.Add((digit == elimDigit ? 1 : 0, cell * 9 + digit));
							}
						}
					}
					else
					{
						// Normal ALS-XZ.
						foreach (int cell in map1.Offsets)
						{
							foreach (int als1Digit in grid.GetCandidatesReversal(cell).GetAllSets())
							{
								int z = -1;
								if (als1Digit == commonDigit) z = 1;
								else if (als1Digit == elimDigit) z = 2;

								candidateOffsets.Add((z, cell * 9 + als1Digit));
							}
						}
						foreach (int cell in map2.Offsets)
						{
							foreach (int als2Digit in grid.GetCandidatesReversal(cell).GetAllSets())
							{
								int z = -2;
								if (als2Digit == commonDigit) z = 1;
								else if (als2Digit == elimDigit) z = 2;

								candidateOffsets.Add((z, cell * 9 + als2Digit));
							}
						}
					}

					accumulator.AddIfDoesNotContain(
						new AlsXzTechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: _alsShowRegions switch
									{
										true => null,
										false => cellOffsets
									},
									candidateOffsets: _alsShowRegions switch
									{
										true => candidateOffsets,
										false => null
									},
									regionOffsets: _alsShowRegions switch
									{
										true => isEsp ? null : new[] { (0, region1), (1, region2) },
										false => null
									},
									links: null)
							},
							rcc));
				}
			}
		}
	}
}
