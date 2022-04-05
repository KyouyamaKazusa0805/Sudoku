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
[SeparatedStepSearcher(0, nameof(XEnabled), true, nameof(YEnabled), false)]
[SeparatedStepSearcher(1, nameof(XEnabled), true, nameof(YEnabled), true)]
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
	public bool XEnabled { get; init; }

	/// <inheritdoc/>
	public bool YEnabled { get; init; }

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <see langword="false"/>.
	/// </remarks>
	public bool DepthFirstSearching { get; set; } = false;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>3000</c>.
	/// </remarks>
	public int MaxCapacity { get; set; } = 3000;

	/// <inheritdoc/>
	/// <remarks>
	/// The default value is <c>
	/// <see cref="SearcherNodeTypes.SoleCell"/> | <see cref="SearcherNodeTypes.SoleDigit"/>
	/// | <see cref="SearcherNodeTypes.LockedCandidates"/>
	/// | <see cref="SearcherNodeTypes.LockedSet"/> | <see cref="SearcherNodeTypes.HiddenSet"/>
	/// </c>.
	/// </remarks>
	public SearcherNodeTypes NodeTypes { get; set; } =
		SearcherNodeTypes.SoleDigit | SearcherNodeTypes.SoleCell
			| SearcherNodeTypes.LockedCandidates
			| SearcherNodeTypes.LockedSet | SearcherNodeTypes.HiddenSet;


	/// <inheritdoc/>
	public Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne)
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
			GatherInferences_SoleCandidate(grid);
			GatherInferences_LockedCandidates(grid);
			GatherInferences_AlmostLockedSet(grid);
			//GatherInferences_AlmostHiddenSet(grid);
			//GatherInferences_UniqueRectangle(grid);
			//GatherInferences_BasicFish(grid);

			// Remove IDs if they don't appear in the lookup table.
			RemoveIdsNotAppearingInLookupDictionary(_weakInferences);
			RemoveIdsNotAppearingInLookupDictionary(_strongInferences);

#if false
			// Display the inferences found.
			PrintInferences(_strongInferences);
			PrintInferences(_weakInferences);
#else
			// Construct chains.
			if (DepthFirstSearching)
			{
				Dfs_StartWithWeak();
				Dfs_StartWithStrong();
			}
			else
			{
				Bfs();
			}

			var tempList = new Dictionary<AlternatingInferenceChain, ImmutableArray<Conclusion>>();
			foreach (var (nids, startsWithWeak) in _foundChains)
			{
				var chain = new AlternatingInferenceChain(from nid in nids select _nodeLookup[nid], startsWithWeak);
				if (chain.GetConclusions(grid) is { Length: not 0 } conclusions && !tempList.ContainsKey(chain))
				{
					tempList.Add(chain, conclusions);
				}
			}

			foreach (var (chain, conclusions) in tempList)
			{
				// Adds into the accumulator.
				var step = new AlternatingInferenceChainStep(
					conclusions,
					ImmutableArray.Create(
						View.Empty
							+ IChainStepSearcher.GetViewOnCandidates(chain)
							+ IChainStepSearcher.GetViewOnLinks(chain)
					),
					chain,
					XEnabled,
					YEnabled
				);

				if (onlyFindOne)
				{
					return step;
				}

				accumulator.Add(step);
			}

			return null;
#endif
		}
		finally
		{
			// Clears the memory.
			ArrayPool<Node?>.Shared.Return(_nodeLookup);
		}
	}

	#region Other methods
	/// <summary>
	/// Adds the specified node into the collection, or just get the ID value.
	/// </summary>
	/// <param name="nextNode">The next node.</param>
	/// <param name="list">The list.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AddNode(Node nextNode, [NotNull] ref HashSet<int>? list)
	{
		if (_idLookup.TryGetValue(nextNode, out int nextNodeId))
		{
			(list ??= new()).Add(nextNodeId);
		}
		else
		{
			_nodeLookup[_globalId] = nextNode;
			_idLookup.Add(nextNode, _globalId);
			(list ??= new()).Add(_globalId++);
		}
	}

	/// <summary>
	/// To assign the recorded IDs into the dictionary.
	/// </summary>
	/// <param name="list">The list of IDs.</param>
	/// <param name="node">The current node.</param>
	/// <param name="inferences">Indicates what dictionary the hash set is assigned to.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void UpdateInferenceTable(HashSet<int>? list, Node node, Dictionary<int, HashSet<int>?> inferences)
	{
		if (list is not null)
		{
			if (_idLookup.TryGetValue(node, out int currentNodeId))
			{
				if (inferences.ContainsKey(currentNodeId))
				{
					inferences[currentNodeId]!.AddRange(list);
				}
				else
				{
					inferences.Add(currentNodeId, list);
				}
			}
			else
			{
				inferences.Add(_globalId++, list);
			}
		}
	}

	/// <summary>
	/// Remove all ID values in the lookup dictionary.
	/// </summary>
	private void RemoveIdsNotAppearingInLookupDictionary(Dictionary<int, HashSet<int>?> inferences)
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
	/// Print the inferences.
	/// </summary>
	/// <param name="inferences">The table of inferences.</param>
	/// <param name="outputHandler">
	/// The handler method that used for the invocation on output the result information.
	/// For example, the following code is okay for this argument:
	/// <code>
	/// PrintInferences(
	///     // Suppose we output the strong inference dictionary.
	///     inferences: _strongInferences,
	/// 
	///     // Here we can call 'Console.WriteLine' to output the result string value.
	///     // In addition, you can also write 'static data => Console.WriteLine(data)'.
	///     outputHandler: Console.WriteLine
	/// );
	/// </code>
	/// </param>
	private void PrintInferences(Dictionary<int, HashSet<int>?> inferences, Action<string> outputHandler)
	{
		const string separator = ", ";

		var sb = new StringHandler();
		foreach (var (id, nextIds) in inferences)
		{
			if (_nodeLookup[id] is not { } node)
			{
				continue;
			}

			sb.Append("Node ");
			sb.Append(node.ToSimpleString());
			sb.Append(": ");

			if (nextIds is not null)
			{
				foreach (int nextId in nextIds)
				{
					sb.Append(_nodeLookup[nextId]!.ToSimpleString());
					sb.Append(separator);
				}

				sb.RemoveFromEnd(separator.Length);
			}
			else
			{
				sb.Append("<null>");
			}

			sb.AppendLine();
		}

		outputHandler(sb.ToStringAndClear());
	}

	/// <summary>
	/// To print the whole chain via the ID. The method is only used for calling by the debugger.
	/// </summary>
	/// <param name="chainIds">The IDs.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private string PrintChainData(int[] chainIds) =>
		string.Join(" -> ", from id in chainIds select _nodeLookup[id]!.ToString());
	#endregion

	#region Chaining methods
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
			using var pendingOn = new Bag<int>();
			using var pendingOff = new Bag<int>();
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
			using var pendingOn = new Bag<int>();
			using var pendingOff = new Bag<int>();
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

								// NOTE: HERE WE PASS THE ARGUMENTS OUT OF ORDER ON PURPOSE!
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
	/// Start to construct the chain using the depth-first searching algorithm,
	/// with the weak inference as the beginning node.
	/// </summary>
	private void Dfs_StartWithWeak()
	{
		var chain = new Bag<int>();
		foreach (var (id, nextIds) in _weakInferences)
		{
			if (_nodeLookup[id]!.IsGroupedNode)
			{
				// Optimization: Skip the nodes using more than 1 cell if they start with weak.
				// Because the grouped nodes cannot be used as a start node.
				continue;
			}

			if (nextIds is null)
			{
				continue;
			}

			chain.Add(id);

			foreach (int nextId in nextIds)
			{
				chain.Add(nextId);

				nextStrong(ref chain, nextId);

				chain.Remove();
			}

			chain.Remove();
		}

		chain.Dispose();


		void nextStrong(ref Bag<int> chain, int id)
		{
			if (!_strongInferences.TryGetValue(id, out var nextIds) || nextIds is null)
			{
				return;
			}

			foreach (int nextId in nextIds)
			{
				if (chain.Contains(nextId))
				{
					continue;
				}

				chain.Add(nextId);

				nextWeak(ref chain, nextId);

				chain.Remove();
			}
		}

		void nextWeak(ref Bag<int> chain, int id)
		{
			if (!_weakInferences.TryGetValue(id, out var nextIds) || nextIds is null)
			{
				return;
			}

			foreach (int nextId in nextIds)
			{
				if (chain[0] == nextId)
				{
					// Found.
					int[] finalArray = chain.ImmutelyAdd(nextId).ToArray();

					_foundChains.Add((finalArray, true));

					return;
				}

				if (chain.Contains(nextId))
				{
					continue;
				}

				chain.Add(nextId);

				nextStrong(ref chain, nextId);

				chain.Remove();
			}
		}
	}

	/// <summary>
	/// Start to construct the chain using the depth-first searching algorithm,
	/// with the strong inference as the beginning node.
	/// </summary>
	private void Dfs_StartWithStrong()
	{
		var chain = new Bag<int>();
		foreach (var (id, nextIds) in _strongInferences)
		{
			if (!_weakInferences.ContainsKey(id))
			{
				continue;
			}

			if (nextIds is null)
			{
				continue;
			}

			chain.Add(id);

			foreach (int nextId in nextIds)
			{
				chain.Add(nextId);

				nextWeak(ref chain, nextId);

				chain.Remove();
			}

			chain.Remove();
		}

		chain.Dispose();


		void nextWeak(ref Bag<int> chain, int id)
		{
			if (!_weakInferences.TryGetValue(id, out var nextIds) || nextIds is null)
			{
				return;
			}

			foreach (int nextId in nextIds)
			{
				if (chain.Contains(nextId))
				{
					continue;
				}

				chain.Add(nextId);

				nextStrong(ref chain, nextId);

				chain.Remove();
			}
		}

		void nextStrong(ref Bag<int> chain, int id)
		{
			if (!_strongInferences.TryGetValue(id, out var nextIds) || nextIds is null)
			{
				return;
			}

			foreach (int nextId in nextIds)
			{
				if (chain[0] == nextId)
				{
					// Found.
					int[] finalArray = chain.ImmutelyAdd(nextId).ToArray();

					_foundChains.Add((finalArray, false));

					return;
				}

				if (chain.Contains(nextId))
				{
					continue;
				}

				chain.Add(nextId);

				nextWeak(ref chain, nextId);

				chain.Remove();
			}
		}
	}
	#endregion

	#region Inference-searching methods
	/// <summary>
	/// Gather the strong and weak inferences on sole candidate nodes.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void GatherInferences_SoleCandidate(in Grid grid)
	{
		// Sole candidate -> Sole candidate.
		foreach (int candidate in grid)
		{
			byte cell = (byte)(candidate / 9), digit = (byte)(candidate % 9);
			var node = new SoleCandidateNode((byte)(candidate / 9), (byte)(candidate % 9));
			getStrong(grid);
			getWeak(grid);


			void getStrong(in Grid grid)
			{
				HashSet<int>? list = null;

				// Get bi-location regions.
				if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
				{
					foreach (var label in Regions)
					{
						int region = cell.ToRegionIndex(label);
						var posCells = (RegionMaps[region] & grid.CandidatesMap[digit]) - cell;
						if (posCells is [var posCell])
						{
							var nextNode = new SoleCandidateNode((byte)posCell, digit);
							AddNode(nextNode, ref list);
						}
					}
				}

				// Get bi-value cell.
				if (NodeTypes.Flags(SearcherNodeTypes.SoleCell))
				{
					short candidateMask = grid.GetCandidates(cell);
					if (PopCount((uint)candidateMask) == 2)
					{
						byte theOtherDigit = (byte)Log2((uint)(candidateMask & ~(1 << digit)));
						var nextNode = new SoleCandidateNode(cell, theOtherDigit);
						AddNode(nextNode, ref list);
					}
				}

				UpdateInferenceTable(list, node, _strongInferences);
			}

			void getWeak(in Grid grid)
			{
				HashSet<int>? list = null;

				if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
				{
					foreach (byte anotherCell in (PeerMaps[cell] & grid.CandidatesMap[digit]) - cell)
					{
						var nextNode = new SoleCandidateNode(anotherCell, digit);
						AddNode(nextNode, ref list);
					}
				}

				if (NodeTypes.Flags(SearcherNodeTypes.SoleCell))
				{
					foreach (byte anotherDigit in grid.GetCandidates(cell) & ~(1 << digit))
					{
						var nextNode = new SoleCandidateNode(cell, anotherDigit);
						AddNode(nextNode, ref list);
					}
				}

				UpdateInferenceTable(list, node, _weakInferences);
			}
		}
	}

	/// <summary>
	/// Gather the strong and weak inferences on locked candidates nodes.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void GatherInferences_LockedCandidates(in Grid grid)
	{
		// Sole candidate -> Locked candidates.
		foreach (int candidate in grid)
		{
			byte cell = (byte)(candidate / 9), digit = (byte)(candidate % 9);
			var node = new SoleCandidateNode(cell, digit);
			getStrong(grid);
			getWeak(grid);


			void getStrong(in Grid grid)
			{
				HashSet<int>? list = null;

				if (NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
				{
					foreach (var label in Regions)
					{
						int region = cell.ToRegionIndex(label);
						var otherCells = RegionMaps[region] & grid.CandidatesMap[digit] - cell;
						if (
							otherCells is not
							{
								Count: > 1 and <= 3,
								CoveredLine: not InvalidFirstSet,
								CoveredRegions: var coveredRegions
							}
						)
						{
							// Optimization:
							// 1) If the number of all other cells in the current region
							// is greater than 3, the region doesn't hold a valid strong inference
							// from the current candidate to a locked candidates,
							// because a locked candidates node at most use 3 cells.
							// 2) If all other cells don't lie in a same row or column, those cells
							// can still not form a locked candidates node.
							continue;
						}

						if (TrailingZeroCount(coveredRegions) >= 9)
						{
							// The cells must be in a same block.
							continue;
						}

						var nextNode = new LockedCandidatesNode(digit, otherCells);
						AddNode(nextNode, ref list);
					}
				}

				UpdateInferenceTable(list, node, _strongInferences);
			}

			void getWeak(in Grid grid)
			{
				HashSet<int>? list = null;
				using var possibleList = new ValueList<Cells>(4);
				var triplets = (stackalloc Cells[3]);

				if (NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
				{
					foreach (var label in Regions)
					{
						int region = cell.ToRegionIndex(label);
						var otherCells = RegionMaps[region] & grid.CandidatesMap[digit] - cell;
						if (otherCells.Count <= 1)
						{
							continue;
						}

						// Okay. Now we get a set of cells.
						// Now we should gather all possible covered rows and columns.
						if (label == Region.Block)
						{
							int block = cell.ToRegionIndex(Region.Block);
							foreach (int subRegions in otherCells.RowMask << 9 | otherCells.ColumnMask << 18)
							{
								var intersectionMap = RegionMaps[block] & RegionMaps[subRegions];
								var subOtherCells = grid.CandidatesMap[digit] & intersectionMap - cell;
								if (subOtherCells.Count is not (var count and not (0 or 1)))
								{
									continue;
								}

								possibleList.Clear();
								if (count == 2)
								{
									possibleList.Add(subOtherCells);
								}
								else// if (count == 3)
								{
									var combinations = subOtherCells & 2;
									possibleList.Add(combinations[0]);
									possibleList.Add(combinations[1]);
									possibleList.Add(combinations[2]);
									possibleList.Add(subOtherCells);
								}

								foreach (var cellsCombination in possibleList)
								{
									var nextNode = new LockedCandidatesNode(digit, cellsCombination);
									AddNode(nextNode, ref list);
								}
							}
						}
						else
						{
							triplets.Clear();
							triplets[0] = RegionMaps[region][0..3];
							triplets[1] = RegionMaps[region][3..6];
							triplets[2] = RegionMaps[region][6..9];

							foreach (ref readonly var triplet in triplets)
							{
								var subOtherCells = triplet & otherCells;
								if (subOtherCells.Count is not (var count and not (0 or 1)))
								{
									continue;
								}

								possibleList.Clear();
								if (count == 2)
								{
									possibleList.Add(subOtherCells);
								}
								else// if (count == 3)
								{
									var combinations = subOtherCells & 2;
									possibleList.Add(combinations[0]);
									possibleList.Add(combinations[1]);
									possibleList.Add(combinations[2]);
									possibleList.Add(subOtherCells);
								}

								foreach (var cellsCombination in possibleList)
								{
									var nextNode = new LockedCandidatesNode(digit, cellsCombination);
									AddNode(nextNode, ref list);
								}
							}
						}
					}
				}

				UpdateInferenceTable(list, node, _weakInferences);
			}
		}

		// Locked candidates -> Sole candidate.
		// Locked candidates -> Locked candidates.
		foreach (var (lineMap, blockMap, intersectionMap, _) in IntersectionMaps.Values)
		{
			foreach (ref readonly var cellCombination in stackalloc[]
			{
				(intersectionMap & 2)[0],
				(intersectionMap & 2)[1],
				(intersectionMap & 2)[2],
				(intersectionMap & 3)[0]
			})
			{
				foreach (byte digit in grid.GetDigitsUnion(cellCombination))
				{
					if ((cellCombination & grid.CandidatesMap[digit]) != cellCombination)
					{
						continue;
					}

					var node = new LockedCandidatesNode(digit, cellCombination);
					getStrong(grid, cellCombination);
					getWeak(grid, cellCombination);


					void getStrong(in Grid grid, in Cells cells)
					{
						HashSet<int>? list = null;

						if (NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
						{
							foreach (int region in cells.CoveredRegions)
							{
								var node = (RegionMaps[region] & grid.CandidatesMap[digit] - cells) switch
								{
									// e.g. aaa==a
									[var onlyCell] => new SoleCandidateNode((byte)onlyCell, digit),

									// e.g. aaa==aaa
									{
										CoveredLine: not InvalidFirstSet,
										CoveredRegions: var coveredRegions
									} otherCells => region.ToRegion() switch
									{
										Region.Block => new LockedCandidatesNode(digit, otherCells),
										_ => TrailingZeroCount(coveredRegions) switch
										{
											< 9 => new LockedCandidatesNode(digit, otherCells),
											_ => default(Node?)
										}
									},

									// Other cases that the following cases cannot satisfy.
									_ => null
								};
								if (node is not null)
								{
									AddNode(node, ref list);
								}
							}
						}

						UpdateInferenceTable(list, node, _strongInferences);
					}

					void getWeak(in Grid grid, in Cells cells)
					{
						HashSet<int>? list = null;
						var triplets = (stackalloc Cells[3]);

						if (NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
						{
							foreach (int region in cells.CoveredRegions)
							{
								var otherCells = grid.CandidatesMap[digit] & RegionMaps[region] - cells;
								if (region.ToRegion() == Region.Block)
								{
									foreach (int subRegion in otherCells.RowMask << 9 | otherCells.ColumnMask << 18)
									{
										var intersectionMap = RegionMaps[region] & RegionMaps[subRegion];
										var subOtherCells = otherCells & intersectionMap;
										switch (subOtherCells)
										{
											case [var cell]:
											{
												AddNode(new SoleCandidateNode((byte)cell, digit), ref list);

												break;
											}
											case { Count: var count }:
											{
												for (int i = 0; i < count; i++)
												{
													foreach (var cellsCombination in subOtherCells & i)
													{
														AddNode(
															cellsCombination is [var onlyCell]
																? new SoleCandidateNode((byte)onlyCell, digit)
																: new LockedCandidatesNode(digit, cellsCombination),
															ref list
														);
													}
												}

												break;
											}
										}
									}
								}
								else
								{
									triplets.Clear();
									triplets[0] = RegionMaps[region][0..3];
									triplets[1] = RegionMaps[region][3..6];
									triplets[2] = RegionMaps[region][6..9];

									foreach (ref readonly var triplet in triplets)
									{
										var subOtherCells = triplet & otherCells;
										if (subOtherCells is not { Count: var count and not 0 })
										{
											continue;
										}

										for (int i = 0; i < count; i++)
										{
											foreach (var cellsCombination in subOtherCells & i)
											{
												if (cellsCombination is [var onlyCell])
												{
													if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
													{
														AddNode(
															new SoleCandidateNode((byte)onlyCell, digit),
															ref list);
													}
												}
												else
												{
													AddNode(
														new LockedCandidatesNode(digit, cellsCombination),
														ref list);
												}
											}
										}
									}
								}
							}
						}

						UpdateInferenceTable(list, node, _weakInferences);
					}
				}
			}
		}
	}

	/// <summary>
	/// Gather the strong and weak inferences on almost locked sets nodes.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void GatherInferences_AlmostLockedSet(in Grid grid)
	{
		var alses = IAlmostLockedSetsStepSearcher.Gather(grid);
		foreach (var als in alses)
		{
			short digitsMask = als.DigitsMask;
			var alsMap = als.Map;

			// Sole candidate -> Almost locked sets.
			// Locked candidates -> Almost locked sets.
			// Almost locked sets -> Almost locked sets.
			// Almost locked sets -> Sole candidate.
			// Almost locked sets -> Locked candidates.
			foreach (byte digit in digitsMask)
			{
				var digitCellsUsed = grid.CandidatesMap[digit] & alsMap;
				var cellsNotUsed = alsMap - digitCellsUsed;
				var node = new AlmostLockedSetNode(digit, digitCellsUsed, cellsNotUsed);
				getStrong(grid);
				getWeak(grid);


				void getStrong(in Grid grid)
				{
					HashSet<int>? list = null;
					HashSet<int>? list2 = null;

					if (NodeTypes.Flags(SearcherNodeTypes.LockedSet))
					{
						foreach (int region in digitCellsUsed.CoveredRegions)
						{
							var otherCells = RegionMaps[region] & grid.CandidatesMap[digit] - digitCellsUsed;
							if (otherCells.Count is 0 or > 3)
							{
								continue;
							}

							if (
								region.ToRegion() is var label && (
									label == Region.Block
									&& otherCells.CoveredLine == InvalidFirstSet
									|| label is Region.Row or Region.Column
									&& TrailingZeroCount(otherCells.CoveredRegions) >= 9
								)
							)
							{
								continue;
							}

							if (otherCells is [var onlyCell])
							{
								var nextNode = new SoleCandidateNode((byte)onlyCell, digit);
								AddNode(nextNode, ref list);
								AddNode(node, ref list2);
								UpdateInferenceTable(list2, nextNode, _strongInferences);
							}
							else
							{
								var nextNode = new LockedCandidatesNode(digit, otherCells);
								AddNode(nextNode, ref list);
								AddNode(node, ref list2);
								UpdateInferenceTable(list2, nextNode, _strongInferences);
							}
						}

						foreach (byte theOtherDigit in (short)(digitsMask & ~(1 << digit)))
						{
							var theOtherCells = grid.CandidatesMap[theOtherDigit] & alsMap;
							var nextNode = new AlmostLockedSetNode(theOtherDigit, theOtherCells, alsMap - theOtherCells);
							AddNode(nextNode, ref list);
						}
					}

					UpdateInferenceTable(list, node, _strongInferences);
				}

				void getWeak(in Grid grid)
				{
					HashSet<int>? list = null;
					var triplets = (stackalloc Cells[3]);

					if (NodeTypes.Flags(SearcherNodeTypes.LockedSet))
					{
						foreach (int region in digitCellsUsed.CoveredRegions)
						{
							var otherCells = grid.CandidatesMap[digit] & RegionMaps[region] - digitCellsUsed;
							if (region.ToRegion() == Region.Block)
							{
								foreach (int subRegion in otherCells.RowMask << 9 | otherCells.ColumnMask << 18)
								{
									var intersectionMap = RegionMaps[region] & RegionMaps[subRegion];
									var subOtherCells = otherCells & intersectionMap;
									switch (subOtherCells)
									{
										case [var cell]:
										{
											AddNode(new SoleCandidateNode((byte)cell, digit), ref list);

											break;
										}
										case { Count: var count }:
										{
											for (int i = 0; i < count; i++)
											{
												foreach (var cellsCombination in subOtherCells & i)
												{
													AddNode(
														cellsCombination is [var onlyCell]
															? new SoleCandidateNode((byte)onlyCell, digit)
															: new LockedCandidatesNode(digit, cellsCombination),
														ref list
													);
												}
											}

											break;
										}
									}
								}
							}
							else
							{
								triplets.Clear();
								triplets[0] = RegionMaps[region][0..3];
								triplets[1] = RegionMaps[region][3..6];
								triplets[2] = RegionMaps[region][6..9];

								foreach (ref readonly var triplet in triplets)
								{
									var subOtherCells = triplet & otherCells;
									if (subOtherCells is not { Count: var count and not 0 })
									{
										continue;
									}

									for (int i = 0; i < count; i++)
									{
										foreach (var cellsCombination in subOtherCells & i)
										{
											if (cellsCombination is [var onlyCell])
											{
												if (NodeTypes.Flags(SearcherNodeTypes.SoleDigit))
												{
													var nextNode = new SoleCandidateNode((byte)onlyCell, digit);
													AddNode(nextNode, ref list);
												}
											}
											else
											{
												if (NodeTypes.Flags(SearcherNodeTypes.LockedCandidates))
												{
													var nextNode = new LockedCandidatesNode(digit, cellsCombination);
													AddNode(nextNode, ref list);
												}
											}
										}
									}
								}
							}
						}
					}

					UpdateInferenceTable(list, node, _weakInferences);
				}
			}
		}
	}
	#endregion
}
