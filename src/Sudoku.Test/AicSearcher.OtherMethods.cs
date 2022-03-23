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
	/// Print the inferences.
	/// </summary>
	/// <param name="inferences">The table of inferences.</param>
	private void PrintInferences(Dictionary<int, HashSet<int>?> inferences)
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

		_output.WriteLine(sb.ToStringAndClear());
	}

	/// <summary>
	/// To print the whole chain via the ID. The method is only used for calling by the debugger.
	/// </summary>
	/// <param name="chainIds">The IDs.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private string PrintChainData(int[] chainIds) =>
		string.Join(" -> ", from id in chainIds select _nodeLookup[id]!.ToString());
}
