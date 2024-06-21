namespace Sudoku.Analytics.Chaining;

using static CachedChainingComparers;
using NodeDictionary = Dictionary<Candidate, HashSet<Node>>;

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
		var result = new SortedSet<ChainOrLoop>(ChainComparer);
		foreach (var cell in EmptyCells)
		{
			foreach (var digit in (Mask)(grid.GetCandidates(cell) & (Mask)~(1 << Solution.GetDigit(cell))))
			{
				var node = new Node(cell, digit, true, false);
				if (FindChain(in grid, node, onlyFindOne, result) is { } chain1)
				{
					return (ChainOrLoop[])[chain1];
				}
				if (FindChain(in grid, ~node, onlyFindOne, result) is { } chain2)
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
		var result = new SortedSet<MultipleForcingChains>(MultipleForcingChainsComparer);
		foreach (var cell in EmptyCells & ~BivalueCells)
		{
			var (nodesSupposedOn_GroupedByDigit, nodesSupposedOff_GroupedByDigit) = (new NodeDictionary(), new NodeDictionary());
			var (nodesSupposedOn_InCell, nodesSupposedOff_InCell) = default((HashSet<Node>, HashSet<Node>));

			var digitsMask = grid.GetCandidates(cell);
			foreach (var digit in digitsMask)
			{
				var currentNode = new Node(cell, digit, true, false);
				var (nodesSupposedOn, nodesSupposedOff) = FindNodesImplicitTo(currentNode);

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

					var (nodesSupposedOn_GroupedByHouse, nodesSupposedOff_GroupedByHouse) = (new NodeDictionary(), new NodeDictionary());
					var (nodesSupposedOn_InHouse, nodesSupposedOff_InHouse) = (new HashSet<Node>(NodeMapComparer), new HashSet<Node>(NodeMapComparer));
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
							var other = new Node(otherCandidate, true, false);
							var (otherNodesSupposedOn_InHouse, otherNodesSupposedOff_InHouse) = FindNodesImplicitTo(other);
							nodesSupposedOn_GroupedByHouse.Add(otherCandidate, otherNodesSupposedOn_InHouse);
							nodesSupposedOff_GroupedByHouse.Add(otherCandidate, otherNodesSupposedOff_InHouse);
							nodesSupposedOn_InHouse.IntersectWith(otherNodesSupposedOn_InHouse);
							nodesSupposedOff_InHouse.IntersectWith(otherNodesSupposedOff_InHouse);
						}
					}

					////////////////////////////////////////
					// Collect with region forcing chains //
					////////////////////////////////////////
					#region Region forcing chains - On
					foreach (var node in nodesSupposedOn_InHouse)
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
							var branchNode = nodesSupposedOn_GroupedByHouse[c * 9 + digit].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
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
					#endregion
					#region Region forcing chains - Off
					foreach (var node in nodesSupposedOff_InHouse)
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
							var branchNode = nodesSupposedOff_GroupedByHouse[c * 9 + digit].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
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
					#endregion
				}

				nodesSupposedOn_GroupedByDigit.Add(cell * 9 + digit, nodesSupposedOn);
				nodesSupposedOff_GroupedByDigit.Add(cell * 9 + digit, nodesSupposedOff);
				if (nodesSupposedOn_InCell is null)
				{
					(nodesSupposedOn_InCell, nodesSupposedOff_InCell) = (new(NodeMapComparer), new(NodeMapComparer));
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
			#region Cell forcing chains - On
			foreach (var node in nodesSupposedOn_InCell ?? [])
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
					var branchNode = nodesSupposedOn_GroupedByDigit[cell * 9 + d].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					cfc.Add(cell * 9 + d, node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode));
				}
				if (onlyFindOne)
				{
					return (MultipleForcingChains[])[cfc];
				}
				result.Add(cfc);
			}
			#endregion
			#region Cell forcing chains - Off
			foreach (var node in nodesSupposedOff_InCell ?? [])
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
					var branchNode = nodesSupposedOff_GroupedByDigit[cell * 9 + d].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					cfc.Add(cell * 9 + d, node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode));
				}
				if (onlyFindOne)
				{
					return (MultipleForcingChains[])[cfc];
				}
				result.Add(cfc);
			}
			#endregion
		}
		return result.ToArray();
	}

	/// <summary>
	/// Collect all blossom loops appeared in a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="onlyFindOne">Indicates whether the method only find one valid chain.</param>
	/// <returns>All possible multiple forcing chain instances.</returns>
	public static ReadOnlySpan<BlossomLoop> CollectBlossomLoops(ref readonly Grid grid, bool onlyFindOne)
	{
		return [];
	}

	/// <summary>
	/// <para>
	/// Find all possible <see cref="ChainOrLoop"/> patterns starting with <paramref name="startNode"/>,
	/// and to make an confliction with itself.
	/// </para>
	/// <para>
	/// This method will return <see langword="null"/> if <paramref name="onlyFindOne"/> is <see langword="false"/>,
	/// or the method cannot find a valid chain that forms a confliction with <paramref name="startNode"/>.
	/// </para>
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="startNode">The node as the start.</param>
	/// <param name="onlyFindOne">Indicates whether the method only find one valid <see cref="ChainOrLoop"/> and return.</param>
	/// <param name="result">
	/// A collection that stores all possible found <see cref="ChainOrLoop"/> patterns
	/// if <paramref name="onlyFindOne"/> is <see langword="false"/>.
	/// </param>
	/// <returns>The first found <see cref="ChainOrLoop"/> pattern.</returns>
	/// <remarks>
	/// This method uses breadth-first searching (BFS) algorithm.
	/// </remarks>
	private static ChainOrLoop? FindChain(ref readonly Grid grid, Node startNode, bool onlyFindOne, SortedSet<ChainOrLoop> result)
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

	/// <summary>
	/// <para>Finds a list of nodes that can implicitly connects to <paramref name="startNode"/> via a forcing chain.</para>
	/// <para>This method only uses cached fields <see cref="StrongLinkDictionary"/> and <see cref="WeakLinkDictionary"/>.</para>
	/// </summary>
	/// <param name="startNode">The node as the start.</param>
	/// <returns>
	/// Returns a pair of <see cref="HashSet{T}"/> of <see cref="Node"/> instances, indicating all possible nodes
	/// that can implicitly connects to the <paramref name="startNode"/> via the whole forcing chain,
	/// grouped by their own initial states.
	/// </returns>
	/// <remarks>
	/// This method uses breadth-first searching (BFS) algorithm.
	/// </remarks>
	/// <seealso cref="StrongLinkDictionary"/>
	/// <seealso cref="WeakLinkDictionary"/>
	private static (HashSet<Node> OnNodes, HashSet<Node> OffNodes) FindNodesImplicitTo(Node startNode)
	{
		var (pendingNodesSupposedOn, pendingNodesSupposedOff) = (new LinkedList<Node>(), new LinkedList<Node>());
		(startNode.IsOn ? pendingNodesSupposedOn : pendingNodesSupposedOff).AddLast(startNode);
		var (nodesSupposedOn, nodesSupposedOff) = (new HashSet<Node>(NodeMapComparer), new HashSet<Node>(NodeMapComparer));

		while (pendingNodesSupposedOn.Count != 0 || pendingNodesSupposedOff.Count != 0)
		{
			if (pendingNodesSupposedOn.Count != 0)
			{
				var currentNode = pendingNodesSupposedOn.RemoveFirstNode();
				if (WeakLinkDictionary.TryGetValue(currentNode, out var supposedOff))
				{
					foreach (var node in supposedOff)
					{
						var nextNode = new Node(node, currentNode);
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
						var nextNode = new Node(node, currentNode);
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
		return (nodesSupposedOn, nodesSupposedOff);
	}
}
