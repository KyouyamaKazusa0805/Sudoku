namespace SudokuStudio.Rendering;

using static RenderableItemIdentifiers;

/// <summary>
/// Defines a factory type that is used for creating a list of <see cref="FrameworkElement"/>
/// to display for highlighted cells, candidates and so on.
/// </summary>
/// <remarks>
/// All created <see cref="FrameworkElement"/> instances will be tagged as a <see cref="string"/>, whose value is "<c>RenderableFactory</c>",
/// in order to be used for distinction with other controls in the collection.
/// </remarks>
/// <seealso cref="FrameworkElement"/>
[SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
internal static partial class RenderableFactory
{
	/// <summary>
	/// The internal dictionary that describes the tag prefixes of view nodes.
	/// </summary>
	internal static readonly Dictionary<Type, string[]> ViewNodeTagPrefixes = new()
	{
		{ typeof(CellViewNode), ["cell"] },
		{ typeof(CandidateViewNode), ["candidate"] },
		{ typeof(HouseViewNode), ["house"] },
		{ typeof(LinkViewNode), ["cell link", "curve segment", "arrow cap"] },
		{ typeof(ChuteViewNode), ["chute"] },
		{ typeof(BabaGroupViewNode), ["baba group"] }
	};

	/// <summary>
	/// The filters for controls that describes for view node.
	/// </summary>
	private static readonly Dictionary<RenderableItemsUpdatingReason, Func<FrameworkElement, bool>> ReasonNodeFilters = new()
	{
		{ RenderableItemsUpdatingReason.CandidateViewNodeDisplayMode, Filters.CandidateViewNodeDisplayMode },
		{ RenderableItemsUpdatingReason.EliminationDisplayMode, Filters.EliminationDisplayMode },
		{ RenderableItemsUpdatingReason.AssignmentDisplayMode, Filters.AssignmentDisplayMode },
		{ RenderableItemsUpdatingReason.HighlightCandidateScale, Filters.HighlightCandidateScale },
		{ RenderableItemsUpdatingReason.Link, Filters.Link },
		{ RenderableItemsUpdatingReason.StrongLinkDashStyle, Filters.StrongLinkDashStyle },
		{ RenderableItemsUpdatingReason.WeakLinkDashStyle, Filters.WeakLinkDashStyle },
		{ RenderableItemsUpdatingReason.CycleLikeLinkDashStyle, Filters.CycleLikeLinkDashStyle },
		{ RenderableItemsUpdatingReason.OtherLinkDashStyle, Filters.OtherLinkDashStyle },
		{ RenderableItemsUpdatingReason.LinkStrokeThickness, Filters.LinkStrokeThickness },
		{ RenderableItemsUpdatingReason.BabaGrouping, Filters.BabaGrouping },
		{ RenderableItemsUpdatingReason.NormalColorized, control => Filters.Colorized(control, ColorIdentifierKind.Normal) },
		{ RenderableItemsUpdatingReason.AssignmentColorized, control => Filters.Colorized(control, ColorIdentifierKind.Assignment) },
		{ RenderableItemsUpdatingReason.OverlappedAssignmentColorized, control => Filters.Colorized(control, ColorIdentifierKind.OverlappedAssignment) },
		{ RenderableItemsUpdatingReason.EliminationColorized, control => Filters.Colorized(control, ColorIdentifierKind.Elimination) },
		{ RenderableItemsUpdatingReason.CannibalismColorized, control => Filters.Colorized(control, ColorIdentifierKind.Cannibalism) },
		{ RenderableItemsUpdatingReason.ExofinColorized, control => Filters.Colorized(control, ColorIdentifierKind.Exofin) },
		{ RenderableItemsUpdatingReason.EndofinColorized, control => Filters.Colorized(control, ColorIdentifierKind.Endofin) },
		{
			RenderableItemsUpdatingReason.AuxiliaryColorized,
			control => Filters.ColorizedRange(control, [ColorIdentifierKind.Auxiliary1, ColorIdentifierKind.Auxiliary2, ColorIdentifierKind.Auxiliary3])
		},
		{
			RenderableItemsUpdatingReason.AlmostLockedSetColorized,
			control => Filters.ColorizedRange(
				control,
				[ColorIdentifierKind.AlmostLockedSet1, ColorIdentifierKind.AlmostLockedSet2, ColorIdentifierKind.AlmostLockedSet3, ColorIdentifierKind.AlmostLockedSet4, ColorIdentifierKind.AlmostLockedSet5]
			)
		},
		{ RenderableItemsUpdatingReason.ColorPaletteColorized, Filters.ColorizedPaletteId },
	};


	public static partial void UpdateViewUnitControls(SudokuPane pane, RenderableItemsUpdatingReason reason, object? value = null);

	private static partial void RemoveViewUnitControls(SudokuPane pane);

	private static partial void AddViewUnitControls(SudokuPane pane, ViewUnitBindableSource viewUnit);

	private static partial void ForConclusion(SudokuPane sudokuPane, Conclusion conclusion, List<Conclusion> overlapped, AnimatedResultCollection animatedResults);

	private static partial void ForCellNode(SudokuPane sudokuPane, CellViewNode cellNode, AnimatedResultCollection animatedResults);

	private static partial void ForIconNode(SudokuPane sudokuPane, IconViewNode iconNode, AnimatedResultCollection animatedResults);

	private static partial void ForCandidateNode(
		SudokuPane sudokuPane,
		CandidateViewNode candidateNode,
		Conclusion[] conclusions,
		out Conclusion? overlapped,
		AnimatedResultCollection animatedResults
	);

	private static partial void ForCandidateNodeCore(
		ColorIdentifier id,
		Color color,
		Candidate candidate,
		SudokuPaneCell paneCellControl,
		AnimatedResultCollection animatedResults,
		bool isForConclusion = false,
		bool isForElimination = false,
		bool isOverlapped = false
	);

	private static partial void ForHouseNode(
		SudokuPane sudokuPane,
		HouseViewNode houseNode,
		AnimatedResultCollection animatedResults
	);

	private static partial void ForChuteNode(
		SudokuPane sudokuPane,
		ChuteViewNode chuteNode,
		AnimatedResultCollection animatedResults
	);

	private static partial void ForBabaGroupNode(
		SudokuPane sudokuPane,
		BabaGroupViewNode babaGroupNode,
		AnimatedResultCollection animatedResults
	);

	private static partial void ForLinkNodes(
		SudokuPane sudokuPane,
		ReadOnlySpan<LinkViewNode> linkNodes,
		Conclusion[] conclusions,
		AnimatedResultCollection animatedResults
	);

	/// <summary>
	/// Get conclusion suffix of tag.
	/// </summary>
	/// <returns>A <see cref="string"/> text as the result.</returns>
	private static string? GetConclusionTagSuffix(bool isForConclusion, bool isForElimination, bool isOverlapped)
		=> (isForConclusion, isForElimination, isOverlapped) switch
		{
			(true, true, true) => CannibalismConclusionSuffix,
			(true, true, _) => EliminationConclusionSuffix,
			(true, _, true) => OverlappedAssignmentConclusionSuffix,
			(true, _, _) => AssignmentConclusionSuffix,
			_ => null
		};

	/// <summary>
	/// The internal helper method that creates a <see cref="InvalidOperationException"/> instance without any other operation.
	/// </summary>
	/// <typeparam name="T">The type of the return value if the exception were not thrown.</typeparam>
	/// <param name="o">The object.</param>
	/// <param name="range">The range of the argument should be.</param>
	/// <param name="s">The caller expression for argument <paramref name="o"/>.</param>
	/// <returns><typeparamref name="T"/> instance. The value is unnecessary because an exception will be thrown.</returns>
	/// <exception cref="InvalidOperationException">Always throws.</exception>
	[DoesNotReturn]
	private static T? Throw<T>(object? o, int range, [CallerArgumentExpression(nameof(o))] string? s = null)
		=> throw new InvalidOperationException($"The {s} index configured is invalid - it must be between 0 and {range}.");
}

/// <summary>
/// The internal type that filters the controls.
/// </summary>
file static class Filters
{
	/// <summary>
	/// The filter for <see cref="RenderableItemsUpdatingReason.CandidateViewNodeDisplayMode"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="RenderableItemsUpdatingReason.CandidateViewNodeDisplayMode"/>
	public static bool CandidateViewNodeDisplayMode(FrameworkElement control)
		=> TemplateMethod<CandidateViewNode>(control, static (s, element) => s.Contains(element) && !s.Contains(ConclusionSuffixSeparator));

	/// <summary>
	/// The filter for <see cref="RenderableItemsUpdatingReason.EliminationDisplayMode"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="RenderableItemsUpdatingReason.EliminationDisplayMode"/>
	public static bool EliminationDisplayMode(FrameworkElement control)
		=> TemplateMethod<CandidateViewNode>(control, static (s, element) => s.Contains(element) && s.Contains(EliminationConclusionSuffix));

	/// <summary>
	/// The filter for <see cref="RenderableItemsUpdatingReason.AssignmentDisplayMode"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="RenderableItemsUpdatingReason.AssignmentDisplayMode"/>
	public static bool AssignmentDisplayMode(FrameworkElement control)
		=> TemplateMethod<CandidateViewNode>(control, static (s, element) => s.Contains(element) && s.Contains(AssignmentConclusionSuffix));

	/// <summary>
	/// The filter for <see cref="RenderableItemsUpdatingReason.HighlightCandidateScale"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="RenderableItemsUpdatingReason.HighlightCandidateScale"/>
	public static bool HighlightCandidateScale(FrameworkElement control)
		=> TemplateMethod<CandidateViewNode>(control, static (s, element) => s.Contains(element));

	/// <summary>
	/// The filter for <see cref="RenderableItemsUpdatingReason.HighlightBackgroundOpacity"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="RenderableItemsUpdatingReason.HighlightBackgroundOpacity"/>
	public static bool HighlightBackgroundOpacity(FrameworkElement control)
		=> TemplateMethod<CellViewNode, HouseViewNode, ChuteViewNode>(control);

	/// <summary>
	/// The filter for <see cref="RenderableItemsUpdatingReason.Link"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="RenderableItemsUpdatingReason.Link"/>
	public static bool Link(FrameworkElement control)
		=> TemplateMethod<LinkViewNode>(control, static (s, element) => s.Contains(element));

	/// <summary>
	/// The filter for <see cref="RenderableItemsUpdatingReason.BabaGrouping"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="RenderableItemsUpdatingReason.BabaGrouping"/>
	public static bool BabaGrouping(FrameworkElement control)
		=> TemplateMethod<BabaGroupViewNode>(control, static (s, element) => s.Contains(element));

	/// <summary>
	/// The filter for <see cref="RenderableItemsUpdatingReason.StrongLinkDashStyle"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="RenderableItemsUpdatingReason.StrongLinkDashStyle"/>
	public static bool StrongLinkDashStyle(FrameworkElement control)
		=> TemplateMethod<LinkViewNode>(
			control,
			static (s, element) => s.Contains(element) && (s.Contains(StrongInferenceSuffix) || s.Contains(StrongGeneralizedInferenceSuffix))
		);

	/// <summary>
	/// The filter for <see cref="RenderableItemsUpdatingReason.WeakLinkDashStyle"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="RenderableItemsUpdatingReason.WeakLinkDashStyle"/>
	public static bool WeakLinkDashStyle(FrameworkElement control)
		=> TemplateMethod<LinkViewNode>(
			control,
			static (s, element) => s.Contains(element) && (s.Contains(WeakInferenceSuffix) || s.Contains(WeakGeneralizedInferenceSuffix))
		);

	/// <summary>
	/// The filter for <see cref="RenderableItemsUpdatingReason.CycleLikeLinkDashStyle"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="RenderableItemsUpdatingReason.CycleLikeLinkDashStyle"/>
	public static bool CycleLikeLinkDashStyle(FrameworkElement control)
		=> TemplateMethod<LinkViewNode>(control, static (s, element) => s.Contains(element) && s.Contains(DefaultInferenceSuffix));

	/// <summary>
	/// The filter for <see cref="RenderableItemsUpdatingReason.OtherLinkDashStyle"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="RenderableItemsUpdatingReason.OtherLinkDashStyle"/>
	public static bool OtherLinkDashStyle(FrameworkElement control)
		=> TemplateMethod<LinkViewNode>(control, static (s, element) => s.Contains(element) && s.Contains(ConjugateInferenceSuffix));

	/// <summary>
	/// The filter for <see cref="RenderableItemsUpdatingReason.LinkStrokeThickness"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="RenderableItemsUpdatingReason.LinkStrokeThickness"/>
	public static bool LinkStrokeThickness(FrameworkElement control)
		=> TemplateMethod<LinkViewNode>(control, static (s, element) => s.Contains(element));

	/// <summary>
	/// The filter for colorized items (except for <see cref="ColorPalette"/>-based items).
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <param name="kind">The kind to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="ColorPalette"/>
	public static bool Colorized(FrameworkElement control, ColorIdentifierKind kind)
	{
		if (control.Tag is not string s)
		{
			return false;
		}

		var array = (string[])[
			.. ViewNodeTagPrefixes[typeof(CellViewNode)],
			.. ViewNodeTagPrefixes[typeof(CandidateViewNode)],
			.. ViewNodeTagPrefixes[typeof(HouseViewNode)],
			.. ViewNodeTagPrefixes[typeof(ChuteViewNode)]
		];
		foreach (var element in array)
		{
			if (s.Contains(element) && Enum.TryParse<ColorIdentifierKind>(s[(s.IndexOf(ColorizedSuffixSeparator) + 2)..^1], out var final) && final == kind)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// The filter for colorized range items (for <see cref="ColorPalette"/>-based items).
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <param name="kinds">The kinds to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="ColorPalette"/>
	public static bool ColorizedRange(FrameworkElement control, ColorIdentifierKind[] kinds)
	{
		if (control.Tag is not string s)
		{
			return false;
		}

		var array = (string[])[
			.. ViewNodeTagPrefixes[typeof(CellViewNode)],
			.. ViewNodeTagPrefixes[typeof(CandidateViewNode)],
			.. ViewNodeTagPrefixes[typeof(HouseViewNode)],
			.. ViewNodeTagPrefixes[typeof(ChuteViewNode)]
		];
		foreach (var element in array)
		{
			if (s.Contains(element)
				&& Enum.TryParse<ColorIdentifierKind>(s[(s.IndexOf(ColorizedSuffixSeparator) + 2)..], out var final)
				&& Array.IndexOf(kinds, final) != -1)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// The filter for colorized paleete ID items.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool ColorizedPaletteId(FrameworkElement control)
	{
		if (control.Tag is not string s)
		{
			return false;
		}

		var array = (string[])[
			.. ViewNodeTagPrefixes[typeof(CellViewNode)],
			.. ViewNodeTagPrefixes[typeof(CandidateViewNode)],
			.. ViewNodeTagPrefixes[typeof(HouseViewNode)],
			.. ViewNodeTagPrefixes[typeof(ChuteViewNode)]
		];
		foreach (var element in array)
		{
			if (s.Contains(element)
				&& s.IndexOf(IdColorIdentifierSeparator) is var pos and not -1 && int.TryParse(s[(pos + 2)..], out _))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// The template method for view nodes.
	/// </summary>
	/// <typeparam name="T">The type of the node.</typeparam>
	/// <param name="control">The control to be checked.</param>
	/// <param name="tagMatcher">
	/// The matcher method that checks for the tag text, and return a result indicating whether the tag is satisfied.
	/// </param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	private static bool TemplateMethod<T>(FrameworkElement control, Func<string, string, bool> tagMatcher) where T : ViewNode
	{
		if (control.Tag is not string s)
		{
			return false;
		}

		foreach (var element in ViewNodeTagPrefixes[typeof(T)])
		{
			if (tagMatcher(s, element))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// The template method for view nodes.
	/// </summary>
	/// <typeparam name="T1">The first type of the node.</typeparam>
	/// <typeparam name="T2">The second type of the node.</typeparam>
	/// <typeparam name="T3">The third type of the node.</typeparam>
	/// <param name="control">The control to be checked.</param>
	/// <param name="tagMatcher">
	/// The matcher method that checks for the tag text, and return a result indicating whether the tag is satisfied.
	/// </param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	private static bool TemplateMethod<T1, T2, T3>(FrameworkElement control, Func<string, string, bool>? tagMatcher = null)
		where T1 : ViewNode
		where T2 : ViewNode
		where T3 : ViewNode
	{
		if (control.Tag is not string s)
		{
			return false;
		}

		if (tagMatcher is null)
		{
			return true;
		}

		var array = (string[])[.. ViewNodeTagPrefixes[typeof(T1)], .. ViewNodeTagPrefixes[typeof(T2)], .. ViewNodeTagPrefixes[typeof(T3)]];
		foreach (var element in array)
		{
			if (tagMatcher(s, element))
			{
				return true;
			}
		}
		return false;
	}
}
