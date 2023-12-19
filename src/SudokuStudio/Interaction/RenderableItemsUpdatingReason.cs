using Microsoft.UI.Xaml;
using Sudoku.Rendering;
using Sudoku.Rendering.Nodes;
using SudokuStudio.Rendering;

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
	/// Indicates the updating items are <see cref="CandidateViewNode"/>s, changing their circle scale.
	/// </summary>
	/// <seealso cref="CandidateViewNode"/>
	HighlightCandidateCircleScale,

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
	/// Indicates the updating items are view nodes that are defined <see cref="WellKnownColorIdentifierKind.Normal"/>.
	/// </summary>
	/// <seealso cref="WellKnownColorIdentifierKind.Normal"/>
	/// <seealso cref="WellKnownColorIdentifier.Normal"/>
	NormalColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="WellKnownColorIdentifierKind.Assignment"/>.
	/// </summary>
	/// <seealso cref="WellKnownColorIdentifierKind.Assignment"/>
	/// <seealso cref="WellKnownColorIdentifier.Assignment"/>
	AssignmentColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="WellKnownColorIdentifierKind.OverlappedAssignment"/>.
	/// </summary>
	/// <seealso cref="WellKnownColorIdentifierKind.OverlappedAssignment"/>
	/// <seealso cref="WellKnownColorIdentifier.OverlappedAssignment"/>
	OverlappedAssignmentColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="WellKnownColorIdentifierKind.Elimination"/>.
	/// </summary>
	/// <seealso cref="WellKnownColorIdentifierKind.Elimination"/>
	/// <seealso cref="WellKnownColorIdentifier.Elimination"/>
	EliminationColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="WellKnownColorIdentifierKind.Cannibalism"/>.
	/// </summary>
	/// <seealso cref="WellKnownColorIdentifierKind.Cannibalism"/>
	/// <seealso cref="WellKnownColorIdentifier.Cannibalism"/>
	CannibalismColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="WellKnownColorIdentifierKind.Exofin"/>.
	/// </summary>
	/// <seealso cref="WellKnownColorIdentifierKind.Exofin"/>
	/// <seealso cref="WellKnownColorIdentifier.Exofin"/>
	ExofinColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="WellKnownColorIdentifierKind.Endofin"/>.
	/// </summary>
	/// <seealso cref="WellKnownColorIdentifierKind.Endofin"/>
	/// <seealso cref="WellKnownColorIdentifier.Endofin"/>
	EndofinColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="WellKnownColorIdentifierKind.Auxiliary1"/>,
	/// <see cref="WellKnownColorIdentifierKind.Auxiliary2"/> or <see cref="WellKnownColorIdentifierKind.Auxiliary3"/>.
	/// </summary>
	/// <seealso cref="WellKnownColorIdentifierKind.Auxiliary1"/>
	/// <seealso cref="WellKnownColorIdentifierKind.Auxiliary2"/>
	/// <seealso cref="WellKnownColorIdentifierKind.Auxiliary3"/>
	/// <seealso cref="WellKnownColorIdentifier.Auxiliary1"/>
	/// <seealso cref="WellKnownColorIdentifier.Auxiliary2"/>
	/// <seealso cref="WellKnownColorIdentifier.Auxiliary3"/>
	AuxiliaryColorized,

	/// <summary>
	/// Indicates the updating items are view nodes that are defined <see cref="WellKnownColorIdentifierKind.AlmostLockedSet1"/>,
	/// <see cref="WellKnownColorIdentifierKind.AlmostLockedSet2"/>, <see cref="WellKnownColorIdentifierKind.AlmostLockedSet3"/>,
	/// <see cref="WellKnownColorIdentifierKind.AlmostLockedSet4"/> or <see cref="WellKnownColorIdentifierKind.AlmostLockedSet5"/>.
	/// </summary>
	/// <seealso cref="WellKnownColorIdentifierKind.AlmostLockedSet1"/>
	/// <seealso cref="WellKnownColorIdentifierKind.AlmostLockedSet2"/>
	/// <seealso cref="WellKnownColorIdentifierKind.AlmostLockedSet3"/>
	/// <seealso cref="WellKnownColorIdentifierKind.AlmostLockedSet4"/>
	/// <seealso cref="WellKnownColorIdentifierKind.AlmostLockedSet5"/>
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

/// <summary>
/// Defines the filters that filters the renderable item controls displayed in UI.
/// </summary>
internal static class PaneUpdateRenderableItemFilters
{
	/// <summary>
	/// The factory type name.
	/// </summary>
	private const string FactoryTypeName = nameof(RenderableFactory);


	/// <summary>
	/// The filter for <see cref="RenderableItemsUpdatingReason.Link"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="RenderableItemsUpdatingReason.Link"/>
	public static bool Link(FrameworkElement control) => TemplateMethod<LinkViewNode>(control);

	/// <summary>
	/// The filter for <see cref="RenderableItemsUpdatingReason.BabaGrouping"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="RenderableItemsUpdatingReason.BabaGrouping"/>
	public static bool BabaGrouping(FrameworkElement control) => TemplateMethod<BabaGroupViewNode>(control);

	/// <summary>
	/// The template method.
	/// </summary>
	/// <typeparam name="T">The type of the node.</typeparam>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	private static bool TemplateMethod<T>(FrameworkElement control) where T : ViewNode
	{
		if (control.Tag is not string { Length: var p } s || p < FactoryTypeName.Length)
		{
			return false;
		}

		var containsAny = false;
		foreach (var element in RenderableFactory.ViewNodeTagPrefixes[typeof(T)])
		{
			if (s.Contains(element))
			{
				containsAny = true;
				break;
			}
		}

		return containsAny;
	}
}
