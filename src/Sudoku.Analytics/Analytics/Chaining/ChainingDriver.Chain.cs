namespace Sudoku.Analytics.Chaining;

internal partial class ChainingDriver
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
	public static ReadOnlySpan<NamedChain> CollectChains(ref readonly Grid grid, bool onlyFindOne)
	{
		var result = new SortedSet<NamedChain>(ChainingComparers.ChainComparer);
		foreach (var cell in EmptyCells)
		{
			foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~(1 << Solution.GetDigit(cell))))
			{
				var node = new Node((cell * 9 + digit).AsCandidateMap(), true, false);
				if (FindChains(node, in grid, onlyFindOne, result) is { } chain1)
				{
					return (NamedChain[])[chain1];
				}
				if (FindChains(~node, in grid, onlyFindOne, result) is { } chain2)
				{
					return (NamedChain[])[chain2];
				}
			}
		}
		return result.ToArray();
	}

	/// <summary>
	/// <para>
	/// Find all possible <see cref="NamedChain"/> patterns starting with the current node,
	/// and to make an confliction with itself.
	/// </para>
	/// <para>
	/// This method will return <see langword="null"/> if <paramref name="onlyFindOne"/> is <see langword="false"/>,
	/// or the method cannot find a valid chain that forms a confliction with the current node.
	/// </para>
	/// </summary>
	/// <param name="startNode">The current instance.</param>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="onlyFindOne">Indicates whether the method only find one valid <see cref="NamedChain"/> and return.</param>
	/// <param name="result">
	/// A collection that stores all possible found <see cref="NamedChain"/> patterns
	/// if <paramref name="onlyFindOne"/> is <see langword="false"/>.
	/// </param>
	/// <returns>The first found <see cref="NamedChain"/> pattern.</returns>
	/// <seealso cref="NamedChain"/>
	private static NamedChain? FindChains(Node startNode, ref readonly Grid grid, bool onlyFindOne, SortedSet<NamedChain> result)
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
}
