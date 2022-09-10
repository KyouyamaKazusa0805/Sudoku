namespace Sudoku.Solving.Implementations.Searchers;

[StepSearcher]
[SeparatedStepSearcher(0, nameof(NodeTypes), SearcherNodeTypeLevel.SingleDigit)]
[SeparatedStepSearcher(1, nameof(NodeTypes), SearcherNodeTypeLevel.Normal)]
[SeparatedStepSearcher(2, nameof(NodeTypes), SearcherNodeTypeLevel.LockedCandidates)]
[SeparatedStepSearcher(3, nameof(NodeTypes), SearcherNodeTypeLevel.LockedSets)]
[SeparatedStepSearcher(4, nameof(NodeTypes), SearcherNodeTypeLevel.HiddenSets)]
[SeparatedStepSearcher(5, nameof(NodeTypes), SearcherNodeTypeLevel.UniqueRectangles)]
internal sealed partial class AlternatingInferenceChainStepSearcher : IAlternatingInferenceChainStepSearcher
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
	private readonly List<(int[] Ids, bool WeakStart)> _foundChains = new();

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
	[StepSearcherProperty]
	public int MaxCapacity { get; set; } = 3000;

	/// <inheritdoc/>
	public SearcherNodeTypes NodeTypes { get; init; }


	/// <inheritdoc/>
	/// <remarks>
	/// <para><b>Developer notes</b></para>
	/// <para>
	/// This method use a very simple method to search for AICs. We should treat all AICs
	/// as "specialized" discontinuous nice loops. In this way we can get two kinds of chains:
	/// <list type="number">
	/// <item>
	/// Type 1:
	/// <code>
	///   A
	///  / \
	/// B===C
	/// </code>
	/// (Strong link <c>B == C</c>, eliminate A)
	/// </item>
	/// <item>
	/// Type 2:
	/// <code>
	///   A
	/// // \\
	/// B---C
	/// </code>
	/// (Strong link <c>A == B -- C == A</c>, set A)
	/// </item>
	/// </list>
	/// We should search for those two kinds of chains, and then encapsulates to a human-friendly view.
	/// </para>
	/// <para><b>
	/// TODO: We should record the relations for the advanced nodes, in order to create a good look.
	/// </b></para>
	/// </remarks>
	public IStep? GetAll(ICollection<IStep> accumulator, scoped in Grid grid, bool onlyFindOne)
	{
		try
		{
			// Clear all possible lists.
			_strongInferences.Clear();
			_weakInferences.Clear();
			_nodeLookup = ArrayPool<Node?>.Shared.Rent(MaxCapacity);
			_idLookup.Clear();
			_foundChains.Clear();
			_globalId = 0;

			// Gather strong and weak links.
			GatherInferences_Sole(grid);
			GatherInferences_LockedCandidates();
			GatherInferences_LockedSet(grid);
			GatherInferences_HiddenSet(grid);
			GatherInferences_UniqueRectangle(grid);

			// Remove IDs if they don't appear in the lookup table.
			TrimLookup(_weakInferences);
			TrimLookup(_strongInferences);

			// Construct chains.
			Bfs();

			var tempList = new Dictionary<AlternatingInferenceChain, ConclusionList>();
			foreach (var (nids, startsWithWeak) in _foundChains)
			{
				var aics = GatherAics(nids, !startsWithWeak);
				if (aics is null)
				{
					continue;
				}

				foreach (var aic in aics)
				{
					if (aic.GetConclusions(grid) is var conclusions and not [] && !tempList.ContainsKey(aic))
					{
						tempList.Add(aic, conclusions);
					}
				}
			}

			foreach (var (aic, conclusions) in from kvp in tempList orderby kvp.Key.Count select kvp)
			{
				// Adds into the accumulator.
				var step = new AlternatingInferenceChainStep(
					conclusions,
					ImmutableArray.Create(
						View.Empty
							| IChainStepSearcher.GetViewOnCandidates(aic, grid)
							| IChainStepSearcher.GetViewOnLinks(aic)
					),
					aic
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
		foreach (var id in inferences.Keys)
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
	/// <returns>
	/// Returns a pair of IDs that describes for the two nodes
	/// <paramref name="a"/> and <paramref name="b"/> registered.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private (int AId, int BId) AppendInference(
		scoped in Node a,
		scoped in Node b,
		Dictionary<int, HashSet<int>?> inferences)
	{
		int bId;
		if (_idLookup.TryGetValue(a, out var aId))
		{
			if (_idLookup.TryGetValue(b, out bId))
			{
				if (inferences.ContainsKey(aId))
				{
					(inferences[aId] ??= new()).Add(bId);
					return (aId, bId);
				}
				else
				{
					inferences.Add(aId, new() { bId });
					return (aId, bId);
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
					return (aId, bId);
				}
				else
				{
					inferences.Add(aId, new() { bId });
					return (aId, bId);
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
					return (aId, bId);
				}
				else
				{
					inferences.Add(aId, new() { bId });
					return (aId, bId);
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
					return (aId, bId);
				}
				else
				{
					inferences.Add(aId, new() { bId });
					return (aId, bId);
				}
			}
		}
	}

	/// <summary>
	/// Start to construct the chain using breadth-first searching algorithm.
	/// </summary>
	private void Bfs()
	{
		Unsafe.SkipInit(out int[] onToOff);
		Unsafe.SkipInit(out int[] offToOn);
		try
		{
			// Rend the array as the light-weighted linked list, where the indices correspond to the node IDs.
			// For example, if the chain uses IDs 1, 3, 6 and 10, the linked list will be like:
			// [1] -> 3, [3] -> 6, [6] -> 10, [10] -> 1.
			onToOff = ArrayPool<int>.Shared.Rent(MaxCapacity);
			offToOn = ArrayPool<int>.Shared.Rent(MaxCapacity);

			// Iterate on each node to get the chain, using breadth-first searching algorithm.
			for (var id = 0; id < _globalId; id++)
			{
				if (_nodeLookup[id] is { IsGrouped: false })
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
					var currentId = pendingOn.Peek();
					pendingOn.Remove();

					if (_weakInferences.TryGetValue(currentId, out var nextIds) && nextIds is not null)
					{
						foreach (var nextId in nextIds)
						{
							if (id == nextId)
							{
								// Found.
								onToOff[id] = currentId;

								_foundChains.Add((getChainIdAndRelations(onToOff, offToOn, id), true));

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
					var currentId = pendingOff.Peek();
					pendingOff.Remove();

					if (_strongInferences.TryGetValue(currentId, out var nextIds) && nextIds is not null)
					{
						foreach (var nextId in nextIds)
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
					var currentId = pendingOff.Peek();
					pendingOff.Remove();

					if (_strongInferences.TryGetValue(currentId, out var nextIds) && nextIds is not null)
					{
						foreach (var nextId in nextIds)
						{
							if (id == nextId)
							{
								// Found.
								offToOn[id] = currentId;

								_foundChains.Add((getChainIdAndRelations(offToOn, onToOff, id), false));

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
					var currentId = pendingOn.Peek();
					pendingOn.Remove();

					if (_weakInferences.TryGetValue(currentId, out var nextIds) && nextIds is not null)
					{
						foreach (var nextId in nextIds)
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

		static int[] getChainIdAndRelations(int[] onToOff, int[] offToOn, int id)
		{
			var resultList = new List<int>(12) { id };

			int i = 0, temp = id;
			var revisited = false;
			while (temp != id || !revisited)
			{
				temp = ((i & 1) == 0 ? onToOff : offToOn)[temp];

				revisited = true;

				resultList.Add(temp);

				i++;
			}

			return resultList.ToArray();
		}
	}

	/// <summary>
	/// Gather AICs.
	/// </summary>
	/// <param name="nodeIds">The node IDs.</param>
	/// <param name="isStrong">Indicates whether the AIC starts with strong inference.</param>
	/// <returns>The final list of AICs.</returns>
	private List<AlternatingInferenceChain>? GatherAics(int[] nodeIds, bool isStrong)
	{
		if (IAlternatingInferenceChainStepSearcher.IsNodesRedundant(nodeIds))
		{
			return null;
		}

		if (nodeIds.Length <= 4)
		{
			// Bug fix (But bug is not fixed, only filtered by this condition).
			// This bug is a weird one that I can't find out how to reproduce it.
			// The step searcher may find some weird "Normal" AICs that only contains 4 nodes.
			// Some adjacent node pairs of this chain are not "Normal" or even incorrect strong or weak
			// inferences.
			return null;
		}

		return new() { new(from id in nodeIds select _nodeLookup[id]!.Value, isStrong) };
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
				for (var house = 0; house < 27; house++)
				{
					var targetDigitMap = CandidatesMap[digit] & HousesMap[house];
					if (targetDigitMap is [var cell1, var cell2])
					{
						// Both strong and weak inferences.
						var node1 = new Node(digit, (byte)cell1);
						var node2 = new Node(digit, (byte)cell2);

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

							var node1 = new Node(digit, (byte)cell1);
							var node2 = new Node(digit, (byte)cell2);

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
			foreach (var cell in EmptyCells)
			{
				var mask = grid.GetCandidates(cell);
				if (BivalueCells.Contains(cell))
				{
					// Both strong and weak inferences.
					var d1 = TrailingZeroCount(mask);
					var d2 = mask.GetNextSet(d1);
					var node1 = new Node((byte)d1, (byte)cell);
					var node2 = new Node((byte)d2, (byte)cell);

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
						for (var j = i + 1; j < length; j++)
						{
							var node1 = new Node((byte)digits[i], (byte)cell);
							var node2 = new Node((byte)digits[j], (byte)cell);

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
				var cells = CandidatesMap[digit] & HousesMap[house];
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
		void checkFirstFourCases(
			scoped in CellMap cells,
			byte digit,
			int offset,
			delegate*<in CellMap, short> maskSelector)
		{
			var houseMask = maskSelector(cells);
			switch (PopCount((uint)houseMask))
			{
				case 2:
				{
					// Both strong and weak inference.
					var firstHouse = TrailingZeroCount(houseMask);
					var secondHouse = houseMask.GetNextSet(firstHouse);
					var firstHouseCells = cells & HousesMap[firstHouse + offset];
					var secondHouseCells = cells & HousesMap[secondHouse + offset];
					var node1 = new Node(digit, firstHouseCells);
					var node2 = new Node(digit, secondHouseCells);

					AppendInference(node1, node2, _strongInferences);
					AppendInference(node2, node1, _strongInferences);

					internalAppendWeakInferences(node1, node2, digit);

					break;
				}
				case 3 when cells.Count != 3:
				{
					// Only weak inference.
					var firstHouse = TrailingZeroCount(houseMask);
					var secondHouse = houseMask.GetNextSet(firstHouse);
					var thirdHouse = houseMask.GetNextSet(secondHouse);
					var firstHouseCells = cells & HousesMap[firstHouse + offset];
					var secondHouseCells = cells & HousesMap[secondHouse + offset];
					var thirdHouseCells = cells & HousesMap[thirdHouse + offset];
					var node1 = new Node(digit, firstHouseCells);
					var node2 = new Node(digit, secondHouseCells);
					var node3 = new Node(digit, thirdHouseCells);

					internalAppendWeakInferences(node1, node2, digit);
					internalAppendWeakInferences(node1, node3, digit);
					internalAppendWeakInferences(node2, node3, digit);

					break;
				}
			}
		}

		void checkLastCase(scoped in CellMap cells, byte digit, byte house)
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
			if (!IEmptyRectangleStepSearcher.IsEmptyRectangle(cells, house, out var row, out var column))
			{
				// The current cells don't form a valid empty rectangle (i.e. case 5).
				return;
			}

			var intersectionCell = (HousesMap[row] & HousesMap[column])[0];
			switch (cells.Count)
			{
				case 4:
				{
					CellMap targetRowCells, targetColumnCells;
					if (cells.Contains(intersectionCell))
					{
						var rowCells1 = HousesMap[row] & cells;
						var columnCells1 = (HousesMap[row] & cells) - intersectionCell;
						var rowCells2 = (HousesMap[row] & cells) - intersectionCell;
						var columnCells2 = HousesMap[column] & cells;

						(targetRowCells, targetColumnCells) = rowCells1.Count == 2 && columnCells1.Count == 2
							? (rowCells1, columnCells1)
							: (rowCells2, columnCells2);
					}
					else
					{
						(targetRowCells, targetColumnCells) = (HousesMap[row] & cells, HousesMap[column] & cells);
					}

					var node1 = new Node(digit, targetRowCells);
					var node2 = new Node(digit, targetColumnCells);

					AppendInference(node1, node2, _strongInferences);
					AppendInference(node2, node1, _strongInferences);
					AppendInference(node1, node2, _weakInferences);
					AppendInference(node2, node1, _weakInferences);

					break;
				}
				case 5:
				{
					var rowCells1 = HousesMap[row] & cells;
					var columnCells1 = (HousesMap[column] & cells) - intersectionCell;
					var rowCells2 = (HousesMap[row] & cells) - intersectionCell;
					var columnCells2 = HousesMap[column] & cells;

					var case1Node1 = new Node(digit, rowCells1);
					var case1Node2 = new Node(digit, columnCells1);
					var case2Node1 = new Node(digit, rowCells2);
					var case2Node2 = new Node(digit, columnCells2);

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

		void internalAppendWeakInferences(scoped in Node node1, scoped in Node node2, byte digit)
		{
			foreach (var node1Cells in node1.Cells | 3)
			{
				var tempNode1 = new Node(digit, node1Cells);

				foreach (var node2Cells in node2.Cells | 3)
				{
					var tempNode2 = new Node(digit, node2Cells);

					if (!tempNode1.IsGrouped && !tempNode2.IsGrouped)
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

		static short blockMaskSelector(in CellMap cells) => cells.BlockMask;

		static short rowMaskSelector(in CellMap cells) => cells.RowMask;

		static short columnMaskSelector(in CellMap cells) => cells.ColumnMask;
	}

	/// <summary>
	/// Gather strong inferences on almost locked sets nodes.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void GatherInferences_LockedSet(scoped in Grid grid)
	{
		if (!NodeTypes.Flags(SearcherNodeTypes.LockedSet))
		{
			return;
		}

		foreach (var als in IAlmostLockedSetsStepSearcher.Gather(grid))
		{
			if (als.IsBivalueCell)
			{
				// This case has been handled.
				continue;
			}

			var map = als.Map;
			foreach (var strongInferenceList in als.StrongLinks)
			{
				var digit1 = TrailingZeroCount(strongInferenceList);
				var digit2 = strongInferenceList.GetNextSet(digit1);
				var cells1 = map & CandidatesMap[digit1];
				var cells2 = map & CandidatesMap[digit2];
				var node1 = new Node((byte)digit1, cells1);
				var node2 = new Node((byte)digit2, cells2);

				// Strong inferences.
				AppendInference(node1, node2, _strongInferences);
				AppendInference(node2, node1, _strongInferences);

				// Weak inferences if worth.
				if (!cells1.IsInIntersection)
				{
					var coveredHouse = TrailingZeroCount(cells1.CoveredHouses);
					var uncoveredCells = HousesMap[coveredHouse] - cells1;
					foreach (var cells in uncoveredCells | uncoveredCells.Count)
					{
						if (!cells.IsInIntersection)
						{
							// Filters the cases that the target elimination node are not locked candidates.
							// They can also be used in chaining, but most of scenarios they are useless.
							continue;
						}

						var tempNode = new Node((byte)digit1, cells);

						AppendInference(node1, tempNode, _weakInferences);
						AppendInference(tempNode, node1, _weakInferences);
					}
				}
				if (!cells2.IsInIntersection)
				{
					var coveredHouse = TrailingZeroCount(cells2.CoveredHouses);
					var uncoveredCells = HousesMap[coveredHouse] - cells2;
					foreach (var cells in uncoveredCells | uncoveredCells.Count)
					{
						if (!cells.IsInIntersection)
						{
							// Filters the cases that the target elimination node are not locked candidates.
							// They can also be used in chaining, but most of scenarios they are useless.
							continue;
						}

						var tempNode = new Node((byte)digit2, cells);

						AppendInference(node2, tempNode, _weakInferences);
						AppendInference(tempNode, node2, _weakInferences);
					}
				}
			}
		}
	}

	/// <summary>
	/// Gather weak inferences on almost hidden set nodes.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void GatherInferences_HiddenSet(scoped in Grid grid)
	{
		if (!NodeTypes.Flags(SearcherNodeTypes.HiddenSet))
		{
			return;
		}

		foreach (var ahs in IAlmostHiddenSetsStepSearcher.Gather(grid))
		{
			foreach (var (digit1, cells1, digit2, cells2) in ahs.WeakLinks)
			{
				var node1 = new Node((byte)digit1, cells1);
				var node2 = new Node((byte)digit2, cells2);

				AppendInference(node1, node2, _weakInferences);
				AppendInference(node2, node1, _weakInferences);
			}
		}
	}

	/// <summary>
	/// Gather strong or weak inferences on almost unique rectangle nodes.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void GatherInferences_UniqueRectangle(scoped in Grid grid)
	{
		if (!NodeTypes.Flags(SearcherNodeTypes.UniqueRectangle))
		{
			return;
		}

		foreach (var cellsArray in UniqueRectanglePatterns)
		{
			if (!IUniqueRectangleStepSearcher.CheckPreconditions(grid, cellsArray, false))
			{
				continue;
			}

			// Get all candidates that all four cells appeared.
			var mask = grid.GetDigitsUnion(cellsArray);

			// Iterate on each possible digit combination.
			scoped var allDigitsInThem = mask.GetAllSets();
			for (int i = 0, length = allDigitsInThem.Length; i < length - 1; i++)
			{
				var digit1 = allDigitsInThem[i];
				for (var j = i + 1; j < length; j++)
				{
					var digit2 = allDigitsInThem[j];

					// All possible UR patterns should contain at least one cell
					// with both 'd1' and 'd2' containing digits.
					var comparer = (short)(1 << digit1 | 1 << digit2);
					var isNotPossibleUr = true;
					foreach (var cell in cellsArray)
					{
						if (PopCount((uint)(grid.GetCandidates(cell) & comparer)) == 2)
						{
							isNotPossibleUr = false;
							break;
						}
					}
					if (isNotPossibleUr)
					{
						continue;
					}

					var otherDigitsMask = (short)(grid.GetDigitsUnion(cellsArray) & ~comparer);

					// Checks for AUR case 1:
					// https://github.com/SunnieShine/Sudoku/issues/319#issuecomment-1192217764
					checkForAurCase1(grid, cellsArray, otherDigitsMask);

					// Checks for AUR case 2:
					// https://github.com/SunnieShine/Sudoku/issues/319#issuecomment-1192218237
					checkForAurCase2(grid, cellsArray, comparer);
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool isUniversalUr(scoped in Grid grid, int[] cellsArray)
		{
			var m1 = grid.GetCandidates(cellsArray[0]);
			var m2 = grid.GetCandidates(cellsArray[1]);
			var m3 = grid.GetCandidates(cellsArray[2]);
			var m4 = grid.GetCandidates(cellsArray[3]);
			return PopCount((uint)m1) == 3 && m1 == m2 && m2 == m3 && m3 == m4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void checkForAurCase1(scoped in Grid grid, int[] cellsArray, short otherDigitsMask)
		{
			switch (PopCount((uint)otherDigitsMask))
			{
				case 0:
				case 1 when !isUniversalUr(grid, cellsArray):
				case > 2:
				{
					// Invalid case. There are 3 cases to be invalid:
					//
					// Case 1: PopCount((uint)otherDigitsMask) returns 0.
					// The grid is invalid. We can just throw an exception to report about this,
					// but this will be handled before searching valid steps. I mean, this invalid
					// case will be found by ManualSolver instance, and directly exits
					// to search any possible steps.
					//
					// Case 2: PopCount((uint)otherDigitsMask) returns 1.
					// This will cause the structure to be a UR type 2 or 5,
					// except one case that all four cells containing the extra digit.
					// The sketch is:
					//     abc | abc
					//     abc | abc
					//
					// Case 3: PopCount((uint)otherDigitsMask) returns a value greater than 2.
					// We cannot find any possible AUR nodes in this case.
					return;
				}
			}

			// Strong inferences.
			var extraDigit1 = TrailingZeroCount(otherDigitsMask);
			var extraDigit2 = otherDigitsMask.GetNextSet(extraDigit1);
			var cells1 = CandidatesMap[extraDigit1] & (CellMap)cellsArray;
			var cells2 = CandidatesMap[extraDigit2] & (CellMap)cellsArray;
			var node1 = new Node((byte)extraDigit1, cells1, (CellMap)cellsArray - cells1);
			var node2 = new Node((byte)extraDigit2, cells2, (CellMap)cellsArray - cells2);

			AppendInference(node1, node2, _strongInferences);
			AppendInference(node2, node1, _strongInferences);

			// Weak inferences if worth.
			appendWeakInference(cells1, (byte)extraDigit1, node1);
			appendWeakInference(cells2, (byte)extraDigit2, node2);


			void appendWeakInference(scoped in CellMap currentCells, byte extraDigit, scoped in Node node)
			{
				var uncoveredCells = PopCount((uint)currentCells.CoveredHouses) switch
				{
					// No covered houses. e.g.
					// x x
					//   x
					0 => +currentCells & CandidatesMap[extraDigit],

					// One covered house. e.g.
					// x x | x
					1 => HousesMap[TrailingZeroCount(currentCells.CoveredHouses)] - currentCells,

					// Two covered houses. It means it is a locked candidates.
					// This case should have been handled.
					_ => CellMap.Empty
				};
				if (uncoveredCells)
				{
					return;
				}

				foreach (var cells in uncoveredCells | uncoveredCells.Count)
				{
					if (!cells.IsInIntersection)
					{
						continue;
					}

					var tempNode = new Node(extraDigit, cells);

					AppendInference(node, tempNode, _weakInferences);
					AppendInference(tempNode, node, _weakInferences);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void checkForAurCase2(scoped in Grid grid, int[] cellsArray, short digitsMask)
		{
			var cellsCountThatContainsOtherDigits = 0;
			var (extraCell1, extraCell2) = (-1, -1);
			for (var i = 0; i < 4; i++)
			{
				var cell = cellsArray[i];
				if ((grid.GetCandidates(cell) & ~digitsMask) != 0)
				{
					cellsCountThatContainsOtherDigits++;
					if (cellsCountThatContainsOtherDigits == 1)
					{
						extraCell1 = cell;
					}
					else if (cellsCountThatContainsOtherDigits == 2)
					{
						extraCell2 = cell;
					}
					else
					{
						// We only check two cells that contains extra digit.
						return;
					}
				}
			}

			var extraCells = CellMap.Empty + extraCell1 + extraCell2;
			if (PopCount((uint)extraCells.CoveredHouses) != 2)
			{
				// They should be a locked candidates.
				return;
			}

			var digit1 = TrailingZeroCount(digitsMask);
			var digit2 = digitsMask.GetNextSet(digit1);
			var cells1 = CandidatesMap[digit1] & extraCells;
			var cells2 = CandidatesMap[digit2] & extraCells;
			var node1 = new Node((byte)digit1, cells1, (CellMap)cellsArray);
			var node2 = new Node((byte)digit2, cells2, (CellMap)cellsArray);

			AppendInference(node1, node2, _weakInferences);
			AppendInference(node2, node1, _weakInferences);
		}
	}
}
