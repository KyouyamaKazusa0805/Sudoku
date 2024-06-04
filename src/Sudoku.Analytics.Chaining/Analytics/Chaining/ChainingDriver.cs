namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Provides a driver module on chaining.
/// </summary>
public static class ChainingDriver
{
#if DEBUG
	/// <summary>
	/// Collect all <see cref="IChainPattern"/> instances appears in a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="chainingRules">Indicates the chaining rules that are used for searching strong or weak inferences.</param>
	/// <param name="strongLinksDictionary">Indicates the strong link dictionary.</param>
	/// <param name="weakLinksDictionary">Indicates the weak link dictionary.</param>
	/// <returns>All possible <see cref="IChainPattern"/> instances.</returns>
#else
	/// <summary>
	/// Collect all <see cref="IChainPattern"/> instances appears in a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="chainingRules">Indicates the chaining rules that are used for searching strong or weak inferences.</param>
	/// <returns>All possible <see cref="IChainPattern"/> instances.</returns>
#endif
	public static ReadOnlySpan<IChainPattern> CollectChainPatterns(
		ref readonly Grid grid,
		ReadOnlySpan<ChainingRule> chainingRules
#if DEBUG
		,
		out LinkDictionary strongLinksDictionary,
		out LinkDictionary weakLinksDictionary
#endif
	)
	{
		// Step 1: Collect for all strong and weak links appeared in the grid.
		var (strongLinks, weakLinks) = (CreateStrong(in grid, chainingRules), CreateWeak(in grid, chainingRules));
		(strongLinksDictionary, weakLinksDictionary) = (strongLinks, weakLinks);

		// Step 2: Iterate on dictionary to get chains.
		var chainsFound = new HashSet<IChainPattern>(
			EqualityComparer<IChainPattern>.Create(
				static (left, right) => (left, right) switch
				{
					(null, null) => true,
					(Chain a, Chain b) => a.Equals(b),
					(Loop a, Loop b) => a.Equals(b),
					(not null, not null) => left.Equals(right, NodeComparison.IgnoreIsOn, ChainPatternComparison.Undirected),
					_ => false
				},
				static obj => obj switch
				{
					Chain c => c.GetHashCode(),
					Loop l => l.GetHashCode(),
					_ => obj.GetHashCode(NodeComparison.IgnoreIsOn, ChainPatternComparison.Undirected)
				}
			)
		);
		foreach (var cell in grid.EmptyCells)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				var node = new Node(cell * 9 + digit, true);
				bfs(node, chainsFound);
				bfs(~node, chainsFound);
			}
		}
		return chainsFound.ToArray();


		void bfs(Node startNode, HashSet<IChainPattern> result)
		{
			var comparer = EqualityComparer<Node>.Create(
				static (left, right) => (left, right) switch
				{
					(null, null) => true,
					(not null, not null) => left.Equals(right, NodeComparison.IgnoreIsOn),
					_ => false
				},
				static obj => obj.GetHashCode(NodeComparison.IgnoreIsOn)
			);
			var pendingStrong = new LinkedList<Node>();
			var pendingWeak = new LinkedList<Node>();
			var visitedStrong = new HashSet<Node>(comparer);
			var visitedWeak = new HashSet<Node>(comparer);
			(startNode.IsOn ? pendingWeak : pendingStrong).AddLast(startNode);

			while (pendingStrong.Count != 0 || pendingWeak.Count != 0)
			{
				while (pendingStrong.Count != 0)
				{
					var currentNode = pendingStrong.First!.Value;
					pendingStrong.RemoveFirst();
					if (weakLinks.TryGetValue(currentNode, out var nodes))
					{
						foreach (var node in nodes)
						{
							if (node == ~startNode)
							{
								result.Add(new Chain(new(node, currentNode), true)); // Discontinuous Nice Loop 2) Strong -> Strong.
							}

							// This step will filter duplicate nodes in order not to make a internal loop on chains.
							if (!currentNode.IsAncestorOf(node) && visitedWeak.Add(node))
							{
								pendingWeak.AddLast(new Node(node, currentNode));
							}
						}
					}
				}
				while (pendingWeak.Count != 0)
				{
					var currentNode = pendingWeak.First!.Value;
					pendingWeak.RemoveFirst();
					if (strongLinks.TryGetValue(currentNode, out var nodes))
					{
						foreach (var node in nodes)
						{
							if (node == startNode)
							{
								var resultNode = new Node(node, currentNode);
								if (resultNode.AncestorsLength >= 4)
								{
									result.Add(new Loop(resultNode)); // Continuous Nice Loop 3) Strong -> Weak.
								}
							}
							if (node == ~startNode)
							{
								var resultNode = new Node(node, currentNode);
								result.Add(new Chain(resultNode, false)); // Discontinuous Nice Loop 1) Weak -> Weak.
							}

							if (!currentNode.IsAncestorOf(node) && visitedStrong.Add(node))
							{
								pendingStrong.AddLast(new Node(node, currentNode));
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Creates a <see cref="LinkDictionary"/> instance that holds a list of strong link relations.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="chainingRules">The chaining rules.</param>
	/// <returns>A <see cref="LinkDictionary"/> as result.</returns>
	private static LinkDictionary CreateStrong(ref readonly Grid grid, ReadOnlySpan<ChainingRule> chainingRules)
	{
		var result = new LinkDictionary();
		foreach (var chainingRule in chainingRules)
		{
			chainingRule.CollectStrongLinks(in grid, result);
		}
		return result;
	}

	/// <summary>
	/// Creates a <see cref="LinkDictionary"/> instance that holds a list of weak link relations.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="chainingRules">The chaining rules.</param>
	/// <returns>A <see cref="LinkDictionary"/> as result.</returns>
	private static LinkDictionary CreateWeak(ref readonly Grid grid, ReadOnlySpan<ChainingRule> chainingRules)
	{
		var result = new LinkDictionary();
		foreach (var chainingRule in chainingRules)
		{
			chainingRule.CollectWeakLinks(in grid, result);
		}
		return result;
	}
}
