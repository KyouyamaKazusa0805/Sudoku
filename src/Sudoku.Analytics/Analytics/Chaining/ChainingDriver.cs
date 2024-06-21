namespace Sudoku.Analytics.Chaining;

using static CachedChainingComparers;
using NodeDictionary = Dictionary<Candidate, HashSet<Node>>;

/// <summary>
/// Provides a driver that can generate normal chains and forcing chains.
/// </summary>
internal static class ChainingDriver
{
	/// <summary>
	/// Collect all <see cref="ChainOrLoop"/> instances appears in a grid.
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
		var result = new SortedSet<ChainOrLoop>(ChainComparer);
		foreach (var cell in EmptyCells)
		{
			foreach (var digit in (Mask)(grid.GetCandidates(cell) & (Mask)~(1 << Solution.GetDigit(cell))))
			{
				var node = new Node(cell, digit, true, false);
				if (bfs(in grid, node) is { } chain1)
				{
					return (ChainOrLoop[])[chain1];
				}
				if (bfs(in grid, ~node) is { } chain2)
				{
					return (ChainOrLoop[])[chain2];
				}
			}
		}
		return result.ToArray();


		ChainOrLoop? bfs(ref readonly Grid grid, Node startNode)
		{
			var pendingNodesSupposedOn = new LinkedList<Node>();
			var pendingNodesSupposedOff = new LinkedList<Node>();
			(startNode.IsOn ? pendingNodesSupposedOff : pendingNodesSupposedOn).AddLast(startNode);

			var visitedNodesSupposedOn = new HashSet<Node>(NodeMapComparer);
			var visitedNodesSupposedOff = new HashSet<Node>(NodeMapComparer);
			visitedNodesSupposedOn.Add(startNode);
			visitedNodesSupposedOff.Add(startNode);

			while (pendingNodesSupposedOn.Count != 0 || pendingNodesSupposedOff.Count != 0)
			{
				while (pendingNodesSupposedOn.Count != 0)
				{
					var currentNode = pendingNodesSupposedOn.RemoveFirstNode();
					if (WeakLinkDictionary.TryGetValue(currentNode, out var nodesSupposedOff))
					{
						foreach (var nodeSupposedOff in nodesSupposedOff)
						{
							var nextNode = new Node(nodeSupposedOff, currentNode);

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
							// The second argument must be 'NodeComparison.IgnoreIsOn' because we should explicitly ignore them.
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
							var nextNode = new Node(nodeSupposedOn, currentNode);

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
	}

	/// <summary>
	/// Collect all multiple forcing chain instances appears in a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="rules">
	/// Indicates the rule instances that will create strong and weak links by their own represented concept.
	/// </param>
	/// <param name="onlyFindOne">Indicates whether the method only find one valid chain.</param>
	/// <returns>All possible multiple forcing chain instances.</returns>
	/// <example>
	/// Test example:
	/// <code><![CDATA[
	/// 070096000003520680006004702480000020000000000510000000000000070000030509168000003
	/// ]]></code>
	/// This example only contains one forcing chains pattern and direct singles.
	/// </example>
	public static ReadOnlySpan<MultipleForcingChains> CollectMultipleChains(ref readonly Grid grid, ReadOnlySpan<ChainingRule> rules, bool onlyFindOne)
	{
		var result = new SortedSet<MultipleForcingChains>(MultipleForcingChainsComparer);
		foreach (var cell in EmptyCells & ~BivalueCells)
		{
			var digitsMask = grid.GetCandidates(cell);
			var nodesSupposedOn_ImplicitlyToCurrentNode_GroupedByDigit = new NodeDictionary();
			var nodesSupposedOff_ImplicitlyToCurrentNode_GroupedByDigit = new NodeDictionary();
			var nodesSupposedOn_ImplicitlyToCurrentNode_InCell = default(HashSet<Node>);
			var nodesSupposedOff_ImplicitlyToCurrentNode_InCell = default(HashSet<Node>);
			foreach (var digit in digitsMask)
			{
				var currentNode = new Node(cell, digit, true, false);
				bfs(currentNode, out var nodesSupposedOn_ImplicitlyToCurrentNode, out var nodesSupposedOff_ImplicitlyToCurrentNode);

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

					var nodesSupposedOn_ImplicitlyToCurrentNode_GroupedByHouse = new NodeDictionary();
					var nodesSupposedOff_ImplicitlyToCurrentNode_GroupedByHouse = new NodeDictionary();
					var nodesSupposedOn_ImplicitlyToCurrentNode_InHouse = new HashSet<Node>(NodeMapComparer);
					var nodesSupposedOff_ImplicitlyToCurrentNode_InHouse = new HashSet<Node>(NodeMapComparer);
					foreach (var otherCell in cellsInHouse)
					{
						if (otherCell == cell)
						{
							nodesSupposedOn_ImplicitlyToCurrentNode_GroupedByHouse.Add(otherCell * 9 + digit, nodesSupposedOn_ImplicitlyToCurrentNode);
							nodesSupposedOff_ImplicitlyToCurrentNode_GroupedByHouse.Add(otherCell * 9 + digit, nodesSupposedOff_ImplicitlyToCurrentNode);
							nodesSupposedOn_ImplicitlyToCurrentNode_InHouse.UnionWith(nodesSupposedOn_ImplicitlyToCurrentNode);
							nodesSupposedOff_ImplicitlyToCurrentNode_InHouse.UnionWith(nodesSupposedOff_ImplicitlyToCurrentNode);
						}
						else
						{
							var other = new Node(otherCell, digit, true, false);
							bfs(
								other,
								out var otherNodesSupposedOn_ImplicitlyToCurrentNode_InHouse,
								out var otherNodesSupposedOff_ImplicitlyToCurrentNode_InHouse
							);
							nodesSupposedOn_ImplicitlyToCurrentNode_GroupedByHouse.Add(otherCell * 9 + digit, otherNodesSupposedOn_ImplicitlyToCurrentNode_InHouse);
							nodesSupposedOff_ImplicitlyToCurrentNode_GroupedByHouse.Add(otherCell * 9 + digit, otherNodesSupposedOff_ImplicitlyToCurrentNode_InHouse);
							nodesSupposedOn_ImplicitlyToCurrentNode_InHouse.IntersectWith(otherNodesSupposedOn_ImplicitlyToCurrentNode_InHouse);
							nodesSupposedOff_ImplicitlyToCurrentNode_InHouse.IntersectWith(otherNodesSupposedOff_ImplicitlyToCurrentNode_InHouse);
						}
					}

					////////////////////////////////////////
					// Collect with region forcing chains //
					////////////////////////////////////////
					foreach (var node in nodesSupposedOn_ImplicitlyToCurrentNode_InHouse)
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

						var mfc = new MultipleForcingChains(conclusion);
						foreach (var c in cellsInHouse)
						{
							var branchNode = nodesSupposedOn_ImplicitlyToCurrentNode_GroupedByHouse[c * 9 + digit].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
							mfc.Add(
								c * 9 + digit,
								node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode)
							);
						}
						if (onlyFindOne)
						{
							return (MultipleForcingChains[])[mfc];
						}
						result.Add(mfc);
					}
					foreach (var node in nodesSupposedOff_ImplicitlyToCurrentNode_InHouse)
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

						var mfc = new MultipleForcingChains(conclusion);
						foreach (var c in cellsInHouse)
						{
							var branchNode = nodesSupposedOff_ImplicitlyToCurrentNode_GroupedByHouse[c * 9 + digit].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
							mfc.Add(
								c * 9 + digit,
								node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode)
							);
						}
						if (onlyFindOne)
						{
							return (MultipleForcingChains[])[mfc];
						}
						result.Add(mfc);
					}
				}

				nodesSupposedOn_ImplicitlyToCurrentNode_GroupedByDigit.Add(cell * 9 + digit, nodesSupposedOn_ImplicitlyToCurrentNode);
				nodesSupposedOff_ImplicitlyToCurrentNode_GroupedByDigit.Add(cell * 9 + digit, nodesSupposedOff_ImplicitlyToCurrentNode);
				if (nodesSupposedOn_ImplicitlyToCurrentNode_InCell is null)
				{
					nodesSupposedOn_ImplicitlyToCurrentNode_InCell = new(NodeMapComparer);
					nodesSupposedOff_ImplicitlyToCurrentNode_InCell = new(NodeMapComparer);
					nodesSupposedOn_ImplicitlyToCurrentNode_InCell.UnionWith(nodesSupposedOn_ImplicitlyToCurrentNode);
					nodesSupposedOff_ImplicitlyToCurrentNode_InCell.UnionWith(nodesSupposedOff_ImplicitlyToCurrentNode);
				}
				else
				{
					nodesSupposedOn_ImplicitlyToCurrentNode_InCell.IntersectWith(nodesSupposedOn_ImplicitlyToCurrentNode);
					nodesSupposedOff_ImplicitlyToCurrentNode_InCell!.IntersectWith(nodesSupposedOff_ImplicitlyToCurrentNode);
				}
			}

			//////////////////////////////////////
			// Collect with cell forcing chains //
			//////////////////////////////////////
			foreach (var node in nodesSupposedOn_ImplicitlyToCurrentNode_InCell ?? [])
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
					var branchNode = nodesSupposedOn_ImplicitlyToCurrentNode_GroupedByDigit[cell * 9 + d].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					cfc.Add(cell * 9 + d, node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode));
				}
				if (onlyFindOne)
				{
					return (MultipleForcingChains[])[cfc];
				}
				result.Add(cfc);
			}
			foreach (var node in nodesSupposedOff_ImplicitlyToCurrentNode_InCell ?? [])
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
					var branchNode = nodesSupposedOff_ImplicitlyToCurrentNode_GroupedByDigit[cell * 9 + d].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					cfc.Add(cell * 9 + d, node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode));
				}
				if (onlyFindOne)
				{
					return (MultipleForcingChains[])[cfc];
				}
				result.Add(cfc);
			}
		}
		return result.ToArray();


		static void bfs(Node startNode, out HashSet<Node> resultNodesSupposedOn, out HashSet<Node> resultNodesSupposedOff)
		{
			var (pendingNodesSupposedOn, pendingNodesSupposedOff) = (new LinkedList<Node>(), new LinkedList<Node>());
			(startNode.IsOn ? pendingNodesSupposedOn : pendingNodesSupposedOff).AddLast(startNode);
			(resultNodesSupposedOn, resultNodesSupposedOff) = (new(NodeMapComparer), new(NodeMapComparer));

			while (pendingNodesSupposedOn.Count != 0 || pendingNodesSupposedOff.Count != 0)
			{
				if (pendingNodesSupposedOn.Count != 0)
				{
					var currentNode = pendingNodesSupposedOn.RemoveFirstNode();
					if (WeakLinkDictionary.TryGetValue(currentNode, out var nodesSupposedOff))
					{
						foreach (var node in nodesSupposedOff)
						{
							var nextNode = new Node(node, currentNode);
							if (resultNodesSupposedOn.Contains(~nextNode))
							{
								// Contradiction is found.
								return;
							}

							if (resultNodesSupposedOff.Add(nextNode))
							{
								pendingNodesSupposedOff.AddLast(nextNode);
							}
						}
					}
				}
				else
				{
					var currentNode = pendingNodesSupposedOff.RemoveFirstNode();
					if (StrongLinkDictionary.TryGetValue(currentNode, out var nodesSupposedOn))
					{
						foreach (var node in nodesSupposedOn)
						{
							var nextNode = new Node(node, currentNode);
							if (resultNodesSupposedOff.Contains(~nextNode))
							{
								// Contradiction is found.
								return;
							}

							if (resultNodesSupposedOn.Add(nextNode))
							{
								pendingNodesSupposedOn.AddLast(nextNode);
							}
						}
					}
				}
			}
		}
	}
}
