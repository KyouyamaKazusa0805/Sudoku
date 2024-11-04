namespace Sudoku.Analytics.Construction.Chaining;

internal partial class ChainingDriver
{
	/// <summary>
	/// Collect all multiple forcing chains on applying to a unique rectangle, appeared in a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="onlyFindOne">Indicates whether the method only find one valid chain.</param>
	/// <returns>All possible multiple forcing chain instances.</returns>
	public static ReadOnlySpan<RectangleForcingChains> CollectRectangleMultipleChains(ref readonly Grid grid, bool onlyFindOne)
	{
		var result = new SortedSet<RectangleForcingChains>(ChainingComparers.MultipleForcingChainsComparer);
		foreach (var urCells in UniqueRectanglePattern.AllPatterns)
		{
			var cells = urCells.AsCellMap();
			if ((EmptyCells & cells) != cells)
			{
				// Not all 4 cells are empty cells.
				continue;
			}

			// Collect all digits of the rectangle, to determine which digits can be used as a rectangle.
			var allDigitsMask = grid[cells].GetAllSets();
			if (allDigitsMask.Length is 0 or 1)
			{
				// No enough digits available.
				continue;
			}

			// Iterate on each combination of pair of digits, as ones used by rectangle.
			foreach (var digitPair in allDigitsMask.GetSubsets(2))
			{
				var (d1, d2) = (digitPair[0], digitPair[1]);

				// Determine whether such digits can be filled in diagonal cells.
				// If a rectangle is correct, we should guarantee both digits can be filled twice in rectangle pattern,
				// in order to make a valid deadly pattern (i.e. guarantee having at least one possible unavoidable set).
				var suchDigitsCanBeFilledInDiagonalCells = true;
				foreach (var ((c1, c4), (c2, c3)) in (((urCells[0], urCells[3]), (urCells[1], urCells[2])), ((urCells[1], urCells[2]), (urCells[0], urCells[3]))))
				{
					if (((grid.GetCandidates(c1) & grid.GetCandidates(c4)) >> d1 & 1) == 0
						|| ((grid.GetCandidates(c2) & grid.GetCandidates(c3)) >> d2 & 1) == 0)
					{
						suchDigitsCanBeFilledInDiagonalCells = false;
						break;
					}
					break;
				}
				if (!suchDigitsCanBeFilledInDiagonalCells)
				{
					continue;
				}

				// If so, the UR pattern is valid. Now check for forcing chains.
				var branchStartCandidates = CandidateMap.Empty;
				foreach (var cell in urCells)
				{
					foreach (var digit in (Mask)(grid.GetCandidates(cell) & ~(1 << d1 | 1 << d2)))
					{
						branchStartCandidates.Add(cell * 9 + digit);
					}
				}

				// Collect branches for all possible candidates to be iterated,
				// and determine whether at least one candidate can be considered as a conclusion from all different start candidates.
				var nodesSupposedOnGrouped = new Dictionary<Candidate, HashSet<Node>>();
				var nodesSupposedOffGrouped = new Dictionary<Candidate, HashSet<Node>>();
				var resultNodesSupposedOn = default(HashSet<Node>);
				var resultNodesSupposedOff = default(HashSet<Node>);
				foreach (var candidate in branchStartCandidates)
				{
					var currentNode = new Node(candidate.AsCandidateMap(), true);
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

				var step1 = rfcOn(urCells, in grid, in branchStartCandidates, nodesSupposedOnGrouped, resultNodesSupposedOn);
				if (!step1.IsEmpty)
				{
					return step1;
				}

				var step2 = rfcOff(urCells, in grid, in branchStartCandidates, nodesSupposedOffGrouped, resultNodesSupposedOff);
				if (!step2.IsEmpty)
				{
					return step2;
				}
			}
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

		ReadOnlySpan<RectangleForcingChains> rfcOn(
			Cell[] urCells,
			ref readonly Grid grid,
			scoped ref readonly CandidateMap branchStartCandidates,
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

				var rfc = new RectangleForcingChains(urCells, conclusion);
				foreach (var candidate in branchStartCandidates)
				{
					var branchNode = onNodes[candidate].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					rfc.Add(candidate, node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode));
				}
				if (onlyFindOne)
				{
					return (RectangleForcingChains[])[rfc];
				}
				result.Add(rfc);
			}
			return [];
		}

		ReadOnlySpan<RectangleForcingChains> rfcOff(
			Cell[] urCells,
			ref readonly Grid grid,
			scoped ref readonly CandidateMap branchStartCandidates,
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

				var rfc = new RectangleForcingChains(urCells);
				foreach (var candidate in branchStartCandidates)
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
					return (RectangleForcingChains[])[rfc];
				}
				result.Add(rfc);
			}
			return [];
		}
	}
}
