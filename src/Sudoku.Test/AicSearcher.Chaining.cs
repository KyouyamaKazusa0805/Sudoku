namespace Sudoku.Test;

partial class AicSearcher
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
	/// Indicates the lookup table that can get the target <see cref="Node"/> instance
	/// via the corresponding ID value specified as the index.
	/// </summary>
	private Node?[] _id2NodeLookup = null!;

	/// <summary>
	/// Indicates the lookup table that can get the target ID value
	/// via the corresponding <see cref="Node"/> instance.
	/// </summary>
	/// <seealso cref="Node"/>
	private readonly Dictionary<Node, int> _node2IdLookup = new();

	/// <summary>
	/// Indicates all possible found chains, that stores the IDs of the each node.
	/// </summary>
	private readonly List<(int[] ChainIds, bool StartsWithWeakInference)> _foundChains = new();

	/// <summary>
	/// Indicates the global ID value.
	/// </summary>
	private int _globalId;


	/// <summary>
	/// Remove all ID values in the lookup dictionary.
	/// </summary>
	private void RemoveIdsNotAppearingInLookupDictionary(Dictionary<int, HashSet<int>?> inferences)
	{
		foreach (int id in inferences.Keys)
		{
			if (_id2NodeLookup[id] is null)
			{
				inferences.Remove(id);
			}
		}
	}

	/// <summary>
	/// Start to construct the chain, with the weak inference as the beginning node.
	/// </summary>
	private void StartWithWeak()
	{
		var chain = new Bag<int>();
		foreach (var (id, nextIds) in _weakInferences)
		{
			if (_id2NodeLookup[id]!.IsGroupedNode)
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
	/// Start to construct the chain, with the strong inference as the beginning node.
	/// </summary>
	private void StartWithStrong()
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

	/// <summary>
	/// Adds the specified node into the collection, or just get the ID value.
	/// </summary>
	/// <param name="nextNode">The next node.</param>
	/// <param name="list">The list.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AddNode(Node nextNode, [NotNull] ref HashSet<int>? list)
	{
		if (_node2IdLookup.TryGetValue(nextNode, out int nextNodeId))
		{
			(list ??= new()).Add(nextNodeId);
		}
		else
		{
			_id2NodeLookup[_globalId] = nextNode;
			_node2IdLookup.Add(nextNode, _globalId);
			(list ??= new()).Add(_globalId++);
		}
	}

	/// <summary>
	/// To assign the recorded IDs into the dictionary.
	/// </summary>
	/// <param name="list">The list of IDs.</param>
	/// <param name="currentNode">The current node.</param>
	/// <param name="inferences">Indicates what dictionary the hash set is assigned to.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AssignOrUpdateHashSet(
		HashSet<int>? list, Node currentNode, Dictionary<int, HashSet<int>?> inferences)
	{
		if (list is not null)
		{
			if (_node2IdLookup.TryGetValue(currentNode, out int currentNodeId))
			{
				if (list is not null)
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
			}
			else
			{
				inferences.Add(_globalId++, list);
			}
		}
	}
}
