namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a link type.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
[Flags]
public enum LinkType
{
	/// <summary>
	/// Indicates the placeholder of the enumeration. The value can be represented in cases "None" or "Unknown".
	/// </summary>
	Unknown = 0,

	/// <summary>
	/// Indicates the link type is a single digit (X rule).
	/// </summary>
	[ChainingRule<XChainingRule>]
	SingleDigit = 1 << 0,

	/// <summary>
	/// Indicates the link type is a single cell (Y rule).
	/// </summary>
	[ChainingRule<YChainingRule>]
	SingleCell = 1 << 1,

	/// <summary>
	/// Indicates the link type is a locked candidates.
	/// </summary>
	[ChainingRule<LockedCandidatesChainingRule>]
	LockedCandidates = 1 << 2,

	/// <summary>
	/// Indicates the link type is an almost locked set.
	/// </summary>
	[ChainingRule<AlmostLockedSetsChainingRule>]
	AlmostLockedSets = 1 << 3,

	/// <summary>
	/// Indicates the link type is a kraken normal fish.
	/// </summary>
	[ChainingRule<KrakenNormalFishChainingRule>]
	KrakenNormalFish = 1 << 5,

	/// <summary>
	/// Indicates the link type is an XYZ-Wing.
	/// </summary>
	[ChainingRule<XyzWingChainingRule>]
	XyzWing = 1 << 6,

	/// <summary>
	/// Indicates the link type is an almost unique rectangle.
	/// </summary>
	[ChainingRule<AlmostUniqueRectangleChainingRule>]
	AlmostUniqueRectangle = 1 << 7,

	/// <summary>
	/// Indicates the link type is an almost avoidable rectangle.
	/// </summary>
	[ChainingRule<AlmostAvoidableRectangleChainingRule>]
	AlmostAvoidableRectangle = 1 << 8
}
