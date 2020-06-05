using System.Collections.Generic;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual.LastResorts;
using static Sudoku.Constants.Processings;
using static Sudoku.Constants.RegionLabel;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Encapsulates a <b>guardian</b> technique searcher.
	/// </summary>
	[TechniqueDisplay("Guardian")]
	public sealed class GuardianTechniqueSearcher : SdpTechniqueSearcher
	{
		/// <summary>
		/// Indicates the invalid label (used in the start of the recursion).
		/// </summary>
		private const RegionLabel InvalidLabel = (RegionLabel)(-1);


		/// <summary>
		/// Indicates the priority of this technique.
		/// </summary>
		public static int Priority { get; set; } = 55;

		/// <summary>
		/// Indicates whether the technique is enabled.
		/// </summary>
		public static bool IsEnabled { get; set; } = false;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			// Check POM eliminations first.
			bool[] elimKeys = { false, false, false, false, false, false, false, false, false };
			var infos = new Bag<TechniqueInfo>();
			new PomTechniqueSearcher().GetAll(infos, grid);
			foreach (PomTechniqueInfo info in infos)
			{
				elimKeys[info.Digit] = true;
			}

			var tempList = new List<int>();
			var tempGuardians = new List<GridMap>();
			for (int digit = 0; digit < 9; digit++)
			{
				var candMap = CandMaps[digit];
				if (candMap.IsEmpty || !elimKeys[digit])
				{
					continue;
				}

				foreach (int cell in candMap)
				{
					tempList.Clear();
					tempGuardians.Clear();
					var loop = GridMap.Empty;

					f(cell, InvalidLabel, -1);

					void f(int cell, RegionLabel lastLabel, int lastRegion)
					{
						tempList.Add(cell);
						loop.Add(cell);
						if (lastRegion != -1)
						{
							tempGuardians.Add((RegionMaps[lastRegion] & candMap) - loop);
						}

						bool flag = false;
						foreach (int tempRegion in loop.Regions)
						{
							if ((RegionMaps[tempRegion] & loop).Count >= 3)
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							return;
						}

						for (var label = Block; label <= Column; label++)
						{
							if (label == lastLabel)
							{
								continue;
							}

							int region = GetRegion(cell, label);
							foreach (int nextCell in RegionMaps[region] & candMap)
							{
								if (nextCell == cell)
								{
									continue;
								}

								if (tempList[0] == nextCell && loop.Count >= 5 && (loop.Count & 1) == 1)
								{
									tempGuardians.Add((RegionMaps[region] & candMap) - loop);

									// Check eliminations.
									var guardians = GridMap.Empty;
									foreach (var guardian in tempGuardians)
									{
										guardians |= guardian;
									}
									if (guardians.Count > 20)
									{
										continue;
									}

									var peerMap = guardians.PeerIntersection;
									if (peerMap.IsEmpty)
									{
										continue;
									}

									var elimMap = peerMap & candMap;
									if (elimMap.IsEmpty)
									{
										continue;
									}

									var conclusions = new List<Conclusion>();
									foreach (int elimCell in elimMap)
									{
										conclusions.Add(new Conclusion(Elimination, elimCell, digit));
									}

									var candidateOffsets = new List<(int, int)>();
									foreach (int loopCell in loop)
									{
										candidateOffsets.Add((0, loopCell * 9 + digit));
									}
									foreach (int guardianCell in guardians)
									{
										candidateOffsets.Add((1, guardianCell * 9 + digit));
									}

									accumulator.Add(
										new GuardianTechniqueInfo(
											conclusions,
											views: new[]
											{
												new View(
													cellOffsets: null,
													candidateOffsets,
													regionOffsets: null,
													links: null)
											},
											digit,
											loop,
											guardians));

									return;
								}
								else if (!loop[nextCell])
								{
									f(nextCell, label, region);

									loop.Remove(nextCell);
									tempList.RemoveLastElement();
									tempGuardians.RemoveLastElement();
								}
							}
						}
					}
				}
			}
		}
	}
}
