namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher that searches for alternating inference chain steps.
/// </summary>
public interface IAlternatingInferenceChainStepSearcher : IChainStepSearcher
{
	/// <summary>
	/// Indicates whether the step searcher will check on strong inferences between same-digit node.
	/// </summary>
	bool XEnabled { get; init; }

	/// <summary>
	/// Indicates whether the step searcher will check on strong inferences between sole candidate nodes
	/// in a same cell.
	/// </summary>
	bool YEnabled { get; init; }

	/// <summary>
	/// Indicates whether the searcher uses DFS algorithm to search for chains.
	/// </summary>
	bool DepthFirstSearching { get; set; }

	/// <summary>
	/// Indicates the maximum capacity used for the allocation on shared memory.
	/// </summary>
	int MaxCapacity { get; set; }

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
	SearcherNodeTypes NodeTypes { get; set; }
}
