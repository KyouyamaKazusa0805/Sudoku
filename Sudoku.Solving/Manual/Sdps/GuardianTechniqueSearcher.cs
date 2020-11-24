using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Manual.LastResorts;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.LinkType;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Encapsulates a <b>guardian</b> technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.Guardian))]
	public sealed class GuardianTechniqueSearcher : SdpTechniqueSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(55);


		/// <inheritdoc/>
		public override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
		{
			// Check POM eliminations first.
			var eliminationMaps = (stackalloc GridMap[9]);
			var infos = new List<TechniqueInfo>();
			new PomTechniqueSearcher().GetAll(infos, grid);
			foreach (PomTechniqueInfo info in infos)
			{
				ref var map = ref eliminationMaps[info.Digit];
				foreach (var conclusion in info.Conclusions)
				{
					map.AddAnyway(conclusion.Cell);
				}
			}

			var resultAccumulator = new List<GuardianTechniqueInfo>();
			for (int digit = 0; digit < 9; digit++)
			{
				var eliminations = eliminationMaps[digit];
				if (eliminations.IsEmpty)
				{
					continue;
				}

				foreach (int elimination in eliminations)
				{
					var loops = new List<(GridMap Map, GridMap Guardians, IReadOnlyList<Link> Links)>();
					var tempLoop = new List<int>();
					var globalMap = CandMaps[digit] - new GridMap(elimination);
					foreach (int cell in globalMap)
					{
						var loopMap = GridMap.Empty;
						loops.Clear();
						tempLoop.Clear();
						f(cell, (RegionLabel)(-1), GridMap.Empty);

						if (loops.Count == 0)
						{
							continue;
						}

						foreach (var (map, guardians, links) in loops)
						{
							var elims = from c in guardians.PeerIntersection & CandMaps[digit]
										select new Conclusion(Elimination, c, digit);
							if (elims.None())
							{
								continue;
							}

							var candidateOffsets = new List<DrawingInfo>();
							candidateOffsets.AddRange(
								from c in map select new DrawingInfo(0, c * 9 + digit));
							candidateOffsets.AddRange(
								from c in guardians select new DrawingInfo(1, c * 9 + digit));

							resultAccumulator.Add(
								new GuardianTechniqueInfo(
									elims.ToArray(),
									new View[] { new(null, candidateOffsets, null, links) },
									digit,
									map,
									guardians));
						}

						// This function is used for recursion.
						// You can't change it to the static local function or normal methods,
						// because it'll cause stack-overflowing.
						// One example is:
						// 009050007060030080000009200100700800002400005080000040010820600000010000300007010
						void f(int cell, RegionLabel lastLabel, GridMap guardians)
						{
							loopMap.AddAnyway(cell);
							tempLoop.Add(cell);

							for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
							{
								if (label == lastLabel)
								{
									continue;
								}

								int region = GetRegion(cell, label);
								var otherCellsMap = RegionMaps[region] & globalMap - cell;
								if (otherCellsMap.Count != 1)
								{
									continue;
								}

								int anotherCell = otherCellsMap.First;
								if (tempLoop.Count is var count and >= 5 && (count & 1) != 0
									&& tempLoop[0] == anotherCell)
								{
									var links = new List<Link>();
									for (int i = 0; i < tempLoop.Count - 1; i++)
									{
										links.Add(
											new(
												tempLoop[i] * 9 + digit,
												tempLoop[i + 1] * 9 + digit,
												Line));
									}
									links.Add(new(tempLoop[^1] * 9 + digit, tempLoop[0] * 9 + digit, Line));

									loops.Add((
										loopMap,
										CreateGuardianMap(cell, anotherCell, digit, guardians),
										links));
								}
								else if (!loopMap[anotherCell])
								{
									f(
										anotherCell,
										label,
										CreateGuardianMap(cell, anotherCell, digit, guardians));
								}
							}

							loopMap.Remove(cell);
							tempLoop.RemoveLastElement();
						}
					}
				}
			}

			unsafe
			{
				var set = new Set<GuardianTechniqueInfo>(resultAccumulator);
				resultAccumulator.Clear();
				resultAccumulator.AddRange(set);
				resultAccumulator.Sort(&cmp);
				accumulator.AddRange(resultAccumulator);

				static int cmp(in GuardianTechniqueInfo l, in GuardianTechniqueInfo r) =>
					(l.Loop.Count + l.Guardians.Count).CompareTo(r.Loop.Count + r.Guardians.Count);
			}
		}

		/// <summary>
		/// Create the guardian map.
		/// </summary>
		/// <param name="cell1">The first cell.</param>
		/// <param name="cell2">The second cell.</param>
		/// <param name="digit">The current digit.</param>
		/// <param name="guardians">
		/// (<see langword="in"/> parameter) The current guardian cells.
		/// This map may not contain cells that lies on the region
		/// that <paramref name="cell1"/> and <paramref name="cell2"/> both lies on.
		/// </param>
		/// <returns>All guardians.</returns>
		private static GridMap CreateGuardianMap(int cell1, int cell2, int digit, in GridMap guardians)
		{
			var tempMap = GridMap.Empty;
			foreach (int coveredRegion in new GridMap { cell1, cell2 }.CoveredRegions)
			{
				tempMap |= RegionMaps[coveredRegion];
			}

			tempMap &= CandMaps[digit];
			tempMap |= guardians;

			return new(tempMap) { ~cell1, ~cell2 };
		}
	}
}
