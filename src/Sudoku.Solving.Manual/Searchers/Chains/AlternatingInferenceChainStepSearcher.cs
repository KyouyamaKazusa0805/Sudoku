namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Alternating Inference Chain</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Short chains:
/// <list type="bullet">
/// <item>Irregular Wings</item>
/// </list>
/// </item>
/// <item>
/// Normal chains:
/// <list type="bullet">
/// <item>Discontinuous Nice Loop</item>
/// <item>Alternating Inference Chain</item>
/// <!--<item>Continuous Nice Loop</item>-->
/// </list>
/// </item>
/// <item>
/// Grouped chains (which means the nodes are not limited in sole candidates):
/// <list type="bullet">
/// <item>Grouped Discontinuous Nice Loop</item>
/// <item>Grouped Alternating Inference Chain</item>
/// <!--<item>Grouped Continuous Nice Loop</item>-->
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher]
[SeparatedStepSearcher(0, nameof(NodeTypes), SearcherNodeTypes.SoleDigit)]
[SeparatedStepSearcher(1, nameof(NodeTypes), SearcherNodeTypes.SoleDigit | SearcherNodeTypes.SoleCell)]
[SeparatedStepSearcher(2, nameof(NodeTypes), SearcherNodeTypes.SoleDigit | SearcherNodeTypes.SoleCell | SearcherNodeTypes.LockedCandidates)]
public sealed partial class AlternatingInferenceChainStepSearcher : IAlternatingInferenceChainStepSearcher
{
	/// <summary>
	/// Indicates the field that stores the temporary strong inferences during the searching.
	/// </summary>
	/// <remarks>
	/// The value uses a <see cref="Dictionary{TKey, TValue}"/> to store the table of strong inferences, where:
	/// <list type="table">
	/// <listheader>
	/// <term>Item</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term>Key</term>
	/// <description>The ID of a node.</description>
	/// </item>
	/// <item>
	/// <term>Value</term>
	/// <description>
	/// All possible IDs that corresponds to their own node respectively,
	/// one of which can form a strong inference with the <b>Key</b> node.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso cref="Dictionary{TKey, TValue}"/>
	private readonly Dictionary<int, HashSet<int>?> _strongInferences = new();

	/// <summary>
	/// Indicates the field that stores the temporary weak inferences during the searching.
	/// </summary>
	/// <remarks>
	/// The value uses a <see cref="Dictionary{TKey, TValue}"/> to store the table of weak inferences, where:
	/// <list type="table">
	/// <listheader>
	/// <term>Item</term>
	/// <description>Meaning</description>
	/// </listheader>
	/// <item>
	/// <term>Key</term>
	/// <description>The ID of a node.</description>
	/// </item>
	/// <item>
	/// <term>Value</term>
	/// <description>
	/// All possible IDs that corresponds to their own node respectively,
	/// one of which can form a weak inference with the <b>Key</b> node.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso cref="Dictionary{TKey, TValue}"/>
	private readonly Dictionary<int, HashSet<int>?> _weakInferences = new();

	/// <summary>
	/// Indicates the lookup table that can get the target ID value
	/// via the corresponding <see cref="Node"/> instance.
	/// </summary>
	/// <seealso cref="Node"/>
	private readonly Dictionary<Node, int> _idLookup = new();

	/// <summary>
	/// Indicates all possible found chains, that stores the IDs of the each node.
	/// </summary>
	private readonly List<(int[] ChainIds, bool StartsWithWeakInference)> _foundChains = new();

	/// <summary>
	/// Indicates the global ID value.
	/// </summary>
	private int _globalId;

	/// <summary>
	/// Indicates the lookup table that can get the target <see cref="Node"/> instance
	/// via the corresponding ID value specified as the index.
	/// </summary>
	private Node?[] _nodeLookup = null!;


	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>3000</c>.
	/// </remarks>
	public int MaxCapacity { get; set; } = 3000;

	/// <inheritdoc/>
	public SearcherNodeTypes NodeTypes { get; init; }


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		try
		{
			// Clear all possible lists.
			_strongInferences.Clear();
			_weakInferences.Clear();
			_nodeLookup = ArrayPool<Node?>.Shared.Rent(MaxCapacity);
			_idLookup.Clear();
			_foundChains.Clear();

			// Gather strong and weak links.
			GatherInferences_Sole(grid);
			GatherInferences_LockedCandidates();

			// Remove IDs if they don't appear in the lookup table.
			TrimLookup(_weakInferences);
			TrimLookup(_strongInferences);

			// Construct chains.
			Bfs();

			var tempList = new Dictionary<AlternatingInferenceChain, ConclusionList>();
			foreach (var (nids, startsWithWeak) in _foundChains)
			{
				var chain = new AlternatingInferenceChain(from nid in nids select _nodeLookup[nid], !startsWithWeak);
				if (chain.GetConclusions(grid) is var conclusions and not []
					&& !tempList.ContainsKey(chain)
					&& !chain.IsRedundant)
				{
					tempList.Add(chain, conclusions);
				}
			}

			foreach (var (chain, conclusions) in from kvp in tempList orderby kvp.Key.Count select kvp)
			{
				// Adds into the accumulator.
				var step = new AlternatingInferenceChainStep(
					conclusions,
					ImmutableArray.Create(
						View.Empty
							| IChainStepSearcher.GetViewOnCandidates(chain)
							| IChainStepSearcher.GetViewOnLinks(chain)
					),
					chain,
					NodeTypes.Flags(SearcherNodeTypes.SoleDigit),
					NodeTypes.Flags(SearcherNodeTypes.SoleCell)
				);

				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}

			return null;
		}
		finally
		{
			// Clears the memory.
			ArrayPool<Node?>.Shared.Return(_nodeLookup);
		}
	}

	/// <summary>
	/// Remove all ID values not appearing in the lookup dictionary.
	/// </summary>
	private void TrimLookup(Dictionary<int, HashSet<int>?> inferences)
	{
		foreach (int id in inferences.Keys)
		{
			if (_nodeLookup[id] is null)
			{
				inferences.Remove(id);
			}
		}
	}

	/// <summary>
	/// To append a new strong or weak inference that starts with node <paramref name="a"/> to node <paramref name="b"/>.
	/// </summary>
	/// <param name="a">The first node to be constructed as a strong inference.</param>
	/// <param name="b">The second node to be constructed as a strong inference.</param>
	/// <param name="inferences">The inferences list you want to add.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AppendInference(Node a, Node b, Dictionary<int, HashSet<int>?> inferences)
	{
		int bId;
		if (_idLookup.TryGetValue(a, out int aId))
		{
			if (_idLookup.TryGetValue(b, out bId))
			{
				if (inferences.ContainsKey(aId))
				{
					(inferences[aId] ??= new()).Add(bId);
				}
				else
				{
					inferences.Add(aId, new() { bId });
				}
			}
			else
			{
				bId = ++_globalId;
				_nodeLookup[bId] = b;
				_idLookup.Add(b, bId);

				if (inferences.ContainsKey(aId))
				{
					(inferences[aId] ??= new()).Add(bId);
				}
				else
				{
					inferences.Add(aId, new() { bId });
				}
			}
		}
		else
		{
			aId = ++_globalId;
			_nodeLookup[_globalId] = a;
			_idLookup.Add(a, _globalId);

			if (_idLookup.TryGetValue(b, out bId))
			{
				if (inferences.ContainsKey(aId))
				{
					(inferences[aId] ??= new()).Add(bId);
				}
				else
				{
					inferences.Add(aId, new() { bId });
				}
			}
			else
			{
				bId = ++_globalId;
				_nodeLookup[bId] = b;
				_idLookup.Add(b, bId);

				if (inferences.ContainsKey(aId))
				{
					(inferences[aId] ??= new()).Add(bId);
				}
				else
				{
					inferences.Add(aId, new() { bId });
				}
			}
		}
	}

	/// <summary>
	/// Start to construct the chain using breadth-first searching algorithm.
	/// </summary>
	private void Bfs()
	{
		// Rend the array as the light-weighted linked list,
		// where the indices correspond to the node IDs.
		// For example, if the chain uses IDs 1, 3, 6 and 10, the linked list will be like:
		// [1] -> 3, [3] -> 6, [6] -> 10, [10] -> 1.
		Unsafe.SkipInit(out int[] onToOff);
		Unsafe.SkipInit(out int[] offToOn);
		try
		{
			onToOff = ArrayPool<int>.Shared.Rent(MaxCapacity);
			offToOn = ArrayPool<int>.Shared.Rent(MaxCapacity);

			// Iterate on each node to get the chain, using breadth-first searching algorithm.
			for (int id = 0; id < _globalId; id++)
			{
				if (_nodeLookup[id] is { IsGroupedNode: false })
				{
					Array.Fill(onToOff, -1);
					Array.Fill(offToOn, -1);
					bfsWeakStart(onToOff, offToOn, id);
				}

				if (_weakInferences.ContainsKey(id))
				{
					Array.Fill(onToOff, -1);
					Array.Fill(offToOn, -1);
					bfsStrongStart(onToOff, offToOn, id);
				}
			}
		}
		finally
		{
			// Return the rent memory.
			ArrayPool<int>.Shared.Return(onToOff);
			ArrayPool<int>.Shared.Return(offToOn);
		}


		void bfsWeakStart(int[] onToOff, int[] offToOn, int id)
		{
			using scoped var pendingOn = new Bag<int>();
			using scoped var pendingOff = new Bag<int>();
			pendingOn.Add(id);
			onToOff[id] = id;

			while (pendingOn.Count != 0 || pendingOff.Count != 0)
			{
				while (pendingOn.Count != 0)
				{
					int currentId = pendingOn.Peek();
					pendingOn.Remove();

					if (_weakInferences.TryGetValue(currentId, out var nextIds) && nextIds is not null)
					{
						foreach (int nextId in nextIds)
						{
							if (id == nextId)
							{
								// Found.
								onToOff[id] = currentId;

								_foundChains.Add((getChainIds(onToOff, offToOn, id), true));

								return;
							}

							if (onToOff[nextId] == -1)
							{
								onToOff[nextId] = currentId;
								pendingOff.Add(nextId);
							}
						}
					}
				}

				while (pendingOff.Count != 0)
				{
					int currentId = pendingOff.Peek();
					pendingOff.Remove();

					if (_strongInferences.TryGetValue(currentId, out var nextIds) && nextIds is not null)
					{
						foreach (int nextId in nextIds)
						{
							if (offToOn[nextId] == -1)
							{
								offToOn[nextId] = currentId;
								pendingOn.Add(nextId);
							}
						}
					}
				}
			}
		}

		void bfsStrongStart(int[] onToOff, int[] offToOn, int id)
		{
			using scoped var pendingOn = new Bag<int>();
			using scoped var pendingOff = new Bag<int>();
			pendingOff.Add(id);
			offToOn[id] = id;

			while (pendingOff.Count != 0 || pendingOn.Count != 0)
			{
				while (pendingOff.Count != 0)
				{
					int currentId = pendingOff.Peek();
					pendingOff.Remove();

					if (_strongInferences.TryGetValue(currentId, out var nextIds) && nextIds is not null)
					{
						foreach (int nextId in nextIds)
						{
							if (id == nextId)
							{
								// Found.
								offToOn[id] = currentId;

								_foundChains.Add((getChainIds(offToOn, onToOff, id), false));

								return;
							}

							if (offToOn[nextId] == -1)
							{
								offToOn[nextId] = currentId;
								pendingOn.Add(nextId);
							}
						}
					}
				}

				while (pendingOn.Count != 0)
				{
					int currentId = pendingOn.Peek();
					pendingOn.Remove();

					if (_weakInferences.TryGetValue(currentId, out var nextIds) && nextIds is not null)
					{
						foreach (int nextId in nextIds)
						{
							if (onToOff[nextId] == -1)
							{
								onToOff[nextId] = currentId;
								pendingOff.Add(nextId);
							}
						}
					}
				}
			}
		}

		static int[] getChainIds(int[] onToOff, int[] offToOn, int id)
		{
			var resultList = new List<int>(12) { id };
			for (var (i, temp, revisit) = (0, id, false); temp != id || !revisit; i++)
			{
				temp = ((i & 1) == 0 ? onToOff : offToOn)[temp];

				revisit = true;

				resultList.Add(temp);
			}

			return resultList.ToArray();
		}
	}

	/// <summary>
	/// Gather the strong and weak inferences on sole candidate nodes.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void GatherInferences_Sole(scoped in Grid grid)
	{
		if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
		{
			for (byte digit = 0; digit < 9; digit++)
			{
				for (int house = 0; house < 27; house++)
				{
					var targetDigitMap = CandidatesMap[digit] & HouseMaps[house];
					if (targetDigitMap is [var cell1, var cell2])
					{
						// Both strong and weak inferences.
						var node1 = new SoleCandidateNode((byte)cell1, digit);
						var node2 = new SoleCandidateNode((byte)cell2, digit);

						AppendInference(node1, node2, _strongInferences);
						AppendInference(node2, node1, _strongInferences);
						AppendInference(node1, node2, _weakInferences);
						AppendInference(node2, node1, _weakInferences);
					}
					else
					{
						// Only weak inferences.
						foreach (var cellPair in targetDigitMap & 2)
						{
							cell1 = cellPair[0];
							cell2 = cellPair[1];

							var node1 = new SoleCandidateNode((byte)cell1, digit);
							var node2 = new SoleCandidateNode((byte)cell2, digit);

							AppendInference(node1, node2, _weakInferences);
							AppendInference(node2, node1, _weakInferences);
						}
					}
				}
			}
		}

		if (NodeTypes.Flags(SearcherNodeTypes.SoleCell))
		{
			// Iterate on each cell, to get all strong relations.
			foreach (int cell in EmptyCells)
			{
				short mask = grid.GetCandidates(cell);
				if (BivalueCells.Contains(cell))
				{
					// Both strong and weak inferences.
					int d1 = TrailingZeroCount(mask);
					int d2 = mask.GetNextSet(d1);
					var node1 = new SoleCandidateNode((byte)cell, (byte)d1);
					var node2 = new SoleCandidateNode((byte)cell, (byte)d2);

					AppendInference(node1, node2, _strongInferences);
					AppendInference(node2, node1, _strongInferences);
					AppendInference(node1, node2, _weakInferences);
					AppendInference(node2, node1, _weakInferences);
				}
				else
				{
					// Only weak inferences.
					scoped var digits = mask.GetAllSets();
					for (int i = 0, length = digits.Length; i < length - 1; i++)
					{
						for (int j = i + 1; j < length; j++)
						{
							var node1 = new SoleCandidateNode((byte)cell, (byte)digits[i]);
							var node2 = new SoleCandidateNode((byte)cell, (byte)digits[j]);

							AppendInference(node1, node2, _weakInferences);
							AppendInference(node2, node1, _weakInferences);
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Gather the strong and weak inferences on locked candidates nodes.
	/// </summary>
	/// <remarks>
	/// <para><b>Developer notes</b></para>
	/// <para>
	/// When we implement on this, we should consider 5 cases in total:
	/// <list type="number">
	/// <item><b>One digit lies in 2 blocks in a row.</b></item>
	/// <item><b>One digit lies in 2 blocks in a column.</b></item>
	/// <item><b>One digit lies in 2 rows in a block.</b></item>
	/// <item><b>One digit lies in 2 columns in a block.</b></item>
	/// <item>
	/// <b>One digit lies in 1 row and 1 column in a block.</b>
	/// We should treat it as a special case because the intersection cell may be used
	/// in both locked candidates nodes. We should consider on both cases.
	/// </item>
	/// </list>
	/// </para>
	/// </remarks>
	private unsafe void GatherInferences_LockedCandidates()
	{
		if (!NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
		{
			return;
		}

		for (byte house = 0; house < 27; house++)
		{
			for (byte digit = 0; digit < 9; digit++)
			{
				var cells = CandidatesMap[digit] & HouseMaps[house];
				if (cells.Count < 3)
				{
					// The current house doesn't contain a strong or weak inference related to locked candidates nodes.
					continue;
				}

				// Check for cases.
				if (house < 9)
				{
					// In a same block. Here we should handle the cases 3 and 4.
					checkFirstFourCases(cells, digit, 9, &rowMaskSelector);
					checkFirstFourCases(cells, digit, 18, &columnMaskSelector);

					if (cells.Count is 4 or 5)
					{
						// Special: check for the last case.
						// Here we add a condition 'cells.Count is 4 or 5' because other cases have already been handled.
						checkLastCase(cells, digit, house);
					}
				}
				else
				{
					// In a same line. Here we should handle the cases 1 and 2.
					checkFirstFourCases(cells, digit, 0, &blockMaskSelector);
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void checkFirstFourCases(scoped in Cells cells, byte digit, int offset, delegate*<in Cells, short> maskSelector)
		{
			short houseMask = maskSelector(cells);
			switch (PopCount((uint)houseMask))
			{
				case 2:
				{
					// Both strong and weak inference.
					int firstHouse = TrailingZeroCount(houseMask);
					int secondHouse = houseMask.GetNextSet(firstHouse);
					var firstHouseCells = cells & HouseMaps[firstHouse + offset];
					var secondHouseCells = cells & HouseMaps[secondHouse + offset];
					var node1 = (Node)(
						firstHouseCells is [var firstHouseCell]
							? new SoleCandidateNode((byte)firstHouseCell, digit)
							: new LockedCandidatesNode(digit, firstHouseCells)
					);
					var node2 = (Node)(
						secondHouseCells is [var secondHouseCell]
							? new SoleCandidateNode((byte)secondHouseCell, digit)
							: new LockedCandidatesNode(digit, secondHouseCells)
					);

					AppendInference(node1, node2, _strongInferences);
					AppendInference(node2, node1, _strongInferences);

					internalAppendWeakInferences(node1, node2, digit);

					break;
				}
				case 3 when cells.Count != 3:
				{
					// Only weak inference.
					int firstHouse = TrailingZeroCount(houseMask);
					int secondHouse = houseMask.GetNextSet(firstHouse);
					int thirdHouse = houseMask.GetNextSet(secondHouse);
					var firstHouseCells = cells & HouseMaps[firstHouse + offset];
					var secondHouseCells = cells & HouseMaps[secondHouse + offset];
					var thirdHouseCells = cells & HouseMaps[thirdHouse + offset];
					var node1 = (Node)(
						firstHouseCells is [var firstHouseCell]
							? new SoleCandidateNode((byte)firstHouseCell, digit)
							: new LockedCandidatesNode(digit, firstHouseCells)
					);
					var node2 = (Node)(
						secondHouseCells is [var secondHouseCell]
							? new SoleCandidateNode((byte)secondHouseCell, digit)
							: new LockedCandidatesNode(digit, secondHouseCells)
					);
					var node3 = (Node)(
						thirdHouseCells is [var thirdHouseCell]
							? new SoleCandidateNode((byte)thirdHouseCell, digit)
							: new LockedCandidatesNode(digit, thirdHouseCells)
					);

					internalAppendWeakInferences(node1, node2, digit);
					internalAppendWeakInferences(node1, node3, digit);
					internalAppendWeakInferences(node2, node3, digit);

					break;
				}
			}
		}

		void checkLastCase(scoped in Cells cells, byte digit, byte house)
		{
			// Checks for the last case.

			// Generally, all possible sub-cases need handling are:
			//
			//     .----------------------------------.
			//     |   (1)    |    (2)    |    (3)    |
			//     |  x x x   |   x . x   |   x x x   |
			//     |  . . .   |   . x .   |   . x .   |
			//     |  . x .   |   . x .   |   . x .   |
			//     '----------------------------------'
			//
			// We should handle:
			//     Graph (1) - should only handle one case (r13c2 == r1c13 and r13c2 -- r1c13)
			//     Graph (2) - should only handle one case (r23c2 == r1c13 and r23c2 -- r1c13)
			//     Graph (3) - should handle two cases:
			//         Sub-case 1: r1c123 == r23c2 and r1c123 -- r23c2
			//         Sub-case 2: r1c13 == r123c2 and r1c13 -- r123c2.
			//
			// Luckily, those 3 sub-cases can be pre-checked as empty rectangle.
			// Therefore, we should call 'IEmptyRectangleStepSearcher.IsEmptyRectangle' at first.
			if (!IEmptyRectangleStepSearcher.IsEmptyRectangle(cells, house, out int row, out int column))
			{
				// The current cells don't form a valid empty rectangle (i.e. case 5).
				return;
			}

			int intersectionCell = (HouseMaps[row] & HouseMaps[column])[0];
			switch (cells.Count)
			{
				case 4:
				{
					Cells targetRowCells, targetColumnCells;
					if (cells.Contains(intersectionCell))
					{
						var rowCells1 = HouseMaps[row] & cells;
						var columnCells1 = (HouseMaps[row] & cells) - intersectionCell;
						var rowCells2 = (HouseMaps[row] & cells) - intersectionCell;
						var columnCells2 = HouseMaps[column] & cells;

						(targetRowCells, targetColumnCells) = rowCells1.Count == 2 && columnCells1.Count == 2
							? (rowCells1, columnCells1)
							: (rowCells2, columnCells2);
					}
					else
					{
						(targetRowCells, targetColumnCells) = (HouseMaps[row] & cells, HouseMaps[column] & cells);
					}

					var node1 = new LockedCandidatesNode(digit, targetRowCells);
					var node2 = new LockedCandidatesNode(digit, targetColumnCells);

					AppendInference(node1, node2, _strongInferences);
					AppendInference(node2, node1, _strongInferences);
					AppendInference(node1, node2, _weakInferences);
					AppendInference(node2, node1, _weakInferences);

					break;
				}
				case 5:
				{
					var rowCells1 = HouseMaps[row] & cells;
					var columnCells1 = (HouseMaps[column] & cells) - intersectionCell;
					var rowCells2 = (HouseMaps[row] & cells) - intersectionCell;
					var columnCells2 = HouseMaps[column] & cells;

					var case1Node1 = new LockedCandidatesNode(digit, rowCells1);
					var case1Node2 = new LockedCandidatesNode(digit, columnCells1);
					var case2Node1 = new LockedCandidatesNode(digit, rowCells2);
					var case2Node2 = new LockedCandidatesNode(digit, columnCells2);

					AppendInference(case1Node1, case1Node2, _strongInferences);
					AppendInference(case1Node2, case1Node1, _strongInferences);
					AppendInference(case1Node1, case1Node2, _weakInferences);
					AppendInference(case1Node2, case1Node1, _weakInferences);
					AppendInference(case2Node1, case2Node2, _strongInferences);
					AppendInference(case2Node2, case2Node1, _strongInferences);
					AppendInference(case2Node1, case2Node2, _weakInferences);
					AppendInference(case2Node2, case2Node1, _weakInferences);

					break;
				}
			}
		}

		void internalAppendWeakInferences(Node node1, Node node2, byte digit)
		{
			foreach (var node1Cells in node1.Cells | 3)
			{
				var tempNode1 = (Node)(
					node1Cells is [var node1Cell]
						? new SoleCandidateNode((byte)node1Cell, digit)
						: new LockedCandidatesNode(digit, node1Cells)
				);

				foreach (var node2Cells in node2.Cells | 3)
				{
					var tempNode2 = (Node)(
						node2Cells is [var node2Cell]
							? new SoleCandidateNode((byte)node2Cell, digit)
							: new LockedCandidatesNode(digit, node2Cells)
					);

					if ((tempNode1, tempNode2) is (SoleCandidateNode, SoleCandidateNode))
					{
						// Both two nodes are sole candidate nodes.
						// The case has already been handled by another method.
						continue;
					}

					AppendInference(tempNode1, tempNode2, _weakInferences);
					AppendInference(tempNode2, tempNode1, _weakInferences);
				}
			}
		}

		static short blockMaskSelector(in Cells cells) => cells.BlockMask;

		static short rowMaskSelector(in Cells cells) => cells.RowMask;

		static short columnMaskSelector(in Cells cells) => cells.ColumnMask;
	}
}
