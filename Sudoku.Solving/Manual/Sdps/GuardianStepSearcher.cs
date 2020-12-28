using System;
using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Solving.Extensions;
using Sudoku.Solving.Manual.LastResorts;
using static Sudoku.Constants.Processings;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Encapsulates a <b>guardian</b> technique searcher.
	/// </summary>
	public sealed class GuardianStepSearcher : SdpStepSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(55, nameof(TechniqueCode.Guardian))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			// Check POM eliminations first.
			var eliminationMaps = (stackalloc Cells[9]);
			var infos = new List<StepInfo>();
			new PomStepSearcher().GetAll(infos, grid);
			foreach (PomStepInfo info in infos)
			{
				ref var map = ref eliminationMaps[info.Digit];
				foreach (var conclusion in info.Conclusions)
				{
					map.AddAnyway(conclusion.Cell);
				}
			}

			var resultAccumulator = new List<GuardianStepInfo>();
			for (int digit = 0; digit < 9; digit++)
			{
				var eliminations = eliminationMaps[digit];
				if (eliminations.IsEmpty)
				{
					continue;
				}

				foreach (int elimination in eliminations)
				{
					var loops = new List<(Cells, Cells, IReadOnlyList<Link>)>();
					var tempLoop = new List<int>();
					var globalMap = CandMaps[digit] - new Cells(elimination);
					foreach (int cell in globalMap)
					{
						var loopMap = Cells.Empty;
						loops.Clear();
						tempLoop.Clear();
						f(cell, (RegionLabel)byte.MaxValue, Cells.Empty);

						if (loops.Count == 0)
						{
							continue;
						}

						foreach (var (map, guardians, links) in loops)
						{
							var elimMap = guardians.PeerIntersection & CandMaps[digit];
							if (elimMap.IsEmpty)
							{
								continue;
							}

							var conclusions = new Conclusion[elimMap.Count];
							int i = 0;
							foreach (int c in elimMap)
							{
								conclusions[i++] = new(ConclusionType.Elimination, c, digit);
							}

							var candidateOffsets = new List<DrawingInfo>();
							foreach (int c in map)
							{
								candidateOffsets.Add(new(0, c * 9 + digit));
							}
							foreach (int c in guardians)
							{
								candidateOffsets.Add(new(1, c * 9 + digit));
							}

							resultAccumulator.Add(
								new GuardianStepInfo(
									conclusions,
									new View[] { new() { Candidates = candidateOffsets, Links = links } },
									digit,
									map,
									guardians));
						}

						// This function is used for recursion.
						// You can't change it to the static local function or normal methods,
						// because it'll cause stack-overflowing.
						// One example is:
						// 009050007060030080000009200100700800002400005080000040010820600000010000300007010
						void f(int cell, RegionLabel lastLabel, Cells guardians)
						{
							loopMap.AddAnyway(cell);
							tempLoop.Add(cell);

							for (var label = RegionLabel.Block; label <= RegionLabel.Column; label++)
							{
								if (label == lastLabel)
								{
									continue;
								}

								int region = label.ToRegion(cell);
								var otherCellsMap = RegionMaps[region] & globalMap - cell;
								if (otherCellsMap.Count != 1)
								{
									continue;
								}

								int anotherCell = otherCellsMap[0];
								if (tempLoop.Count is var count and >= 5 && (count & 1) != 0
									&& tempLoop[0] == anotherCell)
								{
									loops.Add((
										loopMap,
										CreateGuardianMap(cell, anotherCell, digit, guardians),
										tempLoop.GetLinks()));
								}
								else if (!loopMap.Contains(anotherCell))
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
				var set = new Set<GuardianStepInfo>(resultAccumulator);
				resultAccumulator.Clear();
				resultAccumulator.AddRange(set);
				resultAccumulator.Sort(&cmp);
				accumulator.AddRange(resultAccumulator);
			}

			static int cmp(in GuardianStepInfo l, in GuardianStepInfo r) =>
				(l.Loop.Count + l.Guardians.Count).CompareTo(r.Loop.Count + r.Guardians.Count);
		}

		/// <summary>
		/// Create the guardian map.
		/// </summary>
		/// <param name="cell1">The first cell.</param>
		/// <param name="cell2">The second cell.</param>
		/// <param name="digit">The current digit.</param>
		/// <param name="guardians">
		/// (<see langword="in"/> parameter) The current guardian cells.
		/// This map may not contain cells that lies in the region
		/// that <paramref name="cell1"/> and <paramref name="cell2"/> both lies in.
		/// </param>
		/// <returns>All guardians.</returns>
		private static Cells CreateGuardianMap(int cell1, int cell2, int digit, in Cells guardians)
		{
			var tempMap = Cells.Empty;
			foreach (int coveredRegion in new Cells { cell1, cell2 }.CoveredRegions)
			{
				tempMap |= RegionMaps[coveredRegion];
			}

			tempMap &= CandMaps[digit];
			tempMap |= guardians;

			return new(tempMap) { ~cell1, ~cell2 };
		}
	}
}
