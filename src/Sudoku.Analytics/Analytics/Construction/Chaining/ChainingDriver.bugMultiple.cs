namespace Sudoku.Analytics.Construction.Chaining;

internal partial class ChainingDriver
{
	/// <summary>
	/// Collect all multiple forcing chains on applying to a bi-value universal grave, appeared in a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="onlyFindOne">Indicates whether the method only find one valid chain.</param>
	/// <returns>All possible multiple forcing chain instances.</returns>
	public static ReadOnlySpan<BivalueUniversalGraveForcingChains> CollectBivalueUniversalGraveMultipleChains(ref readonly Grid grid, bool onlyFindOne)
	{
		var result = new SortedSet<BivalueUniversalGraveForcingChains>(ChainingComparers.MultipleForcingChainsComparer);
		var trueCandidates = TrueCandidate.GetAllTrueCandidates(in grid);
		if (!trueCandidates)
		{
			return [];
		}

		// Collect branches for all possible candidates to be iterated,
		// and determine whether at least one candidate can be considered as a conclusion from all different start candidates.
		var nodesSupposedOnGrouped = new Dictionary<Candidate, HashSet<Node>>();
		var nodesSupposedOffGrouped = new Dictionary<Candidate, HashSet<Node>>();
		var resultNodesSupposedOn = default(HashSet<Node>);
		var resultNodesSupposedOff = default(HashSet<Node>);
		foreach (var candidate in trueCandidates)
		{
			var currentNode = new Node(candidate.AsCandidateMap(), true, false);
			var (nodesSupposedOn, nodesSupposedOff) = FindForcingChains(currentNode);

			nodesSupposedOnGrouped.Add(candidate, nodesSupposedOn);
			nodesSupposedOffGrouped.Add(candidate, nodesSupposedOff);
			if (resultNodesSupposedOn is null)
			{
				resultNodesSupposedOn = new(ChainingComparers.NodeMapComparer);
				resultNodesSupposedOff = new(ChainingComparers.NodeMapComparer);
				resultNodesSupposedOn.UnionWith(nodesSupposedOn);
				resultNodesSupposedOff.UnionWith(nodesSupposedOff);
			}
			else
			{
				Debug.Assert(resultNodesSupposedOff is not null);
				resultNodesSupposedOn.IntersectWith(nodesSupposedOn);
				resultNodesSupposedOff.IntersectWith(nodesSupposedOff);
			}
		}

		var step1 = rfcOn(in trueCandidates, in grid, nodesSupposedOnGrouped, resultNodesSupposedOn);
		if (!step1.IsEmpty)
		{
			return step1;
		}

		var step2 = rfcOff(in trueCandidates, in grid, nodesSupposedOffGrouped, resultNodesSupposedOff);
		if (!step2.IsEmpty)
		{
			return step2;
		}

		return result.ToArray();


		static Conclusion[] getThoroughConclusions(ref readonly Grid grid, MultipleForcingChains mfc)
		{
			// Modify conclusions in order to check more thoroughly.
			var map = CandidateMap.Empty;
			foreach (var branch in mfc.Values)
			{
				map |= branch[1].Map;
			}

			var newConclusions = new List<Conclusion>();
			foreach (var candidate in map.PeerIntersection)
			{
				if (grid.Exists(candidate) is true)
				{
					newConclusions.Add(new(Elimination, candidate));
				}
			}
			return newConclusions.Count == 0 ? [] : [.. newConclusions];
		}

		ReadOnlySpan<BivalueUniversalGraveForcingChains> rfcOn(
			scoped ref readonly CandidateMap trueCandidates,
			ref readonly Grid grid,
			Dictionary<Candidate, HashSet<Node>> onNodes,
			HashSet<Node>? resultOnNodes
		)
		{
			foreach (var node in resultOnNodes ?? [])
			{
				if (node.IsGroupedNode)
				{
					// Grouped nodes are not supported as target node.
					continue;
				}

				var conclusion = new Conclusion(Assignment, node.Map[0]);
				if (grid.Exists(conclusion.Candidate) is not true)
				{
					continue;
				}

				var rfc = new BivalueUniversalGraveForcingChains(in trueCandidates, conclusion);
				foreach (var candidate in trueCandidates)
				{
					var branchNode = onNodes[candidate].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					rfc.Add(candidate, node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode));
				}
				if (onlyFindOne)
				{
					return (BivalueUniversalGraveForcingChains[])[rfc];
				}
				result.Add(rfc);
			}
			return [];
		}

		ReadOnlySpan<BivalueUniversalGraveForcingChains> rfcOff(
			scoped ref readonly CandidateMap trueCandidates,
			ref readonly Grid grid,
			Dictionary<Candidate, HashSet<Node>> offNodes,
			HashSet<Node>? resultOffNodes
		)
		{
			foreach (var node in resultOffNodes ?? [])
			{
				if (node.IsGroupedNode)
				{
					// Grouped nodes are not supported as target node.
					continue;
				}

				var conclusion = new Conclusion(Elimination, node.Map[0]);
				if (grid.Exists(conclusion.Candidate) is not true)
				{
					continue;
				}

				var rfc = new BivalueUniversalGraveForcingChains(in trueCandidates);
				foreach (var candidate in trueCandidates)
				{
					var branchNode = offNodes[candidate].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					rfc.Add(candidate, node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode));
				}
				if (getThoroughConclusions(in grid, rfc) is not { Length: not 0 } conclusions)
				{
					continue;
				}

				rfc.Conclusions = conclusions;
				if (onlyFindOne)
				{
					return (BivalueUniversalGraveForcingChains[])[rfc];
				}
				result.Add(rfc);
			}
			return [];
		}
	}
}
