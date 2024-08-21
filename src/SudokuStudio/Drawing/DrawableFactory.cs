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
		foreach (var child in
			from targetControl in (FrameworkElement[])[.. from children in pane._children select children.MainGrid, pane.MainGrid]
			let c = targetControl as GridLayout
			where c is not null
			select c.Children)
		{
			child.RemoveAllViewUnitControls();
		}
		pane.ViewUnitUsedCandidates = [];
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

		var pencilmarkMode = ((App)Application.Current).Preference.UIPreferences.DisplayCandidates;
		var usedCandidates = CandidateMap.Empty;
		var (controlAddingActions, overlapped, links) = (new AnimatedResultCollection(), new List<Conclusion>(), new List<ILinkViewNode>());

		// Iterate on each view node, and get their own corresponding controls.
		foreach (var viewNode in view)
		{
			(
				viewNode switch
				{
					CellViewNode c => () => ForCellNode(pane, c, controlAddingActions),
					IconViewNode i => () => ForIconNode(pane, i, controlAddingActions),
					CandidateViewNode c => () => onCandidateViewNode(c),
					HouseViewNode h => () => ForHouseNode(pane, h, controlAddingActions),
					ChuteViewNode c => () => ForChuteNode(pane, c, controlAddingActions),
					BabaGroupViewNode b => () => ForBabaGroupNode(pane, b, controlAddingActions),
					ILinkViewNode l => () => links.Add(l),
					_ => default(Action)
				}
			)?.Invoke();


			void onCandidateViewNode(CandidateViewNode c)
			{
				ForCandidateNode(pane, c, conclusions, out var o, controlAddingActions);
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
			ForConclusion(pane, conclusion, overlapped, controlAddingActions);

			usedCandidates.Add(conclusion.Candidate);
		}

		// Finally, iterate on links.
		// The links are special to be handled - they will create a list of line controls.
		// We should handle it at last.
		ForLinkNodes(pane, links.AsReadOnlySpan(), conclusions, controlAddingActions);

		foreach (var (animator, adder) in controlAddingActions)
		{
			(animator + adder)();
		}

		// Update property to get highlighted candidates.
		pane.ViewUnitUsedCandidates = usedCandidates;
	}


	private static partial void ForConclusion(SudokuPane sudokuPane, Conclusion conclusion, List<Conclusion> overlapped, AnimatedResultCollection animatedResults);
	private static partial void ForCellNode(SudokuPane sudokuPane, CellViewNode cellNode, AnimatedResultCollection animatedResults);
	private static partial void ForIconNode(SudokuPane sudokuPane, IconViewNode iconNode, AnimatedResultCollection animatedResults);
	private static partial void ForCandidateNode(SudokuPane sudokuPane, CandidateViewNode candidateNode, Conclusion[] conclusions, out Conclusion? overlapped, AnimatedResultCollection animatedResults);
	private static partial void ForCandidateNodeCore(ColorIdentifier id, Color color, Candidate candidate, SudokuPaneCell paneCellControl, AnimatedResultCollection animatedResults, bool isForConclusion = false, bool isForElimination = false, bool isOverlapped = false);
	private static partial void ForHouseNode(SudokuPane sudokuPane, HouseViewNode houseNode, AnimatedResultCollection animatedResults);
	private static partial void ForChuteNode(SudokuPane sudokuPane, ChuteViewNode chuteNode, AnimatedResultCollection animatedResults);
	private static partial void ForBabaGroupNode(SudokuPane sudokuPane, BabaGroupViewNode babaGroupNode, AnimatedResultCollection animatedResults);
	private static partial void ForLinkNodes(SudokuPane sudokuPane, ReadOnlySpan<ILinkViewNode> linkNodes, Conclusion[] conclusions, AnimatedResultCollection animatedResults);
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
		var controls = (
			from control in @this.OfType<FrameworkElement>()
			where control.Tag is nameof(DrawableFactory)
			select control
		).ToArray();
		foreach (var control in controls)
		{
			@this.Remove(control);
		}
	}
}
