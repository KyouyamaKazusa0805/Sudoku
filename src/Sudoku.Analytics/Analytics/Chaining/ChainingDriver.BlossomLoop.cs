namespace Sudoku.Analytics.Chaining;

using CellsDistribution = Dictionary<Cell, SortedSet<Node>>;
using HousesDistribution = Dictionary<(House, Digit), SortedSet<Node>>;

internal partial class ChainingDriver
{
	/// <summary>
	/// Collect all blossom loops appeared in a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="onlyFindOne">Indicates whether the method only find one valid chain.</param>
	/// <param name="supportedRules">Indicates all supported rules to be used by checking eliminations.</param>
	/// <returns>All possible multiple forcing chain instances.</returns>
	public static ReadOnlySpan<BlossomLoop> CollectBlossomLoops(ref readonly Grid grid, bool onlyFindOne, ChainingRules supportedRules)
	{
		var result = new List<BlossomLoop>();

		// Collect all branches, in order to group them up by its start and end candidate.
		var allBranches = new Dictionary<Candidate, SortedDictionary<Candidate, Node>>();
		foreach (var cell in EmptyCells)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				var startCandidate = cell * 9 + digit;
				var startNode = new Node(startCandidate.AsCandidateMap(), true, false);
				foreach (var endNode in FindForcingChains(startNode).OnNodes)
				{
					if (endNode.Map is not [var endCandidate])
					{
						continue;
					}

					if (!allBranches.TryAdd(startCandidate, new() { { endCandidate, endNode } }))
					{
						allBranches[startCandidate].TryAdd(endCandidate, endNode);
					}
				}
			}
		}

		// For cell.
		foreach (var startCell in EmptyCells & ~BivalueCells)
		{
			var (cellsDistribution, housesDistribution) = distributionsCell(in grid, startCell);
			cellToCell(in grid, cellsDistribution, startCell, supportedRules);
			cellToHouse(in grid, housesDistribution, startCell, supportedRules);
		}

		// For house.
		for (var startHouse = 0; startHouse < 27; startHouse++)
		{
			foreach (var startDigit in grid[HousesMap[startHouse] & EmptyCells])
			{
				// Check whether the length of the digits appeared in house is at least 3.
				// If not, the pattern may be degenerated to a normal continuous nice loop, or just not contain such digit.
				if ((CandidatesMap[startDigit] & HousesMap[startHouse]).Count >= 3)
				{
					var (cellsDistribution, housesDistribution) = distributionsHouse(in grid, startHouse, startDigit);
					houseToCell(in grid, cellsDistribution, startHouse, startDigit, supportedRules);
					houseToHouse(in grid, housesDistribution, startHouse, startDigit, supportedRules);
				}
			}
		}
		return result.AsReadOnlySpan();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static CandidateMap getExitsMap(StrongForcingChain[] strongForcingChains)
			=> [.. from branch in strongForcingChains select branch[0].Map[0]];

		static CandidateMap getRootMap(SortedSet<Node> distribution)
		{
			var rootMap = CandidateMap.Empty;
			foreach (var node in distribution)
			{
				rootMap.Add(node.Root.Map[0]);
			}
			return rootMap;
		}

		static ConclusionSet collectConclusions(ref readonly Grid grid, List<Link> patternLinks, ref readonly CandidateMap exits, ChainingRules supportedRules)
		{
			var result = ConclusionSet.Empty;

			// Collect on eliminations from weak links.
			foreach (var link in patternLinks)
			{
				foreach (var conclusion in ChainOrLoop.GetConclusions(in grid, link))
				{
					result.Add(conclusion);
				}
			}

			// Collect on patterns (like ALSes).
			var context = new ChainingRuleLoopConclusionContext(in grid, patternLinks.AsReadOnlySpan());
			foreach (var rule in supportedRules)
			{
				rule.GetLoopConclusions(ref context);
				result |= context.Conclusions;
			}

			// Collect on end-point nodes.
			foreach (var candidate in exits.PeerIntersection)
			{
				if (grid.Exists(candidate) is true)
				{
					result.Add(new Conclusion(Elimination, candidate));
				}
			}
			return result;
		}

		void cellToCell(ref readonly Grid grid, CellsDistribution cellsDistribution, Cell startCell, ChainingRules supportedRules)
		{
			// Iterate on cells' distribution.
			foreach (var (currentStartCell, cellDistribution) in cellsDistribution)
			{
				if (currentStartCell == startCell)
				{
					continue;
				}

				var digitsMask = grid.GetCandidates(startCell);
				if (cellDistribution.Count != PopCount((uint)digitsMask))
				{
					continue;
				}

				if (getRootMap(cellDistribution).GetDigitsFor(startCell) != digitsMask)
				{
					continue;
				}

				// Calculate conclusions.
				var patternLinks = new List<Link>();
				var arrayCellDistribution = cellDistribution.ToArray();
				var strongForcingChains = from branch in arrayCellDistribution select new StrongForcingChain(branch);
				for (var i = 0; i < cellDistribution.Count; i++)
				{
					patternLinks.AddRange(strongForcingChains[i].Links);
				}
				var conclusions = collectConclusions(in grid, patternLinks, getExitsMap(strongForcingChains), supportedRules);
				if (!conclusions)
				{
					// There's no eliminations found.
					continue;
				}

				// Collect pattern.
				var blossomLoop = new BlossomLoop([.. conclusions]);
				for (var i = 0; i < cellDistribution.Count; i++)
				{
					blossomLoop.Add(arrayCellDistribution[i].Root.Map[0], strongForcingChains[i]);
				}
				result.Add(blossomLoop);
			}
		}

		void cellToHouse(ref readonly Grid grid, HousesDistribution housesDistribution, Cell startCell, ChainingRules supportedRules)
		{
			// Iterate on houses' distribution.
			foreach (var ((startCurrentHouse, startCurrentDigit), houseDistribution) in housesDistribution)
			{
				if (startCell.ToHouse(HouseType.Block) == startCurrentHouse
					|| startCell.ToHouse(HouseType.Row) == startCurrentHouse
					|| startCell.ToHouse(HouseType.Column) == startCurrentHouse)
				{
					continue;
				}

				var digitsMask = grid.GetCandidates(startCell);
				if (houseDistribution.Count != PopCount((uint)digitsMask))
				{
					continue;
				}

				if (getRootMap(houseDistribution).GetDigitsFor(startCell) != digitsMask)
				{
					continue;
				}

				// Calculate conclusions.
				var patternLinks = new List<Link>();
				var arrayCellDistribution = houseDistribution.ToArray();
				var strongForcingChains = from branch in arrayCellDistribution select new StrongForcingChain(branch);
				for (var i = 0; i < houseDistribution.Count; i++)
				{
					patternLinks.AddRange(strongForcingChains[i].Links);
				}
				var conclusions = collectConclusions(in grid, patternLinks, getExitsMap(strongForcingChains), supportedRules);
				if (!conclusions)
				{
					// There's no eliminations found.
					continue;
				}

				// Collect pattern.
				var blossomLoop = new BlossomLoop([.. conclusions]);
				for (var i = 0; i < houseDistribution.Count; i++)
				{
					blossomLoop.Add(arrayCellDistribution[i].Root.Map[0], strongForcingChains[i]);
				}
				result.Add(blossomLoop);
			}
		}

		void houseToCell(ref readonly Grid grid, CellsDistribution cellsDistribution, House startHouse, Digit startDigit, ChainingRules supportedRules)
		{
			// Iterate on cells' distribution.
			foreach (var (currentStartCell, cellDistribution) in cellsDistribution)
			{
				if (currentStartCell.ToHouse(HouseType.Block) == startHouse
					|| currentStartCell.ToHouse(HouseType.Row) == startHouse
					|| currentStartCell.ToHouse(HouseType.Column) == startHouse)
				{
					continue;
				}

				var cells = HousesMap[startHouse] & CandidatesMap[startDigit];
				if (cellDistribution.Count != cells.Count)
				{
					continue;
				}

				if ((getRootMap(cellDistribution).Cells & HousesMap[startHouse]) != cells)
				{
					continue;
				}

				// Calculate conclusions.
				var patternLinks = new List<Link>();
				var arrayCellDistribution = cellDistribution.ToArray();
				var strongForcingChains = from branch in arrayCellDistribution select new StrongForcingChain(branch);
				for (var i = 0; i < cellDistribution.Count; i++)
				{
					patternLinks.AddRange(strongForcingChains[i].Links);
				}
				var conclusions = collectConclusions(in grid, patternLinks, getExitsMap(strongForcingChains), supportedRules);
				if (!conclusions)
				{
					// There's no eliminations found.
					continue;
				}

				// Collect pattern.
				var blossomLoop = new BlossomLoop([.. conclusions]);
				for (var i = 0; i < cellDistribution.Count; i++)
				{
					blossomLoop.Add(arrayCellDistribution[i].Root.Map[0], strongForcingChains[i]);
				}
				result.Add(blossomLoop);
			}
		}

		void houseToHouse(ref readonly Grid grid, HousesDistribution housesDistribution, House startHouse, Digit startDigit, ChainingRules supportedRules)
		{
			// Iterate on houses' distribution.
			foreach (var ((startCurrentHouse, startCurrentDigit), houseDistribution) in housesDistribution)
			{
				if (startCurrentHouse == startHouse)
				{
					continue;
				}

				var cells = HousesMap[startCurrentHouse] & CandidatesMap[startDigit];
				if (houseDistribution.Count != cells.Count)
				{
					continue;
				}

				if (getRootMap(houseDistribution).Cells != cells)
				{
					continue;
				}

				// Calculate conclusions.
				var patternLinks = new List<Link>();
				var arrayCellDistribution = houseDistribution.ToArray();
				var strongForcingChains = from branch in arrayCellDistribution select new StrongForcingChain(branch);
				for (var i = 0; i < houseDistribution.Count; i++)
				{
					patternLinks.AddRange(strongForcingChains[i].Links);
				}
				var conclusions = collectConclusions(in grid, patternLinks, getExitsMap(strongForcingChains), supportedRules);
				if (!conclusions)
				{
					// There's no eliminations found.
					continue;
				}

				// Collect pattern.
				var blossomLoop = new BlossomLoop([.. conclusions]);
				for (var i = 0; i < houseDistribution.Count; i++)
				{
					blossomLoop.Add(arrayCellDistribution[i].Root.Map[0], strongForcingChains[i]);
				}
				result.Add(blossomLoop);
			}
		}

		(CellsDistribution, HousesDistribution) distributionsCell(ref readonly Grid grid, Cell startCell)
		{
			var cellsDistribution = new CellsDistribution();
			var housesDistribution = new HousesDistribution();
			foreach (var startDigit in grid.GetCandidates(startCell))
			{
				var startCandidate = startCell * 9 + startDigit;
				if (allBranches.TryGetValue(startCandidate, out var dictionarySubview))
				{
					foreach (var (endCandidate, endNode) in dictionarySubview)
					{
						var (endCell, endDigit) = (endCandidate / 9, endCandidate % 9);
						if (!cellsDistribution.TryAdd(endCell, [endNode]))
						{
							cellsDistribution[endCell].Add(endNode);
						}

						foreach (var houseType in HouseTypes)
						{
							var house = endCell.ToHouse(houseType);
							var entry = (house, endDigit);
							if (!housesDistribution.TryAdd(entry, [endNode]))
							{
								housesDistribution[entry].Add(endNode);
							}
						}
					}
				}
			}
			return (cellsDistribution, housesDistribution);
		}

		(CellsDistribution, HousesDistribution) distributionsHouse(ref readonly Grid grid, House startHouse, Digit startDigit)
		{
			var cellsDistribution = new CellsDistribution();
			var housesDistribution = new HousesDistribution();
			foreach (var cell in HousesMap[startHouse] & CandidatesMap[startDigit])
			{
				var startCandidate = cell * 9 + startDigit;
				if (allBranches.TryGetValue(startCandidate, out var dictionarySubview))
				{
					foreach (var (endCandidate, endNode) in dictionarySubview)
					{
						var (endCell, endDigit) = (endCandidate / 9, endCandidate % 9);
						if (!cellsDistribution.TryAdd(endCell, [endNode]))
						{
							cellsDistribution[endCell].Add(endNode);
						}

						foreach (var houseType in HouseTypes)
						{
							var house = endCell.ToHouse(houseType);
							var entry = (house, endDigit);
							if (!housesDistribution.TryAdd(entry, [endNode]))
							{
								housesDistribution[entry].Add(endNode);
							}
						}
					}
				}
			}
			return (cellsDistribution, housesDistribution);
		}
	}
}
