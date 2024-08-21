namespace SudokuStudio.Drawing;

internal partial class DrawableFactory
{
	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for conclusions.
	/// </summary>
	/// <param name="sudokuPane">
	/// The target sudoku pane.
	/// This instance provides with user-defined customized properties used for displaying elements, e.g. background color.
	/// </param>
	/// <param name="conclusion">The conclusion to be displayed.</param>
	/// <param name="overlapped">A collection that stores for overlapped candidates.</param>
	/// <param name="animatedResults">A list that stores the final actions to adding controls into the sudoku pane.</param>
	private static partial void ForConclusion(SudokuPane sudokuPane, Conclusion conclusion, List<Conclusion> overlapped, AnimatedResultCollection animatedResults)
	{
		var (type, candidate) = conclusion;
		if (sudokuPane._children[candidate / 9] is not { } paneCellControl)
		{
			return;
		}

		var isOverlapped = overlapped.Exists(conclusion => conclusion.Candidate == candidate);
		var id = (type, isOverlapped) switch
		{
			(Assignment, true) => ColorIdentifierKind.OverlappedAssignment,
			(Assignment, _) => ColorIdentifierKind.Assignment,
			(Elimination, true) => ColorIdentifierKind.Cannibalism,
			_ => (ColorIdentifier)ColorIdentifierKind.Elimination
		};
		ForCandidateNodeCore(
			id,
			IdentifierConversion.GetColor(id),
			candidate,
			paneCellControl,
			animatedResults,
			true,
			conclusion.ConclusionType == Elimination,
			isOverlapped
		);
	}

	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for <see cref="CellViewNode"/>.
	/// </summary>
	/// <param name="sudokuPane">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="cellNode">The node to be displayed.</param>
	/// <param name="animatedResults">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='animatedResults']"/>
	/// </param>
	/// <seealso cref="CellViewNode"/>
	private static partial void ForCellNode(SudokuPane sudokuPane, CellViewNode cellNode, AnimatedResultCollection animatedResults)
	{
		var (id, cell) = cellNode;
		if (sudokuPane._children[cell] is not { } paneCellControl)
		{
			return;
		}

		var control = new Border
		{
			BorderThickness = new(0),
			Tag = nameof(DrawableFactory),
			Opacity = 0,
			Background = new SolidColorBrush(IdentifierConversion.GetColor(id)),
			CornerRadius = sudokuPane.CellsInnerCornerRadius,
			Margin = sudokuPane.CellsInnerPadding
		};

		GridLayout.SetRowSpan(control, 3);
		GridLayout.SetColumnSpan(control, 3);
		Canvas.SetZIndex(control, -2);

		if (sudokuPane.EnableAnimationFeedback)
		{
			control.OpacityTransition = new();
		}

		animatedResults.Add(
			(
				() => paneCellControl.MainGrid.Children.Add(control),
				() => control.Opacity = (double)sudokuPane.HighlightBackgroundOpacity
			)
		);
	}

	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for <see cref="IconViewNode"/>.
	/// </summary>
	/// <param name="sudokuPane">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="iconNode">The node to be displayed.</param>
	/// <param name="animatedResults">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='animatedResults']"/>
	/// </param>
	/// <seealso cref="IconViewNode"/>
	private static partial void ForIconNode(SudokuPane sudokuPane, IconViewNode iconNode, AnimatedResultCollection animatedResults)
	{
		var id = iconNode.Identifier;
		var cell = iconNode.Cell;
		if (sudokuPane._children[cell] is not { } paneCellControl)
		{
			return;
		}

		var control = create(
			iconNode switch
			{
				CircleViewNode => static () => new CircleRing(),
				CrossViewNode => static () => new Cross(),
				TriangleViewNode => static () => new Triangle(),
				DiamondViewNode => static () => new Diamond()
			}
		);

		GridLayout.SetRowSpan(control, 3);
		GridLayout.SetColumnSpan(control, 3);
		Canvas.SetZIndex(control, -2);

		if (sudokuPane.EnableAnimationFeedback)
		{
			control.OpacityTransition = new();
		}

		animatedResults.Add((() => paneCellControl.MainGrid.Children.Add(control), () => control.Opacity = 1));


		Control create(Func<Control> instanceCreator)
		{
			var result = instanceCreator();
			result.BorderThickness = new(0);
			result.Tag = nameof(DrawableFactory);
			result.Background = new SolidColorBrush(IdentifierConversion.GetColor(id));
			result.Opacity = 0;
			result.Margin = result switch { Star or Triangle or Diamond => new(3, 0, 0, 0), _ => new(6) };
			return result;
		}
	}

	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for <see cref="CandidateViewNode"/>.
	/// </summary>
	/// <param name="sudokuPane">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="candidateNode">
	/// <inheritdoc cref="ForCellNode(SudokuPane, CellViewNode, AnimatedResultCollection)" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="conclusions">Indicates the conclusion collection. The argument is used for checking cannibalism.</param>
	/// <param name="overlapped">
	/// Indicates the collection that returns a possible <see cref="Conclusion"/> value indicating
	/// what candidate conflicts with the current node while displaying. If no overlapped conclusion, <see langword="null"/>.
	/// </param>
	/// <param name="animatedResults">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='animatedResults']"/>
	/// </param>
	/// <seealso cref="CandidateViewNode"/>
	private static partial void ForCandidateNode(
		SudokuPane sudokuPane,
		CandidateViewNode candidateNode,
		Conclusion[] conclusions,
		out Conclusion? overlapped,
		AnimatedResultCollection animatedResults
	)
	{
		overlapped = null;

		var (id, candidate) = candidateNode;
		var cell = candidate / 9;
		if (sudokuPane._children[cell] is not { } paneCellControl)
		{
			return;
		}

		if (conclusions.ConflictWith(candidate, out var conclusionOverlapped))
		{
			// This will be rendered as cannibalism or assignment overlapping cases. We may not handle on this here.
			overlapped = conclusionOverlapped;
			return;
		}

		ForCandidateNodeCore(id, IdentifierConversion.GetColor(id), candidate, paneCellControl, animatedResults);
	}

	/// <summary>
	/// The core method called by <see cref="ForCandidateNode(SudokuPane, CandidateViewNode, Conclusion[], out Conclusion?, AnimatedResultCollection)"/>.
	/// </summary>
	/// <param name="id">The color identifier.</param>
	/// <param name="color">The color to be used on rendering.</param>
	/// <param name="candidate">The candidate to be rendered.</param>
	/// <param name="paneCellControl">The pane cell control that stores the rendered control.</param>
	/// <param name="animatedResults">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='animatedResults']"/>
	/// </param>
	/// <param name="isForConclusion">Indicates whether the operation draws for a conclusion.</param>
	/// <param name="isForElimination">Indicates whether the operation draws for an elimination.</param>
	/// <param name="isOverlapped">Indicates whether the operation draws for an overlapped conclusion.</param>
	/// <seealso cref="ForCandidateNode(SudokuPane, CandidateViewNode, Conclusion[], out Conclusion?, AnimatedResultCollection)"/>
	private static partial void ForCandidateNodeCore(
		ColorIdentifier id,
		Color color,
		Candidate candidate,
		SudokuPaneCell paneCellControl,
		AnimatedResultCollection animatedResults,
		bool isForConclusion,
		bool isForElimination,
		bool isOverlapped
	)
	{
		const float epsilon = 1E-2F;
		if (paneCellControl is not
			{
				ActualSize: var size,
				BasePane:
				{
					Width: var fallbackSize, // WinUI issue makes 'paneCellControl.ActualSize' equal to 0 in help message window.
					HighlightCandidateCircleScale: var highlightScale,
					EnableAnimationFeedback: var enableAnimation,
					CandidateViewNodeDisplayMode: var candidateDisplayMode,
					EliminationDisplayMode: var eliminationDisplayMode,
					AssignmentDisplayMode: var assignmentDisplayMode
				}
			})
		{
			return;
		}

		var converter = new RxCyConverter();
		var resultSize = size is var (x, y) && (x.NearlyEquals(0, epsilon) || y.NearlyEquals(0, epsilon)) && fallbackSize / 9 is var f
			? new((float)f, (float)f)
			: size;
		var (width, height) = resultSize / 3F * (float)highlightScale;
		var control = (isForConclusion, isForElimination, candidateDisplayMode, eliminationDisplayMode, assignmentDisplayMode) switch
		{
			(true, true, _, EliminationDisplay.CircleSolid, _) or (true, false, _, _, AssignmentDisplay.CircleSolid) => new Ellipse
			{
				Width = width,
				Height = height,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Fill = getFillBrush(color),
				Tag = nameof(DrawableFactory),
				Opacity = enableAnimation ? 0 : 1
			},
			(true, true, _, EliminationDisplay.CircleHollow, _) or (true, false, _, _, AssignmentDisplay.CircleHollow) => new Ellipse
			{
				Width = width,
				Height = height,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Stroke = new SolidColorBrush(color),
				StrokeThickness = (width + height) / 2 * 3 / 20,
				Tag = nameof(DrawableFactory),
				Opacity = enableAnimation ? 0 : 1
			},
			(true, true, _, EliminationDisplay.Cross or EliminationDisplay.Slash or EliminationDisplay.Backslash, _) => new Cross
			{
				Width = width,
				Height = height,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Background = new SolidColorBrush(color),
				StrokeThickness = (width + height) / 2 * 3 / 20,
				Tag = nameof(DrawableFactory),
				Opacity = enableAnimation ? 0 : 1,
				ForwardLineVisibility = eliminationDisplayMode switch
				{
					EliminationDisplay.Cross or EliminationDisplay.Slash => Visibility.Visible,
					_ => Visibility.Collapsed
				},
				BackwardLineVisibility = eliminationDisplayMode switch
				{
					EliminationDisplay.Cross or EliminationDisplay.Backslash => Visibility.Visible,
					_ => Visibility.Collapsed
				}
			},
			(true, _, _, _, _) or (_, _, CandidateViewNodeDisplay.CircleSolid, _, _) => new Ellipse
			{
				Width = width,
				Height = height,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Fill = getFillBrush(color),
				Tag = nameof(DrawableFactory),
				Opacity = enableAnimation ? 0 : 1
			},
			(_, _, CandidateViewNodeDisplay.CircleHollow, _, _) => new Ellipse
			{
				Width = width,
				Height = height,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Stroke = new SolidColorBrush(color),
				StrokeThickness = (width + height) / 2 * 3 / 20,
				Tag = nameof(DrawableFactory),
				Opacity = enableAnimation ? 0 : 1
			},
			(_, _, CandidateViewNodeDisplay.SquareHollow, _, _) => new Rectangle
			{
				Width = width,
				Height = height,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Stroke = new SolidColorBrush(color),
				StrokeThickness = (width + height) / 2 * 3 / 20,
				Tag = nameof(DrawableFactory),
				Opacity = enableAnimation ? 0 : 1
			},
			(_, _, CandidateViewNodeDisplay.SquareSolid, _, _) => new Rectangle
			{
				Width = width,
				Height = height,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Fill = getFillBrush(color),
				Tag = nameof(DrawableFactory),
				Opacity = enableAnimation ? 0 : 1,
			},
			(_, _, CandidateViewNodeDisplay.RoundedRectangleHollow, _, _) => new Rectangle
			{
				Width = width,
				Height = height,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Fill = new SolidColorBrush(color),
				Tag = nameof(DrawableFactory),
				Opacity = enableAnimation ? 0 : 1,
				RadiusX = width / 3,
				RadiusY = height / 3
			},
			(_, _, CandidateViewNodeDisplay.RoundedRectangleSolid, _, _) => new Rectangle
			{
				Width = width,
				Height = height,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Fill = getFillBrush(color),
				Tag = nameof(DrawableFactory),
				Opacity = enableAnimation ? 0 : 1,
				RadiusX = width / 3,
				RadiusY = height / 3
			},
			_ => default(FrameworkElement)!
		};

		var digit = candidate % 9;
		GridLayout.SetRow(control, digit / 3);
		GridLayout.SetColumn(control, digit % 3);
		Canvas.SetZIndex(control, -1);

		if (enableAnimation)
		{
			control.OpacityTransition = new();
		}

		animatedResults.Add((() => paneCellControl.MainGrid.Children.Add(control), () => control.Opacity = 1));


		static Brush getFillBrush(Color color)
#if !true
			=> new RadialGradientBrush
			{
				Center = new(.5, .5),
				GradientOrigin = new(.5, .5),
				RadiusX = .64,
				RadiusY = .64,
				SpreadMethod = GradientSpreadMethod.Pad,
				MappingMode = BrushMappingMode.RelativeToBoundingBox,
				GradientStops = { new() { Color = color }, new() { Color = Colors.Transparent, Offset = 1.5 } }
			};
#else
			=> new SolidColorBrush(color);
#endif
	}

	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for <see cref="HouseViewNode"/>.
	/// </summary>
	/// <param name="sudokuPane">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="houseNode">
	/// <inheritdoc cref="ForCellNode(SudokuPane, CellViewNode, AnimatedResultCollection)" path="/param[@name='cellNode']"/>
	/// </param>
	/// <param name="animatedResults">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='animatedResults']"/>
	/// </param>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="houseNode"/> stores invalid data of property <see cref="HouseViewNode.House"/>.
	/// </exception>
	/// <seealso cref="HouseViewNode"/>
	private static partial void ForHouseNode(SudokuPane sudokuPane, HouseViewNode houseNode, AnimatedResultCollection animatedResults)
	{
		var (id, house) = houseNode;
		if (sudokuPane.MainGrid is not { } gridControl)
		{
			return;
		}

		var (row, column, rowSpan, columnSpan) = house switch
		{
			>= 0 and < 9 => (house / 3 * 3 + 2, house % 3 * 3 + 2, 3, 3),
			>= 9 and < 18 => (house - 9 + 2, 2, 1, 9),
			>= 18 and < 27 => (2, house - 18 + 2, 9, 1),
			_ => Throw<(int, int, int, int)>(house, 27)
		};

		var control = new Border
		{
			Background = new SolidColorBrush(IdentifierConversion.GetColor(id)),
			Tag = nameof(DrawableFactory),
			Opacity = sudokuPane.EnableAnimationFeedback ? 0 : (double)sudokuPane.HighlightBackgroundOpacity,
			Margin = house switch
			{
				>= 0 and < 9 => new(12),
				>= 9 and < 18 => new(6, 12, 6, 12),
				>= 18 and < 27 => new(12, 6, 12, 6),
				_ => Throw<Thickness>(house, 27)
			},
			CornerRadius = house switch { >= 0 and < 9 => new(12), >= 9 and < 27 => new(18), _ => Throw<CornerRadius>(house, 27) },
			BorderThickness = new(0)
		};

		GridLayout.SetRow(control, row);
		GridLayout.SetColumn(control, column);
		GridLayout.SetRowSpan(control, rowSpan);
		GridLayout.SetColumnSpan(control, columnSpan);
		Canvas.SetZIndex(control, -3);

		if (sudokuPane.EnableAnimationFeedback)
		{
			control.OpacityTransition = new();
		}

		animatedResults.Add((() => gridControl.Children.Add(control), () => control.Opacity = (double)sudokuPane.HighlightBackgroundOpacity));
	}

	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for <see cref="ChuteViewNode"/>.
	/// </summary>
	/// <param name="sudokuPane">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="chuteNode">
	/// <inheritdoc cref="ForCellNode(SudokuPane, CellViewNode, AnimatedResultCollection)" path="/param[@name='cellNode']"/>
	/// </param>
	/// <param name="animatedResults">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='animatedResults']"/>
	/// </param>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="chuteNode"/> stores invalid data of property <see cref="ChuteViewNode.ChuteIndex"/>.
	/// </exception>
	/// <seealso cref="ChuteViewNode"/>
	private static partial void ForChuteNode(SudokuPane sudokuPane, ChuteViewNode chuteNode, AnimatedResultCollection animatedResults)
	{
		if (sudokuPane.MainGrid is not { } gridControl)
		{
			return;
		}

		var (id, chute) = chuteNode;
		var (row, column, rowSpan, columnSpan) = chute switch
		{
			>= 0 and < 3 => (chute * 3 + 2, 2, 3, 9),
			>= 3 and < 6 => (2, (chute - 3) * 3 + 2, 9, 3),
			_ => Throw<(int, int, int, int)>(chute, 6)
		};

		var control = new Border
		{
			Background = new SolidColorBrush(IdentifierConversion.GetColor(id)),
			Tag = nameof(DrawableFactory),
			Opacity = sudokuPane.EnableAnimationFeedback ? 0 : (double)sudokuPane.HighlightBackgroundOpacity,
			Margin = chute switch { >= 0 and < 3 => new(6, 12, 6, 12), >= 3 and < 6 => new(12, 6, 12, 6), _ => Throw<Thickness>(chute, 6) },
			CornerRadius = new(18),
			BorderThickness = new(0)
		};

		GridLayout.SetRow(control, row);
		GridLayout.SetColumn(control, column);
		GridLayout.SetRowSpan(control, rowSpan);
		GridLayout.SetColumnSpan(control, columnSpan);
		Canvas.SetZIndex(control, -4);

		if (sudokuPane.EnableAnimationFeedback)
		{
			control.OpacityTransition = new();
		}

		animatedResults.Add((() => gridControl.Children.Add(control), () => control.Opacity = (double)sudokuPane.HighlightBackgroundOpacity));
	}

	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for <see cref="BabaGroupViewNode"/>.
	/// </summary>
	/// <param name="sudokuPane">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="babaGroupNode">
	/// <inheritdoc cref="ForCellNode(SudokuPane, CellViewNode, AnimatedResultCollection)" path="/param[@name='cellNode']"/>
	/// </param>
	/// <param name="animatedResults">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='animatedResults']"/>
	/// </param>
	/// <seealso cref="BabaGroupViewNode"/>
	private static partial void ForBabaGroupNode(SudokuPane sudokuPane, BabaGroupViewNode babaGroupNode, AnimatedResultCollection animatedResults)
	{
		var (id, cell, @char) = babaGroupNode;
		if (sudokuPane._children[cell] is not { } paneCellControl)
		{
			return;
		}

		var control = new Border
		{
			BorderThickness = new(0),
			Tag = nameof(DrawableFactory),
			Opacity = sudokuPane.EnableAnimationFeedback ? 0 : (double)sudokuPane.HighlightBackgroundOpacity,
			Child = new TextBlock
			{
				Text = @char.ToString(),
				FontSize = PencilmarkTextConversion.GetFontSizeSimple(sudokuPane.ApproximateCellWidth, sudokuPane.BabaGroupLabelFontScale) * 1.618,
				FontFamily = sudokuPane.BabaGroupLabelFont,
				Foreground = new SolidColorBrush(sudokuPane.BabaGroupLabelColor),
				FontWeight = FontWeights.Bold,
				FontStyle = FontStyle.Italic,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				TextAlignment = TextAlignment.Center
			}
		};

		GridLayout.SetRowSpan(control, 3);
		GridLayout.SetColumnSpan(control, 3);
		Canvas.SetZIndex(control, -1);

		if (sudokuPane.EnableAnimationFeedback)
		{
			control.OpacityTransition = new();
		}

		animatedResults.Add((() => paneCellControl.MainGrid.Children.Add(control), () => control.Opacity = 1));
	}

	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for <see cref="ChainLinkViewNode"/>s.
	/// </summary>
	/// <param name="sudokuPane">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="linkNodes">
	/// <inheritdoc cref="ForCellNode(SudokuPane, CellViewNode, AnimatedResultCollection)" path="/param[@name='cellNode']"/>
	/// </param>
	/// <param name="conclusions">Indicates the conclusions. The value is used for appending links between tail node and conclusion.</param>
	/// <param name="animatedResults">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion}, AnimatedResultCollection)" path="/param[@name='animatedResults']"/>
	/// </param>
	/// <remarks>
	/// This method is special: We should handle all <see cref="ChainLinkViewNode"/>s together.
	/// </remarks>
	private static partial void ForLinkNodes(
		SudokuPane sudokuPane,
		ReadOnlySpan<ILinkViewNode> linkNodes,
		Conclusion[] conclusions,
		AnimatedResultCollection animatedResults
	)
	{
		if (sudokuPane.MainGrid is not { } gridControl)
		{
			return;
		}

		foreach (var control in new PathCreator(sudokuPane, new(gridControl), conclusions).CreateLinks(linkNodes))
		{
			GridLayout.SetRow(control, 2);
			GridLayout.SetColumn(control, 2);
			GridLayout.SetRowSpan(control, 9);
			GridLayout.SetColumnSpan(control, 9);
			Canvas.SetZIndex(control, -1);

			if (sudokuPane.EnableAnimationFeedback)
			{
				control.OpacityTransition = new();
			}

			animatedResults.Add((() => gridControl.Children.Add(control), () => control.Opacity = 1));
		}
	}
}

/// <summary>
/// Extracted type that creates the <see cref="Path"/> instances.
/// </summary>
/// <param name="Pane">Indicates the sudoku pane control.</param>
/// <param name="Converter">Indicates the position converter.</param>
/// <param name="Conclusions">Indicates the conclusions of the whole chain.</param>
/// <seealso cref="Path"/>
file sealed record PathCreator(SudokuPane Pane, SudokuPanePositionConverter Converter, Conclusion[] Conclusions)
{
	/// <summary>
	/// Indicates the rotate angle (45 degrees).
	/// </summary>
	private const double RotateAngle = PI / 4;

	/// <summary>
	/// Indicates the square root of 2.
	/// </summary>
	private const double SqrtOf2 = 1.4142135623730951;


	/// <summary>
	/// Creates a list of <see cref="Shape"/> instances via the specified link view nodes.
	/// </summary>
	/// <param name="nodes">The link view nodes.</param>
	/// <returns>A <see cref="Shape"/> instance.</returns>
	public ReadOnlySpan<Shape> CreateLinks(ReadOnlySpan<ILinkViewNode> nodes)
	{
		// Iterate on each inference to draw the links and grouped nodes (if so).
		var ((ow, oh), _) = Converter;
		var ((cellSize, _), _, _, _) = Converter;
		var points = getPoints(nodes);
		var drawnGroupedNodes = new List<CandidateMap>();
		var result = new List<Shape>();
		foreach (var node in nodes)
		{
			var (_, start, end) = node;
			var isStrong = node switch { ChainLinkViewNode { IsStrongLink: var i } => i, _ => default(bool?) };
			var dashArray = node switch
			{
				ChainLinkViewNode { IsStrongLink: var i } => [.. i ? Pane.StrongLinkDashStyle : Pane.WeakLinkDashStyle],
				_ => default(DoubleCollection)
			};

			// Find two candidates with a minimal distance.
			var (distance, pt1, pt2) = (double.MaxValue, default(Point), default(Point));
			if (node.ElementType == typeof(CandidateMap))
			{
				foreach (var s in (CandidateMap)start)
				{
					var tempPoint1 = Converter.GetPosition(s);
					foreach (var e in (CandidateMap)end)
					{
						var tempPoint2 = Converter.GetPosition(e);
						var d = tempPoint1.DistanceTo(tempPoint2);
						if (d <= distance)
						{
							(distance, pt1, pt2) = (d, tempPoint1, tempPoint2);
						}
					}
				}
			}
			else if (node.ElementType == typeof(Cell))
			{
				pt1 = Converter.GetPosition((Cell)start * 9 + 4);
				pt2 = Converter.GetPosition((Cell)end * 9 + 4);
				distance = pt1.DistanceTo(pt2);
			}

			// If the distance of two points is lower than the one of two adjacent candidates,
			// the link will be ignored to be drawn because of too narrow.
			var ((pt1x, pt1y), (pt2x, pt2y)) = (pt1, pt2);
			if (distance <= cellSize * SqrtOf2 || distance <= cellSize * SqrtOf2)
			{
				goto DrawGroupNodeOutlines;
			}

			var (deltaX, deltaY) = (pt2.X - pt1.X, pt2.Y - pt1.Y);
			var alpha = Atan2(deltaY, deltaX);
			adjust(pt1, pt2, out var p1, out _, alpha, cellSize);

			// Check whether the link will pass through a candidate used in pattern.
			var linkPassesThroughUsedCandidates = false;
			var (dx1, dy1) = (deltaX, deltaY);
			foreach (var point in points)
			{
				if (point == pt1 || point == pt2)
				{
					// Skip itself.
					continue;
				}

				var (dx2, dy2) = (point.X - p1.X, point.Y - p1.Y);
				if (Sign(dx1) == Sign(dx2) && Sign(dy1) == Sign(dy2)
					&& Abs(dx2) <= Abs(dx1) && Abs(dy2) <= Abs(dy1)
					&& (dx1 == 0 || dy1 == 0 || (dx1 / dy1).NearlyEquals(dx2 / dy2, epsilon: 1E-1))
					&& node.ElementType == typeof(CandidateMap))
				{
					linkPassesThroughUsedCandidates = true;
					break;
				}
			}

			// Now cut the link.
			if (node.ElementType == typeof(CandidateMap))
			{
				cut(ref pt1, ref pt2, cellSize);
			}

			if (linkPassesThroughUsedCandidates)
			{
				// The end points are rotated 45 degrees (counterclockwise for the start point, clockwise for the end point).
				const double bezierLength = 30.0;
				var oldPt1 = new Point(pt1x, pt1y);
				var oldPt2 = new Point(pt2x, pt2y);
				rotate(oldPt1, ref pt1, -RotateAngle);
				rotate(oldPt2, ref pt2, RotateAngle);

				var interim1Alpha = alpha - RotateAngle;
				var bx1 = pt1.X + bezierLength * Cos(interim1Alpha);
				var by1 = pt1.Y + bezierLength * Sin(interim1Alpha);
				var interim2Alpha = alpha + RotateAngle;
				var bx2 = pt2.X - bezierLength * Cos(interim2Alpha);
				var by2 = pt2.Y - bezierLength * Sin(interim2Alpha);
				correctOffsetOfPoint(ref pt1, ow, oh);
				correctOffsetOfPoint(ref pt2, ow, oh);
				correctOffsetOfDouble(ref bx1, ow);
				correctOffsetOfDouble(ref bx2, oh);
				correctOffsetOfDouble(ref by1, ow);
				correctOffsetOfDouble(ref by2, oh);
				result.Add(
					new Path
					{
						Stroke = new SolidColorBrush(Pane.LinkColor),
						StrokeThickness = (double)Pane.ChainStrokeThickness,
						StrokeDashArray = dashArray,
						Data = new GeometryGroup
						{
							Children = [
								new PathGeometry
								{
									Figures = [
										new PathFigure
										{
											StartPoint = pt1,
											IsClosed = false,
											IsFilled = false,
											Segments = [new BezierSegment { Point1 = new(bx1, by1), Point2 = new(bx2, by2), Point3 = pt2 }]
										}
									]
								}
							]
						},
						Tag = nameof(DrawableFactory),
						Opacity = Pane.EnableAnimationFeedback ? 0 : 1
					}
				);
				result.Add(
					new Path
					{
						Stroke = new SolidColorBrush(Pane.LinkColor),
						StrokeThickness = (double)Pane.ChainStrokeThickness,
						Data = new GeometryGroup { Children = ArrowCap(new(bx2, by2), pt2) },
						Tag = nameof(DrawableFactory)
					}
				);
			}
			else
			{
				// Draw the link.
				correctOffsetOfPoint(ref pt1, ow, oh);
				correctOffsetOfPoint(ref pt2, ow, oh);
				result.Add(
					new Path
					{
						Stroke = new SolidColorBrush(Pane.LinkColor),
						StrokeThickness = (double)Pane.ChainStrokeThickness,
						StrokeDashArray = dashArray,
						Data = new GeometryGroup { Children = [new LineGeometry { StartPoint = pt1, EndPoint = pt2 }] },
						Tag = nameof(DrawableFactory),
						Opacity = Pane.EnableAnimationFeedback ? 0 : 1
					}
				);

				// Append arrow cap.
				if (node.ElementType == typeof(CandidateMap))
				{
					result.Add(
						new Path
						{
							Stroke = new SolidColorBrush(Pane.LinkColor),
							StrokeThickness = (double)Pane.ChainStrokeThickness,
							Data = new GeometryGroup { Children = ArrowCap(pt1, pt2) },
							Tag = nameof(DrawableFactory),
							Opacity = Pane.EnableAnimationFeedback ? 0 : 1
						}
					);
				}
			}

		DrawGroupNodeOutlines:
			if (node.ElementType == typeof(CandidateMap))
			{
				// If the start node or end node is a grouped node, we should append a rectangle to highlight it.
				var (s, e) = ((CandidateMap)start, (CandidateMap)end);
				if (s.Count != 1 && !drawnGroupedNodes.Contains(s))
				{
					drawnGroupedNodes.AddRef(in s);
					result.Add(drawRectangle(in s));
				}
				if (e.Count != 1 && !drawnGroupedNodes.Contains(e))
				{
					drawnGroupedNodes.AddRef(in e);
					result.Add(drawRectangle(in e));
				}
			}


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void adjust(Point pt1, Point pt2, out Point p1, out Point p2, double alpha, double cs)
			{
				if (node.ElementType == typeof(CandidateMap))
				{
					(p1, p2, var tempDelta) = (pt1, pt2, cs / 2);
					var (px, py) = (tempDelta * Cos(alpha), tempDelta * Sin(alpha));
					p1.X += px;
					p1.Y += py;
					p2.X -= px;
					p2.Y -= py;
				}
				else
				{
					(p1, p2) = (pt1, pt2);
				}
			}
		}
		return result.AsReadOnlySpan();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void rotate(Point pt1, ref Point pt2, double angle)
		{
			// Translate 'pt2' to (0, 0).
			pt2.X -= pt1.X;
			pt2.Y -= pt1.Y;

			// Rotate.
			var (sinAngle, cosAngle, (xAct, yAct)) = (Sin(angle), Cos(angle), pt2);
			pt2.X = xAct * cosAngle - yAct * sinAngle;
			pt2.Y = xAct * sinAngle + yAct * cosAngle;
			pt2.X += pt1.X;
			pt2.Y += pt1.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void cut(ref Point pt1, ref Point pt2, double cs)
		{
			var ((pt1x, pt1y), (pt2x, pt2y)) = (pt1, pt2);
			var slope = Abs((pt2y - pt1y) / (pt2x - pt1x));
			var (x, y) = (cs / Sqrt(1 + slope * slope), cs * Sqrt(slope * slope / (1 + slope * slope)));
			if (pt1y > pt2y && pt1x.NearlyEquals(pt2x))
			{
				pt1.Y -= cs / 2;
				pt2.Y += cs / 2;
			}
			else if (pt1y < pt2y && pt1x.NearlyEquals(pt2x))
			{
				pt1.Y += cs / 2;
				pt2.Y -= cs / 2;
			}
			else if (pt1y.NearlyEquals(pt2y) && pt1x > pt2x)
			{
				pt1.X -= cs / 2;
				pt2.X += cs / 2;
			}
			else if (pt1y.NearlyEquals(pt2y) && pt1x < pt2x)
			{
				pt1.X += cs / 2;
				pt2.X -= cs / 2;
			}
			else if (pt1y > pt2y && pt1x > pt2x)
			{
				pt1.X -= x / 2;
				pt1.Y -= y / 2;
				pt2.X += x / 2;
				pt2.Y += y / 2;
			}
			else if (pt1y > pt2y && pt1x < pt2x)
			{
				pt1.X += x / 2;
				pt1.Y -= y / 2;
				pt2.X -= x / 2;
				pt2.Y += y / 2;
			}
			else if (pt1y < pt2y && pt1x > pt2x)
			{
				pt1.X -= x / 2;
				pt1.Y += y / 2;
				pt2.X += x / 2;
				pt2.Y -= y / 2;
			}
			else if (pt1y < pt2y && pt1x < pt2x)
			{
				pt1.X += x / 2;
				pt1.Y += y / 2;
				pt2.X -= x / 2;
				pt2.Y -= y / 2;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void correctOffsetOfPoint(ref Point point, double ow, double oh)
		{
			// We should correct the offset because canvas storing link view nodes are not aligned as the sudoku pane.
			point.X -= ow;
			point.Y -= oh;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void correctOffsetOfDouble(ref double value, double offset)
			// We should correct the offset because canvas storing link view nodes are not aligned as the sudoku pane.
			=> value -= offset;

		HashSet<Point> getPoints(ReadOnlySpan<ILinkViewNode> nodes)
		{
			var points = new HashSet<Point>();
			foreach (var node in nodes)
			{
				var (_, start, end) = node;
				if (node.ElementType == typeof(CandidateMap))
				{
					foreach (var startCandidate in (CandidateMap)start)
					{
						points.Add(Converter.GetPosition(startCandidate));
					}
					foreach (var endCandidate in (CandidateMap)end)
					{
						points.Add(Converter.GetPosition(endCandidate));
					}
				}
				else if (node.ElementType == typeof(Cell))
				{
					points.Add(Converter.GetPosition((Cell)start * 9 + 4));
					points.Add(Converter.GetPosition((Cell)end * 9 + 4));
				}
			}
			foreach (var (_, candidate) in Conclusions)
			{
				var point = Converter.GetPosition(candidate);
				points.Add(point);
			}
			return points;
		}

		Rectangle drawRectangle(ref readonly CandidateMap nodeCandidates)
		{
			var (firstCandidate, lastCandidate) = (nodeCandidates[0], nodeCandidates[^1]);
			var topLeft = Converter.GetPosition(firstCandidate, Position.TopLeft);
			var bottomRight = Converter.GetPosition(lastCandidate, Position.BottomRight);
			var fill = new SolidColorBrush(Pane.GroupedNodeBackgroundColor);
			var stroke = new SolidColorBrush(Pane.GroupedNodeStrokeColor);
			var result = new Rectangle
			{
				Stroke = stroke,
				StrokeThickness = 1.5,
				Fill = fill,
				RadiusX = 10,
				RadiusY = 10,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top,
				Tag = nameof(DrawableFactory),
				Opacity = Pane.EnableAnimationFeedback ? 0 : 1
			};

			// Try to arrange rectangle position.
			// A simple way is to record all rows and columns spanned for the candidate list,
			// in order to find four data:
			//   1) The minimal row
			//   2) The maximal row
			//   3) The minimal column
			//   4) The maximal column
			// and then find a minimal rectangle that can cover all of those candidates by those four data.
			const int logicalMaxValue = 100;
			var (minRow, minColumn, maxRow, maxColumn) = (Candidate.MaxValue, Candidate.MaxValue, Candidate.MinValue, Candidate.MinValue);
			var (minRowValue, minColumnValue, maxRowValue, maxColumnValue) = (logicalMaxValue, logicalMaxValue, -1, -1);
			foreach (var candidate in nodeCandidates)
			{
				var cell = candidate / 9;
				var digit = candidate % 9;
				var rowValue = cell / 9 * 3 + digit / 3;
				var columnValue = cell % 9 * 3 + digit % 3;
				if (rowValue <= minRowValue)
				{
					(minRowValue, minRow) = (rowValue, candidate);
				}
				if (rowValue >= maxRowValue)
				{
					(maxRowValue, maxRow) = (rowValue, candidate);
				}
				if (columnValue <= minColumnValue)
				{
					(minColumnValue, minColumn) = (columnValue, candidate);
				}
				if (columnValue >= maxColumnValue)
				{
					(maxColumnValue, maxColumn) = (columnValue, candidate);
				}
			}

			var topLeftY = Converter.GetPosition(minRow, Position.TopLeft).Y;
			var topLeftX = Converter.GetPosition(minColumn, Position.TopLeft).X;
			var bottomRightY = Converter.GetPosition(maxRow, Position.BottomRight).Y;
			var bottomRightX = Converter.GetPosition(maxColumn, Position.BottomRight).X;
			var rectanglePositionTopLeft = new Thickness(topLeftX - ow, topLeftY - ow, 0, 0);
			(result.Width, result.Height, result.Margin) = (bottomRightX - topLeftX, bottomRightY - topLeftY, rectanglePositionTopLeft);
			return result;
		}
	}


	/// <summary>
	/// Creates a list of <see cref="Geometry"/> instances via two <see cref="Point"/>s indicating start and end point respectively,
	/// meaning the arrow cap lines besides the line.
	/// </summary>
	/// <param name="pt1">The start point.</param>
	/// <param name="pt2">The end point.</param>
	/// <returns>An instance of type <see cref="IEnumerable{T}"/> of <see cref="Geometry"/>.</returns>
	private static GeometryCollection ArrowCap(Point pt1, Point pt2)
	{
		var arrowLength = 10.0;
		var theta = 30.0;
		var angle = Atan2(pt1.Y - pt2.Y, pt1.X - pt2.X) * 180 / PI;
		var angle1 = (angle + theta) * PI / 180;
		var angle2 = (angle - theta) * PI / 180;
		var topX = arrowLength * Cos(angle1);
		var topY = arrowLength * Sin(angle1);
		var bottomX = arrowLength * Cos(angle2);
		var bottomY = arrowLength * Sin(angle2);
		return [
			new LineGeometry { StartPoint = new(pt2.X + topX, pt2.Y + topY), EndPoint = pt2 },
			new LineGeometry { StartPoint = new(pt2.X + bottomX, pt2.Y + bottomY), EndPoint = pt2 }
		];
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// <para>Fast determines whether the specified conclusion list contains the specified candidate.</para>
	/// <para>This method is used for checking cannibalism.</para>
	/// </summary>
	/// <param name="conclusions">The conclusion collection.</param>
	/// <param name="candidate">The candidate to be determined.</param>
	/// <param name="conclusion">The overlapped result.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	public static bool ConflictWith(this Conclusion[] conclusions, Candidate candidate, [NotNullWhen(true)] out Conclusion? conclusion)
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
