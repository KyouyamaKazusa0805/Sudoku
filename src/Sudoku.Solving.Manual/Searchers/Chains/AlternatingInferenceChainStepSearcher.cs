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
	/// </c>.
	/// </remarks>
	public SearcherNodeTypes NodeTypes { get; set; } =
		SearcherNodeTypes.SoleDigit | SearcherNodeTypes.SoleCell
			| SearcherNodeTypes.LockedCandidates;


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
			GatherInferences_SoleAndLocked(grid);

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

			var tempList = new Dictionary<AlternatingInferenceChain, ConclusionList>();
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
	/// To construct a strong or weak inference from node <paramref name="a"/> to <paramref name="b"/>.
	/// </summary>
	/// <param name="a">The first node to be constructed as a strong inference.</param>
	/// <param name="b">The second node to be constructed as a strong inference.</param>
	/// <param name="inferences">The inferences list you want to add.</param>
	private void ConstructInference(Node a, Node b, Dictionary<int, HashSet<int>?> inferences)
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
	/// To print the whole chain via the ID. The method is only used for calling by the debugger.
	/// </summary>
	/// <param name="chainIds">The IDs.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private string PrintChainData(int[] chainIds)
		=> string.Join(" -> ", from id in chainIds select _nodeLookup[id]!.ToString());
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
	/// Gather the strong and weak inferences on sole candidate nodes and locked candidates nodes.
	/// </summary>
	/// <param name="grid">The grid.</param>
	private void GatherInferences_SoleAndLocked(in Grid grid)
	{
		// Iterate on each region, to get all possible links.
		for (byte house = 9; house < 27; house++)
		{
			for (byte digit = 0; digit < 9; digit++)
			{
				if ((ValueMaps[digit] & HouseMaps[house]) is not [])
				{
					// The current house contains the current digit,
					// which is not allowed in searching for strong and weak inferences.
					continue;
				}

				var emptyCells = HouseMaps[house] & CandMaps[digit];
				short blockMask = emptyCells.BlockMask;
				switch (PopCount((uint)blockMask))
				{
					case 2: // Both strong and weak inferences.
					{
						int h1 = TrailingZeroCount(blockMask);
						int h2 = blockMask.GetNextSet(h1);
						var node1Cells = emptyCells & HouseMaps[h1];
						var node2Cells = emptyCells & HouseMaps[h2];
						var node1 = (Node)(
							node1Cells is [var node1Cell]
								? new SoleCandidateNode((byte)node1Cell, digit)
								: new LockedCandidatesNode(digit, node1Cells)
						);
						var node2 = (Node)(
							node2Cells is [var node2Cell]
								? new SoleCandidateNode((byte)node2Cell, digit)
								: new LockedCandidatesNode(digit, node2Cells)
						);

						switch (node1, node2)
						{
							case (SoleCandidateNode a, SoleCandidateNode b)
							when NodeTypes.Flags(SearcherNodeTypes.SoleDigit):
							{
								ConstructInference(a, b, _strongInferences);
								ConstructInference(b, a, _strongInferences);
								ConstructInference(a, b, _weakInferences);
								ConstructInference(b, a, _weakInferences);

								break;
							}
							case var _ when NodeTypes.Flags(SearcherNodeTypes.LockedCandidates):
							{
								ConstructInference(node1, node2, _strongInferences);
								ConstructInference(node2, node1, _strongInferences);

								// TODO: Separate and enumerate all combinations on a locked candidates node.
								ConstructInference(node1, node2, _weakInferences);
								ConstructInference(node2, node1, _weakInferences);

								break;
							}
						}

						break;
					}
					case 3: // Weak inferences.
					{
						int h1 = TrailingZeroCount(blockMask);
						int h2 = blockMask.GetNextSet(h1);
						int h3 = blockMask.GetNextSet(h2);
						var node1Cells = emptyCells & HouseMaps[h1];
						var node2Cells = emptyCells & HouseMaps[h2];
						var node3Cells = emptyCells & HouseMaps[h3];
						var node1 = (Node)(
							node1Cells is [var node1Cell]
								? new SoleCandidateNode((byte)node1Cell, digit)
								: new LockedCandidatesNode(digit, node1Cells)
						);
						var node2 = (Node)(
							node2Cells is [var node2Cell]
								? new SoleCandidateNode((byte)node2Cell, digit)
								: new LockedCandidatesNode(digit, node2Cells)
						);
						var node3 = (Node)(
							node3Cells is [var node3Cell]
								? new SoleCandidateNode((byte)node3Cell, digit)
								: new LockedCandidatesNode(digit, node3Cells)
						);

						internalAdd(node1, node2);
						internalAdd(node1, node3);
						internalAdd(node2, node3);

						break;

						
						void internalAdd(Node node1, Node node2)
						{
							switch (node1, node2)
							{
								case (SoleCandidateNode a, SoleCandidateNode b)
								when NodeTypes.Flags(SearcherNodeTypes.SoleDigit):
								{
									ConstructInference(a, b, _weakInferences);
									ConstructInference(b, a, _weakInferences);

									break;
								}
								case var _ when NodeTypes.Flags(SearcherNodeTypes.LockedCandidates):
								{
									// TODO: Separate and enumerate all combinations on a locked candidates node.
									ConstructInference(node1, node2, _weakInferences);
									ConstructInference(node2, node1, _weakInferences);

									break;
								}
							}
						}
					}
				}
			}
		}

		for (byte house = 0; house < 9; house++)
		{
			for (byte digit = 0; digit < 9; digit++)
			{
				if ((ValueMaps[digit] & HouseMaps[house]) is not [])
				{
					// The current house contains the current digit,
					// which is not allowed in searching for strong and weak inferences.
					continue;
				}

				var emptyCells = HouseMaps[house] & CandMaps[digit];
				int houseMask = emptyCells.RowMask << 9 | emptyCells.ColumnMask << 18;
				switch (PopCount((uint)houseMask))
				{
					case 2: // Both strong and weak inferences.
					{
						int h1 = TrailingZeroCount(houseMask);
						int h2 = houseMask.GetNextSet(h1);
						var node1Cells = emptyCells & HouseMaps[h1];
						var node2Cells = emptyCells & HouseMaps[h2];
						var node1 = (Node)(
							node1Cells is [var node1Cell]
								? new SoleCandidateNode((byte)node1Cell, digit)
								: new LockedCandidatesNode(digit, node1Cells)
						);
						var node2 = (Node)(
							node2Cells is [var node2Cell]
								? new SoleCandidateNode((byte)node2Cell, digit)
								: new LockedCandidatesNode(digit, node2Cells)
						);

						switch (node1, node2)
						{
							case (SoleCandidateNode a, SoleCandidateNode b)
							when NodeTypes.Flags(SearcherNodeTypes.SoleDigit):
							{
								ConstructInference(a, b, _strongInferences);
								ConstructInference(b, a, _strongInferences);
								ConstructInference(a, b, _weakInferences);
								ConstructInference(b, a, _weakInferences);

								break;
							}
							case var _ when NodeTypes.Flags(SearcherNodeTypes.LockedCandidates):
							{
								ConstructInference(node1, node2, _strongInferences);
								ConstructInference(node2, node1, _strongInferences);

								// TODO: Separate and enumerate all combinations on a locked candidates node.
								ConstructInference(node1, node2, _weakInferences);
								ConstructInference(node2, node1, _weakInferences);

								break;
							}
						}

						break;
					}
					case >= 3: // Weak inferences.
					{
						var houses = houseMask.GetAllSets();
						for (int i = 0, length = houses.Length; i < length - 1; i++)
						{
							for (int j = i + 1; j < length; j++)
							{
								var node1Cells = emptyCells & HouseMaps[houses[i]];
								var node2Cells = emptyCells & HouseMaps[houses[j]];
								var node1 = (Node)(
									node1Cells is [var node1Cell]
										? new SoleCandidateNode((byte)node1Cell, digit)
										: new LockedCandidatesNode(digit, node1Cells)
								);
								var node2 = (Node)(
									node2Cells is [var node2Cell]
										? new SoleCandidateNode((byte)node2Cell, digit)
										: new LockedCandidatesNode(digit, node2Cells)
								);

								switch (node1, node2)
								{
									case (SoleCandidateNode a, SoleCandidateNode b)
									when NodeTypes.Flags(SearcherNodeTypes.SoleDigit):
									{
										ConstructInference(a, b, _weakInferences);
										ConstructInference(b, a, _weakInferences);

										break;
									}
									case var _ when NodeTypes.Flags(SearcherNodeTypes.LockedCandidates):
									{
										// TODO: Separate and enumerate all combinations on a locked candidates node.
										ConstructInference(node1, node2, _weakInferences);
										ConstructInference(node2, node1, _weakInferences);

										break;
									}
								}
							}
						}

						break;
					}
				}
			}
		}

		// Iterate on each cell, to get all strong relations.
		if (NodeTypes.Flags(SearcherNodeTypes.SoleCell))
		{
			foreach (int cell in EmptyMap)
			{
				short mask = grid.GetCandidates(cell);
				if (BivalueMap.Contains(cell))
				{
					// Both strong and weak inferences.
					int d1 = TrailingZeroCount(mask);
					int d2 = mask.GetNextSet(d1);
					var node1 = new SoleCandidateNode((byte)cell, (byte)d1);
					var node2 = new SoleCandidateNode((byte)cell, (byte)d2);

					ConstructInference(node1, node2, _strongInferences);
					ConstructInference(node2, node1, _strongInferences);
					ConstructInference(node1, node2, _weakInferences);
					ConstructInference(node2, node1, _weakInferences);
				}
				else
				{
					// Only weak inferences.
					var digits = mask.GetAllSets();
					for (int i = 0, length = digits.Length; i < length - 1; i++)
					{
						for (int j = i + 1; j < length; j++)
						{
							var node1 = new SoleCandidateNode((byte)cell, (byte)digits[i]);
							var node2 = new SoleCandidateNode((byte)cell, (byte)digits[j]);

							ConstructInference(node1, node2, _weakInferences);
							ConstructInference(node2, node1, _weakInferences);
						}
					}
				}
			}
		}
	}
	#endregion
}
