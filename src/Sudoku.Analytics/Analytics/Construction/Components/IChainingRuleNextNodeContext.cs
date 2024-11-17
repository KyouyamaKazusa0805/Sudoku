namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a context type that is used for finding for next nodes from the specified node.
/// </summary>
public interface IChainingRuleNextNodeContext : IContext
{
	/// <summary>
	/// Indicates the collected nodes.
	/// </summary>
	public abstract HashSet<Node> Nodes { get; set; }

	/// <summary>
	/// Indicates the current node.
	/// </summary>
	public abstract Node CurrentNode { get; }

	/// <summary>
	/// Indicates the options used.
	/// </summary>
	public abstract StepGathererOptions Options { get; }
}
