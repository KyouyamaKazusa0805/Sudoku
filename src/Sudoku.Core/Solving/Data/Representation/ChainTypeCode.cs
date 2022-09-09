namespace Sudoku.Solving.Data.Representation;

/// <summary>
/// Indicates a type code for a chain.
/// </summary>
public enum ChainTypeCode : byte
{
	/// <summary>
	/// Indicates the M-Wing.
	/// </summary>
	MWing = 1,

	/// <summary>
	/// Indicates the split wing.
	/// </summary>
	SplitWing,

	/// <summary>
	/// Indicates the local wing.
	/// </summary>
	LocalWing,

	/// <summary>
	/// Indicates the hybrid wing.
	/// </summary>
	HybridWing,

	/// <summary>
	/// Indicates the X-Chain.
	/// </summary>
	XChain,

	/// <summary>
	/// Indicates the X-Cycle (Fishy Cycle).
	/// </summary>
	FishyCycle,

	/// <summary>
	/// Indicates the XY-Chain.
	/// </summary>
	XyChain,

	/// <summary>
	/// Indicates the XY-Cycle.
	/// </summary>
	XyCycle,

	/// <summary>
	/// Indicates the continuous nice loop.
	/// </summary>
	ContinuousNiceLoop,

	/// <summary>
	/// Indicates the XY-X-Chain.
	/// </summary>
	XyXChain,

	/// <summary>
	/// Indicates the discontinuous nice loop.
	/// </summary>
	DiscontinuousNiceLoop,

	/// <summary>
	/// Indicates the alternating inference chain.
	/// </summary>
	Aic,

	/// <summary>
	/// Indicates the cell forcing chains.
	/// </summary>
	CellForcingChains,

	/// <summary>
	/// Indicates the region forcing chains (i.e. house forcing chains).
	/// </summary>
	RegionForcingChains,

	/// <summary>
	/// Indicates the contradiction forcing chains.
	/// </summary>
	ContradictionForcingChains,

	/// <summary>
	/// Indicates the double forcing chains.
	/// </summary>
	DoubleForcingChains,

	/// <summary>
	/// Indicates the dynamic cell forcing chains.
	/// </summary>
	DynamicCellForcingChains,

	/// <summary>
	/// Indicates the dynamic region forcing chains (i.e. dynamic house forcing chains).
	/// </summary>
	DynamicRegionForcingChains,

	/// <summary>
	/// Indicates the dynamic contradiction forcing chains.
	/// </summary>
	DynamicContradictionForcingChains,

	/// <summary>
	/// Indicates the dynamic double forcing chains.
	/// </summary>
	DynamicDoubleForcingChains
}
