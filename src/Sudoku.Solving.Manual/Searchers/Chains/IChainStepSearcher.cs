namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for chain steps.
/// </summary>
public interface IChainStepSearcher : IStepSearcher
{
	/// <summary>
	/// Creates an array of presentation data of candidates
	/// via the specified instance of type <see cref="AlternatingInferenceChain"/>.
	/// </summary>
	/// <param name="chain">The chain.</param>
	/// <returns>An array of presentation data of candidates.</returns>
	protected static sealed CandidateViewNode[] GetViewOnCandidates(AlternatingInferenceChain chain)
	{
		var realChainNodes = chain.RealChainNodes;
		var result = new List<CandidateViewNode>(realChainNodes.Length);
		for (int i = 0; i < realChainNodes.Length; i++)
		{
			if (realChainNodes[i] is { Cells: var cells, Digit: var digit })
			{
				// TODO: Get grouped node.
				foreach (int cell in cells)
				{
					result.Add(
						new(
							(i & 1) switch { 0 => DisplayColorKind.Auxiliary1, _ => DisplayColorKind.Normal },
							cell * 9 + digit
						)
					);
				}
			}
		}

		return result.ToArray();
	}

	/// <summary>
	/// Creates an array of the presentation data of links
	/// via the specified instance of type <see cref="AlternatingInferenceChain"/>.
	/// </summary>
	/// <param name="chain">The chain.</param>
	/// <returns>An array of presentation data of links.</returns>
	protected static sealed LinkViewNode[] GetViewOnLinks(AlternatingInferenceChain chain)
	{
		if (
			chain.RealChainNodes is not (
				[
					{ Cells: var firstCells, Digit: var firstDigit },
					..,
					{ Cells: var lastCells, Digit: var lastDigit }
				] realChainNodes and { Length: var length }
			)
		)
		{
			throw new InvalidOperationException("Invalid status.");
		}

		var result = new LinkViewNode[length + 1];
		for (int i = 0; i < length - 1; i++)
		{
			if (realChainNodes[i] is { Cells: var aCells, Digit: var aDigit }
				&& realChainNodes[i + 1] is { Cells: var bCells, Digit: var bDigit })
			{
				result[i] = new(DisplayColorKind.Normal, new(aDigit, aCells), new(bDigit, bCells), (Inference)(i & 1));
			}
		}

		result[length] = new(
			DisplayColorKind.Normal,
			new(lastDigit, lastCells),
			new(firstDigit, firstCells),
			Inference.Strong
		);

		return result;
	}

	/// <summary>
	/// Print the inferences.
	/// </summary>
	/// <param name="inferences">The table of inferences.</param>
	/// <param name="nodeLookup">The node lookup table.</param>
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
	protected static sealed void PrintInferences(
		Dictionary<int, HashSet<int>?> inferences, Node?[] nodeLookup, Action<string> outputHandler)
	{
		const string separator = ", ";

		scoped var sb = new StringHandler();
		foreach (var (id, nextIds) in inferences)
		{
			if (nodeLookup[id] is not { } node)
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
					sb.Append(nodeLookup[nextId]!.ToSimpleString());
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
	/// <param name="nodeLookup">The node lookup table.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static sealed string GetRawChainData(int[] chainIds, Node?[] nodeLookup)
		=> string.Join(" -> ", from id in chainIds select nodeLookup[id]!.ToString());
}
