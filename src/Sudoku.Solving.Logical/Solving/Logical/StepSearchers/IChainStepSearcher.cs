namespace Sudoku.Solving.Logical.Prototypes;

/// <summary>
/// Defines a step searcher that searches for chain steps.
/// </summary>
public interface IChainStepSearcher : IStepSearcher
{
	/// <summary>
	/// Creates an array of presentation data of candidates
	/// via the specified instance of type <see cref="AlternatingInferenceChain"/>
	/// and the specified grid as a candidates reference table.
	/// </summary>
	/// <param name="chain">The chain.</param>
	/// <param name="grid">The grid used.</param>
	/// <returns>An array of presentation data of candidates.</returns>
	protected static sealed CandidateViewNode[] GetViewOnCandidates(AlternatingInferenceChain chain, scoped in Grid grid)
	{
		var realChainNodes = chain.RealChainNodes;
		var result = new List<CandidateViewNode>(realChainNodes.Length);

#if false
		byte alsIndex = 0, urIndex = 0;
#endif
		for (var i = 0; i < realChainNodes.Length; i++)
		{
			if (realChainNodes[i] is { Cells: var cells, Digit: var digit } currentNode)
			{
				// Normal highlight candidates.
				foreach (var cell in cells)
				{
					result.Add(
						new(
							(i & 1) switch { 0 => DisplayColorKind.Auxiliary1, _ => DisplayColorKind.Normal },
							cell * 9 + digit
						)
					);
				}

#if false
				// Special case.
				switch (currentNode)
				{
					case { Type: NodeType.AlmostLockedSets, FullCells: var allCells }:
					{
						short potentialDigits = grid.GetDigitsUnion(allCells);
						short digitsShouldHighlight = (short)(potentialDigits & ~(1 << digit));
						foreach (int tempDigit in digitsShouldHighlight)
						{
							foreach (int tempCell in (short)(grid.CandidatesMap[tempDigit] & allCells))
							{
								result.Add(
									new(
										DisplayColorKind.AlmostLockedSet1 + alsIndex,
										tempCell * 9 + tempDigit
									)
								);
							}
						}

						// Sets the ALS to the next color.
						alsIndex = (byte)((alsIndex + 1) % 5);
						break;
					}
					case { Type: NodeType.AlmostUniqueRectangle, FullCells: var allCells }:
					{
						short digitsShouldHighlight = grid.GetDigitsUnion(allCells);
						foreach (int tempDigit in (short)(digitsShouldHighlight & ~(1 << digit)))
						{
							foreach (int tempCell in (short)(grid.CandidatesMap[tempDigit] & allCells))
							{
								result.Add(
									new(
										DisplayColorKind.Auxiliary1 + urIndex,
										tempCell * 9 + tempDigit
									)
								);
							}
						}

						// Sets the AUR to the next color.
						urIndex = (byte)((urIndex + 1) % 3);
						break;
					}
				}
#endif
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
#pragma warning disable format
		if (chain is not
			{
				IsStrong: var isStrong,
				IsContinuousNiceLoop: var isCnl,
				RealChainNodes:
				[
					{ Cells: var firstCells, Digit: var firstDigit },
					..,
					{ Cells: var lastCells, Digit: var lastDigit }
				] realChainNodes and { Length: var length }
			})
#pragma warning restore format
		{
			throw new InvalidOperationException("Invalid status.");
		}

		var result = new LinkViewNode[isStrong || isCnl ? length + 1 : length];
		for (var i = 0; i < length - 1; i++)
		{
			if (realChainNodes[i] is { Cells: var aCells, Digit: var aDigit }
				&& realChainNodes[i + 1] is { Cells: var bCells, Digit: var bDigit })
			{
				result[i] = new(DisplayColorKind.Normal, new(aDigit, aCells), new(bDigit, bCells), (Inference)(i & 1));
			}
		}

		if (isStrong || isCnl)
		{
			result[length] = new(
				DisplayColorKind.Normal,
				new(lastDigit, lastCells),
				new(firstDigit, firstCells),
				isStrong ? Inference.Strong : Inference.Weak
			);
		}

		return result;
	}

	/// <summary>
	/// Print the inferences.
	/// </summary>
	/// <param name="inferences">The table of inferences.</param>
	/// <param name="nodeLookup">The node lookup table.</param>
	/// <param name="outputHandler">
	/// The handler method that used for the invocation on output the result information.
	/// </param>
	/// <remarks>
	/// For example, the following code is okay for calling this method:
	/// <code><![CDATA[
	/// PrintInferences(_strongInferences, _nodeLookup, Console.WriteLine);
	/// ]]></code>
	/// </remarks>
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
				foreach (var nextId in nextIds)
				{
					sb.Append(nodeLookup[nextId]!.Value.ToSimpleString());
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
