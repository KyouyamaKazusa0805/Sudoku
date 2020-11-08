using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.DocComments;
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
	[TechniqueDisplay(nameof(TechniqueCode.Guardian))]
	public sealed class GuardianTechniqueSearcher : SdpTechniqueSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(55);


		/// <inheritdoc/>
		public unsafe override void GetAll(IList<TechniqueInfo> accumulator, in SudokuGrid grid)
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
							var candidateOffsets = new List<DrawingInfo>();
							candidateOffsets.AddRange(
								from c in map select new DrawingInfo(0, c * 9 + digit));
							candidateOffsets.AddRange(
								from c in guardians select new DrawingInfo(1, c * 9 + digit));

							resultAccumulator.Add(
								new GuardianTechniqueInfo(
									(
										from c in guardians.PeerIntersection & CandMaps[digit]
										select new Conclusion(Elimination, c, digit)
									).ToArray(),
									new View[]
									{
										new View(
											null,
											candidateOffsets,
											null,
											links)
									},
									digit,
									map,
									guardians));
						}

						void f(int cell, RegionLabel lastLabel, GridMap guardians)
						{
							loopMap.AddAnyway(cell);
							tempLoop.Add(cell);

							for (var label = Block; label <= Column; label++)
							{
								if (label == lastLabel)
								{
									continue;
								}

								int region = GetRegion(cell, label);
								var otherCellsMap = RegionMaps[region] & new GridMap(globalMap) { ~cell };
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
												LinkType.Line));
									}
									links.Add(
										new(tempLoop[^1] * 9 + digit, tempLoop[0] * 9 + digit, LinkType.Line));

									loops.Add((
										loopMap,
										new GridMap(
											RegionMaps[new GridMap { cell, anotherCell }.CoveredRegions.First()]
											& CandMaps[digit]
											| guardians)
										{
											~cell,
											~anotherCell
										},
										links));
								}
								else if (!loopMap[anotherCell])
								{
									f(
										anotherCell,
										label,
										new GridMap(
											RegionMaps[new GridMap { cell, anotherCell }.CoveredRegions.First()]
											& CandMaps[digit]
											| guardians)
										{
											~cell,
											~anotherCell
										});
								}
							}

							loopMap.Remove(cell);
							tempLoop.RemoveLastElement();
						}
					}
				}
			}

			var set = new Set<GuardianTechniqueInfo>(resultAccumulator);
			resultAccumulator.Clear();
			resultAccumulator.AddRange(set);
			resultAccumulator.Sort(&cmp);
			accumulator.AddRange(resultAccumulator);

			static int cmp(in GuardianTechniqueInfo l, in GuardianTechniqueInfo r) =>
				(l.Loop.Count + l.Guardians.Count).CompareTo(r.Loop.Count + r.Guardians.Count);
		}
	}
}
