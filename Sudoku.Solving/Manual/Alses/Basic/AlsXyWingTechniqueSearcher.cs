using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Alses.Basic
{
	/// <summary>
	/// Encapsulates an <b>almost locked sets XY-Wing</b> (ALS-XY-Wing) technique.
	/// </summary>
	[TechniqueDisplay("Almost Locked Sets XY-Wing")]
	public sealed class AlsXyWingTechniqueSearcher : AlsTechniqueSearcher
	{
		/// <inheritdoc/>
		public AlsXyWingTechniqueSearcher(bool allowOverlapping, bool alsShowRegions, bool allowAlsCycles)
			: base(allowOverlapping, alsShowRegions, allowAlsCycles)
		{
		}


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 60;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = true;


		/// <inheritdoc/>
		[SuppressMessage("", "IDE0004")]
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var rccs = new List<(Als _left, Als _right, short _mask)>();
			var alses = Als.GetAllAlses(grid).ToArray();

			// Gather all RCCs.
			for (int i = 0, length = alses.Length; i < length - 1; i++)
			{
				var als1 = alses[i];
				var (_, _, mask1, map1, _, _) = als1;
				for (int j = i + 1; j < length; j++)
				{
					var als2 = alses[j];
					var (_, _, mask2, map2, _, _) = als2;
					var map = map1 | map2;
					if (map.AllSetsAreInOneRegion(out _) || map1.Overlaps(map2))
					{
						continue;
					}

					short mask = (short)(mask1 & mask2);
					if (mask == 0)
					{
						continue;
					}

					short rccMask = 0;
					foreach (int digit in mask.GetAllSets())
					{
						if ((map & CandMaps[digit]).AllSetsAreInOneRegion(out _))
						{
							rccMask |= (short)(1 << digit);
						}
					}
					if (rccMask == 0)
					{
						continue;
					}

					rccs.Add((als1, als2, rccMask));
				}
			}

			// Now check them.
			for (int i = 0, count = rccs.Count; i < count - 1; i++)
			{
				var (als11, als12, mask1) = rccs[i];
				for (int j = i + 1; j < count; j++)
				{
					var (als21, als22, mask2) = rccs[j];
					if (mask1 == mask2 && mask1.IsPowerOfTwo() && mask2.IsPowerOfTwo())
					{
						// Cannot form a XY-Wing.
						continue;
					}

					if (!(als11 == als21 ^ als12 == als22 || als11 == als22 ^ als12 == als21))
					{
						continue;
					}

					// Get the logical order of three ALSes.
					var (a, b, c) = true switch
					{
						_ when als11 == als21 => (als12, als22, als11),
						_ when als11 == als22 => (als12, als21, als11),
						_ when als12 == als21 => (als11, als22, als12),
						_ => (als11, als21, als12)
					};

					var (_, aRegion, aMask, aMap, _, _) = a;
					var (_, bRegion, bMask, bMap, _, _) = b;
					var (_, cRegion, _, cMap, _, _) = c;
					var map = aMap | bMap;
					if (map == aMap || map == bMap)
					{
						continue;
					}

					if (!_allowOverlapping && (aMap.Overlaps(bMap) || aMap.Overlaps(cMap) || bMap.Overlaps(cMap)))
					{
						continue;
					}

					foreach (int digit1 in mask1.GetAllSets())
					{
						foreach (int digit2 in mask2.GetAllSets())
						{
							if (digit1 == digit2)
							{
								continue;
							}

							short finalX = (short)(1 << digit1);
							short finalY = (short)(1 << digit2);
							short digitsMask = (short)(aMask & bMask & ~(finalX | finalY));
							if (digitsMask == 0)
							{
								continue;
							}

							// Gather eliminations.
							short finalZ = 0;
							var conclusions = new List<Conclusion>();
							foreach (int digit in digitsMask.GetAllSets())
							{
								var elimMap = (
									((aMap | bMap) & CandMaps[digit]).PeerIntersection
									& CandMaps[digit]) - (aMap | bMap | cMap);
								if (elimMap.IsEmpty)
								{
									continue;
								}

								finalZ |= (short)(1 << digit);
								foreach (int cell in elimMap)
								{
									conclusions.Add(new Conclusion(Elimination, cell, digit));
								}
							}
							if (conclusions.Count == 0)
							{
								continue;
							}

							// Record highlight candidates and cells.
							var cellOffsets = new List<(int, int)>();
							var candidateOffsets = new List<(int, int)>();
							foreach (int cell in aMap)
							{
								short mask = grid.GetCandidatesReversal(cell);
								short alsDigitsMask = (short)(mask & ~(finalX | finalZ));
								short xDigitsMask = (short)(mask & (finalX));
								short zDigitsMask = (short)(mask & finalZ);
								foreach (int digit in alsDigitsMask.GetAllSets())
								{
									candidateOffsets.Add((-1, cell * 9 + digit));
								}
								foreach (int digit in xDigitsMask.GetAllSets())
								{
									candidateOffsets.Add((1, cell * 9 + digit));
								}
								foreach (int digit in zDigitsMask.GetAllSets())
								{
									candidateOffsets.Add((2, cell * 9 + digit));
								}
							}
							foreach (int cell in bMap)
							{
								short mask = grid.GetCandidatesReversal(cell);
								short alsDigitsMask = (short)(mask & ~(finalY | finalZ));
								short yDigitsMask = (short)(mask & finalY);
								short zDigitsMask = (short)(mask & finalZ);
								foreach (int digit in alsDigitsMask.GetAllSets())
								{
									candidateOffsets.Add((-1, cell * 9 + digit));
								}
								foreach (int digit in yDigitsMask.GetAllSets())
								{
									candidateOffsets.Add((1, cell * 9 + digit));
								}
								foreach (int digit in zDigitsMask.GetAllSets())
								{
									candidateOffsets.Add((2, cell * 9 + digit));
								}
							}
							foreach (int cell in cMap)
							{
								short mask = grid.GetCandidatesReversal(cell);
								short alsDigitsMask = (short)(mask & ~(finalX | finalY));
								short xyDigitsMask = (short)(mask & (finalX | finalY));
								foreach (int digit in alsDigitsMask.GetAllSets())
								{
									candidateOffsets.Add((-3, cell * 9 + digit));
								}
								foreach (int digit in xyDigitsMask.GetAllSets())
								{
									candidateOffsets.Add((1, cell * 9 + digit));
								}
							}

							accumulator.Add(
								new AlsXyWingTechniqueInfo(
									conclusions,
									views: new[]
									{
										new View(
											cellOffsets: _alsShowRegions ? null : cellOffsets,
											candidateOffsets: _alsShowRegions ? candidateOffsets : null,
											regionOffsets:
												_alsShowRegions
													? new[] { (-1, aRegion), (-2, bRegion), (-3, cRegion) }
													: null,
											links: null)
									},
									als1: a,
									als2: b,
									bridgeAls: c,
									xDigitsMask: finalX,
									yDigitsMask: finalY,
									zDigitsMask: finalZ));
						}
					}
				}
			}
		}
	}
}
