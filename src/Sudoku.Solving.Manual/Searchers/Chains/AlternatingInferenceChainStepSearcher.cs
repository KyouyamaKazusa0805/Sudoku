namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Provides with an <b>Alternating Inference Chain</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Non-grouped chains:
/// <list type="bullet">
/// <item>
/// Irregular Wings:
/// <list type="bullet">
/// <item>W-Wing (Although it can be searched via <see cref="IIregularWingStepSearcher"/>)</item>
/// <item>M-Wing</item>
/// <item>Split Wing</item>
/// <item>Local Wing</item>
/// <item>Hybrid Wing</item>
/// <item>Purple Cow</item>
/// </list>
/// </item>
/// <item>Discontinuous Nice Loop</item>
/// <item>Alternating Inference Chain</item>
/// <item>Continuous Nice Loop</item>
/// </list>
/// </item>
/// <item>
/// Grouped chains:
/// <list type="bullet">
/// <item>Grouped Irregular Wings</item>
/// <item>Grouped Discontinuous Nice Loop</item>
/// <item>Grouped Alternating Inference Chain</item>
/// <item>Grouped Continuous Nice Loop</item>
/// </list>
/// </item>
/// </list>
/// </summary>
[StepSearcher]
[SeparatedStepSearcher(0, nameof(NodeTypes), SearcherNodeTypeLevel.SingleDigit)]
[SeparatedStepSearcher(1, nameof(NodeTypes), SearcherNodeTypeLevel.Normal)]
[SeparatedStepSearcher(2, nameof(NodeTypes), SearcherNodeTypeLevel.LockedCandidates)]
[SeparatedStepSearcher(3, nameof(NodeTypes), SearcherNodeTypeLevel.LockedSets)]
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
	/// Indicates a list of self node conversion table.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The dictionary stores a list of relations that can converts from the node specified as ID
	/// to the target node specified as the other ID value, via the specified node relation.
	/// </para>
	/// <para>
	/// This list is used for checking for advanced nodes' relations. For example, ALS node.
	/// Due to the design of the type <see cref="Node"/>, we can just check what cells and digit being used
	/// to determine whether two nodes are same. This way of the design is used because we are desired
	/// to combine advanced nodes to normal nodes. If the rule of the equality checking not only checks
	/// cells and digit being used, but the kind of the node, the advanced nodes may not be well-combined.
	/// </para>
	/// <para>
	/// When we construct an AIC, we should consider to store the advanced relation rule
	/// (i.e. the reason why they are formed a strong or weak inference) between two adjacent nodes,
	/// in order to get the final conversion from the normal node to the advanced node.
	/// Before forming an real and correct AIC structure, we can only use the normal nodes.
	/// This is why we should store the advanced relation for this: advanced nodes can be gotten
	/// from this field. This table is used for this case.
	/// </para>
	/// </remarks>
	/// <seealso cref="Node"/>
	private readonly Dictionary<int, Dictionary<AdjacentNodesRelation, int>> _advancedSelfNodes = new();

	/// <summary>
	/// Indicates the lookup table that can get the target ID value
	/// via the corresponding <see cref="Node"/> instance.
	/// </summary>
	/// <seealso cref="Node"/>
	private readonly Dictionary<Node, int> _idLookup = new();

	/// <summary>
	/// Indicates all possible found chains, that stores the IDs of the each node.
	/// </summary>
	private readonly List<(int[] Ids, AdjacentNodesRelation[][]? RelationsMap, bool WeakStart)> _foundChains = new();

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

	/// <summary>
	/// Determines whether the current step searcher may contains self node relations.
	/// </summary>
	private bool MayContainSelfNodes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => NodeTypes >= SearcherNodeTypes.LockedSet;
	}


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
	/// </remarks>
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
			_globalId = 0;

			// Gather strong and weak links.
			GatherInferences_Sole(grid);
			GatherInferences_LockedCandidates();
			GatherInferences_LockedSet(grid);

			// Remove IDs if they don't appear in the lookup table.
			TrimLookup(_weakInferences);
			TrimLookup(_strongInferences);

			// Construct chains.
			Bfs();

			var tempList = new Dictionary<AlternatingInferenceChain, ConclusionList>();
			foreach (var (nids, relationsMap, startsWithWeak) in _foundChains)
			{
				var aics = GatherAics(nids, !startsWithWeak, relationsMap);
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
		if (_idLookup.TryGetValue(a, out int aId))
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
	/// Append the current node specified as ID value into the field <see cref="_advancedSelfNodes"/>.
	/// </summary>
	/// <param name="aId">The specified ID value.</param>
	/// <param name="bId">The other ID value.</param>
	/// <param name="relation">
	/// The advanced relation. The value cannot be <see cref="AdjacentNodesRelation.Normal"/>.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AppendAsAdvancedNode(int aId, int bId, AdjacentNodesRelation relation)
	{
		if (_advancedSelfNodes.TryGetValue(aId, out var dic))
		{
			if (!dic.ContainsKey(relation))
			{
				dic.Add(relation, bId);
			}
		}
		else
		{
			_advancedSelfNodes.Add(aId, new() { { relation, bId } });
		}
	}

	/// <summary>
	/// Start to construct the chain using breadth-first searching algorithm.
	/// </summary>
	private void Bfs()
	{
		Unsafe.SkipInit(out int[] onToOff);
		Unsafe.SkipInit(out int[] offToOn);
		Unsafe.SkipInit(out AdjacentNodesRelation[][] onToOffRelations);
		Unsafe.SkipInit(out AdjacentNodesRelation[][] offToOnRelations);
		try
		{
			// Rend the array as the light-weighted linked list, where the indices correspond to the node IDs.
			// For example, if the chain uses IDs 1, 3, 6 and 10, the linked list will be like:
			// [1] -> 3, [3] -> 6, [6] -> 10, [10] -> 1.
			onToOff = ArrayPool<int>.Shared.Rent(MaxCapacity);
			offToOn = ArrayPool<int>.Shared.Rent(MaxCapacity);
			onToOffRelations = ArrayPool<AdjacentNodesRelation[]>.Shared.Rent(MaxCapacity);
			offToOnRelations = ArrayPool<AdjacentNodesRelation[]>.Shared.Rent(MaxCapacity);

			// Iterate on each node to get the chain, using breadth-first searching algorithm.
			for (int id = 0; id < _globalId; id++)
			{
				if (_nodeLookup[id] is { IsGroupedNode: false })
				{
					Array.Fill(onToOff, -1);
					Array.Fill(offToOn, -1);
					Array.Fill(onToOffRelations, null!);
					Array.Fill(offToOnRelations, null!);
					bfsWeakStart(onToOff, offToOn, onToOffRelations, offToOnRelations, id);
				}

				if (_weakInferences.ContainsKey(id))
				{
					Array.Fill(onToOff, -1);
					Array.Fill(offToOn, -1);
					Array.Fill(onToOffRelations, null!);
					Array.Fill(offToOnRelations, null!);
					bfsStrongStart(onToOff, offToOn, onToOffRelations, offToOnRelations, id);
				}
			}
		}
		finally
		{
			// Return the rent memory.
			ArrayPool<int>.Shared.Return(onToOff);
			ArrayPool<int>.Shared.Return(offToOn);
			ArrayPool<AdjacentNodesRelation[]>.Shared.Return(onToOffRelations);
			ArrayPool<AdjacentNodesRelation[]>.Shared.Return(offToOnRelations);
		}


		void bfsWeakStart(
			int[] onToOff,
			int[] offToOn,
			AdjacentNodesRelation[][] onToOffRelations,
			AdjacentNodesRelation[][] offToOnRelations,
			int id)
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
								onToOffRelations[id] = GetAdjacentRelations(currentId, id);

								var (a, b) = getChainIdAndRelations(onToOff, offToOn, onToOffRelations, offToOnRelations, id);
								_foundChains.Add((a, b, true));

								return;
							}

							if (onToOff[nextId] == -1)
							{
								onToOff[nextId] = currentId;
								onToOffRelations[nextId] = GetAdjacentRelations(currentId, nextId);
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
								offToOnRelations[nextId] = GetAdjacentRelations(currentId, nextId);
								pendingOn.Add(nextId);
							}
						}
					}
				}
			}
		}

		void bfsStrongStart(
			int[] onToOff,
			int[] offToOn,
			AdjacentNodesRelation[][] onToOffRelations,
			AdjacentNodesRelation[][] offToOnRelations,
			int id)
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
								offToOnRelations[id] = GetAdjacentRelations(currentId, id);

								var (a, b) = getChainIdAndRelations(offToOn, onToOff, offToOnRelations, onToOffRelations, id);
								_foundChains.Add((a, b, false));

								return;
							}

							if (offToOn[nextId] == -1)
							{
								offToOn[nextId] = currentId;
								offToOnRelations[nextId] = GetAdjacentRelations(currentId, nextId);
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
								onToOffRelations[nextId] = GetAdjacentRelations(currentId, nextId);
								pendingOff.Add(nextId);
							}
						}
					}
				}
			}
		}

		static (int[], AdjacentNodesRelation[][]) getChainIdAndRelations(
			int[] onToOff,
			int[] offToOn,
			AdjacentNodesRelation[][] onToOffRelations,
			AdjacentNodesRelation[][] offToOnRelations,
			int id)
		{
			var resultList = new List<int>(12) { id };
			var resultRelations = new List<AdjacentNodesRelation[]>(12);

			int i = 0, temp = id;
			bool revisit = false;
			while (temp != id || !revisit)
			{
				temp = ((i & 1) == 0 ? onToOff : offToOn)[temp];

				revisit = true;

				resultList.Add(temp);
				resultRelations.Add(((i & 1) == 0 ? onToOffRelations : offToOnRelations)[temp]);

				i++;
			}

			resultRelations.Insert(0, resultRelations.Remove());

			return (resultList.ToArray(), resultRelations.ToArray());
		}
	}

	/// <summary>
	/// Gets all possible advanced relations between <paramref name="aId"/> and <paramref name="bId"/>.
	/// If those two nodes don't hold any advanced relations, this method will return an array
	/// of a single element <see cref="AdjacentNodesRelation.Normal"/>
	/// (i.e. code <c><![CDATA[new[] { AdjacentNodesRelation.Normal }]]></c>).
	/// </summary>
	/// <param name="aId">The first node ID.</param>
	/// <param name="bId">The second node ID.</param>
	/// <returns>All possible adjacent node relations.</returns>
	/// <seealso cref="AdjacentNodesRelation.Normal"/>
	private AdjacentNodesRelation[] GetAdjacentRelations(int aId, int bId)
		=> !_advancedSelfNodes.TryGetValue(aId, out var dic)
			? new[] { AdjacentNodesRelation.Normal }
			: (from kvp in dic where kvp.Value == bId select kvp.Key).ToArray();

	/// <summary>
	/// Gather AICs.
	/// </summary>
	/// <param name="nodeIds">The node IDs.</param>
	/// <param name="isStrong">Indicates whether the AIC starts with strong inference.</param>
	/// <param name="relationsMap">The relation map.</param>
	/// <returns>The final list of AICs.</returns>
	private List<AlternatingInferenceChain>? GatherAics(
		int[] nodeIds,
		bool isStrong,
		AdjacentNodesRelation[][]? relationsMap)
	{
		if (IsNodesRedundant(nodeIds))
		{
			return null;
		}

		var result = new List<AlternatingInferenceChain>();
		if (MayContainSelfNodes)
		{
			if (relationsMap is null)
			{
				relationsMap = new AdjacentNodesRelation[nodeIds.Length][];
				Array.Fill(relationsMap, Array.Empty<AdjacentNodesRelation>());
			}

			if (relationsMap.All(static array => array.Length == 0))
			{
				return new() { new(from id in nodeIds select _nodeLookup[id]!.Value, isStrong) };
			}

			var selfNodeProjections = new Dictionary<int, (int Index, int Id)[]>();
			for (int i = 0; i < nodeIds.Length; i++)
			{
				int nodeId = nodeIds[i];
				if (_advancedSelfNodes.TryGetValue(nodeId, out var dic) && i < nodeIds.Length - 1)
				{
					selfNodeProjections.Add(
						nodeId,
						(
							from kvp in dic
							where Array.IndexOf(relationsMap[i], kvp.Key) != -1
							select (i, kvp.Value)
						).ToArray()
					);
				}
			}

			var valuesToProduceCombinations = new (int Index, int Id)[selfNodeProjections.Count][];
			int tempIndex = 0;
			foreach (var (_, array) in selfNodeProjections)
			{
				valuesToProduceCombinations[tempIndex++] = array;
			}
			var combinations = Combinatorics.GetExtractedCombinations(valuesToProduceCombinations).ToArray();
			var finalListOfIds = new List<int[]>();
			for (int i = 0; i < combinations.Length; i++)
			{
				int[] currentArray = new int[nodeIds.Length];
				Array.Copy(nodeIds, 0, currentArray, 0, nodeIds.Length);

				for (int j = 0; j < combinations[i].Length; j++)
				{
					var (index, convertedId) = combinations[i][j];
					currentArray[index] = convertedId;
				}

				finalListOfIds.Add(currentArray);
			}

			foreach (int[] ids in finalListOfIds)
			{
				var resultNodes = new Node[ids.Length];
				for (int i = 0; i < ids.Length; i++)
				{
					resultNodes[i] = _nodeLookup[ids[i]]!.Value;
				}

				result.Add(new(resultNodes, isStrong));
			}
		}
		else
		{
			result.Add(new(from id in nodeIds select _nodeLookup[id]!.Value, isStrong));
		}

		return result;
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
						var node1 = new Node(NodeType.Sole, digit, (byte)cell1);
						var node2 = new Node(NodeType.Sole, digit, (byte)cell2);

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

							var node1 = new Node(NodeType.Sole, digit, (byte)cell1);
							var node2 = new Node(NodeType.Sole, digit, (byte)cell2);

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
					var node1 = new Node(NodeType.Sole, (byte)d1, (byte)cell);
					var node2 = new Node(NodeType.Sole, (byte)d2, (byte)cell);

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
							var node1 = new Node(NodeType.Sole, (byte)digits[i], (byte)cell);
							var node2 = new Node(NodeType.Sole, (byte)digits[j], (byte)cell);

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
		void checkFirstFourCases(
			scoped in Cells cells,
			byte digit,
			int offset,
			delegate*<in Cells, short> maskSelector)
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
					var node1 = firstHouseCells is [var firstHouseCell]
						? new Node(NodeType.Sole, digit, (byte)firstHouseCell)
						: new Node(NodeType.LockedCandidates, digit, firstHouseCells);
					var node2 = secondHouseCells is [var secondHouseCell]
						? new Node(NodeType.Sole, digit, (byte)secondHouseCell)
						: new Node(NodeType.LockedCandidates, digit, secondHouseCells);

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
					var node1 = firstHouseCells is [var firstHouseCell]
						? new Node(NodeType.Sole, digit, (byte)firstHouseCell)
						: new Node(NodeType.LockedCandidates, digit, firstHouseCells);
					var node2 = secondHouseCells is [var secondHouseCell]
						? new Node(NodeType.Sole, digit, (byte)secondHouseCell)
						: new Node(NodeType.LockedCandidates, digit, secondHouseCells);
					var node3 = thirdHouseCells is [var thirdHouseCell]
						? new Node(NodeType.Sole, digit, (byte)thirdHouseCell)
						: new Node(NodeType.LockedCandidates, digit, thirdHouseCells);

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

					var node1 = new Node(
						targetRowCells.Count == 1 ? NodeType.Sole : NodeType.LockedCandidates,
						digit, targetRowCells
					);
					var node2 = new Node(
						targetColumnCells.Count == 1 ? NodeType.Sole : NodeType.LockedCandidates,
						digit, targetColumnCells
					);

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

					var case1Node1 = new Node(NodeType.LockedCandidates, digit, rowCells1);
					var case1Node2 = new Node(NodeType.LockedCandidates, digit, columnCells1);
					var case2Node1 = new Node(NodeType.LockedCandidates, digit, rowCells2);
					var case2Node2 = new Node(NodeType.LockedCandidates, digit, columnCells2);

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
				var tempNode1 = node1Cells is [var node1Cell]
					? new Node(NodeType.Sole, digit, (byte)node1Cell)
					: new Node(NodeType.LockedCandidates, digit, node1Cells);

				foreach (var node2Cells in node2.Cells | 3)
				{
					var tempNode2 = node2Cells is [var node2Cell]
						? new Node(NodeType.Sole, digit, (byte)node2Cell)
						: new Node(NodeType.LockedCandidates, digit, node2Cells);

					if (tempNode1.Type == NodeType.Sole && tempNode2.Type == NodeType.Sole)
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

	/// <summary>
	/// Gather the strong and weak inferences on locked candidates nodes.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void GatherInferences_LockedSet(scoped in Grid grid)
	{
		if (!NodeTypes.Flags(SearcherNodeTypes.LockedSet))
		{
			return;
		}

		int aId, bId;
		foreach (var als in AlmostLockedSet.Gather(grid))
		{
			if (als.IsBivalueCell)
			{
				// This case has been handled.
				continue;
			}

			var map = als.Map;
			foreach (short strongInferenceList in als.StrongLinksMask)
			{
				int digit1 = TrailingZeroCount(strongInferenceList);
				int digit2 = strongInferenceList.GetNextSet(digit1);
				var cells1 = map & CandidatesMap[digit1];
				var cells2 = map & CandidatesMap[digit2];
				var node1 = new Node(NodeType.AlmostLockedSets, (byte)digit1, cells1);
				var node2 = new Node(NodeType.AlmostLockedSets, (byte)digit2, cells2);

				(aId, bId) = AppendInference(node1, node2, _strongInferences);
				if (cells1.InOneHouse)
				{
					AppendAsAdvancedNode(aId, bId, AdjacentNodesRelation.AlmostLockedSet);
				}
				(aId, bId) = AppendInference(node2, node1, _strongInferences);
				if (cells2.InOneHouse)
				{
					AppendAsAdvancedNode(aId, bId, AdjacentNodesRelation.AlmostLockedSet);
				}
			}
		}
	}


	/// <summary>
	/// Checks whether the node list is redundant, which means the list contains duplicate link node IDs
	/// in the non- endpoint nodes.
	/// </summary>
	/// <param name="ids">The list of node IDs.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	private static bool IsNodesRedundant(int[] ids)
	{
		var dic = new Dictionary<int, int>();
		foreach (int id in ids)
		{
			if (!dic.ContainsKey(id))
			{
				dic.Add(id, 1);
			}
			else
			{
				dic[id]++;
			}
		}

		return dic.Values.Count(static value => value >= 2) > 1;
	}
}
