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
	/// To construct a strong or weak inference from node <paramref name="a"/> to <paramref name="b"/>.
	/// </summary>
	/// <param name="a">The first node to be constructed as a strong inference.</param>
	/// <param name="b">The second node to be constructed as a strong inference.</param>
	/// <param name="inferences">The inferences list you want to add.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

						ConstructInference(node1, node2, _strongInferences);
						ConstructInference(node2, node1, _strongInferences);
						ConstructInference(node1, node2, _weakInferences);
						ConstructInference(node2, node1, _weakInferences);
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

							ConstructInference(node1, node2, _weakInferences);
							ConstructInference(node2, node1, _weakInferences);
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
}
