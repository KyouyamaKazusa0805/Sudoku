namespace SudokuStudio.Interaction;

/// <summary>
/// Represents a list of values that describes the reason why nodes should be updated.
/// </summary>
public enum RenderableItemsUpdatingReason
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
	/// Indicates the updating items are <see cref="LinkViewNode"/>s.
	/// </summary>
	/// <seealso cref="LinkViewNode"/>
	Link,

	/// <summary>
	/// Indicates the updating items are <see cref="LinkViewNode"/>s, changing their dash styles.
	/// </summary>
	/// <seealso cref="LinkViewNode"/>
	StrongLinkDashStyle,

	/// <inheritdoc cref="StrongLinkDashStyle"/>
	WeakLinkDashStyle,

	/// <inheritdoc cref="StrongLinkDashStyle"/>
	CycleLikeLinkDashStyle,

	/// <inheritdoc cref="StrongLinkDashStyle"/>
	OtherLinkDashStyle,

	/// <summary>
	/// Indicates the updating items are <see cref="LinkViewNode"/>, changing their stroke thickness.
	/// </summary>
	/// <seealso cref="LinkViewNode"/>
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
	/// <seealso cref="WellKnownColorIdentifier.Normal"/>
	NormalColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.Assignment"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.Assignment"/>
	/// <seealso cref="WellKnownColorIdentifier.Assignment"/>
	AssignmentColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.OverlappedAssignment"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.OverlappedAssignment"/>
	/// <seealso cref="WellKnownColorIdentifier.OverlappedAssignment"/>
	OverlappedAssignmentColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.Elimination"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.Elimination"/>
	/// <seealso cref="WellKnownColorIdentifier.Elimination"/>
	EliminationColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.Cannibalism"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.Cannibalism"/>
	/// <seealso cref="WellKnownColorIdentifier.Cannibalism"/>
	CannibalismColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.Exofin"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.Exofin"/>
	/// <seealso cref="WellKnownColorIdentifier.Exofin"/>
	ExofinColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.Endofin"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.Endofin"/>
	/// <seealso cref="WellKnownColorIdentifier.Endofin"/>
	EndofinColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="ColorIdentifierKind.Auxiliary1"/>,
	/// <see cref="ColorIdentifierKind.Auxiliary2"/> or <see cref="ColorIdentifierKind.Auxiliary3"/>.
	/// </summary>
	/// <seealso cref="ColorIdentifierKind.Auxiliary1"/>
	/// <seealso cref="ColorIdentifierKind.Auxiliary2"/>
	/// <seealso cref="ColorIdentifierKind.Auxiliary3"/>
	/// <seealso cref="WellKnownColorIdentifier.Auxiliary1"/>
	/// <seealso cref="WellKnownColorIdentifier.Auxiliary2"/>
	/// <seealso cref="WellKnownColorIdentifier.Auxiliary3"/>
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
	/// <seealso cref="WellKnownColorIdentifier.AlmostLockedSet1"/>
	/// <seealso cref="WellKnownColorIdentifier.AlmostLockedSet2"/>
	/// <seealso cref="WellKnownColorIdentifier.AlmostLockedSet3"/>
	/// <seealso cref="WellKnownColorIdentifier.AlmostLockedSet4"/>
	/// <seealso cref="WellKnownColorIdentifier.AlmostLockedSet5"/>
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
