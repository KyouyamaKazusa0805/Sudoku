namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for alternating inference chain steps.
/// </summary>
public interface IAlternatingInferenceChainStepSearcher : IChainStepSearcher
{
	/// <summary>
	/// Indicates the maximum capacity used for the allocation on shared memory.
	/// </summary>
	public abstract int MaxCapacity { get; set; }

	/// <summary>
	/// <para>
	/// Indicates the extended nodes to be searched for. Please note that the type of the property
	/// is an enumeration type with bit-fields attribute, which means you can add multiple choices
	/// into the value.
	/// </para>
	/// <para>
	/// You can set the value as a bit-field mask to define your own types to be searched for, where:
	/// <list type="table">
	/// <listheader>
	/// <term>Field name</term>
	/// <description>Description (What kind of nodes can be searched)</description>
	/// </listheader>
	/// <item>
	/// <term><see cref="SearcherNodeTypes.SoleDigit"/></term>
	/// <description>
	/// The strong and weak inferences between 2 sole candidate nodes of a same digit
	/// (i.e. X-Chain).
	/// </description>
	/// </item>
	/// <item>
	/// <term><see cref="SearcherNodeTypes.SoleCell"/></term>
	/// <description>
	/// The strong and weak inferences between 2 sole candidate nodes of a same cell
	/// (i.e. Y-Chain).
	/// </description>
	/// </item>
	/// <item>
	/// <term><see cref="SearcherNodeTypes.LockedCandidates"/></term>
	/// <description>
	/// The strong and weak inferences between 2 nodes, where at least one node is a locked candidates node.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see cref="SearcherNodeTypes.LockedSet"/></term>
	/// <description>
	/// The strong inferences between 2 nodes, where at least one node is an almost locked set node.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see cref="SearcherNodeTypes.HiddenSet"/></term>
	/// <description>
	/// The weak inferences between 2 nodes, where at least one node is an almost hidden set node.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see cref="SearcherNodeTypes.UniqueRectangle"/></term>
	/// <description>
	/// The strong and weak inferences between 2 nodes, where at least one node
	/// is an almost unique rectangle node.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see cref="SearcherNodeTypes.Kraken"/></term>
	/// <description>
	/// The strong and weak inferences between 2 nodes, where at least one node
	/// is a kraken fish node.
	/// </description>
	/// </item>
	/// </list>
	/// Other typed inferences are being considered, such as an XYZ-Wing node, etc.
	/// </para>
	/// </summary>
	public abstract SearcherNodeTypes NodeTypes { get; init; }


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
	protected internal static sealed void PrintInferences(
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
	protected internal static sealed string GetRawChainData(int[] chainIds, Node?[] nodeLookup)
		=> string.Join(" -> ", from id in chainIds select nodeLookup[id]!.ToString());
}
