namespace Sudoku.Compatibility.SudokuExplainer;

/// <summary>
/// Represents an enumeration that describes for a technique usage designed by Sudoku Explainer.
/// </summary>
public enum SudokuExplainerTechnique
{
	/// <summary>
	/// Represents single.
	/// </summary>
	Single,

	/// <summary>
	/// Represents hidden single.
	/// </summary>
	HiddenSingle,

	/// <summary>
	/// Represents direct pointing.
	/// </summary>
	DirectPointing,

	/// <summary>
	/// Represents direct hidden pair.
	/// </summary>
	DirectHiddenPair,

	/// <summary>
	/// Represents naked single.
	/// </summary>
	NakedSingle,

	/// <summary>
	/// Represents direct hidden triplet.
	/// </summary>
	DirectHiddenTriplet,

	/// <summary>
	/// Represents pointing.
	/// </summary>
	Pointing,

	/// <summary>
	/// Represents claiming.
	/// </summary>
	Claiming,

	/// <summary>
	/// Represents naked pair.
	/// </summary>
	NakedPair,

	/// <summary>
	/// Represents X-Wing.
	/// </summary>
	XWing,

	/// <summary>
	/// Represents hidden pair.
	/// </summary>
	HiddenPair,

	/// <summary>
	/// Represent naked triplet.
	/// </summary>
	NakedTriplet,

	/// <summary>
	/// Represent swordfish.
	/// </summary>
	Swordfish,

	/// <summary>
	/// Represents hidden triplet.
	/// </summary>
	HiddenTriplet,

	/// <summary>
	/// Represent turbot fish.
	/// </summary>
	TurbotFish,

	/// <summary>
	/// Represents XY-Wing.
	/// </summary>
	XyWing,

	/// <summary>
	/// Represents XYZ-Wing.
	/// </summary>
	XyzWing,

	/// <summary>
	/// Represents W-Wing.
	/// </summary>
	WWing,

	/// <summary>
	/// Represents WXYZ-Wing.
	/// </summary>
	WXYZWing,

	/// <summary>
	/// Represents unique loop.
	/// </summary>
	UniqueLoop,

	/// <summary>
	/// Represents naked quad.
	/// </summary>
	NakedQuad,

	/// <summary>
	/// Represents jellyfish.
	/// </summary>
	Jellyfish,

	/// <summary>
	/// Represents hidden quad.
	/// </summary>
	HiddenQuad,

	/// <summary>
	/// Represents bi-value universal grave.
	/// </summary>
	BivalueUniversalGrave,

	/// <summary>
	/// Represents bi-value universal grave + n.
	/// </summary>
	BivalueUniversalGravePlusN,

	/// <summary>
	/// Represents aligned pair exclusion.
	/// </summary>
	AlignedPairExclusion,

	/// <summary>
	/// Represents forcing chains / cycles.
	/// </summary>
	ForcingChainCycle,

	/// <summary>
	/// Represents aligned triplet exclusion.
	/// </summary>
	AlignedTripletExclusion,

	/// <summary>
	/// Represents nishio forcing chains.
	/// </summary>
	NishioForcingChain,

	/// <summary>
	/// Represents multiple forcing chains.
	/// </summary>
	MultipleForcingChain,

	/// <summary>
	/// Represents dynamic forcing chains.
	/// </summary>
	DynamicForcingChain,

	/// <summary>
	/// Represents dynamic forcing chains (+).
	/// </summary>
	DynamicForcingChainPlus,

	/// <summary>
	/// Represents nested forcing chains.
	/// </summary>
	NestedForcingChain
}
