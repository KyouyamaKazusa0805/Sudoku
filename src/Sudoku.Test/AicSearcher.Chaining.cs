namespace Sudoku.Test;

partial class AicSearcher
{
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
	/// Start to construct the chain, with the weak inference as the beginning node.
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
	/// Start to construct the chain, with the strong inference as the beginning node.
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
