namespace SudokuStudio.Interaction;

/// <summary>
/// Represents a list of values that describes the reason why nodes should be updated.
/// </summary>
public enum DrawableItemUpdatingReason
{
	/// <summary>
	/// Indicates none will be updated.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the updating items are <see cref="CandidateViewNode"/>s, changing their display shape mode; only for non-eliminations.
	/// </summary>
	/// <seealso cref="CandidateViewNode"/>
	CandidateViewNodeDisplayMode,

	/// <summary>
	/// Indicates the updating items are <see cref="CandidateViewNode"/>s, changing their display shape mode; only for eliminations.
	/// </summary>
	/// <seealso cref="CandidateViewNode"/>
	EliminationDisplayMode,

	/// <summary>
	/// Indicates the updating items are <see cref="CandidateViewNode"/>s, changing their display shape mode; only for eliminations.
	/// </summary>
	/// <seealso cref="CandidateViewNode"/>
	AssignmentDisplayMode,

	/// <summary>
	/// Indicates the updating items are <see cref="CandidateViewNode"/>s, changing their scale.
	/// </summary>
	/// <seealso cref="CandidateViewNode"/>
	HighlightCandidateScale,

	/// <summary>
	/// Indicates the updating items are <see cref="CellViewNode"/>s, <see cref="HouseViewNode"/>s and <see cref="ChuteViewNode"/>s,
	/// changing their background opacity.
	/// </summary>
	/// <seealso cref="CellViewNode"/>
	/// <seealso cref="HouseViewNode"/>
	/// <seealso cref="ChuteViewNode"/>
	HighlightBackgroundOpacity,

	/// <summary>
	/// Indicates the updating items are <see cref="ChainLinkViewNode"/>s.
	/// </summary>
	/// <seealso cref="ChainLinkViewNode"/>
	Link,

	/// <summary>
	/// Indicates the updating items are <see cref="ChainLinkViewNode"/>s, changing their dash styles.
	/// </summary>
	/// <seealso cref="ChainLinkViewNode"/>
	StrongLinkDashStyle,

	/// <inheritdoc cref="StrongLinkDashStyle"/>
	WeakLinkDashStyle,

	/// <inheritdoc cref="StrongLinkDashStyle"/>
	CycleLikeLinkDashStyle,

	/// <inheritdoc cref="StrongLinkDashStyle"/>
	OtherLinkDashStyle,

	/// <summary>
	/// Indicates the updating items are <see cref="ChainLinkViewNode"/>, changing their stroke thickness.
	/// </summary>
	/// <seealso cref="ChainLinkViewNode"/>
	LinkStrokeThickness,

	/// <summary>
	/// Indicates the updating items are <see cref="BabaGroupViewNode"/>s.
	/// </summary>
	/// <seealso cref="BabaGroupViewNode"/>
	BabaGrouping,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.Normal"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.Normal"/>
	/// <seealso cref="ColorIdentifier.Normal"/>
	NormalColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.Assignment"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.Assignment"/>
	/// <seealso cref="ColorIdentifier.Assignment"/>
	AssignmentColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.OverlappedAssignment"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.OverlappedAssignment"/>
	/// <seealso cref="ColorIdentifier.OverlappedAssignment"/>
	OverlappedAssignmentColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.Elimination"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.Elimination"/>
	/// <seealso cref="ColorIdentifier.Elimination"/>
	EliminationColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.Cannibalism"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.Cannibalism"/>
	/// <seealso cref="ColorIdentifier.Cannibalism"/>
	CannibalismColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.Exofin"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.Exofin"/>
	/// <seealso cref="ColorIdentifier.Exofin"/>
	ExofinColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.Endofin"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.Endofin"/>
	/// <seealso cref="ColorIdentifier.Endofin"/>
	EndofinColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.Auxiliary1"/>,
	/// <see cref="ColorIdentifierKind.Auxiliary2"/> or <see cref="ColorIdentifierKind.Auxiliary3"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.Auxiliary1"/>
	/// <seealso cref="ColorIdentifierKind.Auxiliary2"/>
	/// <seealso cref="ColorIdentifierKind.Auxiliary3"/>
	/// <seealso cref="ColorIdentifier.Auxiliary1"/>
	/// <seealso cref="ColorIdentifier.Auxiliary2"/>
	/// <seealso cref="ColorIdentifier.Auxiliary3"/>
	AuxiliaryColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.AlmostLockedSet1"/>,
	/// <see cref="ColorIdentifierKind.AlmostLockedSet2"/>, <see cref="ColorIdentifierKind.AlmostLockedSet3"/>,
	/// <see cref="ColorIdentifierKind.AlmostLockedSet4"/> or <see cref="ColorIdentifierKind.AlmostLockedSet5"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.AlmostLockedSet1"/>
	/// <seealso cref="ColorIdentifierKind.AlmostLockedSet2"/>
	/// <seealso cref="ColorIdentifierKind.AlmostLockedSet3"/>
	/// <seealso cref="ColorIdentifierKind.AlmostLockedSet4"/>
	/// <seealso cref="ColorIdentifierKind.AlmostLockedSet5"/>
	/// <seealso cref="ColorIdentifier.AlmostLockedSet1"/>
	/// <seealso cref="ColorIdentifier.AlmostLockedSet2"/>
	/// <seealso cref="ColorIdentifier.AlmostLockedSet3"/>
	/// <seealso cref="ColorIdentifier.AlmostLockedSet4"/>
	/// <seealso cref="ColorIdentifier.AlmostLockedSet5"/>
	AlmostLockedSetColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined colors defined in color palette.
	/// </summary>
	ColorPaletteColorized,

	/// <summary>
	/// Indicates all nodes should be refreshed.
	/// </summary>
	All
}
