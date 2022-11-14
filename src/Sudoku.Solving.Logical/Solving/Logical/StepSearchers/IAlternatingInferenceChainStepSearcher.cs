namespace Sudoku.Solving.Logical.StepSearchers;

/// <summary>
/// Provides with an <b>Alternating Inference Chain</b> step searcher.
/// The step searcher will include the following techniques:
/// <list type="bullet">
/// <item>
/// Non-grouped chains:
/// <list type="bullet">
/// <item>
/// Irregular Wings:
/// <list type="bullet">
/// <item>W-Wing (Although it can be searched via <see cref="IIrregularWingStepSearcher"/>)</item>
/// <item>M-Wing</item>
/// <item>Split Wing</item>
/// <item>Local Wing</item>
/// <item>Hybrid Wing</item>
/// <item>Purple Cow</item>
/// </list>
/// </item>
/// <item>Discontinuous Nice Loop</item>
/// <item>Alternating Inference Chain</item>
/// <item>Continuous Nice Loop</item>
/// </list>
/// </item>
/// <item>
/// Grouped chains:
/// <list type="bullet">
/// <item>Grouped Irregular Wings</item>
/// <item>Grouped Discontinuous Nice Loop</item>
/// <item>Grouped Alternating Inference Chain</item>
/// <item>Grouped Continuous Nice Loop</item>
/// </list>
/// </item>
/// </list>
/// </summary>
public interface IAlternatingInferenceChainStepSearcher : IChainStepSearcher
{
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
	/// <term><see cref="SearcherNodeTypes.XyzWing"/></term>
	/// <description>
	/// The strong and weak inferences between 2 nodes, where at least one node
	/// is an XYZ-Wing node.
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
	SearcherNodeTypes NodeTypes { get; init; }


	/// <summary>
	/// Checks whether the node list is redundant, which means the list contains duplicate link node IDs
	/// in the non- endpoint nodes.
	/// </summary>
	/// <param name="ids">The list of node IDs.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected static sealed bool IsNodesRedundant(int[] ids) => ids[0] == ids[^1] && ids[1] == ids[^2];
}
