using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.Solving.Annotations;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Manual.Chaining;
using static Sudoku.Constants.Processings;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.GridMap.InitializationOption;
using static Sudoku.Solving.Constants.Processings;

namespace Sudoku.Solving.Manual.Uniqueness.Bugs
{
	/// <summary>
	/// Encapsulates a <b>bivalue universal grave</b> (BUG) technique searcher.
	/// </summary>
	[TechniqueDisplay(nameof(TechniqueCode.BugType1))]
	[SearcherProperty(56)]
	public sealed class BugTechniqueSearcher : UniquenessTechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the searcher should call the extended BUG checker
		/// to find all true candidates.
		/// </summary>
		private readonly bool _extended;


		/// <summary>
		/// Initializes an instance with the region maps.
		/// </summary>
		/// <param name="extended">
		/// A <see cref="bool"/> value indicating whether the searcher should call
		/// the extended BUG checker to search for all true candidates no matter how
		/// difficult searching.
		/// </param>
		public BugTechniqueSearcher(bool extended) => _extended = extended;


		/// <inheritdoc/>
		public override void GetAll(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid)
		{
			var trueCandidates = _extended ? new BugChecker(grid).TrueCandidates : GetTrueCandidatesSimply(grid);
			switch (trueCandidates.Count)
			{
				case 0:
				{
					return;
				}
				case 1:
				{
					// BUG + 1 found.
					accumulator.Add(
						new BugType1TechniqueInfo(
							conclusions: new[] { new Conclusion(Assignment, trueCandidates[0]) },
							views: new[] { new View(new[] { (0, trueCandidates[0]) }) }));
					break;
				}
				default:
				{
					if (CheckSingleDigit(trueCandidates))
					{
						CheckType2(accumulator, trueCandidates);
					}
					else
					{
						if (_extended)
						{
							CheckMultiple(accumulator, grid, trueCandidates);
							CheckXz(accumulator, grid, trueCandidates);
							CheckMultipleWithForcingChains(accumulator, grid, trueCandidates);
						}

						CheckType3Naked(accumulator, grid, trueCandidates);
						CheckType4(accumulator, grid, trueCandidates);
					}

					break;
				}
			}
		}

		/// <summary>
		/// Check type 2.
		/// </summary>
		/// <param name="accumulator">The result list.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		private void CheckType2(IBag<TechniqueInfo> accumulator, IReadOnlyList<int> trueCandidates)
		{
			var selection = from cand in trueCandidates select cand / 9;
			var map = new GridMap(selection, ProcessPeersWithoutItself);
			if (map.IsEmpty)
			{
				return;
			}

			int digit = trueCandidates[0] % 9;
			var elimMap = map & CandMaps[digit];
			if (elimMap.IsEmpty)
			{
				return;
			}

			var conclusions = new List<Conclusion>();
			foreach (int cell in elimMap)
			{
				conclusions.Add(new Conclusion(Elimination, cell, digit));
			}

			var candidateOffsets = new List<(int, int)>();
			foreach (int candidate in trueCandidates)
			{
				candidateOffsets.Add((0, candidate));
			}

			// BUG type 2.
			accumulator.Add(
				new BugType2TechniqueInfo(
					conclusions,
					views: new[] { new View(candidateOffsets) },
					digit,
					cells: selection.ToArray()));
		}

		/// <summary>
		/// Check type 3 (with naked subsets).
		/// </summary>
		/// <param name="accumulator">The result.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		private void CheckType3Naked(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, IReadOnlyList<int> trueCandidates)
		{
			// Check whether all true candidates lie on a same region.
			var map = new GridMap(from c in trueCandidates group c by c / 9 into z select z.Key);
			if (!map.AllSetsAreInOneRegion(out _))
			{
				return;
			}

			// Get the digit mask.
			short digitsMask = 0;
			foreach (int candidate in trueCandidates)
			{
				digitsMask |= (short)(1 << candidate % 9);
			}

			// Iterate on each region that the true candidates lying on.
			foreach (int region in map.CoveredRegions)
			{
				var regionMap = RegionMaps[region];
				var otherCellsMap = (regionMap & EmptyMap) - map;
				if (otherCellsMap.IsEmpty)
				{
					continue;
				}

				// Iterate on each size.
				int[] otherCells = otherCellsMap.ToArray();
				for (int size = 1, length = otherCells.Length; size < length; size++)
				{
					foreach (int[] cells in otherCells.GetSubsets(size))
					{
						short mask = digitsMask;
						foreach (int cell in cells)
						{
							mask |= grid.GetCandidateMask(cell);
						}
						if (mask.CountSet() != size + 1)
						{
							continue;
						}

						var elimMap = (regionMap - cells - map) & EmptyMap;
						if (elimMap.IsEmpty)
						{
							continue;
						}

						var conclusions = new List<Conclusion>();
						foreach (int cell in elimMap)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								if ((mask >> digit & 1) != 0)
								{
									conclusions.Add(new Conclusion(Elimination, cell, digit));
								}
							}
						}
						if (conclusions.Count == 0)
						{
							continue;
						}

						var candidateOffsets = new List<(int, int)>();
						foreach (int cand in trueCandidates)
						{
							candidateOffsets.Add((0, cand));
						}
						foreach (int cell in cells)
						{
							foreach (int digit in grid.GetCandidates(cell))
							{
								candidateOffsets.Add((1, cell * 9 + digit));
							}
						}

						accumulator.Add(
							new BugType3TechniqueInfo(
								conclusions,
								views: new[]
								{
									new View(
										cellOffsets: null,
										candidateOffsets,
										regionOffsets: new[] { (0, region) },
										links: null)
								},
								trueCandidates,
								digits: digitsMask.GetAllSets().ToArray(),
								cells,
								isNaked: true));
					}
				}
			}
		}

		/// <summary>
		/// Check type 4.
		/// </summary>
		/// <param name="accumulator">The result.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		private void CheckType4(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, IReadOnlyList<int> trueCandidates)
		{
			// Conjugate pairs should lie on two cells.
			var candsGroupByCell = from cand in trueCandidates group cand by cand / 9;
			if (candsGroupByCell.Count() != 2)
			{
				return;
			}

			// Check two cell has same region.
			var cells = new List<int>();
			foreach (var candGroupByCell in candsGroupByCell)
			{
				cells.Add(candGroupByCell.Key);
			}

			var regions = new GridMap(cells).CoveredRegions;
			if (regions.None())
			{
				return;
			}

			// Check for each region.
			foreach (int region in regions)
			{
				// Add up all digits.
				var digits = new HashSet<int>();
				foreach (var candGroupByCell in candsGroupByCell)
				{
					foreach (int cand in candGroupByCell)
					{
						digits.Add(cand % 9);
					}
				}

				// Check whether exists a conjugate pair in this region.
				for (int conjuagtePairDigit = 0; conjuagtePairDigit < 9; conjuagtePairDigit++)
				{
					// Check whether forms a conjugate pair.
					short mask = (RegionMaps[region] & CandMaps[conjuagtePairDigit]).GetSubviewMask(region);
					if (mask.CountSet() != 2)
					{
						continue;
					}

					// Check whether the conjugate pair lies on current two cells.
					int c1 = RegionCells[region][mask.SetAt(0)];
					int c2 = RegionCells[region][mask.SetAt(1)];
					if (c1 != cells[0] || c2 != cells[1])
					{
						continue;
					}

					// Check whether all digits contain that digit.
					if (digits.Contains(conjuagtePairDigit))
					{
						continue;
					}

					// BUG type 4 found.
					// Now add up all eliminations.
					var conclusions = new List<Conclusion>();
					foreach (var candGroupByCell in candsGroupByCell)
					{
						int cell = candGroupByCell.Key;
						short digitMask = 0;
						foreach (int cand in candGroupByCell)
						{
							digitMask |= (short)(1 << cand % 9);
						}

						// Bitwise not.
						digitMask = (short)(~digitMask & Grid.MaxCandidatesMask);
						foreach (int d in digitMask.GetAllSets())
						{
							if (conjuagtePairDigit == d || !(grid.Exists(cell, d) is true))
							{
								continue;
							}

							conclusions.Add(new Conclusion(Elimination, cell, d));
						}
					}

					// Check eliminations.
					if (conclusions.Count == 0)
					{
						continue;
					}

					// BUG type 4.
					accumulator.Add(
						new BugType4TechniqueInfo(
							conclusions,
							views: new[]
							{
								new View(
									cellOffsets: null,
									candidateOffsets:
										new List<(int, int)>(from cand in trueCandidates select (0, cand))
										{
											(1, c1 * 9 + conjuagtePairDigit),
											(1, c2 * 9 + conjuagtePairDigit)
										},
									regionOffsets: new[] { (0, region) },
									links: null)
							},
							digits: digits.ToList(),
							cells,
							conjugatePair: new ConjugatePair(c1, c2, conjuagtePairDigit)));
				}
			}
		}

		/// <summary>
		/// Check BUG + n.
		/// </summary>
		/// <param name="accumulator">The result list.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		private void CheckMultiple(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, IReadOnlyList<int> trueCandidates)
		{
			if (trueCandidates.Count > 18)
			{
				return;
			}

			var map = SudokuMap.CreateInstance(trueCandidates);
			if (map.IsEmpty)
			{
				return;
			}

			// BUG + n found.
			// Check eliminations.
			var conclusions = new List<Conclusion>();
			foreach (int candidate in map)
			{
				if (grid.Exists(candidate / 9, candidate % 9) is true)
				{
					conclusions.Add(new Conclusion(Elimination, candidate));
				}
			}
			if (conclusions.Count == 0)
			{
				return;
			}

			// BUG + n.
			accumulator.Add(
				new BugMultipleTechniqueInfo(
					conclusions,
					views: new[] { new View(new List<(int, int)>(from cand in trueCandidates select (0, cand))) },
					candidates: trueCandidates));
		}

		/// <summary>
		/// Check BUG + n with forcing chains.
		/// </summary>
		/// <param name="accumulator">The result list.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		private void CheckMultipleWithForcingChains(
			IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, IReadOnlyList<int> trueCandidates)
		{
			var tempAccumulator = new List<BugMultipleWithFcTechniqueInfo>();

			// Prepare storage and accumulator for cell eliminations.
			var valueToOn = new Dictionary<int, Set<Node>>();
			var valueToOff = new Dictionary<int, Set<Node>>();
			Set<Node>? cellToOn = null, cellToOff = null;
			foreach (int candidate in trueCandidates)
			{
				int cell = candidate / 9, digit = candidate % 9;
				var onToOn = new Set<Node>();
				var onToOff = new Set<Node>();

				onToOn.Add(new Node(cell, digit, true));
				DoChaining(grid, onToOn, onToOff);

				// Collect results for cell chaining.
				valueToOn.Add(candidate, onToOn);
				valueToOff.Add(candidate, onToOff);
				if (cellToOn is null/* || cellToOff is null*/)
				{
					cellToOn = new Set<Node>(onToOn);
					cellToOff = new Set<Node>(onToOff);
				}
				else
				{
					cellToOn &= onToOn;
					cellToOff = cellToOff! & onToOff;
				}
			}

			// Do cell eliminations.
			if (!(cellToOn is null))
			{
				foreach (var p in cellToOn)
				{
					var hint = CreateEliminationHint(trueCandidates, p, valueToOn);
					if (!(hint is null))
					{
						tempAccumulator.Add(hint);
					}
				}
			}
			if (!(cellToOff is null))
			{
				foreach (var p in cellToOff)
				{
					var hint = CreateEliminationHint(trueCandidates, p, valueToOff);
					if (!(hint is null))
					{
						tempAccumulator.Add(hint);
					}
				}
			}

			tempAccumulator.Sort((i1, i2) => i1.Complexity.CompareTo(i2.Complexity));
			accumulator.AddRange(tempAccumulator);
		}

		/// <summary>
		/// Check BUG-XZ.
		/// </summary>
		/// <param name="accumulator">The result list.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="trueCandidates">All true candidates.</param>
		private void CheckXz(IBag<TechniqueInfo> accumulator, IReadOnlyGrid grid, IReadOnlyList<int> trueCandidates)
		{
			if (trueCandidates.Count > 2)
			{
				return;
			}

			int cand1 = trueCandidates[0], cand2 = trueCandidates[1];
			int c1 = cand1 / 9, c2 = cand2 / 9, d1 = cand1 % 9, d2 = cand2 % 9;
			short mask = (short)(1 << d1 | 1 << d2);
			foreach (int cell in (PeerMaps[c1] ^ PeerMaps[c2]) & BivalueMap)
			{
				if (grid.GetCandidateMask(cell) != mask)
				{
					continue;
				}

				// BUG-XZ found.
				var conclusions = new List<Conclusion>();
				bool condition = new GridMap { c1, cell }.AllSetsAreInOneRegion(out _);
				int anotherCell = condition ? c2 : c1;
				int anotherDigit = condition ? d2 : d1;
				foreach (int peer in new GridMap(stackalloc[] { cell, anotherCell }, ProcessPeersWithoutItself))
				{
					if (grid.Exists(peer, anotherDigit) is true)
					{
						conclusions.Add(new Conclusion(Elimination, peer, anotherDigit));
					}
				}
				if (conclusions.Count == 0)
				{
					continue;
				}

				var cellOffsets = new List<(int, int)> { (0, cell) };
				var candidateOffsets = new List<(int, int)>(from c in trueCandidates select (0, c));
				accumulator.Add(
					new BugXzTechniqueInfo(
						conclusions,
						views: new[]
						{
							new View(
								cellOffsets,
								candidateOffsets,
								regionOffsets: null,
								links: null)
						},
						digitMask: mask,
						cells: new[] { c1, c2 },
						extraCell: cell));
			}
		}

		/// <summary>
		/// To get true candidates (but simple mode).
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>All true candidates searched.</returns>
		private IReadOnlyList<int> GetTrueCandidatesSimply(IReadOnlyGrid grid)
		{
			var tempGrid = grid.Clone();
			var bugCells = new List<int>();
			var bugValues = new Dictionary<int, short>();
			short allBugValues = 0;
			GridMap commonCells = default;
			int t = 0;
			for (int region = 0; region < 27; region++)
			{
				for (int digit = 0; digit < 9; digit++)
				{
					// Possible positions of a value in a region.
					short positions = (RegionMaps[region] & CandMaps[digit]).GetSubviewMask(region);
					int cardinality = positions.CountSet();
					if (cardinality != 0 && cardinality != 2)
					{
						// The value does not have zero or two positions
						// in the region.
						// Look for BUG cells.
						var newBugCells = new List<int>();
						foreach (int i in positions.GetAllSets())
						{
							int cell = RegionCells[region][i];
							int cellCardinality = tempGrid.GetCandidateMask(cell).CountSet();
							if (cellCardinality >= 3)
							{
								newBugCells.Add(cell);
							}
						}

						// If there're two or more positions falling in a BUG cell,
						// we cannot decide which one is the BUGgy one. Just do
						// nothing because another region will capture the correct
						// cell.
						if (newBugCells.Count == 1)
						{
							// A new BUG cell has been found (BUG value == 'value').
							int cell = newBugCells[0];
							bugCells.AddIfDoesNotContain(cell);
							bugValues.AddIfKeyDoesNotContain(cell, (short)0);
							short mask = (short)(1 << digit);
							bugValues[cell] |= mask;
							allBugValues |= mask;
							tempGrid[cell, digit] = true;

							if (t++ == 0)
							{
								commonCells = new GridMap(cell);
							}
							else
							{
								commonCells &= new GridMap(cell);
							}

							foreach (int bugCell in bugCells)
							{
								commonCells.Remove(bugCell);
							}

							if (bugCells.Count > 1 && allBugValues.CountSet() > 1 && commonCells.IsEmpty)
							{
								// None of type 1, 2 or 3.
								return Array.Empty<int>();
							}
						}

						if (newBugCells.Count == 0)
						{
							// A value appear more than twice, but no cell has more
							// than two values, which means that the specified pattern
							// is not a BUG pattern.
							return Array.Empty<int>();
						}
					}
				}
			}

			// When BUG values have been removed, all remaining empty cells must
			// have exactly two potential values. Now check it.
			for (int cell = 0; cell < 81; cell++)
			{
				if (tempGrid.GetStatus(cell) == Empty
					&& tempGrid.GetCandidateMask(cell).CountSet() != 2)
				{
					// Not a BUG.
					return Array.Empty<int>();
				}
			}

			// When BUG values have been removed, all remaining candidates must have
			// two positions in each region.
			for (int region = 0; region < 27; region++)
			{
				for (int digit = 0; digit < 9; digit++)
				{
					short mask = (RegionMaps[region] & CandMaps[digit]).GetSubviewMask(region);
					int count = mask.CountSet();
					if (count != 0 && count != 2)
					{
						// Not a BUG.
						return Array.Empty<int>();
					}
				}
			}

			// Record the result.
			var result = new List<int>();
			foreach (var (cell, digitsMask) in bugValues)
			{
				foreach (int digit in digitsMask.GetAllSets())
				{
					result.Add(cell * 9 + digit);
				}
			}

			return result;
		}


		/// <summary>
		/// Check whether all candidates in the list has same digit value.
		/// </summary>
		/// <param name="list">The list of all true candidates.</param>
		/// <returns>A <see cref="bool"/> indicating that.</returns>
		private static bool CheckSingleDigit(IReadOnlyList<int> list)
		{
			int i = 0;
			int comparer = default;
			foreach (int cand in list)
			{
				if (i++ == 0)
				{
					comparer = cand % 9;
					continue;
				}

				if (comparer != cand % 9)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Do chaining. This method is only called by
		/// <see cref="CheckMultipleWithForcingChains(IBag{TechniqueInfo}, IReadOnlyGrid, IReadOnlyList{int})"/>.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="toOn">All nodes to on.</param>
		/// <param name="toOff">All nodes to off.</param>
		/// <returns>The result nodes.</returns>
		/// <seealso cref="CheckMultipleWithForcingChains(IBag{TechniqueInfo}, IReadOnlyGrid, IReadOnlyList{int})"/>
		private static Node[]? DoChaining(IReadOnlyGrid grid, ISet<Node> toOn, ISet<Node> toOff)
		{
			var pendingOn = new Set<Node>(toOn);
			var pendingOff = new Set<Node>(toOff);
			while (pendingOn.Count != 0 || pendingOff.Count != 0)
			{
				if (pendingOn.Count != 0)
				{
					var p = pendingOn.Remove();

					var makeOff = ChainingTechniqueSearcher.GetOnToOff(grid, p, true);
					foreach (var pOff in makeOff)
					{
						var pOn = new Node(pOff.Cell, pOff.Digit, true); // Conjugate
						if (toOn.Contains(pOn))
						{
							// Contradiction found.
							return new[] { pOn, pOff }; // Cannot be both on and off at the same time.
						}
						else if (!toOff.Contains(pOff))
						{
							// Not processed yet.
							toOff.Add(pOff);
							pendingOff.Add(pOff);
						}
					}
				}
				else
				{
					var p = pendingOff.Remove();

					var makeOn = ChainingTechniqueSearcher.GetOffToOn(grid, p, true, true);

					foreach (var pOn in makeOn)
					{
						var pOff = new Node(pOn.Cell, pOn.Digit, false); // Conjugate.
						if (toOff.Contains(pOff))
						{
							// Contradiction found.
							return new[] { pOn, pOff }; // Cannot be both on and off at the same time.
						}
						else if (!toOn.Contains(pOn))
						{
							// Not processed yet.
							toOn.Add(pOn);
							pendingOn.Add(pOn);
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Create the elimination hint. This method is only called by
		/// <see cref="CheckMultipleWithForcingChains(IBag{TechniqueInfo}, IReadOnlyGrid, IReadOnlyList{int})"/>.
		/// </summary>
		/// <param name="trueCandidates">The true candidates.</param>
		/// <param name="target">The target node.</param>
		/// <param name="outcomes">All outcomes.</param>
		/// <returns>The result information instance.</returns>
		/// <seealso cref="CheckMultipleWithForcingChains(IBag{TechniqueInfo}, IReadOnlyGrid, IReadOnlyList{int})"/>
		private static BugMultipleWithFcTechniqueInfo? CreateEliminationHint(
			IReadOnlyList<int> trueCandidates, Node target, IReadOnlyDictionary<int, Set<Node>> outcomes)
		{
			// Build removable nodes.
			var conclusions = new List<Conclusion>
			{
				new Conclusion(target.IsOn ? Assignment : Elimination, target.Cell, target.Digit)
			};

			// Build chains.
			var chains = new Dictionary<int, Node>();
			foreach (int candidate in trueCandidates)
			{
				// Get the node that contains the same cell, digit and isOn property.
				var valueTarget = outcomes[candidate][target];
				chains.Add(candidate, valueTarget);
			}

			var candidateOffsets = new List<(int, int)>();
			foreach (var node in chains.Values)
			{
				candidateOffsets.AddRange(GetCandidateOffsets(node));
			}
			foreach (int candidate in trueCandidates)
			{
				candidateOffsets.Add((2, candidate));
			}

			var links = new List<Link>();
			foreach (var node in chains.Values)
			{
				links.AddRange(GetLinks(node, true));
			}

			return new BugMultipleWithFcTechniqueInfo(
				conclusions,
				views: new[]
				{
					new View(
						cellOffsets: null,
						candidateOffsets,
						regionOffsets: null,
						links)
				},
				candidates: trueCandidates,
				chains);
		}
	}
}
