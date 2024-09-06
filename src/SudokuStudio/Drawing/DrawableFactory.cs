namespace SudokuStudio.Drawing;

/// <summary>
/// Defines a factory type that is used for creating a list of <see cref="FrameworkElement"/>
/// to display for highlighted cells, candidates and so on.
/// </summary>
/// <remarks>
/// All created <see cref="FrameworkElement"/> instances will be tagged as a <see cref="string"/>, whose value is "<c>RenderableFactory</c>",
/// in order to be used for distinction with other controls in the collection.
/// </remarks>
/// <seealso cref="FrameworkElement"/>
internal static partial class DrawableFactory
{
	/// <summary>
	/// Refresh the pan view unit controls incrementally.
	/// </summary>
	/// <param name="pane">The pane.</param>
	/// <param name="old">The original value to be compared.</param>
	/// <param name="new">The new value to be compared.</param>
	public static void IncrementalUpdateViewUnitControls(SudokuPane pane, ViewUnitBindableSource? old, ViewUnitBindableSource? @new)
	{
		// Determine which nodes should be updated.
		// We may not clear all possible controls, in order to optimize the memory usage and performance.
		if (old - @new is not (var negatives, var positives) { IsEmpty: false })
		{
			return;
		}

		// Find all bound controls that have already displayed in control.
		// If some controls can be found in 'negatives', it should be removed.
		foreach (var child in IterateControls(pane))
		{
			foreach (var z in child.FindDrawableControls())
			{
				switch (z.Tag)
				{
					case ViewNode v when negatives.Any(n => n is ViewNode p && p == v): { child.Remove(z); break; }
					case CandidateMap: { child.Remove(z); break; }
					case Conclusion c when negatives.Any(n => n is Conclusion p && p == c): { child.Remove(z); break; }
				}
			}
		}

		// Then add controls.
		AddViewUnitControls(
			pane,
			new()
			{
				Conclusions = [.. from p in positives where p is Conclusion select (Conclusion)p],
				View = [.. from p in positives where p is ViewNode select (ViewNode)p]
			}
		);
	}

	/// <summary>
	/// Refresh the pane view unit controls.
	/// </summary>
	/// <param name="pane">The pane.</param>
	public static void UpdateViewUnitControls(SudokuPane pane)
	{
		RemoveViewUnitControls(pane);
		if (pane.ViewUnit is not null)
		{
			AddViewUnitControls(pane, pane.ViewUnit);
		}
	}

	/// <summary>
	/// Removes all possible controls that are used for displaying elements in a <see cref="ViewUnitBindableSource"/>.
	/// </summary>
	/// <param name="pane">The target pane.</param>
	/// <seealso cref="ViewUnitBindableSource"/>
	private static void RemoveViewUnitControls(SudokuPane pane)
	{
		foreach (var child in IterateControls(pane))
		{
			child.RemoveAllViewUnitControls();
		}
	}

	/// <summary>
	/// Adds a list of <see cref="FrameworkElement"/>s that are used for displaying highlight elements in a <see cref="ViewUnitBindableSource"/>.
	/// </summary>
	/// <param name="pane">The target pane.</param>
	/// <param name="viewUnit">The view unit that you want to display.</param>
	/// <seealso cref="FrameworkElement"/>
	/// <seealso cref="ViewUnitBindableSource"/>
	private static void AddViewUnitControls(SudokuPane pane, ViewUnitBindableSource viewUnit)
	{
		// Check whether the data can be deconstructed.
		if (viewUnit is not { View: var view, Conclusions: var conclusions })
		{
			return;
		}

		var pencilmarkMode = Application.Current.AsApp().Preference.UIPreferences.DisplayCandidates;
		var usedCandidates = CandidateMap.Empty;
		var (controlAddingActions, overlapped, links) = (new AnimatedResultCollection(), new List<Conclusion>(), new List<ILinkViewNode>());

		// Iterate on each view node, and get their own corresponding controls.
		var context = new DrawingContext(pane, controlAddingActions);
		foreach (var viewNode in view)
		{
			(
				viewNode switch
				{
					CellViewNode c => context => ForCellNode(context, c),
					IconViewNode i => context => ForIconNode(context, i),
					CandidateViewNode c => context => onCandidateViewNode(context, c),
					HouseViewNode h => context => ForHouseNode(context, h),
					ChuteViewNode c => context => ForChuteNode(context, c),
					BabaGroupViewNode b => context => ForBabaGroupNode(context, b),
					ILinkViewNode l => _ => links.Add(l),
					_ => default(Action<DrawingContext>)
				}
			)?.Invoke(context);


			void onCandidateViewNode(DrawingContext context, CandidateViewNode c)
			{
				ForCandidateNode(context, c, conclusions, out var o);
				if (o is { } currentOverlappedConclusion)
				{
					overlapped.Add(currentOverlappedConclusion);
				}
				usedCandidates.Add(c.Candidate);
			}
		}

		// Then iterate on each conclusions. Those conclusions will also be rendered as real controls.
		foreach (var conclusion in conclusions)
		{
			ForConclusion(context, conclusion, overlapped);
			usedCandidates.Add(conclusion.Candidate);
		}

		// Finally, iterate on links.
		// The links are special to be handled - they will create a list of line controls.
		// We should handle it at last.
		ForLinkNodes(context, links.AsReadOnlySpan(), view.OfType<CandidateViewNode>(), conclusions);
		controlAddingActions.ForEach(static p => (p.Animating + p.Adding)());
	}

	/// <summary>
	/// Iterate <see cref="UIElementCollection"/> values that may contain drawable-item-based controls.
	/// </summary>
	/// <param name="pane">The pane.</param>
	private static IEnumerable<UIElementCollection> IterateControls(SudokuPane pane)
		=>
		from targetControl in (from children in pane._children select children.MainGrid).Append(pane.MainGrid)
		select targetControl.Children;


	private static partial void ForConclusion(DrawingContext context, Conclusion conclusion, List<Conclusion> overlapped);
	private static partial void ForCellNode(DrawingContext context, CellViewNode cellNode);
	private static partial void ForIconNode(DrawingContext context, IconViewNode iconNode);
	private static partial void ForCandidateNode(DrawingContext context, CandidateViewNode candidateNode, Conclusion[] conclusions, out Conclusion? overlapped);
	private static partial void ForHouseNode(DrawingContext context, HouseViewNode houseNode);
	private static partial void ForChuteNode(DrawingContext context, ChuteViewNode chuteNode);
	private static partial void ForBabaGroupNode(DrawingContext context, BabaGroupViewNode babaGroupNode);
	private static partial void ForLinkNodes(DrawingContext context, ReadOnlySpan<ILinkViewNode> linkNodes, ReadOnlySpan<CandidateViewNode> candidateNodes, Conclusion[] conclusions);
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Removes all possible <see cref="FrameworkElement"/>s that is used for displaying elements in a <see cref="ViewUnitBindableSource"/>.
	/// </summary>
	/// <param name="this">The collection.</param>
	public static void RemoveAllViewUnitControls(this UIElementCollection @this)
	{
		foreach (var control in @this.FindDrawableControls())
		{
			@this.Remove(control);
		}
	}

	/// <summary>
	/// Returns a list of <see cref="FrameworkElement"/> instances that bound with drawable items.
	/// </summary>
	/// <param name="collection">The pane.</param>
	public static IEnumerable<FrameworkElement> FindDrawableControls(this UIElementCollection collection)
		=> (
			from child in collection.OfType<FrameworkElement>()
			where child.Tag is IDrawableItem and (ViewNode or CandidateMap or Conclusion)
			select child
		).ToArray();
}
