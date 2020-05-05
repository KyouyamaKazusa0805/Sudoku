using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.GridMap.InitializeOption;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates an <b>almost locked sets XY-Wing</b> technique.
	/// </summary>
	[TechniqueDisplay("Almost Locked Sets XY-Wing")]
	public sealed class AlsXyWingTechniqueSearcher : AlsTechniqueSearcher
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
		public AlsXyWingTechniqueSearcher(bool allowOverlapping, bool alsShowRegions) =>
			(_allowOverlapping, _alsShowRegions) = (allowOverlapping, alsShowRegions);


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 60;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var span = (Span<(Als, Als, Als, Als)>)stackalloc (Als, Als, Als, Als)[4];
			var rccs = Rcc.GetAllRccs(grid, _allowOverlapping).ToArray();
			for (int i = 0, length = rccs.Length; i < length - 1; i++)
			{
				var rcc1 = rccs[i];
				for (int j = i + 1; j < length; j++)
				{
					var rcc2 = rccs[j];

					var (als11, als12, commonDigit1, commonRegion1) = rcc1;
					var (als21, als22, commonDigit2, commonRegion2) = rcc2;

					if (commonDigit1 == commonDigit2 || commonRegion1 == commonRegion2)
					{
						continue;
					}

					Als l = default, m = default, r = default;
					bool findSame = false;
					span[0] = (als11, als21, als12, als22);
					span[1] = (als11, als22, als12, als21);
					span[2] = (als12, als21, als11, als22);
					span[3] = (als12, als22, als11, als21);
					foreach (var (als1, als2, als3, als4) in span)
					{
						if (als1 == als2)
						{
							(l, m, r) = (als3, als1, als4);
							findSame = true;
							break;
						}
					}
					if (!findSame)
					{
						continue;
					}

					// 'l' and 'r' are the result ALSes. ALS-XY-Wing found.
					// Now we should check elimination.
					// But firstly, we should check all digits appearing
					// in two ALSes.
					var (region1, _, digitMask1, relativePos1, digits1, map1) = l;
					var (region2, _, digitMask2, relativePos2, digits2, map2) = r;
					foreach (int elimDigit in (digitMask1 | digitMask2).GetAllSets())
					{
						if (elimDigit == commonDigit1 || elimDigit == commonDigit2)
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
						var als1RegionCells =
							from pos in relativePos1 select RegionCells[region1][pos];
						var als2RegionCells =
							from pos in relativePos2 select RegionCells[region2][pos];
						foreach (int cell in als1RegionCells)
						{
							if (!(grid.Exists(cell, elimDigit) is true))
							{
								continue;
							}

							tempList.Add(cell);
						}
						foreach (int cell in als2RegionCells)
						{
							if (!(grid.Exists(cell, elimDigit) is true))
							{
								continue;
							}

							tempList.Add(cell);
						}

						var elimMap = new GridMap(tempList, ProcessPeersWithoutItself);
						if (elimMap.IsEmpty)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int cell in elimMap.Offsets)
						{
							if (grid.Exists(cell, elimDigit) is true)
							{
								conclusions.Add(new Conclusion(Elimination, cell, elimDigit));
							}
						}

						if (conclusions.Count == 0)
						{
							continue;
						}

						// Record highlight cells.
						var cellOffsets = new List<(int, int)>();
						cellOffsets.AddRange(from cell in map1.Offsets select (0, cell));
						cellOffsets.AddRange(from cell in map2.Offsets select (1, cell));
						cellOffsets.AddRange(from cell in m.Map.Offsets select (2, cell));

						// Record highlight candidates.
						var candidateOffsets = new List<(int, int)>();
						foreach (int cell in als1RegionCells)
						{
							foreach (int als1Digit in grid.GetCandidatesReversal(cell).GetAllSets())
							{
								int z = -1;
								if (als1Digit == commonDigit1) z = 1;
								else if (als1Digit == elimDigit) z = 2;

								candidateOffsets.Add((z, cell * 9 + als1Digit));
							}
						}
						foreach (int cell in als2RegionCells)
						{
							foreach (int als2Digit in grid.GetCandidatesReversal(cell).GetAllSets())
							{
								int z = -2;
								if (als2Digit == commonDigit2) z = 1;
								else if (als2Digit == elimDigit) z = 2;

								candidateOffsets.Add((z, cell * 9 + als2Digit));
							}
						}
						foreach (int cell in m.Map.Offsets)
						{
							foreach (int digit in grid.GetCandidatesReversal(cell).GetAllSets())
							{
								int z = -3;
								if (digit == commonDigit1 || digit == commonDigit2) z = 1;

								candidateOffsets.Add((z, cell * 9 + digit));
							}
						}

						accumulator.AddIfDoesNotContain(
							new AlsXyWingTechniqueInfo(
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
											true => new[] { (0, region1), (1, region2), (2, m.Region) },
											false => null
										},
										links: null)
								},
								rcc1,
								rcc2));
					}
				}
			}
		}
	}
}
