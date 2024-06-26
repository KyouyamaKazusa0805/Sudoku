namespace Sudoku.Analytics.Chaining;

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
		var result = new SortedSet<ChainOrLoop>(CachedChainingComparers.ChainComparer);
		foreach (var cell in EmptyCells)
		{
			foreach (var digit in (Mask)(grid.GetCandidates(cell) & (Mask)~(1 << Solution.GetDigit(cell))))
			{
				var node = new Node(cell, digit, true, false);
				if (node.FindChains(in grid, onlyFindOne, result) is { } chain1)
				{
					return (ChainOrLoop[])[chain1];
				}
				if ((~node).FindChains(in grid, onlyFindOne, result) is { } chain2)
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
		var result = new SortedSet<MultipleForcingChains>(CachedChainingComparers.MultipleForcingChainsComparer);
		foreach (var cell in EmptyCells & ~BivalueCells)
		{
			var nodesSupposedOn_GroupedByDigit = new Dictionary<Candidate, HashSet<Node>>();
			var nodesSupposedOff_GroupedByDigit = new Dictionary<Candidate, HashSet<Node>>();
			var nodesSupposedOn_InCell = default(HashSet<Node>);
			var nodesSupposedOff_InCell = default(HashSet<Node>);
			var digitsMask = grid.GetCandidates(cell);
			foreach (var digit in digitsMask)
			{
				var currentNode = new Node(cell, digit, true, false);
				var (nodesSupposedOn, nodesSupposedOff) = currentNode.FindForcingChains();

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

					var nodesSupposedOn_GroupedByHouse = new Dictionary<Candidate, HashSet<Node>>();
					var nodesSupposedOff_GroupedByHouse = new Dictionary<Candidate, HashSet<Node>>();
					var nodesSupposedOn_InHouse = new HashSet<Node>(CachedChainingComparers.NodeMapComparer);
					var nodesSupposedOff_InHouse = new HashSet<Node>(CachedChainingComparers.NodeMapComparer);
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
							var (otherNodesSupposedOn_InHouse, otherNodesSupposedOff_InHouse) = other.FindForcingChains();
							nodesSupposedOn_GroupedByHouse.Add(otherCandidate, otherNodesSupposedOn_InHouse);
							nodesSupposedOff_GroupedByHouse.Add(otherCandidate, otherNodesSupposedOff_InHouse);
							nodesSupposedOn_InHouse.IntersectWith(otherNodesSupposedOn_InHouse);
							nodesSupposedOff_InHouse.IntersectWith(otherNodesSupposedOff_InHouse);
						}
					}

					////////////////////////////////////////
					// Collect with region forcing chains //
					////////////////////////////////////////
#pragma warning disable CS9080
					unsafe
					{
						var _a = rfcOn(in grid, digit, in cellsInHouse, nodesSupposedOn_GroupedByHouse, nodesSupposedOn_InHouse);
						if (!_a.IsEmpty)
						{
							return _a;
						}

						var _b = rfcOff(in grid, digit, in cellsInHouse, nodesSupposedOff_GroupedByHouse, nodesSupposedOff_InHouse);
						if (!_b.IsEmpty)
						{
							return _b;
						}
					}
#pragma warning restore CS9080
				}

				nodesSupposedOn_GroupedByDigit.Add(cell * 9 + digit, nodesSupposedOn);
				nodesSupposedOff_GroupedByDigit.Add(cell * 9 + digit, nodesSupposedOff);
				if (nodesSupposedOn_InCell is null)
				{
					nodesSupposedOn_InCell = new(CachedChainingComparers.NodeMapComparer);
					nodesSupposedOff_InCell = new(CachedChainingComparers.NodeMapComparer);
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
			var _c = cfcOn(in grid, cell, nodesSupposedOn_GroupedByDigit, nodesSupposedOn_InCell, digitsMask);
			if (!_c.IsEmpty)
			{
				return _c;
			}

			var _d = cfcOff(in grid, cell, nodesSupposedOff_GroupedByDigit, nodesSupposedOff_InCell, digitsMask);
			if (!_d.IsEmpty)
			{
				return _d;
			}
		}
		return result.ToArray();


		ReadOnlySpan<MultipleForcingChains> cfcOn(
			ref readonly Grid grid,
			Cell cell,
			Dictionary<Candidate, HashSet<Node>> onNodes,
			HashSet<Node>? resultOnNodes,
			Mask digitsMask
		)
		{
			foreach (var node in resultOnNodes ?? [])
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
					var branchNode = onNodes[cell * 9 + d].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					cfc.Add(cell * 9 + d, node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode));
				}
				if (onlyFindOne)
				{
					return (MultipleForcingChains[])[cfc];
				}
				result.Add(cfc);
			}
			return [];
		}

		ReadOnlySpan<MultipleForcingChains> cfcOff(
			ref readonly Grid grid,
			Cell cell,
			Dictionary<Candidate, HashSet<Node>> offNodes,
			HashSet<Node>? resultOffNodes,
			Mask digitsMask
		)
		{
			foreach (var node in resultOffNodes ?? [])
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
					var branchNode = offNodes[cell * 9 + d].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					cfc.Add(cell * 9 + d, node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode));
				}
				if (onlyFindOne)
				{
					return (MultipleForcingChains[])[cfc];
				}
				result.Add(cfc);
			}
			return [];
		}

		ReadOnlySpan<MultipleForcingChains> rfcOn(
			ref readonly Grid grid,
			Digit digit,
			ref readonly CellMap cellsInHouse,
			Dictionary<Candidate, HashSet<Node>> onNodes,
			HashSet<Node> houseOnNodes
		)
		{
			foreach (var node in houseOnNodes)
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

				var rfc = new MultipleForcingChains(conclusion);
				foreach (var c in cellsInHouse)
				{
					var branchNode = onNodes[c * 9 + digit].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					rfc.Add(
						c * 9 + digit,
						node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode)
					);
				}
				if (onlyFindOne)
				{
					return (MultipleForcingChains[])[rfc];
				}
				result.Add(rfc);
			}
			return [];
		}

		ReadOnlySpan<MultipleForcingChains> rfcOff(
			ref readonly Grid grid,
			Digit digit,
			ref readonly CellMap cellsInHouse,
			Dictionary<Candidate, HashSet<Node>> offNodes,
			HashSet<Node> houseOffNodes
		)
		{
			foreach (var node in houseOffNodes)
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

				var rfc = new MultipleForcingChains(conclusion);
				foreach (var c in cellsInHouse)
				{
					var branchNode = offNodes[c * 9 + digit].First(n => n.Equals(node, NodeComparison.IncludeIsOn));
					rfc.Add(
						c * 9 + digit,
						node.IsOn ? new StrongForcingChain(branchNode) : new WeakForcingChain(branchNode)
					);
				}
				if (onlyFindOne)
				{
					return (MultipleForcingChains[])[rfc];
				}
				result.Add(rfc);
			}
			return [];
		}
	}

	/// <summary>
	/// Collect all blossom loops appeared in a grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="onlyFindOne">Indicates whether the method only find one valid chain.</param>
	/// <returns>All possible blossom loop instances.</returns>
	public static ReadOnlySpan<BlossomLoop> CollectBlossomLoops(ref readonly Grid grid, bool onlyFindOne)
	{
		var result = new List<BlossomLoop>();

		// Collect for all possible forcing chains that can connect with start and end candidates, with both nodes "on" state.
		var (startCandidates, endCandidates) = (CandidateMap.Empty, new Dictionary<Candidate, CandidateMap>());
		var routeDictionary = new Dictionary<BlossomLoopEntry, HashSet<Node>>();
		foreach (var cell in EmptyCells)
		{
			foreach (var digit in grid.GetCandidates(cell))
			{
				var startCandidate = cell * 9 + digit;
				foreach (var node in new Node(startCandidate, true, false).FindForcingChains().OnNodes)
				{
					if (node is (false, [var endCandidate]))
					{
						var entry = new BlossomLoopEntry(startCandidate, endCandidate);
						if (!routeDictionary.TryAdd(entry, [node]))
						{
							routeDictionary[entry].Add(node);
						}

						startCandidates.Add(startCandidate);
						if (!endCandidates.TryAdd(startCandidate, [endCandidate]))
						{
							endCandidates.GetValueRef(in startCandidate).Add(endCandidate);
						}
					}
				}
			}
		}

		// Iterate on each combination (cell or house) to get branches.
		foreach (var startCandidate in startCandidates)
		{
			var (cell, digit) = (startCandidate / 9, startCandidate % 9);
			var digitsMask = grid.GetCandidates(cell);
			if (startCandidates.GetDigitsFor(cell) == digitsMask && TrailingZeroCount(digitsMask) == digit)
			{
				match(startCandidate, -1, in grid, endCandidates);
			}

			foreach (var house in cell.ToHouseIndices())
			{
				var possibleCells = HousesMap[house] & CandidatesMap[digit];
				if ((HousesMap[house] & startCandidates / digit) == possibleCells && possibleCells[0] == cell)
				{
					match(startCandidate, house, in grid, endCandidates);
				}
			}
		}
		return result.AsReadOnlySpan();


		void match(
			Candidate startCandidate,
			House house,
			ref readonly Grid grid,
			Dictionary<Candidate, CandidateMap> endCandidates
		)
		{
			var (cell, digit) = (startCandidate / 9, startCandidate % 9);
			var (startCandidates, startsWithCell) = (CandidateMap.Empty, house == -1);
			if (startsWithCell)
			{
				foreach (var d in grid.GetCandidates(cell))
				{
					startCandidates.Add(cell * 9 + d);
				}
			}
			else
			{
				foreach (var c in HousesMap[house] & CandidatesMap[digit])
				{
					startCandidates.Add(c * 9 + digit);
				}
			}

			var validStartCandidates = CandidateMap.Empty;
			var branches = new Dictionary<Candidate, CandidateMap>(startCandidates.Count);
			foreach (var c in startCandidates)
			{
				ref readonly var possibleEndCandidatesMap = ref endCandidates.GetValueRef(in c);
				if (!@ref.IsNullRef(in possibleEndCandidatesMap))
				{
					branches.Add(c, possibleEndCandidatesMap);
				}
			}
			if (branches.Count != startCandidates.Count)
			{
				return;
			}

			// For cell.
			foreach (var tempCell in startsWithCell ? EmptyCells - cell : EmptyCells)
			{
				// Check whether all branches collected contain this cell as the end point.
				var allBranchesContainTempCell = true;
				var branchesModified = new Candidate[startCandidates.Count][];
				var i = 0;
				foreach (var key in branches.Keys)
				{
					ref readonly var map = ref branches.GetValueRef(in key);
					if (!map.Contains(tempCell))
					{
						allBranchesContainTempCell = false;
						break;
					}

					var finalMap = CandidateMap.Empty;
					foreach (var d in map.GetDigitsFor(tempCell))
					{
						finalMap.Add(tempCell * 9 + d);
					}
					if (!finalMap)
					{
						allBranchesContainTempCell = false;
						break;
					}

					branchesModified[i++] = [.. finalMap];
				}
				if (!allBranchesContainTempCell)
				{
					continue;
				}

				foreach (var candidates in branchesModified.GetExtractedCombinations())
				{
					var (endCandidatesDigitsMask, pattern) = ((Mask)0, new BlossomLoop());
					foreach (var candidate in candidates)
					{
						endCandidatesDigitsMask |= (Mask)(1 << candidate % 9);
					}
					if (endCandidatesDigitsMask != grid.GetCandidates(tempCell))
					{
						continue;
					}

					for (i = 0; i < candidates.Length; i++)
					{
						var candidate = candidates[i];
						var entry = new BlossomLoopEntry(startCandidates[i], candidate);
						var node = routeDictionary[entry].First(node => node.Map is [var c] && c == candidate);
						pattern.Add(entry, new(node));
					}
					result.Add(pattern);
				}
			}

			// For house.
			var lastHousesMask = startsWithCell ? HouseMaskOperations.AllHousesMask : HouseMaskOperations.AllHousesMask & ~(1 << house);
			foreach (var tempHouse in lastHousesMask)
			{
				for (var tempDigit = 0; tempDigit < 9; tempDigit++)
				{
					if (!(HousesMap[tempHouse] & CandidatesMap[tempDigit]))
					{
						continue;
					}

					var allBranchesInTempHouse = true;
					var branchesModified = new Candidate[startCandidates.Count][];
					var i = 0;
					foreach (var key in branches.Keys)
					{
						ref readonly var map = ref branches.GetValueRef(in key);
						if (!map.Any(c => HousesMap[tempHouse].Contains(c / 9) && c % 9 == tempDigit))
						{
							allBranchesInTempHouse = false;
							break;
						}

						var finalMap = CandidateMap.Empty;
						foreach (var c in map / tempDigit & HousesMap[tempHouse])
						{
							finalMap.Add(c * 9 + tempDigit);
						}
						if (!finalMap)
						{
							allBranchesInTempHouse = false;
							break;
						}

						branchesModified[i++] = [.. finalMap];
					}
					if (!allBranchesInTempHouse)
					{
						continue;
					}

					foreach (var candidates in branchesModified.GetExtractedCombinations())
					{
						var (endCellsMap, pattern) = (CellMap.Empty, new BlossomLoop());
						foreach (var candidate in candidates)
						{
							if (HousesMap[tempHouse].Contains(candidate / 9) && candidate % 9 == tempDigit)
							{
								endCellsMap.Add(candidate / 9);
							}
						}
						if (endCellsMap != (HousesMap[tempHouse] & CandidatesMap[tempDigit]))
						{
							continue;
						}

						for (i = 0; i < candidates.Length; i++)
						{
							var candidate = candidates[i];
							var entry = new BlossomLoopEntry(startCandidates[i], candidate);
							var node = routeDictionary[entry].First(node => node.Map is [var c] && c == candidate);
							pattern.Add(entry, new(node));
						}
						result.Add(pattern);
					}
				}
			}
		}
	}
}
