namespace Sudoku.Test;

partial class AicSearcher
{
	/// <summary>
	/// Start to construct the chain, with the weak inference as the beginning node.
	/// </summary>
	partial void Dfs_StartWithWeak()
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
	partial void Dfs_StartWithStrong()
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
}
