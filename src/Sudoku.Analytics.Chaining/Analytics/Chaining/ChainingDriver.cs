namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Provides a driver module on chaining.
/// </summary>
public static class ChainingDriver
{
	/// <summary>
	/// Collect all <see cref="IChainPattern"/> instances appears in a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="rules">
	/// Indicates the rule instances that will create strong and weak links by their own represented concept.
	/// </param>
	/// <returns>All possible <see cref="IChainPattern"/> instances.</returns>
	public static ReadOnlySpan<IChainPattern> CollectChainPatterns(ref readonly Grid grid, ReadOnlySpan<ChainingRule> rules)
	{
		// Step 1: Collect for all strong and weak links appeared in the grid.
		var (strongLinks, weakLinks) = (new LinkDictionary(), new LinkDictionary());
		foreach (var chainingRule in rules)
		{
			chainingRule.CollectStrongLinks(in grid, strongLinks);
		}
		foreach (var chainingRule in rules)
		{
			chainingRule.CollectStrongLinks(in grid, weakLinks);
		}

		// Step 2: Iterate on dictionary to get chains.
		var patternEqualityComparer = EqualityComparer<IChainPattern>.Create(
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
		);
		var foundPatterns = new HashSet<IChainPattern>(patternEqualityComparer);
		foreach (var cell in grid.EmptyCells)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				var node = new Node(cell * 9 + digit, true);
				bfs(node, foundPatterns);
				bfs(~node, foundPatterns);
			}
		}

		// Step 3: Check eliminations. If a chain doesn't contain any possible conclusions,
		// it will be removed from the result collection.
		var finalCollection = new List<IChainPattern>();
		foreach (var pattern in foundPatterns)
		{
			if (pattern.GetConclusions(in grid))
			{
				finalCollection.Add(pattern);
			}
		}
		return finalCollection.AsReadOnlySpan();


		void bfs(Node startNode, HashSet<IChainPattern> result)
		{
			var traversedNodesComparer = EqualityComparer<Node>.Create(
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
			var visitedStrong = new HashSet<Node>(traversedNodesComparer);
			var visitedWeak = new HashSet<Node>(traversedNodesComparer);
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
}
