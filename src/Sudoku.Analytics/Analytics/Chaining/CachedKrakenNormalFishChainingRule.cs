namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on normal fish rule (i.e. <see cref="LinkType.KrakenNormalFish"/>).
/// </summary>
/// <seealso cref="LinkType.KrakenNormalFish"/>
internal sealed class CachedKrakenNormalFishChainingRule : ChainingRule
{
	/// <inheritdoc/>
	internal override void CollectLinks(ref readonly Grid grid, LinkDictionary strongLinks, LinkDictionary weakLinks)
	{
		// Collect for available rows and columns.
		var sets = (stackalloc HouseMask[9]);
		sets.Clear();
		for (var digit = 0; digit < 9; digit++)
		{
			if (ValuesMap[digit].Count == 9)
			{
				continue;
			}

			for (var house = 9; house < 27; house++)
			{
				if ((CandidatesMap[digit] & HousesMap[house]).Count >= 2)
				{
					sets[digit] |= 1 << house;
				}
			}
		}

		// Iterate on each combination of base and cover sets.
		for (var size = 2; size <= 4; size++)
		{
			for (var digit = 0; digit < 9; digit++)
			{
				collect(true, size, digit, sets);
				collect(false, size, digit, sets);
			}
		}

		void collect(bool isRow, int size, Digit digit, ReadOnlySpan<HouseMask> sets)
		{
			var baseSetsToIterate = (sets[digit] & ~(isRow ? HouseMaskOperations.AllColumnsMask : HouseMaskOperations.AllRowsMask)).GetAllSets();
			var coverSetsToIterate = (sets[digit] & ~(isRow ? HouseMaskOperations.AllRowsMask : HouseMaskOperations.AllColumnsMask)).GetAllSets();
			if (baseSetsToIterate.Length < size || coverSetsToIterate.Length < size)
			{
				return;
			}

			foreach (var bs in baseSetsToIterate.GetSubsets(size))
			{
				var (baseSetsMap, baseSetIsValid) = (CellMap.Empty, true);
				foreach (var p in bs)
				{
					var cells = CandidatesMap[digit] & HousesMap[p];
					if (cells.IsInIntersection)
					{
						baseSetIsValid = false;
						break;
					}
					baseSetsMap |= cells;
				}
				if (!baseSetIsValid)
				{
					continue;
				}

				var baseSetsMask = HouseMaskOperations.Create(bs);
				foreach (var cs in coverSetsToIterate.GetSubsets(size))
				{
					var coverSetsMap = CellMap.Empty;
					foreach (var p in cs)
					{
						var cells = CandidatesMap[digit] & HousesMap[p];
						coverSetsMap |= cells;
					}

					var fins = baseSetsMap & ~coverSetsMap;
					if (!fins || !fins.IsInIntersection)
					{
						continue;
					}

					// Strong.
					var node1 = new Node((baseSetsMap & ~fins) * digit, false, true);
					var node2 = new Node(fins * digit, true, true);
					var coverSetsMask = HouseMaskOperations.Create(cs);
					var fish = new Fish(digit, baseSetsMask, coverSetsMask, in fins, in CellMap.Empty);
					strongLinks.AddEntry(node1, node2, true, fish);

					// Weak.
					var elimMap = coverSetsMap & ~baseSetsMap;
					var cells3 = baseSetsMap & ~fins;
					if (IsPow2(cells3.BlockMask))
					{
						continue;
					}

					var node3 = new Node(cells3 * digit, true, true);
					foreach (var cells4 in elimMap | 3)
					{
						if (!cells4.IsInIntersection)
						{
							continue;
						}

						var node4 = new Node(cells4 * digit, false, true);
						weakLinks.AddEntry(node3, node4, false, fish);
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	internal override void CollectExtraViewNodes(ref readonly Grid grid, ChainOrLoop pattern, ref View[] views)
	{
		var (view, id) = (views[0], ColorIdentifier.Auxiliary2);
		var candidatesMap = grid.CandidatesMap;
		foreach (var link in pattern.Links)
		{
			if (link.GroupedLinkPattern is not Fish { Digit: var digit, BaseSets: var baseSets, Exofins: var fins })
			{
				continue;
			}

			foreach (var baseSet in baseSets)
			{
				foreach (var cell in candidatesMap[digit] & HousesMap[baseSet] & ~fins)
				{
					var candidate = cell * 9 + digit;
					if (view.FindCandidate(candidate) is { } candidateViewNode)
					{
						view.Remove(candidateViewNode);
					}
					view.Add(new CandidateViewNode(id, candidate));
				}
			}
		}
	}

	/// <inheritdoc/>
	internal override ConclusionSet CollectLoopConclusions(Loop loop, ref readonly Grid grid)
	{
		var result = ConclusionSet.Empty;
		var candidatesMap = grid.CandidatesMap;
		foreach (var link in loop.Links)
		{
			if (link.GroupedLinkPattern is not Fish { Digit: var digit, BaseSets: var baseSets, CoverSets: var coverSets })
			{
				continue;
			}

			var elimMap = CellMap.Empty;
			foreach (var coverSet in coverSets)
			{
				elimMap |= HousesMap[coverSet] & candidatesMap[digit];
			}
			foreach (var baseSet in baseSets)
			{
				elimMap &= ~HousesMap[baseSet];
			}

			result.AddRange(from cell in elimMap select new Conclusion(Elimination, cell, digit));
		}
		return result;
	}
}
