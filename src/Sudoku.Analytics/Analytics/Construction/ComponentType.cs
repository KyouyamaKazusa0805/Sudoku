namespace Sudoku.Analytics.Construction;

/// <summary>
/// Represents a type of component.
/// </summary>
public enum ComponentType
{
	/// <summary>
	/// Indicates the component is excluder.
	/// </summary>
	Excluder,

	/// <summary>
	/// Indicates the component is chain.
	/// </summary>
	Chain,

	/// <summary>
	/// Indicates the component is chain node.
	/// </summary>
	ChainNode,

	/// <summary>
	/// Indicates the component is multiparent chain node.
	/// </summary>
	MultiparentChainNode,

	/// <summary>
	/// Indicates the component is chain link.
	/// </summary>
	ChainLink,

	/// <summary>
	/// Indicates the component is chain link dictionary.
	/// </summary>
	ChainLinkDictionary,

	/// <summary>
	/// Indicates the component is forcing chains.
	/// </summary>
	ForcingChains,

	/// <summary>
	/// Indicates the component is death blossom branch.
	/// </summary>
	DeathBlossomBranch,

	/// <summary>
	/// Indicates the component is multiple forcing chains.
	/// </summary>
	MultipleForcingChains,

	/// <summary>
	/// Indicates the component is binary forcing chains.
	/// </summary>
	BinaryForcingChains,

	/// <summary>
	/// Indicates the component is blossom loop.
	/// </summary>
	BlossomLoop,

	/// <summary>
	/// Indicates the component is whip assignment.
	/// </summary>
	WhipAssignment,

	/// <summary>
	/// Indicates the component is whip node.
	/// </summary>
	WhipNode,

	/// <summary>
	/// Indicates the component is locked member.
	/// </summary>
	LockedMember,

	/// <summary>
	/// Indicates the component is double exocet.
	/// </summary>
	DoubleExocet,

	/// <summary>
	/// Indicates the component is complex senior exocet.
	/// </summary>
	ComplexSeniorExocet
}
