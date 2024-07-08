namespace Sudoku.Analytics.Chaining;

using CellsDistribution = Dictionary<Cell, SortedSet<Node>>;
using HousesDistribution = Dictionary<(House, Digit), SortedSet<Node>>;

/// <summary>
/// Provides a driver that can generate normal chains and forcing chains.
/// </summary>
internal static class ChainingDriver
{
	/// <summary>
	/// Collect all chains and loops appeared in a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="onlyFindOne">Indicates whether the method only find one valid step.</param>
	/// <returns>All possible <see cref="ChainOrLoop"/> instances.</returns>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/developer-notes" />
	/// A valid chain can only belong to the following three cases:
	/// <list type="number">
	/// <item>
	/// <para><b>Discontinuous Nice Loop</b></para>
	/// <para>
	/// Start with weak link, alternating strong and weak links and return to itself of a weak link
	/// (with an even number of nodes, odd number of links).
	/// </para>
	/// </item>
	/// <item>
	/// <para><b>Discontinuous Nice Loop</b></para>
	/// <para>
	/// Start with strong link, alternating strong and weak links and return to itself of a strong link
	/// (with an even number of nodes, even number of links).
	/// </para>
	/// </item>
	/// <item>
	/// <para><b>Continuous Nice Loop</b></para>
	/// <para>
	/// Start with strong link, alternating strong and weak links and return to itself of a weak link
	/// (with an even number of nodes, even number of links).
	/// </para>
	/// </item>
	/// </list>
	/// </remarks>
	public static ReadOnlySpan<ChainOrLoop> CollectChains(ref readonly Grid grid, bool onlyFindOne)
	{
		var result = new SortedSet<ChainOrLoop>(ChainingComparers.ChainComparer);
		foreach (var cell in EmptyCells)
		{
			foreach (var digit in (Mask)(grid.GetCandidates(cell) & (Mask)~(1 << Solution.GetDigit(cell))))
			{
				var node = new Node((cell * 9 + digit).AsCandidateMap(), true, false);
				if (FindChains(node, in grid, onlyFindOne, result) is { } chain1)
				{
					return (ChainOrLoop[])[chain1];
				}
				if (FindChains(~node, in grid, onlyFindOne, result) is { } chain2)
				{
					return (ChainOrLoop[])[chain2];
				}
			}
		}
		return result.ToArray();
	}

	/// <summary>
	/// Collect all multiple forcing chains appeared in a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="onlyFindOne">Indicates whether the method only find one valid chain.</param>
	/// <returns>All possible multiple forcing chain instances.</returns>
	/// <example>
	/// Test example:
	/// <code><![CDATA[
	/// 070096000003520680006004702480000020000000000510000000000000070000030509168000003
	/// ]]></code>
	/// This example only contains one forcing chains pattern and direct singles.
	/// </example>
	public static ReadOnlySpan<MultipleForcingChains> CollectMultipleChains(ref readonly Grid grid, bool onlyFindOne)
	{
		var result = new SortedSet<MultipleForcingChains>(ChainingComparers.MultipleForcingChainsComparer);
		foreach (var cell in EmptyCells & ~BivalueCells)
		{
			var nodesSupposedOn_GroupedByDigit = new Dictionary<Candidate, HashSet<Node>>();
			var nodesSupposedOff_GroupedByDigit = new Dictionary<Candidate, HashSet<Node>>();
			var nodesSupposedOn_InCell = default(HashSet<Node>);
			var nodesSupposedOff_InCell = default(HashSet<Node>);
			var digitsMask = grid.GetCandidates(cell);
			foreach (var digit in digitsMask)
			{
				var currentNode = new Node((cell * 9 + digit).AsCandidateMap(), true, false);
				var (nodesSupposedOn, nodesSupposedOff) = FindForcingChains(currentNode);

				// Iterate on three house types, to collect with region forcing chains.
				foreach (var houseType in HouseTypes)
				{
					var house = cell.ToHouseIndex(houseType);
					var cellsInHouse = HousesMap[house] & CandidatesMap[digit];
					if (cellsInHouse.Count <= 2)
					{
						// There's no need iterating on such house because the chain only contains 2 branches,
						// which means it can be combined into one normal chain.
						continue;
					}

					var firstCellInHouse = cellsInHouse[0];
					if (firstCellInHouse != cell)
					{
						// We should skip the other cells in the house, in order to avoid duplicate forcing chains.
						continue;
					}

					var nodesSupposedOn_GroupedByHouse = new Dictionary<Candidate, HashSet<Node>>();
					var nodesSupposedOff_GroupedByHouse = new Dictionary<Candidate, HashSet<Node>>();
					var nodesSupposedOn_InHouse = new HashSet<Node>(ChainingComparers.NodeMapComparer);
					var nodesSupposedOff_InHouse = new HashSet<Node>(ChainingComparers.NodeMapComparer);
					foreach (var otherCell in cellsInHouse)
					{
						var otherCandidate = otherCell * 9 + digit;
						if (otherCell == cell)
						{
							nodesSupposedOn_GroupedByHouse.Add(otherCandidate, nodesSupposedOn);
							nodesSupposedOff_GroupedByHouse.Add(otherCandidate, nodesSupposedOff);
							nodesSupposedOn_InHouse.UnionWith(nodesSupposedOn);
							nodesSupposedOff_InHouse.UnionWith(nodesSupposedOff);
						}
						else
						{
							var other = new Node(otherCandidate.AsCandidateMap(), true, false);
							var (otherNodesSupposedOn_InHouse, otherNodesSupposedOff_InHouse) = FindForcingChains(other);
							nodesSupposedOn_GroupedByHouse.Add(otherCandidate, otherNodesSupposedOn_InHouse);
							nodesSupposedOff_GroupedByHouse.Add(otherCandidate, otherNodesSupposedOff_InHouse);
							nodesSupposedOn_InHouse.IntersectWith(otherNodesSupposedOn_InHouse);
							nodesSupposedOff_InHouse.IntersectWith(otherNodesSupposedOff_InHouse);
						}
					}

					////////////////////////////////////////
					// Collect with region forcing chains //
					////////////////////////////////////////
					var _a = rfcOn(in grid, digit, in cellsInHouse, nodesSupposedOn_GroupedByHouse, nodesSupposedOn_InHouse);
					if (!_a.IsEmpty)
					{
						return _a;
					}

					var _b = rfcOff(in grid, digit, in cellsInHouse, nodesSupposedOff_GroupedByHouse, nodesSupposedOff_InHouse);
					if (!_b.IsEmpty)
					{
						return _b;
					}
				}

				nodesSupposedOn_GroupedByDigit.Add(cell * 9 + digit, nodesSupposedOn);
				nodesSupposedOff_GroupedByDigit.Add(cell * 9 + digit, nodesSupposedOff);
				if (nodesSupposedOn_InCell is null)
				{
					nodesSupposedOn_InCell = new(ChainingComparers.NodeMapComparer);
					nodesSupposedOff_InCell = new(ChainingComparers.NodeMapComparer);
					nodesSupposedOn_InCell.UnionWith(nodesSupposedOn);
					nodesSupposedOff_InCell.UnionWith(nodesSupposedOff);
				}
				else
				{
					Debug.Assert(nodesSupposedOff_InCell is not null);
					nodesSupposedOn_InCell.IntersectWith(nodesSupposedOn);
					nodesSupposedOff_InCell.IntersectWith(nodesSupposedOff);
				}
			}

			//////////////////////////////////////
			// Collect with cell forcing chains //
			//////////////////////////////////////
			var _c = cfcOn(in grid, cell, nodesSupposedOn_GroupedByDigit, nodesSupposedOn_InCell, digitsMask);
			if (!_c.IsEmpty)
			{
				return _c;
			}

			var _d = cfcOff(in grid, cell, nodesSupposedOff_GroupedByDigit, nodesSupposedOff_InCell, digitsMask);
			if (!_d.IsEmpty)
			{
				return _d;
			}
		}
		return result.ToArray();


		ReadOnlySpan<MultipleForcingChains> cfcOn(
			ref readonly Grid grid,
			Cell cell,
			Dictionary<Candidate, HashSet<Node>> onNodes,
			HashSet<Node>? resultOnNodes,
			Mask digitsMask
		)
		{
			foreach (var node in resultOnNodes ?? [])
			{
				if (node.IsGroupedNode)
				{
					// Grouped nodes are not supported as target node.
					continue;
				}

				var conclusion = new Conclusion(node.IsOn ? Assignment : Elimination, node.Map[0]);
				if (grid.Exists(conclusion.Candidate) is not true)
				{
					continue;
				}

				var cfc = new MultipleForcingChains(conclusion);
				foreach (var d in digitsMask)
				{
					var branchNode = onNodes[cell * 9 + d].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					cfc.Add(cell * 9 + d, node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode));
				}
				if (onlyFindOne)
				{
					return (MultipleForcingChains[])[cfc];
				}
				result.Add(cfc);
			}
			return [];
		}

		ReadOnlySpan<MultipleForcingChains> cfcOff(
			ref readonly Grid grid,
			Cell cell,
			Dictionary<Candidate, HashSet<Node>> offNodes,
			HashSet<Node>? resultOffNodes,
			Mask digitsMask
		)
		{
			foreach (var node in resultOffNodes ?? [])
			{
				if (node.IsGroupedNode)
				{
					// Grouped nodes are not supported as target node.
					continue;
				}

				var conclusion = new Conclusion(node.IsOn ? Assignment : Elimination, node.Map[0]);
				if (grid.Exists(conclusion.Candidate) is not true)
				{
					continue;
				}

				var cfc = new MultipleForcingChains(conclusion);
				foreach (var d in digitsMask)
				{
					var branchNode = offNodes[cell * 9 + d].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					cfc.Add(cell * 9 + d, node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode));
				}
				if (onlyFindOne)
				{
					return (MultipleForcingChains[])[cfc];
				}
				result.Add(cfc);
			}
			return [];
		}

		ReadOnlySpan<MultipleForcingChains> rfcOn(
			ref readonly Grid grid,
			Digit digit,
			scoped ref readonly CellMap cellsInHouse,
			Dictionary<Candidate, HashSet<Node>> onNodes,
			HashSet<Node> houseOnNodes
		)
		{
			foreach (var node in houseOnNodes)
			{
				if (node.IsGroupedNode)
				{
					// Grouped nodes are not supported as target node.
					continue;
				}

				var conclusion = new Conclusion(node.IsOn ? Assignment : Elimination, node.Map[0]);
				if (grid.Exists(conclusion.Candidate) is not true)
				{
					continue;
				}

				var rfc = new MultipleForcingChains(conclusion);
				foreach (var c in cellsInHouse)
				{
					var branchNode = onNodes[c * 9 + digit].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					rfc.Add(
						c * 9 + digit,
						node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode)
					);
				}
				if (onlyFindOne)
				{
					return (MultipleForcingChains[])[rfc];
				}
				result.Add(rfc);
			}
			return [];
		}

		ReadOnlySpan<MultipleForcingChains> rfcOff(
			ref readonly Grid grid,
			Digit digit,
			scoped ref readonly CellMap cellsInHouse,
			Dictionary<Candidate, HashSet<Node>> offNodes,
			HashSet<Node> houseOffNodes
		)
		{
			foreach (var node in houseOffNodes)
			{
				if (node.IsGroupedNode)
				{
					// Grouped nodes are not supported as target node.
					continue;
				}

				var conclusion = new Conclusion(node.IsOn ? Assignment : Elimination, node.Map[0]);
				if (grid.Exists(conclusion.Candidate) is not true)
				{
					continue;
				}

				var rfc = new MultipleForcingChains(conclusion);
				foreach (var c in cellsInHouse)
				{
					var branchNode = offNodes[c * 9 + digit].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					rfc.Add(
						c * 9 + digit,
						node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode)
					);
				}
				if (onlyFindOne)
				{
					return (MultipleForcingChains[])[rfc];
				}
				result.Add(rfc);
			}
			return [];
		}
	}

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
		var allBranchesMap = new Dictionary<Candidate, CandidateMap>();
		foreach (var cell in EmptyCells)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				var startCandidate = cell * 9 + digit;
				var startNode = new Node(startCandidate.AsCandidateMap(), true, false);
				var endNodes = FindForcingChains(startNode).OnNodes;
				foreach (var endNode in endNodes)
				{
					if (endNode.Map is not [var endCandidate])
					{
						continue;
					}

					if (!allBranches.TryAdd(startCandidate, new() { { endCandidate, endNode } }))
					{
						allBranches[startCandidate].TryAdd(endCandidate, endNode);
					}

					if (!allBranchesMap.TryAdd(startCandidate, endCandidate.AsCandidateMap()))
					{
						ref var map = ref allBranchesMap.GetValueRef(in startCandidate);
						map.Add(endCandidate);
					}
				}
			}
		}

		// For cell.
		foreach (var startCell in EmptyCells & ~BivalueCells)
		{
			var (cellsDistribution, housesDistribution) = getDistributions(in grid, startCell);
			cellToCell(in grid, cellsDistribution, startCell, supportedRules);
			cellToHouse(in grid, housesDistribution, startCell, supportedRules);
		}

		// For house.
		for (var house = 0; house < 27; house++)
		{
			foreach (var digit in grid[HousesMap[house] & EmptyCells])
			{
				if ((CandidatesMap[digit] & HousesMap[house]).Count < 3)
				{
					// It may be a normal continuous nice loop.
					continue;
				}


			}
		}

		return result.AsReadOnlySpan();


		static CandidateMap getRootMap(SortedSet<Node> distribution)
		{
			var rootMap = CandidateMap.Empty;
			foreach (var node in distribution)
			{
				rootMap.Add(node.Root.Map[0]);
			}
			return rootMap;
		}

		static ConclusionSet collectConclusions(ref readonly Grid grid, List<Link> patternLinks, ChainingRules supportedRules)
		{
			var result = ConclusionSet.Empty;
			foreach (var link in patternLinks)
			{
				foreach (var conclusion in ChainOrLoop.GetConclusions(in grid, link))
				{
					result.Add(conclusion);
				}
			}

			var context = new ChainingRuleLoopConclusionCollectingContext(in grid, patternLinks.AsReadOnlySpan());
			foreach (var rule in supportedRules)
			{
				result |= rule.CollectLoopConclusions(in context);
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
				var conclusions = collectConclusions(in grid, patternLinks, supportedRules);
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
				if (startCell.ToHouseIndex(HouseType.Block) == startCurrentHouse
					|| startCell.ToHouseIndex(HouseType.Row) == startCurrentHouse
					|| startCell.ToHouseIndex(HouseType.Column) == startCurrentHouse)
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
				var conclusions = collectConclusions(in grid, patternLinks, supportedRules);
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

		(CellsDistribution, HousesDistribution) getDistributions(ref readonly Grid grid, Cell startCell)
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
							var house = endCell.ToHouseIndex(houseType);
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

	/// <summary>
	/// <para>
	/// Find all possible <see cref="ChainOrLoop"/> patterns starting with the current node,
	/// and to make an confliction with itself.
	/// </para>
	/// <para>
	/// This method will return <see langword="null"/> if <paramref name="onlyFindOne"/> is <see langword="false"/>,
	/// or the method cannot find a valid chain that forms a confliction with the current node.
	/// </para>
	/// </summary>
	/// <param name="startNode">The current instance.</param>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="onlyFindOne">Indicates whether the method only find one valid <see cref="ChainOrLoop"/> and return.</param>
	/// <param name="result">
	/// A collection that stores all possible found <see cref="ChainOrLoop"/> patterns
	/// if <paramref name="onlyFindOne"/> is <see langword="false"/>.
	/// </param>
	/// <returns>The first found <see cref="ChainOrLoop"/> pattern.</returns>
	/// <seealso cref="ChainOrLoop"/>
	private static ChainOrLoop? FindChains(Node startNode, ref readonly Grid grid, bool onlyFindOne, SortedSet<ChainOrLoop> result)
	{
		var pendingNodesSupposedOn = new LinkedList<Node>();
		var pendingNodesSupposedOff = new LinkedList<Node>();
		(startNode.IsOn ? pendingNodesSupposedOff : pendingNodesSupposedOn).AddLast(startNode);

		var visitedNodesSupposedOn = new HashSet<Node>(ChainingComparers.NodeMapComparer);
		var visitedNodesSupposedOff = new HashSet<Node>(ChainingComparers.NodeMapComparer);
		_ = (visitedNodesSupposedOn.Add(startNode), visitedNodesSupposedOff.Add(startNode));

		while (pendingNodesSupposedOn.Count != 0 || pendingNodesSupposedOff.Count != 0)
		{
			while (pendingNodesSupposedOn.Count != 0)
			{
				var currentNode = pendingNodesSupposedOn.RemoveFirstNode();
				if (WeakLinkDictionary.TryGetValue(currentNode, out var nodesSupposedOff))
				{
					foreach (var nodeSupposedOff in nodesSupposedOff)
					{
						var nextNode = nodeSupposedOff >> currentNode;

						////////////////////////////////////////////
						// Continuous Nice Loop 3) Strong -> Weak //
						////////////////////////////////////////////
						if (nodeSupposedOff == startNode && nextNode.AncestorsLength >= 4)
						{
							var loop = new Loop(nextNode);
							if (!loop.GetConclusions(in grid).IsWorthFor(in grid))
							{
								goto Next;
							}

							if (onlyFindOne)
							{
								return loop;
							}

							result.Add(loop);
						Next:;
						}

						/////////////////////////////////////////////////
						// Discontinuous Nice Loop 2) Strong -> Strong //
						/////////////////////////////////////////////////
						if (nodeSupposedOff == ~startNode)
						{
							var chain = new Chain(nextNode);
							if (!chain.GetConclusions(in grid).IsWorthFor(in grid))
							{
								goto Next;
							}

							if (onlyFindOne)
							{
								return chain;
							}
							result.Add(chain);
						Next:;
						}

						// This step will filter duplicate nodes in order not to make a internal loop on chains.
						// The second argument must be 'NodeComparison.IgnoreIsOn' because we should explicitly ignore them
						// no matter what state the node is.
						// This will fix issue #673:
						//   * https://github.com/SunnieShine/Sudoku/issues/673
						// Counter-example:
						//   4.+3.6+85...+57.....8+89.5...3..7..+8+6.2.23..94.+8..+84.....15..6..8+7+3+3..+871.5.+7+68.....2:114 124 324 425 427 627 943 366 667 967 272 273 495 497
						if (!nodeSupposedOff.IsAncestorOf(currentNode, NodeComparison.IgnoreIsOn)
							&& visitedNodesSupposedOff.Add(nodeSupposedOff))
						{
							pendingNodesSupposedOff.AddLast(nextNode);
						}
					}
				}
			}
			while (pendingNodesSupposedOff.Count != 0)
			{
				var currentNode = pendingNodesSupposedOff.RemoveFirstNode();
				if (StrongLinkDictionary.TryGetValue(currentNode, out var nodesSupposedOn))
				{
					foreach (var nodeSupposedOn in nodesSupposedOn)
					{
						var nextNode = nodeSupposedOn >> currentNode;

						/////////////////////////////////////////////
						// Discontinuous Nice Loop 1) Weak -> Weak //
						/////////////////////////////////////////////
						if (nodeSupposedOn == ~startNode)
						{
							var chain = new Chain(nextNode);
							if (!chain.GetConclusions(in grid).IsWorthFor(in grid))
							{
								goto Next;
							}

							if (onlyFindOne)
							{
								return chain;
							}
							result.Add(chain);
						Next:;
						}

						if (!nodeSupposedOn.IsAncestorOf(currentNode, NodeComparison.IgnoreIsOn)
							&& visitedNodesSupposedOn.Add(nodeSupposedOn))
						{
							pendingNodesSupposedOn.AddLast(nextNode);
						}
					}
				}
			}
		}
		return null;
	}

	/// <summary>
	/// <para>Finds a list of nodes that can implicitly connects to current node via a forcing chain.</para>
	/// <para>This method only uses cached fields <see cref="StrongLinkDictionary"/> and <see cref="WeakLinkDictionary"/>.</para>
	/// </summary>
	/// <param name="startNode">The current instance.</param>
	/// <returns>
	/// A pair of <see cref="HashSet{T}"/> of <see cref="Node"/> instances, indicating all possible nodes
	/// that can implicitly connects to the current node via the whole forcing chain, grouped by their own initial states,
	/// encapsulating with type <see cref="ForcingChainInfo"/>.
	/// </returns>
	/// <seealso cref="StrongLinkDictionary"/>
	/// <seealso cref="WeakLinkDictionary"/>
	/// <seealso cref="HashSet{T}"/>
	/// <seealso cref="Node"/>
	/// <seealso cref="ForcingChainInfo"/>
	private static ForcingChainInfo FindForcingChains(Node startNode)
	{
		var (pendingNodesSupposedOn, pendingNodesSupposedOff) = (new LinkedList<Node>(), new LinkedList<Node>());
		(startNode.IsOn ? pendingNodesSupposedOn : pendingNodesSupposedOff).AddLast(startNode);

		var nodesSupposedOn = new HashSet<Node>(ChainingComparers.NodeMapComparer);
		var nodesSupposedOff = new HashSet<Node>(ChainingComparers.NodeMapComparer);
		while (pendingNodesSupposedOn.Count != 0 || pendingNodesSupposedOff.Count != 0)
		{
			if (pendingNodesSupposedOn.Count != 0)
			{
				var currentNode = pendingNodesSupposedOn.RemoveFirstNode();
				if (WeakLinkDictionary.TryGetValue(currentNode, out var supposedOff))
				{
					foreach (var node in supposedOff)
					{
						var nextNode = node >> currentNode;
						if (nodesSupposedOn.Contains(~nextNode))
						{
							// Contradiction is found.
							goto ReturnResult;
						}

						if (nodesSupposedOff.Add(nextNode))
						{
							pendingNodesSupposedOff.AddLast(nextNode);
						}
					}
				}
			}
			else
			{
				var currentNode = pendingNodesSupposedOff.RemoveFirstNode();
				if (StrongLinkDictionary.TryGetValue(currentNode, out var supposedOn))
				{
					foreach (var node in supposedOn)
					{
						var nextNode = node >> currentNode;
						if (nodesSupposedOff.Contains(~nextNode))
						{
							// Contradiction is found.
							goto ReturnResult;
						}

						if (nodesSupposedOn.Add(nextNode))
						{
							pendingNodesSupposedOn.AddLast(nextNode);
						}
					}
				}
			}
		}

	ReturnResult:
		// Returns the found result.
		return new(nodesSupposedOn, nodesSupposedOff);
	}
}
