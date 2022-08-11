namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a link view node shape.
/// </summary>
public sealed class LinkViewNodeShape : DrawingElement
{
	/// <summary>
	/// Indicates the pane size.
	/// </summary>
	private readonly double _paneSize;

	/// <summary>
	/// Indicates the canvas.
	/// </summary>
	private readonly Canvas _canvas = null!;

	/// <summary>
	/// Indicates the link view nodes used.
	/// </summary>
	private readonly LinkViewNode[] _linkViewNodes = null!;


	/// <summary>
	/// Indicates the pane size.
	/// </summary>
	public required double PaneSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _paneSize;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[MemberNotNull(nameof(_canvas))]
		init
		{
			_paneSize = value;

			_canvas = new() { Width = _paneSize, Height = _paneSize };
		}
	}

	/// <summary>
	/// Indicates the outside offset.
	/// </summary>
	public required double OutsideOffset { get; init; }

	/// <summary>
	/// Indicates the link nodes used.
	/// </summary>
	public required LinkViewNode[] Nodes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _linkViewNodes;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[MemberNotNull(nameof(_linkViewNodes))]
		init
		{
			_linkViewNodes = value;

			_canvas.Children.AddRange(
				new PathConstructor
				{
					PaneSize = PaneSize,
					OutsideOffset = OutsideOffset,
					Preference = Preference,
					Conclusions = Conclusions
				}.CreateLinks(value, out var pointPairs)
			);

			var storyboard = new Storyboard();
			foreach (var (path, (pt1, pt2), pathKind) in pointPairs)
			{
				var animation = new PointAnimation
				{
					From = pt1,
					To = pt2,
					Duration = TimeSpan.FromMilliseconds(500)
				};

				Storyboard.SetTarget(animation, path);
				Storyboard.SetTargetProperty(
					animation,
					pathKind switch
					{
						PathKind.Straight =>
							new PropertyPathBuilder()
								.AppendProperty<Path>(nameof(Path.Data))
								.AppendProperty<GeometryGroup>(nameof(GeometryGroup.Children))
								.AppendIndex(0)
								.AppendProperty<LineGeometry>(nameof(LineGeometry.EndPoint))
								.ToString(),
						PathKind.Curve =>
							new PropertyPathBuilder()
								.AppendProperty<Path>(nameof(Path.Data))
								.AppendProperty<GeometryGroup>(nameof(GeometryGroup.Children))
								.AppendIndex(0)
								.AppendProperty<PathGeometry>(nameof(PathGeometry.Figures))
								.AppendIndex(0)
								.AppendProperty<PathFigure>(nameof(PathFigure.Segments))
								.AppendIndex(0)
								.AppendProperty<BezierSegment>(nameof(BezierSegment.Point3))
								.ToString()
					}
				);
				storyboard.Children.Add(animation);
			}

			storyboard.Begin();
		}
	}

	/// <summary>
	/// Indicates the conclusions used.
	/// </summary>
	public required ImmutableArray<Conclusion> Conclusions { get; init; }

	/// <summary>
	/// Indicates the user preference instance.
	/// </summary>
	public required IDrawingPreference Preference { get; init; }

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(LinkViewNodeShape);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] DrawingElement? other) =>
		other is LinkViewNodeShape comparer && ReferenceEquals(Nodes, comparer.Nodes);

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(TypeIdentifier, Nodes);

	/// <inheritdoc/>
	public override Canvas GetControl() => _canvas;
}

/// <summary>
/// Extracted type that constructs the links.
/// </summary>
file sealed class PathConstructor
{
	/// <summary>
	/// Indicates the rotate angle (45 degrees).
	/// </summary>
	private const double RotateAngle = PI / 4;

	/// <summary>
	/// Indicates the square root of 2.
	/// </summary>
	private const double SqrtOf2 = 1.4142135623730951;


	/// <inheritdoc cref="LinkViewNodeShape.PaneSize"/>
	public required double PaneSize { get; init; }

	/// <inheritdoc cref="LinkViewNodeShape.OutsideOffset"/>
	public required double OutsideOffset { get; init; }

	/// <inheritdoc cref="LinkViewNodeShape.Conclusions"/>
	public required ImmutableArray<Conclusion> Conclusions { get; init; }

	/// <inheritdoc cref="LinkViewNodeShape.Preference"/>
	public required IDrawingPreference Preference { get; init; }


	/// <summary>
	/// Creates a list of <see cref="Shape"/> instances via the specified link view nodes.
	/// </summary>
	/// <param name="nodes">The link view nodes.</param>
	/// <param name="pointPairs">The point pairs that are used for animate the line.</param>
	/// <returns>A <see cref="Shape"/> instance.</returns>
	public IEnumerable<Shape> CreateLinks(
		LinkViewNode[] nodes,
		out IList<(Path, (Point, Point), PathKind)> pointPairs)
	{
		var points = getPoints(nodes);
		var result = new List<Shape>();
		pointPairs = new List<(Path, (Point, Point), PathKind)>();

		// Iterate on each inference to draw the links and grouped nodes (if so).
		double cs = PointConversions.CandidateSize(PaneSize, OutsideOffset);
		bool isFirst = true;
		foreach (var (start, end, inference) in nodes)
		{
			_ = PointConversions.GetMouseCenter(PaneSize, OutsideOffset, start) is (var pt1x, var pt1y) pt1;
			_ = PointConversions.GetMouseCenter(PaneSize, OutsideOffset, end) is (var pt2x, var pt2y) pt2;

			var doubleCollection = inference switch
			{
				Inference.Strong => new(),
				Inference.Weak => new() { 3, 1.5 },
				_ => new DoubleCollection { 3, 3 }
			};

			if (inference == Inference.Default)
			{
				// Draw the link.
				correctOffsetOfPoint(ref pt1, OutsideOffset);
				correctOffsetOfPoint(ref pt2, OutsideOffset);

				var shape = new Path()
					.WithStroke(Preference.LinkColor)
					.WithStrokeThickness(2)
					.WithStrokeDashArray(doubleCollection)
					.WithData(
						new GeometryGroup()
							.WithChildren(
								new LineGeometry()
									.WithPoints(pt1, pt2)
									.WithCustomizedArrowCap()
							)
					);
				result.Add(shape);
				pointPairs.Add((shape, (pt1, pt2), PathKind.Straight));
			}
			else
			{
				// If the distance of two points is lower than the one of two adjacent candidates,
				// the link will be emitted to draw because of too narrow.
				double distance = pt1.DistanceTo(pt2);
				if (distance <= cs * SqrtOf2 + OutsideOffset || distance <= cs * SqrtOf2 + OutsideOffset)
				{
					continue;
				}

				double deltaX = pt2.X - pt1.X, deltaY = pt2.Y - pt1.Y;
				double alpha = Atan2(deltaY, deltaX);
				adjust(pt1, pt2, out var p1, out _, alpha, cs, 0);

				// Check if another candidate lies in the direct line.
				bool through = false;
				double dx1 = deltaX, dy1 = deltaY;
				foreach (var point in points)
				{
					if (point == pt1 || point == pt2)
					{
						// The point is itself.
						continue;
					}

					double dx2 = point.X - p1.X, dy2 = point.Y - p1.Y;
					if (Sign(dx1) == Sign(dx2) && Sign(dy1) == Sign(dy2)
						&& Abs(dx2) <= Abs(dx1) && Abs(dy2) <= Abs(dy1)
						&& (dx1 == 0 || dy1 == 0 || (dx1 / dy1).NearlyEquals(dx2 / dy2, epsilon: 1E-1)))
					{
						through = true;
						break;
					}
				}

				// Now cut the link.
				cut(ref pt1, ref pt2, OutsideOffset, cs);

				if (through)
				{
					double bezierLength = 20;

					// The end points are rotated 45 degrees
					// (counterclockwise for the start point, clockwise for the end point).
					var oldPt1 = new Point(pt1x, pt1y);
					var oldPt2 = new Point(pt2x, pt2y);
					rotate(oldPt1, ref pt1, -RotateAngle);
					rotate(oldPt2, ref pt2, RotateAngle);

					double interim1Alpha = alpha - RotateAngle;
					double bx1 = pt1.X + bezierLength * Cos(interim1Alpha), by1 = pt1.Y + bezierLength * Sin(interim1Alpha);
					double interim2Alpha = alpha + RotateAngle;
					double bx2 = pt2.X - bezierLength * Cos(interim2Alpha), by2 = pt2.Y - bezierLength * Sin(interim2Alpha);

					correctOffsetOfPoint(ref pt1, OutsideOffset);
					correctOffsetOfPoint(ref pt2, OutsideOffset);
					correctOffsetOfDouble(ref bx1, OutsideOffset);
					correctOffsetOfDouble(ref bx2, OutsideOffset);
					correctOffsetOfDouble(ref by1, OutsideOffset);
					correctOffsetOfDouble(ref by2, OutsideOffset);

					var shape = new Path()
						.WithStroke(Preference.LinkColor)
						.WithStrokeThickness(2)
						.WithStrokeDashArray(doubleCollection)
						.WithData(
							new GeometryGroup()
								.WithChildren(
									new PathGeometry()
										.WithFigures(
											new PathFigure()
												.WithStartPoint(pt1)
												.WithIsClosed(false)
												.WithIsFilled(false)
												.WithSegments(
													new BezierSegment()
														.WithInterimPoints(bx1, by1, bx2, by2)
														.WithEndPoint(pt2)
												)
										)
										.WithCustomizedArrowCap(pt1, pt2)
								)
						);
					result.Add(shape);
					pointPairs.Add((shape, (pt1, pt2), PathKind.Curve));
				}
				else
				{
					// Draw the link.
					correctOffsetOfPoint(ref pt1, OutsideOffset);
					correctOffsetOfPoint(ref pt2, OutsideOffset);

					var shape = new Path()
						.WithStroke(Preference.LinkColor)
						.WithStrokeThickness(2)
						.WithStrokeDashArray(doubleCollection)
						.WithData(
							new GeometryGroup()
								.WithChildren(
									new LineGeometry()
										.WithPoints(pt1, pt2)
										.WithCustomizedArrowCap()
								)
						);
					result.Add(shape);
					pointPairs.Add((shape, (pt1, pt2), PathKind.Straight));
				}
			}

			// Fills the background of the grouped nodes.
			if (isFirst)
			{
				isFirst = false;

				if (start.Cells.Count > 1)
				{
					pt1 = PointConversions.GetMouseTopLeft(PaneSize, OutsideOffset, start);
					pt2 = PointConversions.GetMouseBottomRight(PaneSize, OutsideOffset, start);
					correctOffsetOfPoint(ref pt1, OutsideOffset);
					correctOffsetOfPoint(ref pt2, OutsideOffset);
					result.Add(
						new Rectangle()
							.WithRadius(10)
							.WithCanvasOffset(pt1)
							.WithWidth(Abs(pt2.X - pt1.X))
							.WithHeight(Abs(pt2.Y - pt1.Y))
							.WithStroke(Preference.GroupedLinkNodeColor)
							.WithStrokeThickness(1)
							.WithFill(Preference.GroupedLinkNodeColor with { A = 32 })
					);
				}
			}

			if (end.Cells.Count > 1)
			{
				pt1 = PointConversions.GetMouseTopLeft(PaneSize, OutsideOffset, end);
				pt2 = PointConversions.GetMouseBottomRight(PaneSize, OutsideOffset, end);
				correctOffsetOfPoint(ref pt1, OutsideOffset);
				correctOffsetOfPoint(ref pt2, OutsideOffset);
				result.Add(
					new Rectangle()
						.WithRadius(10)
						.WithCanvasOffset(pt1)
						.WithWidth(Abs(pt2.X - pt1.X))
						.WithHeight(Abs(pt2.Y - pt1.Y))
						.WithStroke(Preference.GroupedLinkNodeColor)
						.WithStrokeThickness(1)
						.WithFill(Preference.GroupedLinkNodeColor with { A = 32 })
				);
			}
		}

		return result;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void rotate(scoped in Point pt1, scoped ref Point pt2, double angle)
		{
			// Translate 'pt2' to (0, 0).
			pt2 = pt2 with { X = pt2.X - pt1.X, Y = pt2.Y - pt1.Y };

			// Rotate.
			double sinAngle = Sin(angle), cosAngle = Cos(angle);
			double xAct = pt2.X, yAct = pt2.Y;
			pt2.X = (float)(xAct * cosAngle - yAct * sinAngle);
			pt2.Y = (float)(xAct * sinAngle + yAct * cosAngle);

			pt2 = pt2 with { X = pt2.X + pt1.X, Y = pt2.Y + pt1.Y };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void adjust(
			scoped in Point pt1, scoped in Point pt2, out Point p1, out Point p2,
			double alpha, double candidateSize, double offset)
		{
			(p1, p2, double tempDelta) = (pt1, pt2, candidateSize / 2 + offset);
			int px = (int)(tempDelta * Cos(alpha)), py = (int)(tempDelta * Sin(alpha));

			p1 = p1 with { X = p1.X + px, Y = p1.Y + py };
			p2 = p2 with { X = p2.X - px, Y = p2.Y - py };
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void cut(scoped ref Point pt1, scoped ref Point pt2, double offset, double cs)
		{
			var ((pt1x, pt1y), (pt2x, pt2y)) = (pt1, pt2);
			double slope = Abs((pt2y - pt1y) / (pt2x - pt1x));
			double x = cs / (float)Sqrt(1 + slope * slope);
			double y = cs * (float)Sqrt(slope * slope / (1 + slope * slope));
			double innerOffset = offset / 8;
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
			// We should correct the offset because canvas storing link view nodes are not aligned
			// as the sudoku pane.
			=> point = point with { X = point.X - offset, Y = point.Y - offset };

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void correctOffsetOfDouble(scoped ref double value, double offset)
			// We should correct the offset because canvas storing link view nodes are not aligned
			// as the sudoku pane.
			=> value -= offset;

		HashSet<Point> getPoints(LinkViewNode[] nodes)
		{
			var points = new HashSet<Point>();
			foreach (var linkNode in nodes)
			{
				// Gets the center point.
				var a = PointConversions.GetMouseCenter(PaneSize, OutsideOffset, linkNode.Start);
				var b = PointConversions.GetMouseCenter(PaneSize, OutsideOffset, linkNode.End);

				// Adds them to the collection.
				points.Add(a);
				points.Add(b);
			}

			foreach (var conclusion in Conclusions)
			{
				// Gets the center point.
				var c = PointConversions.GetMousePointInCenter(PaneSize, OutsideOffset, conclusion.Cell, conclusion.Digit);

				// Adds them to the collection.
				points.Add(c);
			}

			return points;
		}
	}
}

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
