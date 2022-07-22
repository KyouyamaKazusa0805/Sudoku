namespace Sudoku.Runtime.AnalysisServices;

/// <summary>
/// Defines a node type.
/// </summary>
[EnumSwitchExpressionRoot("GetName", DefaultBehavior = EnumSwitchExpressionDefaultBehavior.ReturnNull, MethodDescription = "Gets the name of the field of the current type.", ThisParameterDescription = "A node type instance.", ReturnValueDescription = "The name of the node type.")]
public enum NodeType
{
	/// <summary>
	/// Indicates the node type is a sole candidate.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Sole candidate")]
	Sole,

	/// <summary>
	/// Indicates the node type is a locked candidates.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Locked candidates")]
	LockedCandidates,

	/// <summary>
	/// Indicates the node type is an almost locked sets.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Almost locked set")]
	AlmostLockedSets,

	/// <summary>
	/// Indicates the node type is an almost hidden set.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Almost hidden set")]
	AlmostHiddenSets,

	/// <summary>
	/// Indicates the node type is an almost unique rectangle.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Almost unique rectangle")]
	AlmostUniqueRectangle,

	/// <summary>
	/// Indicates the node type is an almost avoidable rectangle.
	/// </summary>
	[EnumSwitchExpressionArm("GetName", "Almost avoidable rectangle")]
	AlmostAvoidableRectangle,
}
