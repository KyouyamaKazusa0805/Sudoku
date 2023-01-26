#undef RECORD_SHAPE_REDNERING_DATA

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
			yield return children.MainGrid; // cell / candidate / baba group
		}

		yield return targetPage.SudokuPane.MainGrid; // house / chute / link
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
			if (targetControl is GridLayout { Children: var children })
			{
				children.RemoveAllViewUnitControls();
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
		var linx = new List<LinkViewNode>();
		foreach (var viewNode in view.BasicNodes)
		{
			(
				viewNode switch
				{
					CellViewNode c => () => CreateForCellViewNode(targetPage, c),
					CandidateViewNode c => () =>
					{
						CreateForCandidateViewNode(targetPage, c, conclusions, out var o);
						if (o is { } currentOverlappedConclusion)
						{
							overlapped.Add(currentOverlappedConclusion);
						}
#pragma warning disable format
					}, // Guess what? This comma is troublesome on formatting.
#pragma warning restore format
					HouseViewNode h => () => CreateForHouseViewNode(targetPage, h),
					ChuteViewNode c => () => CreateForChuteViewNode(targetPage, c),
					BabaGroupViewNode b => () => CreateBabaGroupViewNode(targetPage, b),
					LinkViewNode l => () => linx.Add(l),
					_ => default(Action?)
				}
			)?.Invoke();
		}

		foreach (var conclusion in conclusions)
		{
			CreateForConclusion(targetPage, conclusion, overlapped);
		}

		CreateForLinkViewNodes(targetPage, linx.ToArray(), conclusions);
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
			Opacity = targetPage.SudokuPane.HighlightBackgroundOpacity
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
			Opacity = targetPage.SudokuPane.HighlightBackgroundOpacity
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

	private static void CreateForChuteViewNode(AnalyzePage targetPage, ChuteViewNode chuteNode)
	{
		var (id, chute) = chuteNode;
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
			Opacity = targetPage.SudokuPane.HighlightBackgroundOpacity
		};

		var (row, column, rowSpan, columnSpan) = chute switch
		{
			>= 0 and < 3 => (chute * 3 + 2, 2, 3, 9),
			>= 3 and < 6 => (2, (chute - 3) * 3 + 2, 9, 3),
			_ => throw new InvalidOperationException(nameof(chute))
		};

		GridLayout.SetRow(control, row);
		GridLayout.SetColumn(control, column);
		GridLayout.SetRowSpan(control, rowSpan);
		GridLayout.SetColumnSpan(control, columnSpan);

		gridControl.Children.Add(control);
	}

	private static void CreateBabaGroupViewNode(AnalyzePage targetPage, BabaGroupViewNode babaGroupNode)
	{
		var (id, cell, @char) = babaGroupNode;
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
			Opacity = targetPage.SudokuPane.HighlightBackgroundOpacity,
			Child = new TextBlock
			{
				Text = @char.ToString(),
				FontSize = PencilmarkTextConversion.GetFontSize(
					targetPage.SudokuPane.ApproximateCellWidth,
					targetPage.SudokuPane.BabaGroupLabelFontScale
				),
				FontFamily = targetPage.SudokuPane.BabaGroupLabelFont,
				Foreground = new SolidColorBrush(targetPage.SudokuPane.BabaGroupLabelColor),
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

	private static void CreateForLinkViewNodes(AnalyzePage targetPage, LinkViewNode[] linkNodes, ImmutableArray<Conclusion> conclusions)
	{
		var gridControl = targetPage.SudokuPane.MainGrid;
		if (gridControl is null)
		{
			return;
		}

		var pathCreator = new PathCreator(targetPage, new(gridControl), conclusions);
		foreach (var link in
			pathCreator.CreateLinks(
				linkNodes
#if RECORD_SHAPE_REDNERING_DATA
				,
				out _
#endif
			)
		)
		{
			GridLayout.SetRowSpan(link, 9);
			GridLayout.SetColumnSpan(link, 9);

			gridControl.Children.Add(link);
		}
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

	/// <summary>
	/// Gets the customized arrow cap geometry instances that can be used as property <see cref="GeometryGroup.Children"/>.
	/// </summary>
	public static GeometryCollection WithCustomizedArrowCap(this PathGeometry @this, Point point1, Point point2)
	{
		var pt1 = point1;
		var pt2 = point2;
		var arrowLength = 10.0;
		var theta = 30.0;
		var angle = Atan2(pt1.Y - pt2.Y, pt1.X - pt2.X) * 180 / PI;
		var angle1 = (angle + theta + 22.5) * PI / 180;
		var angle2 = (angle - theta + 22.5) * PI / 180;
		var topX = arrowLength * Cos(angle1);
		var topY = arrowLength * Sin(angle1);
		var bottomX = arrowLength * Cos(angle2);
		var bottomY = arrowLength * Sin(angle2);

		var arrowX = pt2.X + topX;
		var arrowY = pt2.Y + topY;
		var a = new LineGeometry { StartPoint = new(arrowX, arrowY), EndPoint = pt2 };

		arrowX = pt2.X + bottomX;
		arrowY = pt2.Y + bottomY;
		var b = new LineGeometry { StartPoint = new(arrowX, arrowY), EndPoint = pt2 };

		return new() { @this, a, b };
	}

	/// <summary>
	/// Gets the customized arrow cap geometry instances that can be used as property <see cref="GeometryGroup.Children"/>.
	/// </summary>
	public static GeometryCollection WithCustomizedArrowCap(this LineGeometry @this)
	{
		var pt1 = @this.StartPoint;
		var pt2 = @this.EndPoint;
		var arrowLength = 10.0;
		var theta = 30.0;
		var angle = Atan2(pt1.Y - pt2.Y, pt1.X - pt2.X) * 180 / PI;
		var angle1 = (angle + theta) * PI / 180;
		var angle2 = (angle - theta) * PI / 180;
		var topX = arrowLength * Cos(angle1);
		var topY = arrowLength * Sin(angle1);
		var bottomX = arrowLength * Cos(angle2);
		var bottomY = arrowLength * Sin(angle2);

		var arrowX = pt2.X + topX;
		var arrowY = pt2.Y + topY;
		var a = new LineGeometry { StartPoint = new(arrowX, arrowY), EndPoint = pt2 };

		arrowX = pt2.X + bottomX;
		arrowY = pt2.Y + bottomY;
		var b = new LineGeometry { StartPoint = new(arrowX, arrowY), EndPoint = pt2 };

		return new() { @this, a, b };
	}
}

/// <summary>
/// Extracted type that creates the <see cref="Path"/> instances.
/// </summary>
/// <param name="Page">Indicates the page data.</param>
/// <param name="PositionConverter">Indicates the position converter.</param>
/// <param name="Conclusions">Indicates the conclusions of the whole chain.</param>
/// <seealso cref="Path"/>
file sealed record PathCreator(AnalyzePage Page, SudokuPanePositionConverter PositionConverter, ImmutableArray<Conclusion> Conclusions)
{
	/// <summary>
	/// Indicates the rotate angle (45 degrees).
	/// </summary>
	private const double RotateAngle = PI / 4;

	/// <summary>
	/// Indicates the square root of 2.
	/// </summary>
	private const double SqrtOf2 = 1.4142135623730951;


#if RECORD_SHAPE_REDNERING_DATA
	/// <summary>
	/// Creates a list of <see cref="Shape"/> instances via the specified link view nodes.
	/// </summary>
	/// <param name="nodes">The link view nodes.</param>
	/// <param name="pointRenderingData">The point pairs that are used for animate the line.</param>
	/// <returns>A <see cref="Shape"/> instance.</returns>
#else
	/// <summary>
	/// Creates a list of <see cref="Shape"/> instances via the specified link view nodes.
	/// </summary>
	/// <param name="nodes">The link view nodes.</param>
	/// <returns>A <see cref="Shape"/> instance.</returns>
#endif
	public IEnumerable<Shape> CreateLinks(
		LinkViewNode[] nodes
#if RECORD_SHAPE_REDNERING_DATA
		,
		out IEnumerable<LinkShapeRenderingData> pointRenderingData
#endif
	)
	{
		var points = getPoints(nodes);
		var result = new List<Shape>();
#if RECORD_SHAPE_REDNERING_DATA
		var renderingData = new List<LinkShapeRenderingData>();
#endif

		// Iterate on each inference to draw the links and grouped nodes (if so).
		var (outsideOffset, _) = PositionConverter.FirstCellTopLeftPosition;
		var (cs, _) = PositionConverter.CandidateSize;
		foreach (var (_, (startCells, _), (endCells, _), inference) in nodes)
		{
			_ = PositionConverter.GetPosition(startCells[0] * 9 + 4) is (var pt1x, var pt1y) pt1;
			_ = PositionConverter.GetPosition(endCells[0] * 9 + 4) is (var pt2x, var pt2y) pt2;

			var doubleCollection = inference switch
			{
				Inference.Strong or Inference.Default => new(),
				Inference.Weak => new() { 3, 1.5 },
				_ => new DoubleCollection { 3, 3 }
			};

			if (inference == Inference.Default)
			{
				// Draw the link.
				correctOffsetOfPoint(ref pt1, outsideOffset);
				correctOffsetOfPoint(ref pt2, outsideOffset);

				var shape = new Path
				{
					Stroke = new SolidColorBrush(Page.SudokuPane.LinkColor),
					StrokeThickness = 2,
					StrokeDashArray = doubleCollection,
					Data = new GeometryGroup { Children = new GeometryCollection { new LineGeometry { StartPoint = pt1, EndPoint = pt2 } } }
				};

				result.Add(shape);
#if RECORD_SHAPE_REDNERING_DATA
				renderingData.Add(new(shape, (pt1, pt2), PathKind.Straight));
#endif
			}
			else
			{
				// If the distance of two points is lower than the one of two adjacent candidates,
				// the link will be emitted to draw because of too narrow.
				var distance = pt1.DistanceTo(pt2);
				if (distance <= cs * SqrtOf2 + outsideOffset || distance <= cs * SqrtOf2 + outsideOffset)
				{
					continue;
				}

				var deltaX = pt2.X - pt1.X;
				var deltaY = pt2.Y - pt1.Y;
				var alpha = Atan2(deltaY, deltaX);
				adjust(pt1, pt2, out var p1, out _, alpha, cs, 0);

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
				cut(ref pt1, ref pt2, outsideOffset, cs);

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

					correctOffsetOfPoint(ref pt1, outsideOffset);
					correctOffsetOfPoint(ref pt2, outsideOffset);
					correctOffsetOfDouble(ref bx1, outsideOffset);
					correctOffsetOfDouble(ref bx2, outsideOffset);
					correctOffsetOfDouble(ref by1, outsideOffset);
					correctOffsetOfDouble(ref by2, outsideOffset);

					var shape = new Path
					{
						Stroke = new SolidColorBrush(Page.SudokuPane.LinkColor),
						StrokeThickness = 2,
						StrokeDashArray = doubleCollection,
						Data = new GeometryGroup
						{
							Children = new PathGeometry
							{
								Figures = new PathFigureCollection
								{
									new PathFigure
									{
										StartPoint = pt1,
										IsClosed = false,
										IsFilled = false,
										Segments = new PathSegmentCollection
										{
											new BezierSegment { Point1 = new(bx1, by1), Point2 = new(bx2, by2), Point3 = pt2 }
										}
									}
								}
							}.WithCustomizedArrowCap(pt1, pt2)
						}
					};
					result.Add(shape);
#if RECORD_SHAPE_REDNERING_DATA
					renderingData.Add(new(shape, (pt1, pt2), PathKind.Curve));
#endif
				}
				else
				{
					// Draw the link.
					correctOffsetOfPoint(ref pt1, outsideOffset);
					correctOffsetOfPoint(ref pt2, outsideOffset);

					var shape = new Path
					{
						Stroke = new SolidColorBrush(Page.SudokuPane.LinkColor),
						StrokeThickness = 2,
						StrokeDashArray = doubleCollection,
						Data = new GeometryGroup { Children = new LineGeometry { StartPoint = pt1, EndPoint = pt2 }.WithCustomizedArrowCap() }
					};
					result.Add(shape);
#if RECORD_SHAPE_REDNERING_DATA
					renderingData.Add(new(shape, (pt1, pt2), PathKind.Straight));
#endif
				}
			}
		}

#if RECORD_SHAPE_REDNERING_DATA
		pointRenderingData = renderingData;
#endif

		return result;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void rotate(Point pt1, scoped ref Point pt2, double angle)
		{
			// Translate 'pt2' to (0, 0).
			pt2 = pt2 with { X = pt2.X - pt1.X, Y = pt2.Y - pt1.Y };

			// Rotate.
			var sinAngle = Sin(angle);
			var cosAngle = Cos(angle);
			var xAct = pt2.X;
			var yAct = pt2.Y;
			pt2.X = xAct * cosAngle - yAct * sinAngle;
			pt2.Y = xAct * sinAngle + yAct * cosAngle;

			pt2 = pt2 with { X = pt2.X + pt1.X, Y = pt2.Y + pt1.Y };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void adjust(Point pt1, Point pt2, out Point p1, out Point p2, double alpha, double candidateSize, double offset)
		{
			(p1, p2, var tempDelta) = (pt1, pt2, candidateSize / 2 + offset);
			var px = (int)(tempDelta * Cos(alpha));
			var py = (int)(tempDelta * Sin(alpha));

			p1 = p1 with { X = p1.X + px, Y = p1.Y + py };
			p2 = p2 with { X = p2.X - px, Y = p2.Y - py };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void cut(scoped ref Point pt1, scoped ref Point pt2, double offset, double cs)
		{
			var ((pt1x, pt1y), (pt2x, pt2y)) = (pt1, pt2);
			var slope = Abs((pt2y - pt1y) / (pt2x - pt1x));
			var x = cs / Sqrt(1 + slope * slope);
			var y = cs * Sqrt(slope * slope / (1 + slope * slope));
			var innerOffset = offset / 8;
			if (pt1y > pt2y && pt1x.NearlyEquals(pt2x))
			{
				pt1.Y -= cs / 2 - innerOffset;
				pt2.Y += cs / 2 - innerOffset;
			}
			else if (pt1y < pt2y && pt1x.NearlyEquals(pt2x))
			{
				pt1.Y += cs / 2 - innerOffset;
				pt2.Y -= cs / 2 - innerOffset;
			}
			else if (pt1y.NearlyEquals(pt2y) && pt1x > pt2x)
			{
				pt1.X -= cs / 2 - innerOffset;
				pt2.X += cs / 2 - innerOffset;
			}
			else if (pt1y.NearlyEquals(pt2y) && pt1x < pt2x)
			{
				pt1.X += cs / 2 - innerOffset;
				pt2.X -= cs / 2 - innerOffset;
			}
			else if (pt1y > pt2y && pt1x > pt2x)
			{
				pt1.X -= x / 2 - innerOffset; pt1.Y -= y / 2 - innerOffset;
				pt2.X += x / 2 - innerOffset; pt2.Y += y / 2 - innerOffset;
			}
			else if (pt1y > pt2y && pt1x < pt2x)
			{
				pt1.X += x / 2 - innerOffset; pt1.Y -= y / 2 - innerOffset;
				pt2.X -= x / 2 - innerOffset; pt2.Y += y / 2 - innerOffset;
			}
			else if (pt1y < pt2y && pt1x > pt2x)
			{
				pt1.X -= x / 2 - innerOffset; pt1.Y += y / 2 - innerOffset;
				pt2.X += x / 2 - innerOffset; pt2.Y -= y / 2 - innerOffset;
			}
			else if (pt1y < pt2y && pt1x < pt2x)
			{
				pt1.X += x / 2 - innerOffset; pt1.Y += y / 2 - innerOffset;
				pt2.X -= x / 2 - innerOffset; pt2.Y -= y / 2 - innerOffset;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void correctOffsetOfPoint(scoped ref Point point, double offset)
			// We should correct the offset because canvas storing link view nodes are not aligned as the sudoku pane.
			=> point = point with { X = point.X - offset, Y = point.Y - offset };

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void correctOffsetOfDouble(scoped ref double value, double offset)
			// We should correct the offset because canvas storing link view nodes are not aligned as the sudoku pane.
			=> value -= offset;

		HashSet<Point> getPoints(LinkViewNode[] nodes)
		{
			var points = new HashSet<Point>();
			foreach (var (_, (startCells, _), (endCells, _), _) in nodes)
			{
				// Gets the center point.
				var a = PositionConverter.GetPosition(startCells[0]);
				var b = PositionConverter.GetPosition(endCells[0]);

				// Adds them to the collection.
				points.Add(a);
				points.Add(b);
			}

			foreach (var conclusion in Conclusions)
			{
				// Gets the center point.
				var c = PositionConverter.GetPosition(conclusion.Candidate);

				// Adds them to the collection.
				points.Add(c);
			}

			return points;
		}
	}
}

#if RECORD_SHAPE_REDNERING_DATA
/// <summary>
/// Defines a chain line kind.
/// </summary>
file enum PathKind
{
	/// <summary>
	/// Indicates the line is a straight line.
	/// </summary>
	Straight,

	/// <summary>
	/// Indicates the line is a curve (Bezier).
	/// </summary>
	Curve
}

/// <summary>
/// Defines a point pair data.
/// </summary>
/// <param name="Path">
/// The path instance. The path can be a normal straight line or a curve (e.g. a Bezier curve).
/// </param>
/// <param name="PointPair">The start and end point of the line.</param>
/// <param name="Kind">The point kind.</param>
file readonly record struct LinkShapeRenderingData(Path Path, (Point Start, Point End) PointPair, PathKind Kind);
#endif