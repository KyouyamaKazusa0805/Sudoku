namespace SudokuStudio.Views.Interaction;

/// <summary>
/// Defines a factory type that is used for creating a list of <see cref="FrameworkElement"/>
/// to display for highlighted cells, candidates and so on.
/// </summary>
/// <seealso cref="FrameworkElement"/>
internal static class ViewUnitFrameworkElementFactory
{
	/// <summary>
	/// Indicates the tag that is used to describe the control is only used for displaying highlighted elements in a <see cref="ViewUnit"/>.
	/// </summary>
	/// <seealso cref="ViewUnit"/>
	internal const string ViewUnitUIElementControlTag = $"{nameof(ViewUnitFrameworkElementFactory)}.{nameof(FrameworkElement)}";


	/// <summary>
	/// Try to get all possible <see cref="FrameworkElement"/>s that are candidate controls
	/// storing <see cref="ViewUnit"/>-displaying <see cref="FrameworkElement"/>s.
	/// </summary>
	/// <param name="targetPage">The target page.</param>
	/// <returns>
	/// A list of controls, whose <c>Chilren</c> property can be used for removing <see cref="ViewUnit"/>-displaying controls.
	/// </returns>
	/// <seealso cref="FrameworkElement"/>
	/// <seealso cref="ViewUnit"/>
	public static IEnumerable<FrameworkElement> GetViewUnitTargetParentControls(AnalyzePage targetPage)
	{
		foreach (var children in targetPage.SudokuPane._children)
		{
			// Stores cell/candidate view nodes.
			yield return children.MainGrid;
		}

		// Stores house view nodes.
		yield return targetPage.SudokuPane.MainGrid;

		// TODO: Other controls.
	}

	/// <summary>
	/// Removes all possible controls that are used for displaying elements in a <see cref="ViewUnit"/>.
	/// </summary>
	/// <param name="targetPage">The target page.</param>
	/// <seealso cref="ViewUnit"/>
	public static void RemoveViewUnitControls(AnalyzePage targetPage)
	{
		foreach (var targetControl in GetViewUnitTargetParentControls(targetPage))
		{
			switch (targetControl)
			{
				case GridLayout { Children: var children }:
				{
					children.RemoveAllViewUnitControls();
					break;
				}
			}
		}
	}

	/// <summary>
	/// Adds a list of <see cref="FrameworkElement"/>s that are used for displaying highlight elements in a <see cref="ViewUnit"/>.
	/// </summary>
	/// <param name="targetPage">The target page.</param>
	/// <param name="viewUnit">The view unit that you want to display.</param>
	/// <seealso cref="FrameworkElement"/>
	/// <seealso cref="ViewUnit"/>
	public static void AddViewUnitControls(AnalyzePage targetPage, ViewUnit viewUnit)
	{
		var (view, conclusions) = viewUnit;
		var overlapped = new List<Conclusion>();
		foreach (var viewNode in view.BasicNodes)
		{
			switch (viewNode)
			{
				case CellViewNode c:
				{
					CreateForCellViewNode(targetPage, c);
					break;
				}
				case CandidateViewNode c:
				{
					CreateForCandidateViewNode(targetPage, c, conclusions, out var o);
					if (o is { } currentOverlappedConclusion)
					{
						overlapped.Add(currentOverlappedConclusion);
					}

					break;
				}
				case HouseViewNode h:
				{
					CreateForHouseViewNode(targetPage, h);
					break;
				}
			}
		}

		foreach (var conclusion in conclusions)
		{
			CreateForConclusion(targetPage, conclusion, overlapped);
		}
	}

	private static void CreateForConclusion(AnalyzePage targetPage, Conclusion conclusion, List<Conclusion> overlapped)
	{
		var (type, candidate) = conclusion;
		var paneCellControl = targetPage.SudokuPane._children[candidate / 9];
		if (paneCellControl is null)
		{
			return;
		}

		CreateForCandidateViewNodeCore(
			IdentifierConversion.GetColor(
				type switch
				{
					Assignment => DisplayColorKind.Assignment,
					Elimination => overlapped.Exists(conclusion => conclusion.Candidate == candidate) switch
					{
						true => DisplayColorKind.Cannibalism,
						false => DisplayColorKind.Elimination
					}
				}
			),
			candidate,
			paneCellControl
		);
	}

	private static void CreateForCellViewNode(AnalyzePage targetPage, CellViewNode cellNode)
	{
		var (id, cell) = cellNode;
		var paneCellControl = targetPage.SudokuPane._children[cell];
		if (paneCellControl is null)
		{
			return;
		}

		var control = new Border
		{
			Background = new SolidColorBrush(IdentifierConversion.GetColor(id)),
			BorderThickness = new(0),
			Tag = ViewUnitUIElementControlTag,
			Opacity = .15
		};

		GridLayout.SetRowSpan(control, 3);
		GridLayout.SetColumnSpan(control, 3);
		Canvas.SetZIndex(control, -1);

		paneCellControl.MainGrid.Children.Add(control);
	}

	private static void CreateForCandidateViewNode(AnalyzePage targetPage, CandidateViewNode candidateNode, ImmutableArray<Conclusion> conclusions, out Conclusion? overlapped)
	{
		overlapped = null;

		var (id, candidate) = candidateNode;
		var cell = candidate / 9;
		var paneCellControl = targetPage.SudokuPane._children[cell];
		if (paneCellControl is null)
		{
			return;
		}

		if (conclusions.ConflictWith(candidate, out var conclusionOverlapped))
		{
			// This will be rendered as cannibalism or assignment overlapping cases. We may not handle on this here.
			overlapped = conclusionOverlapped;
			return;
		}

		CreateForCandidateViewNodeCore(IdentifierConversion.GetColor(id), candidate, paneCellControl);
	}

	private static void CreateForCandidateViewNodeCore(Color color, int candidate, SudokuPaneCell paneCellControl)
	{
		var (width, height) = paneCellControl.ActualSize / 3F * (float)paneCellControl.BasePane.HighlightCandidateCircleScale;
		var control = new Ellipse
		{
			Width = width,
			Height = height,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Fill = new SolidColorBrush(color),
			Tag = ViewUnitUIElementControlTag
		};

		var digit = candidate % 9;
		GridLayout.SetRow(control, digit / 3);
		GridLayout.SetColumn(control, digit % 3);
		Canvas.SetZIndex(control, -1);

		paneCellControl.MainGrid.Children.Add(control);
	}

	private static void CreateForHouseViewNode(AnalyzePage targetPage, HouseViewNode houseNode)
	{
		var (id, house) = houseNode;
		var gridControl = targetPage.SudokuPane.MainGrid;
		if (gridControl is null)
		{
			return;
		}

		var control = new Border
		{
			Background = new SolidColorBrush(IdentifierConversion.GetColor(id)),
			BorderThickness = new(0),
			Tag = ViewUnitUIElementControlTag,
			Opacity = .15
		};

		var (row, column, rowSpan, columnSpan) = house switch
		{
			>= 0 and < 9 => (house / 3 * 3 + 2, house % 3 * 3 + 2, 3, 3),
			>= 9 and < 18 => (house - 9 + 2, 2, 1, 9),
			>= 18 and < 27 => (2, house - 18 + 2, 9, 1),
			_ => throw new InvalidOperationException(nameof(house))
		};

		GridLayout.SetRow(control, row);
		GridLayout.SetColumn(control, column);
		GridLayout.SetRowSpan(control, rowSpan);
		GridLayout.SetColumnSpan(control, columnSpan);

		gridControl.Children.Add(control);
	}
}

/// <include file='../../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Removes all possible <see cref="FrameworkElement"/>s that is used for displaying elements in a <see cref="ViewUnit"/>.
	/// </summary>
	/// <param name="this">The collection.</param>
	public static void RemoveAllViewUnitControls(this UIElementCollection @this)
	{
		var gathered = new List<FrameworkElement>();
		foreach (var element in @this.OfType<FrameworkElement>())
		{
			if (element.Tag is ViewUnitFrameworkElementFactory.ViewUnitUIElementControlTag)
			{
				gathered.Add(element);
			}
		}

		foreach (var element in gathered)
		{
			@this.Remove(element);
		}
	}

	/// <summary>
	/// <para>Fast determines whether the specified conclusion list contains the specified candidate.</para>
	/// <para>This method is used for checking cannibalisms.</para>
	/// </summary>
	/// <param name="conclusions">The conclusion collection.</param>
	/// <param name="candidate">The candidate to be determined.</param>
	/// <param name="conclusion">The overlapped result.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool ConflictWith(this ImmutableArray<Conclusion> conclusions, int candidate, [NotNullWhen(true)] out Conclusion? conclusion)
	{
		foreach (var current in conclusions)
		{
			if (current.Candidate == candidate)
			{
				conclusion = current;
				return true;
			}
		}

		conclusion = null;
		return false;
	}
}
