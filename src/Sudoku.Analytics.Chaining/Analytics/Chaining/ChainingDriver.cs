namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Provides a driver module on chaining.
/// </summary>
public static class ChainingDriver
{
	/// <summary>
	/// Collect all <see cref="ChainPattern"/> instances appears in a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="rules">
	/// Indicates the rule instances that will create strong and weak links by their own represented concept.
	/// </param>
	/// <returns>All possible <see cref="ChainPattern"/> instances.</returns>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/developer-notes" />
	/// A valid chain can only belong to the following three cases:
	/// <list type="number">
	/// <item>
	/// <b>Discontinuous Nice Loop</b><br/>
	/// Start with weak link, alternating strong and weak links and return to itself by weak link
	/// (with an odd number of nodes).
	/// </item>
	/// <item>
	/// <b>Discontinuous Nice Loop</b><br/>
	/// Start with strong link, alternating strong and weak links and return to itself by strong link
	/// (with an odd number of nodes).
	/// </item>
	/// <item>
	/// <b>Continuous Nice Loop</b><br/>
	/// Start with strong link, alternating strong and weak links and return to itself by weak link
	/// (with an even number of nodes).
	/// </item>
	/// </list>
	/// </remarks>
	public static ReadOnlySpan<ChainPattern> CollectChainPatterns(ref readonly Grid grid, ReadOnlySpan<ChainingRule> rules)
	{
		// Step 1: Collect for all strong and weak links appeared in the grid.
		var (strongLinks, weakLinks) = (new LinkDictionary(), new LinkDictionary());
		foreach (var chainingRule in rules)
		{
			chainingRule.CollectStrongLinks(in grid, strongLinks);
		}
		foreach (var chainingRule in rules)
		{
			chainingRule.CollectWeakLinks(in grid, weakLinks);
		}

		// Step 2: Iterate on dictionary to get chains.
		var foundPatterns = new HashSet<ChainPattern>(LocalComparer.ChainPatternComparer);
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
		var finalCollection = new List<ChainPattern>();
		foreach (var pattern in foundPatterns)
		{
			if (pattern.GetConclusions(in grid))
			{
				finalCollection.Add(pattern);
			}
		}

		// Step 4: Sort found patterns and return.
		finalCollection.Sort();
		return finalCollection.AsReadOnlySpan();


		void bfs(Node startNode, HashSet<ChainPattern> result)
		{
			var pendingStrong = new LinkedList<Node>();
			var pendingWeak = new LinkedList<Node>();
			(startNode.IsOn ? pendingWeak : pendingStrong).AddLast(startNode);

			var visitedStrong = new HashSet<Node>(LocalComparer.NodeMapComparer);
			var visitedWeak = new HashSet<Node>(LocalComparer.NodeMapComparer);
			visitedStrong.Add(startNode);
			visitedWeak.Add(startNode);

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
							var resultNode = new Node(node, currentNode);
							if (node == startNode && resultNode.AncestorsLength >= 4)
							{
								result.Add(new Loop(resultNode)); // Continuous Nice Loop 3) Strong -> Weak.
							}
							if (node == ~startNode)
							{
								result.Add(new Chain(resultNode)); // Discontinuous Nice Loop 2) Strong -> Strong.
							}

							// This step will filter duplicate nodes in order not to make a internal loop on chains.
							if (!node.IsAncestorOf(currentNode, NodeComparison.IgnoreIsOn) && visitedWeak.Add(node))
							{
								pendingWeak.AddLast(resultNode);
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
							var resultNode = new Node(node, currentNode);
							if (node == ~startNode)
							{
								result.Add(new Chain(resultNode)); // Discontinuous Nice Loop 1) Weak -> Weak.
							}

							if (!node.IsAncestorOf(currentNode, NodeComparison.IgnoreIsOn) && visitedStrong.Add(node))
							{
								pendingStrong.AddLast(resultNode);
							}
						}
					}
				}
			}
		}
	}
}

/// <summary>
/// The file-local comparer generator, lazily initialized.
/// </summary>
file static class LocalComparer
{
	/// <summary>
	/// Indicates the backing field of chain pattern comparer instance.
	/// </summary>
	private static IEqualityComparer<ChainPattern>? _chainPatternComparer;

	/// <summary>
	/// Indicates the backing field of node map comparer instance.
	/// </summary>
	private static IEqualityComparer<Node>? _nodeComparer;


	/// <summary>
	/// Creates an instance of type <see cref="EqualityComparer{T}"/> of <see cref="ChainPattern"/> on equality comparison
	/// in order to filter duplicate chains.
	/// </summary>
	/// <returns>An <see cref="EqualityComparer{T}"/> instance.</returns>
	public static IEqualityComparer<ChainPattern> ChainPatternComparer
		=> _chainPatternComparer ??= EqualityComparer<ChainPattern>.Create(
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

	/// <summary>
	/// Creates an instance of type <see cref="EqualityComparer{T}"/> of <see cref="Node"/> on equality comparison
	/// in order to filter duplicate nodes on its containing map, guaranteeing same nodes won't be traversed multiple times.
	/// </summary>
	/// <returns>An <see cref="EqualityComparer{T}"/> instance.</returns>
	public static IEqualityComparer<Node> NodeMapComparer
		=> _nodeComparer ??= EqualityComparer<Node>.Create(
			static (left, right) => (left, right) switch
			{
				(not null, not null) => left.Equals(right, NodeComparison.IgnoreIsOn),
				(null, null) => true,
				_ => false
			},
			static obj => obj.GetHashCode(NodeComparison.IgnoreIsOn)
		);
}
