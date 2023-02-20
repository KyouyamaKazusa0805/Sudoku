namespace SudokuStudio.Interaction;

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
	internal const string InternalTag = $"{nameof(ViewUnitFrameworkElementFactory)}.{nameof(FrameworkElement)}";


	/// <summary>
	/// Try to get all possible <see cref="FrameworkElement"/>s that are candidate controls
	/// storing <see cref="ViewUnit"/>-displaying <see cref="FrameworkElement"/>s.
	/// </summary>
	/// <param name="sudokuPane">The target pane.</param>
	/// <returns>
	/// A list of controls, whose <c>Chilren</c> property can be used for removing <see cref="ViewUnit"/>-displaying controls.
	/// </returns>
	/// <seealso cref="FrameworkElement"/>
	/// <seealso cref="ViewUnit"/>
	public static IEnumerable<FrameworkElement> GetViewUnitTargetParentControls(SudokuPane sudokuPane)
	{
		foreach (var children in sudokuPane._children)
		{
			yield return children.MainGrid; // cell / candidate / baba group
		}

		yield return sudokuPane.MainGrid; // house / chute / link
	}

	/// <summary>
	/// Removes all possible controls that are used for displaying elements in a <see cref="ViewUnit"/>.
	/// </summary>
	/// <param name="sudokuPane">The target pane.</param>
	/// <seealso cref="ViewUnit"/>
	public static void RemoveViewUnitControls(SudokuPane sudokuPane)
	{
		foreach (var targetControl in GetViewUnitTargetParentControls(sudokuPane))
		{
			if (targetControl is GridLayout { Children: var children })
			{
				children.RemoveAllViewUnitControls();
			}
		}
	}

	/// <summary>
	/// Adds a list of <see cref="FrameworkElement"/>s that are used for displaying highlight elements in a <see cref="ViewUnit"/>.
	/// </summary>
	/// <param name="sudokuPane">The target pane.</param>
	/// <param name="viewUnit">The view unit that you want to display.</param>
	/// <seealso cref="FrameworkElement"/>
	/// <seealso cref="ViewUnit"/>
	public static void AddViewUnitControls(SudokuPane sudokuPane, ViewUnit viewUnit)
	{
		if (viewUnit is not { View.BasicNodes: var nodes, Conclusions: var conclusions })
		{
			return;
		}

		var overlapped = new List<Conclusion>();
		var links = new List<LinkViewNode>();
		foreach (var viewNode in nodes)
		{
			(
				viewNode switch
				{
					CellViewNode c => () => ForCellNode(sudokuPane, c),
					CandidateViewNode c => () =>
					{
						ForCandidateNode(sudokuPane, c, conclusions, out var o);
						if (o is { } currentOverlappedConclusion)
						{
							overlapped.Add(currentOverlappedConclusion);
						}
#pragma warning disable format
					}, // Guess what? This comma is troublesome while formatting.
#pragma warning restore format
					HouseViewNode h => () => ForHouseNode(sudokuPane, h),
					ChuteViewNode c => () => ForChuteNode(sudokuPane, c),
					BabaGroupViewNode b => () => BabaGroupNode(sudokuPane, b),
					LinkViewNode l => () => links.Add(l),
					_ => default(Action?)
				}
			)?.Invoke();
		}

		foreach (var conclusion in conclusions)
		{
			ForConclusion(sudokuPane, conclusion, overlapped);
		}

		ForLinkNodes(sudokuPane, links.ToArray(), conclusions);
	}

	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for conclusions.
	/// </summary>
	/// <param name="sudokuPane">
	/// The target sudoku pane. This instance provides with user-defined customized properties used for displaying elements.
	/// e.g. background color.
	/// </param>
	/// <param name="conclusion">The conclusion to be displayed.</param>
	/// <param name="overlapped">A collection that stores for overlapped candidates.</param>
	private static void ForConclusion(SudokuPane sudokuPane, Conclusion conclusion, List<Conclusion> overlapped)
	{
		var (type, candidate) = conclusion;
		var paneCellControl = sudokuPane._children[candidate / 9];
		if (paneCellControl is null)
		{
			return;
		}

		ForCandidateNodeCore(
			IdentifierConversion.GetColor(
				type switch
				{
					Assignment => predicate() ? DisplayColorKind.OverlappedAssignment : DisplayColorKind.Assignment,
					Elimination => predicate() ? DisplayColorKind.Cannibalism : DisplayColorKind.Elimination
				}
			),
			candidate,
			paneCellControl
		);


		bool predicate() => overlapped.Exists(conclusion => conclusion.Candidate == candidate);
	}

	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for <see cref="CellViewNode"/>.
	/// </summary>
	/// <param name="sudokuPane">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion})" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="cellNode">The node to be displayed.</param>
	/// <seealso cref="CellViewNode"/>
	private static void ForCellNode(SudokuPane sudokuPane, CellViewNode cellNode)
	{
		var (id, cell) = cellNode;
		var paneCellControl = sudokuPane._children[cell];
		if (paneCellControl is null)
		{
			return;
		}

		var control = new Border
		{
			Background = new SolidColorBrush(IdentifierConversion.GetColor(id)),
			BorderThickness = new(0),
			Tag = InternalTag,
			Opacity = sudokuPane.HighlightBackgroundOpacity
		};

		GridLayout.SetRowSpan(control, 3);
		GridLayout.SetColumnSpan(control, 3);
		Canvas.SetZIndex(control, -1);

		paneCellControl.MainGrid.Children.Add(control);
	}

	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for <see cref="CandidateViewNode"/>.
	/// </summary>
	/// <param name="sudokuPane">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion})" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="candidateNode">
	/// <inheritdoc cref="ForCellNode(SudokuPane, CellViewNode)" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="conclusions">Indicates the conclusion collection. The argument is used for checking cannibalisms.</param>
	/// <param name="overlapped">
	/// Indicates the collection that returns a possible <see cref="Conclusion"/> value indicating
	/// what candidate conflicts with the current node while displaying. If no overlapped conclusion, <see langword="null"/>.
	/// </param>
	/// <seealso cref="CandidateViewNode"/>
	private static void ForCandidateNode(SudokuPane sudokuPane, CandidateViewNode candidateNode, ImmutableArray<Conclusion> conclusions, out Conclusion? overlapped)
	{
		overlapped = null;

		var (id, candidate) = candidateNode;
		var cell = candidate / 9;
		var paneCellControl = sudokuPane._children[cell];
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

		ForCandidateNodeCore(IdentifierConversion.GetColor(id), candidate, paneCellControl);
	}

	/// <summary>
	/// The core method called by <see cref="ForCandidateNode(SudokuPane, CandidateViewNode, ImmutableArray{Conclusion}, out Conclusion?)"/>.
	/// </summary>
	/// <param name="color">The color to be used on rendering.</param>
	/// <param name="candidate">The candidate to be rendered.</param>
	/// <param name="paneCellControl">The pane cell control that stores the rendered control.</param>
	/// <seealso cref="ForCandidateNode(SudokuPane, CandidateViewNode, ImmutableArray{Conclusion}, out Conclusion?)"/>
	private static void ForCandidateNodeCore(Color color, int candidate, SudokuPaneCell paneCellControl)
	{
		var (width, height) = paneCellControl.ActualSize / 3F * (float)paneCellControl.BasePane.HighlightCandidateCircleScale;
		var control = new Ellipse
		{
			Width = width,
			Height = height,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center,
			Fill = new SolidColorBrush(color),
			Tag = InternalTag
		};

		var digit = candidate % 9;
		GridLayout.SetRow(control, digit / 3);
		GridLayout.SetColumn(control, digit % 3);
		Canvas.SetZIndex(control, -1);

		paneCellControl.MainGrid.Children.Add(control);
	}

	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for <see cref="HouseViewNode"/>.
	/// </summary>
	/// <param name="sudokuPane">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion})" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="houseNode">
	/// <inheritdoc cref="ForCellNode(SudokuPane, CellViewNode)" path="/param[@name='cellNode']"/>
	/// </param>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="houseNode"/> stores invalid data of property <see cref="HouseViewNode.House"/>.
	/// </exception>
	/// <seealso cref="HouseViewNode"/>
	private static void ForHouseNode(SudokuPane sudokuPane, HouseViewNode houseNode)
	{
		var (id, house) = houseNode;
		var gridControl = sudokuPane.MainGrid;
		if (gridControl is null)
		{
			return;
		}

		var control = new Border
		{
			Background = new SolidColorBrush(IdentifierConversion.GetColor(id)),
			BorderThickness = new(0),
			Tag = InternalTag,
			Opacity = sudokuPane.HighlightBackgroundOpacity
		};

		var (row, column, rowSpan, columnSpan) = house switch
		{
			>= 0 and < 9 => (house / 3 * 3 + 2, house % 3 * 3 + 2, 3, 3),
			>= 9 and < 18 => (house - 9 + 2, 2, 1, 9),
			>= 18 and < 27 => (2, house - 18 + 2, 9, 1),
			_ => throw new ArgumentException(
				$"The value '{nameof(houseNode)}' is invalid.",
				nameof(houseNode),
				new InvalidOperationException($"The property '{nameof(HouseViewNode.House)}' of instance '{nameof(houseNode)}' is invalid.")
			)
		};

		GridLayout.SetRow(control, row);
		GridLayout.SetColumn(control, column);
		GridLayout.SetRowSpan(control, rowSpan);
		GridLayout.SetColumnSpan(control, columnSpan);

		gridControl.Children.Add(control);
	}

	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for <see cref="ChuteViewNode"/>.
	/// </summary>
	/// <param name="sudokuPane">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion})" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="chuteNode">
	/// <inheritdoc cref="ForCellNode(SudokuPane, CellViewNode)" path="/param[@name='cellNode']"/>
	/// </param>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="chuteNode"/> stores invalid data of property <see cref="ChuteViewNode.ChuteIndex"/>.
	/// </exception>
	/// <seealso cref="ChuteViewNode"/>
	private static void ForChuteNode(SudokuPane sudokuPane, ChuteViewNode chuteNode)
	{
		var (id, chute) = chuteNode;
		var gridControl = sudokuPane.MainGrid;
		if (gridControl is null)
		{
			return;
		}

		var control = new Border
		{
			Background = new SolidColorBrush(IdentifierConversion.GetColor(id)),
			BorderThickness = new(0),
			Tag = InternalTag,
			Opacity = sudokuPane.HighlightBackgroundOpacity
		};

		var (row, column, rowSpan, columnSpan) = chute switch
		{
			>= 0 and < 3 => (chute * 3 + 2, 2, 3, 9),
			>= 3 and < 6 => (2, (chute - 3) * 3 + 2, 9, 3),
			_ => throw new ArgumentException(
				$"The value '{nameof(chuteNode)}' is invalid.",
				nameof(chuteNode),
				new InvalidOperationException($"The property '{nameof(HouseViewNode.House)}' of instance '{nameof(chuteNode)}' is invalid.")
			)
		};

		GridLayout.SetRow(control, row);
		GridLayout.SetColumn(control, column);
		GridLayout.SetRowSpan(control, rowSpan);
		GridLayout.SetColumnSpan(control, columnSpan);

		gridControl.Children.Add(control);
	}

	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for <see cref="BabaGroupViewNode"/>.
	/// </summary>
	/// <param name="sudokuPane">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion})" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="babaGroupNode">
	/// <inheritdoc cref="ForCellNode(SudokuPane, CellViewNode)" path="/param[@name='cellNode']"/>
	/// </param>
	/// <seealso cref="BabaGroupViewNode"/>
	private static void BabaGroupNode(SudokuPane sudokuPane, BabaGroupViewNode babaGroupNode)
	{
		var (id, cell, @char) = babaGroupNode;
		var paneCellControl = sudokuPane._children[cell];
		if (paneCellControl is null)
		{
			return;
		}

		var control = new Border
		{
			Background = new SolidColorBrush(IdentifierConversion.GetColor(id)),
			BorderThickness = new(0),
			Tag = InternalTag,
			Opacity = sudokuPane.HighlightBackgroundOpacity,
			Child = new TextBlock
			{
				Text = @char.ToString(),
				FontSize = PencilmarkTextConversion.GetFontSizeSimple(sudokuPane.ApproximateCellWidth, sudokuPane.BabaGroupLabelFontScale),
				FontFamily = sudokuPane.BabaGroupLabelFont,
				Foreground = new SolidColorBrush(sudokuPane.BabaGroupLabelColor),
				FontWeight = FontWeights.Bold,
				FontStyle = FontStyle.Italic,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				HorizontalTextAlignment = TextAlignment.Center,
				TextAlignment = TextAlignment.Center
			}
		};

		GridLayout.SetRowSpan(control, 3);
		GridLayout.SetColumnSpan(control, 3);
		Canvas.SetZIndex(control, -1);

		paneCellControl.MainGrid.Children.Add(control);
	}

	/// <summary>
	/// Create <see cref="FrameworkElement"/>s that displays for <see cref="LinkViewNode"/>s.
	/// </summary>
	/// <param name="sudokuPane">
	/// <inheritdoc cref="ForConclusion(SudokuPane, Conclusion, List{Conclusion})" path="/param[@name='sudokuPane']"/>
	/// </param>
	/// <param name="linkNodes">
	/// <inheritdoc cref="ForCellNode(SudokuPane, CellViewNode)" path="/param[@name='cellNode']"/>
	/// </param>
	/// <param name="conclusions">Indicates the conclusions. The value is used for appending links between tail node and conclusion.</param>
	/// <remarks>
	/// This method is special: We should handle all <see cref="LinkViewNode"/>s together.
	/// </remarks>
	private static void ForLinkNodes(SudokuPane sudokuPane, LinkViewNode[] linkNodes, ImmutableArray<Conclusion> conclusions)
	{
		var gridControl = sudokuPane.MainGrid;
		if (gridControl is null)
		{
			return;
		}

		var pathCreator = new PathCreator(sudokuPane, new(gridControl), conclusions);
		foreach (var link in pathCreator.CreateLinks(linkNodes))
		{
			GridLayout.SetRow(link, 2);
			GridLayout.SetColumn(link, 2);
			GridLayout.SetRowSpan(link, 9);
			GridLayout.SetColumnSpan(link, 9);
			Canvas.SetZIndex(link, -1);

			gridControl.Children.Add(link);
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
file sealed record PathCreator(SudokuPane Pane, SudokuPanePositionConverter Converter, ImmutableArray<Conclusion> Conclusions)
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
	public IEnumerable<Path> CreateLinks(LinkViewNode[] nodes)
	{
		var points = getPoints(nodes);
		_ = Converter is (var (ow, oh), _) and ((var cs, _), _, _, _);

		// Iterate on each inference to draw the links and grouped nodes (if so).
		foreach (var node in nodes)
		{
			if (node is not (_, ([var startCell, ..], var startDigit), ([var endCell, ..], var endDigit), var inference))
			{
				continue;
			}

			_ = Converter.GetPosition(startCell * 9 + startDigit) is (var pt1x, var pt1y) pt1;
			_ = Converter.GetPosition(endCell * 9 + endDigit) is (var pt2x, var pt2y) pt2;

			var dashArray = (
				inference switch
				{
					Inference.Strong => Pane.StrongLinkDashStyle,
					Inference.Weak => Pane.WeakLinkDashStyle,
					Inference.Default => Pane.CycleLikeLinkDashStyle,
					_ => Pane.OtherLinkDashStyle
				}
			).ToDoubleCollection();
			switch (inference)
			{
				case Inference.Default:
				{
					correctOffsetOfPoint(ref pt1, ow, oh);
					correctOffsetOfPoint(ref pt2, ow, oh);

					yield return new()
					{
						Stroke = new SolidColorBrush(Pane.LinkColor),
						StrokeThickness = Pane.ChainStrokeThickness,
						StrokeDashArray = dashArray,
						Data = new GeometryGroup { Children = new() { new LineGeometry { StartPoint = pt1, EndPoint = pt2 } } },
						Tag = ViewUnitFrameworkElementFactory.InternalTag
					};

					break;
				}
				default:
				{
					// If the distance of two points is lower than the one of two adjacent candidates,
					// the link will be ignored to be drawn because of too narrow.
					var distance = pt1.DistanceTo(pt2);
					if (distance <= cs * SqrtOf2 || distance <= cs * SqrtOf2)
					{
						continue;
					}

					var deltaX = pt2.X - pt1.X;
					var deltaY = pt2.Y - pt1.Y;
					var alpha = Atan2(deltaY, deltaX);
					adjust(pt1, pt2, out var p1, out _, alpha, cs);

					// Check if another candidate lies in the direct line.
					var through = false;
					var dx1 = deltaX;
					var dy1 = deltaY;
					foreach (var point in points)
					{
						if (point == pt1 || point == pt2)
						{
							// The point is itself.
							continue;
						}

						var dx2 = point.X - p1.X;
						var dy2 = point.Y - p1.Y;
						if (Sign(dx1) == Sign(dx2) && Sign(dy1) == Sign(dy2)
							&& Abs(dx2) <= Abs(dx1) && Abs(dy2) <= Abs(dy1)
							&& (dx1 == 0 || dy1 == 0 || (dx1 / dy1).NearlyEquals(dx2 / dy2, epsilon: 1E-1)))
						{
							through = true;
							break;
						}
					}

					// Now cut the link.
					cut(ref pt1, ref pt2, cs);

					if (through)
					{
						var bezierLength = 20.0;

						// The end points are rotated 45 degrees (counterclockwise for the start point, clockwise for the end point).
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

						yield return new()
						{
							Stroke = new SolidColorBrush(Pane.LinkColor),
							StrokeThickness = Pane.ChainStrokeThickness,
							StrokeDashArray = dashArray,
							Data = new GeometryGroup
							{
								Children = new GeometryCollection
								{
									new PathGeometry
									{
										Figures = new()
										{
											new PathFigure
											{
												StartPoint = pt1,
												IsClosed = false,
												IsFilled = false,
												Segments = new() { new BezierSegment { Point1 = new(bx1, by1), Point2 = new(bx2, by2), Point3 = pt2 } }
											}
										}
									}
								}
							},
							Tag = ViewUnitFrameworkElementFactory.InternalTag
						};
						yield return new()
						{
							Stroke = new SolidColorBrush(Pane.LinkColor),
							StrokeThickness = Pane.ChainStrokeThickness,
							Data = new GeometryGroup { Children = GeometryCollectionFactory.ArrowCap(pt1, pt2) },
							Tag = ViewUnitFrameworkElementFactory.InternalTag
						};
					}
					else
					{
						// Draw the link.
						correctOffsetOfPoint(ref pt1, ow, oh);
						correctOffsetOfPoint(ref pt2, ow, oh);

						yield return new()
						{
							Stroke = new SolidColorBrush(Pane.LinkColor),
							StrokeThickness = Pane.ChainStrokeThickness,
							StrokeDashArray = dashArray,
							Data = new GeometryGroup { Children = new GeometryCollection { new LineGeometry { StartPoint = pt1, EndPoint = pt2 } } },
							Tag = ViewUnitFrameworkElementFactory.InternalTag
						};
						yield return new()
						{
							Stroke = new SolidColorBrush(Pane.LinkColor),
							StrokeThickness = Pane.ChainStrokeThickness,
							Data = new GeometryGroup { Children = GeometryCollectionFactory.ArrowCap(pt1, pt2) },
							Tag = ViewUnitFrameworkElementFactory.InternalTag
						};
					}

					break;
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void rotate(Point pt1, scoped ref Point pt2, double angle)
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
		static void adjust(Point pt1, Point pt2, out Point p1, out Point p2, double alpha, double cs)
		{
			(p1, p2, var tempDelta) = (pt1, pt2, cs / 2);
			var (px, py) = (tempDelta * Cos(alpha), tempDelta * Sin(alpha));

			p1.X += px;
			p1.Y += py;
			p2.X -= px;
			p2.Y -= py;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void cut(scoped ref Point pt1, scoped ref Point pt2, double cs)
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
		static void correctOffsetOfPoint(scoped ref Point point, double ow, double oh)
		{
			// We should correct the offset because canvas storing link view nodes are not aligned as the sudoku pane.
			point.X -= ow;
			point.Y -= oh;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void correctOffsetOfDouble(scoped ref double value, double offset)
			// We should correct the offset because canvas storing link view nodes are not aligned as the sudoku pane.
			=> value -= offset;

		HashSet<Point> getPoints(LinkViewNode[] nodes)
		{
			var points = new HashSet<Point>();
			foreach (var node in nodes)
			{
				if (node is not (_, ([var startCell, ..], var startDigit), ([var endCell, ..], var endDigit), _))
				{
					continue;
				}

				points.Add(Converter.GetPosition(startCell * 9 + startDigit));
				points.Add(Converter.GetPosition(endCell * 9 + endDigit));
			}

			foreach (var (_, candidate) in Conclusions)
			{
				points.Add(Converter.GetPosition(candidate));
			}

			return points;
		}
	}
}

/// <summary>
/// Represents a factory type that can creates a collection of <see cref="Geometry"/> instances via the specified rule.
/// </summary>
/// <seealso cref="Geometry"/>
file static class GeometryCollectionFactory
{
	/// <summary>
	/// Creates a list of <see cref="Geometry"/> instances via two <see cref="Point"/>s indicating start and end point respectively,
	/// meaning the arrow cap lines besides the line.
	/// </summary>
	/// <param name="pt1">The start point.</param>
	/// <param name="pt2">The end point.</param>
	/// <returns>An instance of type <see cref="IEnumerable{T}"/> of <see cref="Geometry"/>.</returns>
	public static GeometryCollection ArrowCap(Point pt1, Point pt2)
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
		return new()
		{
			new LineGeometry { StartPoint = new(pt2.X + topX, pt2.Y + topY), EndPoint = pt2 },
			new LineGeometry { StartPoint = new(pt2.X + bottomX, pt2.Y + bottomY), EndPoint = pt2 }
		};
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
			if (element.Tag is ViewUnitFrameworkElementFactory.InternalTag)
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
