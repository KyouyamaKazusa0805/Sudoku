using System;
using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Models;
using Sudoku.Solving.Manual.Extensions;
using Sudoku.Solving.Manual.LastResorts;
using Sudoku.Techniques;
using static Sudoku.Constants.Tables;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.Sdps
{
	/// <summary>
	/// Encapsulates a <b>guardian</b> technique searcher.
	/// </summary>
	public sealed class GuardianStepSearcher : SdpStepSearcher
	{
		/// <inheritdoc/>
		public override SearchingOptions Options { get; set; } = new(18, DisplayingLevel: DisplayingLevel.B);

		/// <summary>
		/// Indicates the searcher properties.
		/// </summary>
		/// <remarks>
		/// Please note that all technique searches should contain
		/// this static property in order to display on settings window. If the searcher doesn't contain,
		/// when we open the settings window, it'll throw an exception to report about this.
		/// </remarks>
		[Obsolete("Please use the property '" + nameof(Options) + "' instead.", false)]
		public static TechniqueProperties Properties { get; } = new(18, nameof(Technique.Guardian))
		{
			DisplayLevel = 2
		};


		/// <inheritdoc/>
		public override unsafe void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			// Check POM eliminations first.
			var eliminationMaps = stackalloc Cells[9];
			Unsafe.InitBlock(eliminationMaps, 0, (uint)sizeof(Cells) * 9);
			var infos = new List<StepInfo>();
			new PomStepSearcher().GetAll(infos, grid);
			foreach (PomStepInfo info in infos)
			{
				var pMap = eliminationMaps + info.Digit;
				foreach (var conclusion in info.Conclusions)
				{
					pMap->AddAnyway(conclusion.Cell);
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
									guardians
								)
							);
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

								int region = cell.ToRegion(label);
								var otherCellsMap = RegionMaps[region] & new Cells(globalMap) { ~cell };
								if (otherCellsMap.Count != 1)
								{
									continue;
								}

								int anotherCell = otherCellsMap[0];
								if (tempLoop.Count >= 5 && (tempLoop.Count & 1) != 0
									&& tempLoop[0] == anotherCell)
								{
									loops.Add(
										(
											loopMap,
											CreateGuardianMap(cell, anotherCell, digit, guardians),
											tempLoop.GetLinks()
										)
									);
								}
								else if (!loopMap.Contains(anotherCell))
								{
									f(
										anotherCell,
										label,
										CreateGuardianMap(cell, anotherCell, digit, guardians)
									);
								}
							}

							loopMap.Remove(cell);
							tempLoop.RemoveLastElement();
						}
					}
				}
			}

			if (resultAccumulator.Count == 0)
			{
				return;
			}

			accumulator.AddRange(
				from info in resultAccumulator.RemoveDuplicateItems()
				orderby info.Loop.Count, info.Guardians.Count
				select info
			);
		}

		/// <summary>
		/// Create the guardian map.
		/// </summary>
		/// <param name="cell1">The first cell.</param>
		/// <param name="cell2">The second cell.</param>
		/// <param name="digit">The current digit.</param>
		/// <param name="guardians">
		/// The current guardian cells.
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

			return new Cells(tempMap) { ~cell1, ~cell2 };
		}
	}
}
