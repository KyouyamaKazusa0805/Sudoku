namespace SudokuStudio.Drawing;

using static DrawableItemIdentifiers;

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
internal static partial class DrawableFactory
{
	/// <summary>
	/// The internal dictionary that describes the tag prefixes of view nodes.
	/// </summary>
	internal static readonly Dictionary<Type, string[]> ViewNodeTagPrefixes = new()
	{
		{ typeof(CellViewNode), ["cell"] },
		{ typeof(CandidateViewNode), ["candidate"] },
		{ typeof(HouseViewNode), ["house"] },
		{ typeof(ChainLinkViewNode), ["cell link", "curve segment", "arrow cap"] },
		{ typeof(ChuteViewNode), ["chute"] },
		{ typeof(BabaGroupViewNode), ["baba group"] }
	};

	/// <summary>
	/// The filters for controls that describes for view node.
	/// </summary>
	private static readonly Dictionary<DrawableItemUpdatingReason, Func<FrameworkElement, bool>> ReasonNodeFilters = new()
	{
		{ DrawableItemUpdatingReason.CandidateViewNodeDisplayMode, Filters.CandidateViewNodeDisplayMode },
		{ DrawableItemUpdatingReason.EliminationDisplayMode, Filters.EliminationDisplayMode },
		{ DrawableItemUpdatingReason.AssignmentDisplayMode, Filters.AssignmentDisplayMode },
		{ DrawableItemUpdatingReason.HighlightCandidateScale, Filters.HighlightCandidateScale },
		{ DrawableItemUpdatingReason.Link, Filters.Link },
		{ DrawableItemUpdatingReason.StrongLinkDashStyle, Filters.StrongLinkDashStyle },
		{ DrawableItemUpdatingReason.WeakLinkDashStyle, Filters.WeakLinkDashStyle },
		{ DrawableItemUpdatingReason.LinkStrokeThickness, Filters.LinkStrokeThickness },
		{ DrawableItemUpdatingReason.BabaGrouping, Filters.BabaGrouping },
		{ DrawableItemUpdatingReason.NormalColorized, control => Filters.Colorized(control, ColorIdentifierKind.Normal) },
		{ DrawableItemUpdatingReason.AssignmentColorized, control => Filters.Colorized(control, ColorIdentifierKind.Assignment) },
		{ DrawableItemUpdatingReason.OverlappedAssignmentColorized, control => Filters.Colorized(control, ColorIdentifierKind.OverlappedAssignment) },
		{ DrawableItemUpdatingReason.EliminationColorized, control => Filters.Colorized(control, ColorIdentifierKind.Elimination) },
		{ DrawableItemUpdatingReason.CannibalismColorized, control => Filters.Colorized(control, ColorIdentifierKind.Cannibalism) },
		{ DrawableItemUpdatingReason.ExofinColorized, control => Filters.Colorized(control, ColorIdentifierKind.Exofin) },
		{ DrawableItemUpdatingReason.EndofinColorized, control => Filters.Colorized(control, ColorIdentifierKind.Endofin) },
		{
			DrawableItemUpdatingReason.AuxiliaryColorized,
			control => Filters.ColorizedRange(control, [ColorIdentifierKind.Auxiliary1, ColorIdentifierKind.Auxiliary2, ColorIdentifierKind.Auxiliary3])
		},
		{
			DrawableItemUpdatingReason.AlmostLockedSetColorized,
			control => Filters.ColorizedRange(
				control,
				[ColorIdentifierKind.AlmostLockedSet1, ColorIdentifierKind.AlmostLockedSet2, ColorIdentifierKind.AlmostLockedSet3, ColorIdentifierKind.AlmostLockedSet4, ColorIdentifierKind.AlmostLockedSet5]
			)
		},
		{ DrawableItemUpdatingReason.ColorPaletteColorized, Filters.ColorizedPaletteId },
	};


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
		where T : allows ref struct
		=> throw new InvalidOperationException($"The {s} index configured is invalid - it must be between 0 and {range}.");


	public static partial void UpdateViewUnitControls(SudokuPane pane, DrawableItemUpdatingReason reason, object? value = null);

	private static partial void RemoveViewUnitControls(SudokuPane pane);

	private static partial void AddViewUnitControls(SudokuPane pane, ViewUnitBindableSource viewUnit);

	private static partial void ForConclusion(SudokuPane sudokuPane, Conclusion conclusion, List<Conclusion> overlapped, AnimatedResultCollection animatedResults);

	private static partial void ForCellNode(SudokuPane sudokuPane, CellViewNode cellNode, AnimatedResultCollection animatedResults);

	private static partial void ForIconNode(SudokuPane sudokuPane, IconViewNode iconNode, AnimatedResultCollection animatedResults);

	private static partial void ForCandidateNode(SudokuPane sudokuPane, CandidateViewNode candidateNode, Conclusion[] conclusions, out Conclusion? overlapped, AnimatedResultCollection animatedResults);

	private static partial void ForCandidateNodeCore(ColorIdentifier id, Color color, Candidate candidate, SudokuPaneCell paneCellControl, AnimatedResultCollection animatedResults, bool isForConclusion = false, bool isForElimination = false, bool isOverlapped = false);

	private static partial void ForHouseNode(SudokuPane sudokuPane, HouseViewNode houseNode, AnimatedResultCollection animatedResults);

	private static partial void ForChuteNode(SudokuPane sudokuPane, ChuteViewNode chuteNode, AnimatedResultCollection animatedResults);

	private static partial void ForBabaGroupNode(SudokuPane sudokuPane, BabaGroupViewNode babaGroupNode, AnimatedResultCollection animatedResults);

	private static partial void ForLinkNodes(SudokuPane sudokuPane, ReadOnlySpan<ChainLinkViewNode> linkNodes, Conclusion[] conclusions, AnimatedResultCollection animatedResults);
}

/// <summary>
/// The internal type that filters the controls.
/// </summary>
file static class Filters
{
	/// <summary>
	/// The filter for <see cref="DrawableItemUpdatingReason.CandidateViewNodeDisplayMode"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="DrawableItemUpdatingReason.CandidateViewNodeDisplayMode"/>
	public static bool CandidateViewNodeDisplayMode(FrameworkElement control)
		=> TemplateMethod<CandidateViewNode>(control, static (s, element) => s.Contains(element) && !s.Contains(ConclusionSuffixSeparator));

	/// <summary>
	/// The filter for <see cref="DrawableItemUpdatingReason.EliminationDisplayMode"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="DrawableItemUpdatingReason.EliminationDisplayMode"/>
	public static bool EliminationDisplayMode(FrameworkElement control)
		=> TemplateMethod<CandidateViewNode>(control, static (s, element) => s.Contains(element) && s.Contains(EliminationConclusionSuffix));

	/// <summary>
	/// The filter for <see cref="DrawableItemUpdatingReason.AssignmentDisplayMode"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="DrawableItemUpdatingReason.AssignmentDisplayMode"/>
	public static bool AssignmentDisplayMode(FrameworkElement control)
		=> TemplateMethod<CandidateViewNode>(control, static (s, element) => s.Contains(element) && s.Contains(AssignmentConclusionSuffix));

	/// <summary>
	/// The filter for <see cref="DrawableItemUpdatingReason.HighlightCandidateScale"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="DrawableItemUpdatingReason.HighlightCandidateScale"/>
	public static bool HighlightCandidateScale(FrameworkElement control)
		=> TemplateMethod<CandidateViewNode>(control, static (s, element) => s.Contains(element));

	/// <summary>
	/// The filter for <see cref="DrawableItemUpdatingReason.HighlightBackgroundOpacity"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="DrawableItemUpdatingReason.HighlightBackgroundOpacity"/>
	public static bool HighlightBackgroundOpacity(FrameworkElement control)
		=> TemplateMethod<CellViewNode, HouseViewNode, ChuteViewNode>(control);

	/// <summary>
	/// The filter for <see cref="DrawableItemUpdatingReason.Link"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="DrawableItemUpdatingReason.Link"/>
	public static bool Link(FrameworkElement control)
		=> TemplateMethod<ChainLinkViewNode>(control, static (s, element) => s.Contains(element));

	/// <summary>
	/// The filter for <see cref="DrawableItemUpdatingReason.BabaGrouping"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="DrawableItemUpdatingReason.BabaGrouping"/>
	public static bool BabaGrouping(FrameworkElement control)
		=> TemplateMethod<BabaGroupViewNode>(control, static (s, element) => s.Contains(element));

	/// <summary>
	/// The filter for <see cref="DrawableItemUpdatingReason.StrongLinkDashStyle"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="DrawableItemUpdatingReason.StrongLinkDashStyle"/>
	public static bool StrongLinkDashStyle(FrameworkElement control)
		=> TemplateMethod<ChainLinkViewNode>(
			control,
			static (s, element) => s.Contains(element) && s.Contains(StrongInferenceSuffix)
		);

	/// <summary>
	/// The filter for <see cref="DrawableItemUpdatingReason.WeakLinkDashStyle"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="DrawableItemUpdatingReason.WeakLinkDashStyle"/>
	public static bool WeakLinkDashStyle(FrameworkElement control)
		=> TemplateMethod<ChainLinkViewNode>(
			control,
			static (s, element) => s.Contains(element) && s.Contains(WeakInferenceSuffix)
		);

	/// <summary>
	/// The filter for <see cref="DrawableItemUpdatingReason.LinkStrokeThickness"/>.
	/// </summary>
	/// <param name="control">The control to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <seealso cref="DrawableItemUpdatingReason.LinkStrokeThickness"/>
	public static bool LinkStrokeThickness(FrameworkElement control)
		=> TemplateMethod<ChainLinkViewNode>(control, static (s, element) => s.Contains(element));

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
	/// The filter for colorized palette ID items.
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
	/// <typeparam name="TViewNode">The type of the node.</typeparam>
	/// <param name="control">The control to be checked.</param>
	/// <param name="tagMatcher">
	/// The matcher method that checks for the tag text, and return a result indicating whether the tag is satisfied.
	/// </param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	private static bool TemplateMethod<TViewNode>(FrameworkElement control, Func<string, string, bool> tagMatcher)
		where TViewNode : ViewNode
	{
		if (control.Tag is not string s)
		{
			return false;
		}

		foreach (var element in ViewNodeTagPrefixes[typeof(TViewNode)])
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
