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
		// Step 1: Iterate on dictionary to get chains.
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

		// Step 2: Sort found patterns and return.
		return result.ToArray();


		ChainOrLoop? bfs(ref readonly Grid grid, Node startNode)
		{
			var pendingNodesSupposedToOn = new LinkedList<Node>();
			var pendingNodesSupposedToOff = new LinkedList<Node>();
			(startNode.IsOn ? pendingNodesSupposedToOff : pendingNodesSupposedToOn).AddLast(startNode);

			var visitedNodesSupposedToOn = new HashSet<Node>(NodeMapComparer);
			var visitedNodesSupposedToOff = new HashSet<Node>(NodeMapComparer);
			visitedNodesSupposedToOn.Add(startNode);
			visitedNodesSupposedToOff.Add(startNode);

			while (pendingNodesSupposedToOn.Count != 0 || pendingNodesSupposedToOff.Count != 0)
			{
				while (pendingNodesSupposedToOn.Count != 0)
				{
					var currentNode = pendingNodesSupposedToOn.RemoveFirstNode();
					if (CachedLinkPool.WeakLinkDictionary.TryGetValue(currentNode, out var nodesSupposedToOff))
					{
						foreach (var nodeSupposedToOff in nodesSupposedToOff)
						{
							var nextNode = new Node(nodeSupposedToOff, currentNode);

							////////////////////////////////////////////
							// Continuous Nice Loop 3) Strong -> Weak //
							////////////////////////////////////////////
							if (nodeSupposedToOff == startNode && nextNode.AncestorsLength >= 4)
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
							if (nodeSupposedToOff == ~startNode)
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
							// Counter-example:
							//   4.+3.6+85...+57.....8+89.5...3..7..+8+6.2.23..94.+8..+84.....15..6..8+7+3+3..+871.5.+7+68.....2:114 124 324 425 427 627 943 366 667 967 272 273 495 497
							if (!nodeSupposedToOff.IsAncestorOf(currentNode, NodeComparison.IgnoreIsOn)
								&& visitedNodesSupposedToOff.Add(nodeSupposedToOff))
							{
								pendingNodesSupposedToOff.AddLast(nextNode);
							}
						}
					}
				}
				while (pendingNodesSupposedToOff.Count != 0)
				{
					var currentNode = pendingNodesSupposedToOff.RemoveFirstNode();
					if (CachedLinkPool.StrongLinkDictionary.TryGetValue(currentNode, out var nodesSupposedToOn))
					{
						foreach (var nodeSupposedToOn in nodesSupposedToOn)
						{
							var nextNode = new Node(nodeSupposedToOn, currentNode);

							/////////////////////////////////////////////
							// Discontinuous Nice Loop 1) Weak -> Weak //
							/////////////////////////////////////////////
							if (nodeSupposedToOn == ~startNode)
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

							if (!nodeSupposedToOn.IsAncestorOf(currentNode, NodeComparison.IgnoreIsOn)
								&& visitedNodesSupposedToOn.Add(nodeSupposedToOn))
							{
								pendingNodesSupposedToOn.AddLast(nextNode);
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
		// Step 1: Iterate on dictionary to get all forcing chains.
		var result = new SortedSet<MultipleForcingChains>(MultipleForcingChainsComparer);
		foreach (var cell in EmptyCells & ~BivalueCells)
		{
			var digitsMask = grid.GetCandidates(cell);
			var nodesSupposedToOn_ImplicitlyConnectToCurrentNode_GroupedByDigit = new NodeDictionary();
			var nodesSupposedToOff_ImplicitlyConnectToCurrentNode_GroupedByDigit = new NodeDictionary();
			var nodesSupposedToOn_ImplicitlyConnectToCurrentNode_LyingInCell = default(HashSet<Node>);
			var nodesSupposedToOff_ImplicitlyConnectToCurrentNode_LyingInCell = default(HashSet<Node>);
			foreach (var digit in digitsMask)
			{
				var currentNode = new Node(cell, digit, true, false);
				bfs(
					currentNode,
					out var nodesSupposedToOnWhichCanImplicitlyConnectToCurrentNode,
					out var nodesSupposedToOffWhichCanImplicitlyConnectToCurrentNode
				);

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

					var nodesSupposedToOn_ImplicitlyConnectToCurrentNode_GroupedByHouse = new NodeDictionary();
					var nodesSupposedToOff_ImplicitlyConnectToCurrentNode_GroupedByHouse = new NodeDictionary();
					var nodesSupposedToOn_ImplicitlyConnectToCurrentNode_LyingInHouse = new HashSet<Node>(NodeMapComparer);
					var nodesSupposedToOff_ImplicitlyConnectToCurrentNode_LyingInHouse = new HashSet<Node>(NodeMapComparer);
					foreach (var otherCell in cellsInHouse)
					{
						if (otherCell == cell)
						{
							nodesSupposedToOn_ImplicitlyConnectToCurrentNode_GroupedByHouse.Add(otherCell * 9 + digit, nodesSupposedToOnWhichCanImplicitlyConnectToCurrentNode);
							nodesSupposedToOff_ImplicitlyConnectToCurrentNode_GroupedByHouse.Add(otherCell * 9 + digit, nodesSupposedToOffWhichCanImplicitlyConnectToCurrentNode);
							nodesSupposedToOn_ImplicitlyConnectToCurrentNode_LyingInHouse.UnionWith(nodesSupposedToOnWhichCanImplicitlyConnectToCurrentNode);
							nodesSupposedToOff_ImplicitlyConnectToCurrentNode_LyingInHouse.UnionWith(nodesSupposedToOffWhichCanImplicitlyConnectToCurrentNode);
						}
						else
						{
							var other = new Node(otherCell, digit, true, false);
							bfs(
								other,
								out var otherNodesSupposedToOn_ImplicitlyConnectToCurrentNode_LyingInHouse,
								out var otherNodesSupposedToOff_ImplicitlyConnectToCurrentNode_LyingInHouse
							);
							nodesSupposedToOn_ImplicitlyConnectToCurrentNode_GroupedByHouse.Add(otherCell * 9 + digit, otherNodesSupposedToOn_ImplicitlyConnectToCurrentNode_LyingInHouse);
							nodesSupposedToOff_ImplicitlyConnectToCurrentNode_GroupedByHouse.Add(otherCell * 9 + digit, otherNodesSupposedToOff_ImplicitlyConnectToCurrentNode_LyingInHouse);
							nodesSupposedToOn_ImplicitlyConnectToCurrentNode_LyingInHouse.IntersectWith(otherNodesSupposedToOn_ImplicitlyConnectToCurrentNode_LyingInHouse);
							nodesSupposedToOff_ImplicitlyConnectToCurrentNode_LyingInHouse.IntersectWith(otherNodesSupposedToOff_ImplicitlyConnectToCurrentNode_LyingInHouse);
						}
					}

					////////////////////////////////////////
					// Collect with region forcing chains //
					////////////////////////////////////////
					foreach (var node in nodesSupposedToOn_ImplicitlyConnectToCurrentNode_LyingInHouse)
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
							var branchNode = nodesSupposedToOn_ImplicitlyConnectToCurrentNode_GroupedByHouse[c * 9 + digit].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
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
					foreach (var node in nodesSupposedToOff_ImplicitlyConnectToCurrentNode_LyingInHouse)
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
							var branchNode = nodesSupposedToOff_ImplicitlyConnectToCurrentNode_GroupedByHouse[c * 9 + digit].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
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
				}

				nodesSupposedToOn_ImplicitlyConnectToCurrentNode_GroupedByDigit.Add(cell * 9 + digit, nodesSupposedToOnWhichCanImplicitlyConnectToCurrentNode);
				nodesSupposedToOff_ImplicitlyConnectToCurrentNode_GroupedByDigit.Add(cell * 9 + digit, nodesSupposedToOffWhichCanImplicitlyConnectToCurrentNode);
				if (nodesSupposedToOn_ImplicitlyConnectToCurrentNode_LyingInCell is null)
				{
					nodesSupposedToOn_ImplicitlyConnectToCurrentNode_LyingInCell = new(NodeMapComparer);
					nodesSupposedToOff_ImplicitlyConnectToCurrentNode_LyingInCell = new(NodeMapComparer);
					nodesSupposedToOn_ImplicitlyConnectToCurrentNode_LyingInCell.UnionWith(nodesSupposedToOnWhichCanImplicitlyConnectToCurrentNode);
					nodesSupposedToOff_ImplicitlyConnectToCurrentNode_LyingInCell.UnionWith(nodesSupposedToOffWhichCanImplicitlyConnectToCurrentNode);
				}
				else
				{
					nodesSupposedToOn_ImplicitlyConnectToCurrentNode_LyingInCell.IntersectWith(nodesSupposedToOnWhichCanImplicitlyConnectToCurrentNode);
					nodesSupposedToOff_ImplicitlyConnectToCurrentNode_LyingInCell!.IntersectWith(nodesSupposedToOffWhichCanImplicitlyConnectToCurrentNode);
				}
			}

			//////////////////////////////////////
			// Collect with cell forcing chains //
			//////////////////////////////////////
			foreach (var node in nodesSupposedToOn_ImplicitlyConnectToCurrentNode_LyingInCell ?? [])
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
					var branchNode = nodesSupposedToOn_ImplicitlyConnectToCurrentNode_GroupedByDigit[cell * 9 + d].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					cfc.Add(cell * 9 + d, node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode));
				}
				if (onlyFindOne)
				{
					return (MultipleForcingChains[])[cfc];
				}
				result.Add(cfc);
			}
			foreach (var node in nodesSupposedToOff_ImplicitlyConnectToCurrentNode_LyingInCell ?? [])
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
					var branchNode = nodesSupposedToOff_ImplicitlyConnectToCurrentNode_GroupedByDigit[cell * 9 + d].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					cfc.Add(cell * 9 + d, node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode));
				}
				if (onlyFindOne)
				{
					return (MultipleForcingChains[])[cfc];
				}
				result.Add(cfc);
			}
		}

		// Step 2: Return the values.
		return result.ToArray();


		static void bfs(
			Node startNode,
			out HashSet<Node> nodesSupposedToOnWhichCanImplicitlyConnectToCurrentNode,
			out HashSet<Node> nodesSupposedToOffWhichCanImplicitlyConnectToCurrentNode
		)
		{
			var (pendingNodesSupposedToOn, pendingNodesSupposedToOff) = (new LinkedList<Node>(), new LinkedList<Node>());
			(startNode.IsOn ? pendingNodesSupposedToOn : pendingNodesSupposedToOff).AddLast(startNode);
			(nodesSupposedToOnWhichCanImplicitlyConnectToCurrentNode, nodesSupposedToOffWhichCanImplicitlyConnectToCurrentNode) = (new(NodeMapComparer), new(NodeMapComparer));

			while (pendingNodesSupposedToOn.Count != 0 || pendingNodesSupposedToOff.Count != 0)
			{
				if (pendingNodesSupposedToOn.Count != 0)
				{
					var currentNode = pendingNodesSupposedToOn.RemoveFirstNode();
					if (CachedLinkPool.WeakLinkDictionary.TryGetValue(currentNode, out var nodesSupposedToOff))
					{
						foreach (var node in nodesSupposedToOff)
						{
							var nextNode = new Node(node, currentNode);
							if (nodesSupposedToOnWhichCanImplicitlyConnectToCurrentNode.Contains(~nextNode))
							{
								// Contradiction is found.
								return;
							}

							if (nodesSupposedToOffWhichCanImplicitlyConnectToCurrentNode.Add(nextNode))
							{
								pendingNodesSupposedToOff.AddLast(nextNode);
							}
						}
					}
				}
				else
				{
					var currentNode = pendingNodesSupposedToOff.RemoveFirstNode();
					if (CachedLinkPool.StrongLinkDictionary.TryGetValue(currentNode, out var nodesSupposedToOn))
					{
						foreach (var node in nodesSupposedToOn)
						{
							var nextNode = new Node(node, currentNode);
							if (nodesSupposedToOffWhichCanImplicitlyConnectToCurrentNode.Contains(~nextNode))
							{
								// Contradiction is found.
								return;
							}

							if (nodesSupposedToOnWhichCanImplicitlyConnectToCurrentNode.Add(nextNode))
							{
								pendingNodesSupposedToOn.AddLast(nextNode);
							}
						}
					}
				}
			}
		}
	}
}
