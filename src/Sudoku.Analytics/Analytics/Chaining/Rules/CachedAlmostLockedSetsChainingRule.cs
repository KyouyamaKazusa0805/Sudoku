namespace Sudoku.Analytics.Chaining.Rules;

/// <summary>
/// Represents a chaining rule on ALS rule (i.e. <see cref="LinkType.AlmostLockedSets"/>).
/// </summary>
/// <seealso cref="LinkType.AlmostLockedSets"/>
internal sealed class CachedAlmostLockedSetsChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void GetLinks(ref ChainingRuleLinkContext context)
	{
		ref readonly var grid = ref context.Grid;
		var linkOption = context.GetLinkOption(LinkType.AlmostLockedSets);
		var maskTempList = (stackalloc Mask[81]);
		foreach (var als in AlmostLockedSetsModule.CollectAlmostLockedSets(in grid))
		{
			if (als is not (var digitsMask, var cells) { IsBivalueCell: false, StrongLinks: var links, House: var house })
			{
				// This ALS is special case - it only uses 2 digits in a cell.
				// This will be handled as a normal bi-value strong link (Y rule).
				continue;
			}

			// Avoid the ALS chosen contains a sub-subset, meaning some cells held by ALS forms a subset.
			// If so, the ALS can be reduced.
			var isAlsCanBeReduced = false;
			maskTempList.Clear();
			foreach (var cell in cells)
			{
				maskTempList[cell] = grid.GetCandidates(cell);
			}
			foreach (var subsetCells in cells | cells.Count - 1)
			{
				var mask = (Mask)0;
				foreach (var cell in subsetCells)
				{
					mask |= maskTempList[cell];
				}

				if (PopCount((uint)mask) == subsetCells.Count)
				{
					isAlsCanBeReduced = true;
					break;
				}
			}
			if (isAlsCanBeReduced)
			{
				continue;
			}

			// Strong.
			foreach (var digitsPair in links)
			{
				var node1ExtraMap = CandidateMap.Empty;
				foreach (var cell in cells)
				{
					node1ExtraMap.AddRange(from digit in grid.GetCandidates(cell) select cell * 9 + digit);
				}
				var node2ExtraMap = CandidateMap.Empty;
				foreach (var cell in cells)
				{
					node2ExtraMap.AddRange(from digit in grid.GetCandidates(cell) select cell * 9 + digit);
				}

				var digit1 = TrailingZeroCount(digitsPair);
				var digit2 = digitsPair.GetNextSet(digit1);
				var node1Cells = HousesMap[house] & cells & CandidatesMap[digit1];
				var node2Cells = HousesMap[house] & cells & CandidatesMap[digit2];
				var node1 = new Node(node1Cells * digit1, false, true);
				var node2 = new Node(node2Cells * digit2, true, true);
				context.StrongLinks.AddEntry(node1, node2, true, als);
			}

			// Weak.
			// Please note that weak links may not contain pattern objects,
			// because it will be rendered into view nodes; but they are plain ones,
			// behaved as normal locked candidate nodes.
			foreach (var digit in digitsMask)
			{
				var cells3 = CandidatesMap[digit] & cells;
				var node3 = new Node(cells3 * digit, true, true);
				foreach (var cells3House in cells3.SharedHouses)
				{
					var otherCells = HousesMap[cells3House] & CandidatesMap[digit] & ~cells;
					var weakLimit = linkOption switch
					{
						LinkOption.Intersection => 3,
						LinkOption.House or LinkOption.All => otherCells.Count
					};
					foreach (var cells4 in otherCells | weakLimit)
					{
						if (linkOption == LinkOption.Intersection && !cells4.IsInIntersection)
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

		var alsIndex = context.CurrentAlmostLockedSetIndex;
		var result = new List<ViewNode>();
		foreach (var link in pattern.Links)
		{
			if (link.GroupedLinkPattern is not AlmostLockedSet { Cells: var cells })
			{
				continue;
			}

			var linkMap = link.FirstNode.Map | link.SecondNode.Map;
			var id = (ColorIdentifier)(alsIndex + WellKnownColorIdentifierKind.AlmostLockedSet1);
			foreach (var cell in cells)
			{
				var node1 = new CellViewNode(id, cell);
				view.Add(node1);
				result.Add(node1);
				foreach (var digit in grid.GetCandidates(cell))
				{
					var candidate = cell * 9 + digit;
					if (!linkMap.Contains(candidate))
					{
						var node2 = new CandidateViewNode(id, cell * 9 + digit);
						view.Add(node2);
						result.Add(node2);
					}
				}
			}
			alsIndex = (alsIndex + 1) % 5;
		}

		context.CurrentAlmostLockedSetIndex = alsIndex;
		context.ProducedViewNodes = result.AsReadOnlySpan();
	}

	/// <inheritdoc/>
	public override void GetLoopConclusions(ref ChainingRuleLoopConclusionContext context)
	{
		// An example with 19 eliminations:
		// .2.1...7...5..31..6.+1..7..8+2....59..5.3.1...2+1.93.+2.5..1...6...9..2.......2.4...7:821 448 648 848 449 649 388

		// A valid ALS can be eliminated as a real naked subset.
		ref readonly var grid = ref context.Grid;
		var result = ConclusionSet.Empty;
		foreach (var element in context.Links)
		{
			if (element is
				{
					IsStrong: true,
					FirstNode.Map.Digits: var digitsMask1,
					SecondNode.Map.Digits: var digitsMask2,
					GroupedLinkPattern: AlmostLockedSet(var digitsMask, var alsCells)
				})
			{
				var elimDigitsMask = (Mask)(digitsMask & (Mask)~(Mask)(digitsMask1 | digitsMask2));
				foreach (var digit in elimDigitsMask)
				{
					foreach (var cell in alsCells % CandidatesMap[digit])
					{
						result.Add(new Conclusion(Elimination, cell, digit));
					}
				}
			}
		}
		context.Conclusions = result;
	}
}
