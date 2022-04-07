namespace Sudoku.Solving.Manual.Steps;

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
	CellFc,

	/// <summary>
	/// Indicates the region forcing chains.
	/// </summary>
	RegionFc,

	/// <summary>
	/// Indicates the contradiction forcing chains.
	/// </summary>
	ContradictionFc,

	/// <summary>
	/// Indicates the double forcing chains.
	/// </summary>
	DoubleFc,

	/// <summary>
	/// Indicates the dynamic cell forcing chains.
	/// </summary>
	DynamicCellFc,

	/// <summary>
	/// Indicates the dynamic region forcing chains.
	/// </summary>
	DynamicRegionFc,

	/// <summary>
	/// Indicates the dynamic contradiction forcing chains.
	/// </summary>
	DynamicContradictionFc,

	/// <summary>
	/// Indicates the dynamic double forcing chains.
	/// </summary>
	DynamicDoubleFc,
}
