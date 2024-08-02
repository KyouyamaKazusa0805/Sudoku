namespace Sudoku.Analytics.Chaining.Rules;

/// <summary>
/// Represents a chaining rule on normal fish rule (i.e. <see cref="LinkType.KrakenNormalFish"/>).
/// </summary>
/// <seealso cref="LinkType.KrakenNormalFish"/>
internal sealed class CachedKrakenNormalFishChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void GetLinks(ref ChainingRuleLinkContext context)
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
				collect(in context, true, size, digit, sets);
				collect(in context, false, size, digit, sets);
			}
		}


		static void collect(ref readonly ChainingRuleLinkContext context, bool isRow, Digit size, Digit digit, Span<HouseMask> sets)
		{
			var linkOption = context.GetLinkOption(LinkType.KrakenNormalFish);
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
					if (cells.Count < 2)
					{
						// Here we keep the pattern to be "readable", we don't allow Sashimi Kraken Fishes here.
						// Sometimes the Sashimi Kraken Fishes may look very ugly (e.g. X-Wing with only 3 positions).
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
					if (!fins)
					{
						continue;
					}

					var coverSetsMask = HouseMaskOperations.Create(cs);
					var fish = new Fish(digit, baseSetsMask, coverSetsMask, in fins, in CellMap.Empty);
					if (linkOption == LinkOption.Intersection && !fins.IsInIntersection
						|| linkOption == LinkOption.House && !fins.InOneHouse(out _))
					{
						continue;
					}

					// Strong.
					var cells1 = baseSetsMap & ~fins;
					if (cells1.Count < size << 1 || Mask.IsPow2(cells1.BlockMask))
					{
						// A valid fish node cannot be degenerated, or all cells are inside a block.
						continue;
					}

					// Verification: avoid X-Wing nodes that can also be used as UR nodes.
					// This will fix issue #672:
					//   * https://github.com/SunnieShine/Sudoku/issues/672
					// Counter-example:
					//   .+1..6...5..9....1.3....12..2..4+98...+9.1.5..8..68..39...9..3......4..5..91..2+4+97..:714 814 724 824 327 734 834 657 659 169 571 674 184 885
					if (Mask.PopCount(cells1.BlockMask) == 2)
					{
						continue;
					}

					var node1 = new Node(cells1 * digit, false, true);
					var node2 = new Node(fins * digit, true, true);
					context.StrongLinks.AddEntry(node1, node2, true, fish);

					// Weak.
					// Please note that weak links may not contain pattern objects,
					// because it will be rendered into view nodes; but they are plain ones,
					// behaved as normal locked candidate nodes.
					var elimMap = coverSetsMap & ~baseSetsMap;
					var node3 = ~node1;
					var limit = linkOption switch
					{
						LinkOption.Intersection => 3,
						LinkOption.House => Math.Min(elimMap.Count, 9),
						LinkOption.All => elimMap.Count,
						_ => 3
					};
					foreach (var cells4 in elimMap | limit)
					{
						if (linkOption == LinkOption.Intersection && !cells4.IsInIntersection
							|| linkOption == LinkOption.House && !cells4.InOneHouse(out _))
						{
							continue;
						}

						var node4 = new Node(cells4 * digit, false, true);
						context.WeakLinks.AddEntry(node3, node4);
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	public override void GetViewNodes(ref ChainingRuleViewNodeContext context)
	{
		ref readonly var grid = ref context.Grid;
		var pattern = context.Pattern;
		var view = context.View;
		var candidatesMap = grid.CandidatesMap;
		var result = new List<ViewNode>();
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

					var node = new CandidateViewNode(ColorIdentifier.Auxiliary2, candidate);
					view.Add(node);
					result.Add(node);
				}
			}
		}
		context.ProducedViewNodes = result.AsReadOnlySpan();
	}

	/// <inheritdoc/>
	public override void GetLoopConclusions(ref ChainingRuleLoopConclusionContext context)
	{
		ref readonly var grid = ref context.Grid;
		var result = ConclusionSet.Empty;
		var candidatesMap = grid.CandidatesMap;
		var links = context.Links;
		foreach (var link in links)
		{
			if (link is not
				{
					FirstNode.Map.Cells: var firstCells,
					SecondNode.Map.Cells: var secondCells,
					GroupedLinkPattern: Fish { Digit: var digit, BaseSets: var baseSets, CoverSets: var coverSets }
				})
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

			// Filter eliminations on strong links.
			foreach (var otherLink in links)
			{
				if (otherLink is { IsStrong: true, FirstNode.Map.Cells: var map1, SecondNode.Map.Cells: var map2 }
					&& otherLink != link)
				{
					elimMap &= ~map1;
					elimMap &= ~map2;
				}
			}
			result.AddRange(from cell in elimMap select new Conclusion(Elimination, cell, digit));
		}
		context.Conclusions = result;
	}
}
