namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a link view node shape.
/// </summary>
public sealed class LinkViewNodeShape : DrawingElement
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
	private readonly ObservableCollection<LinkViewNode> _linkViewNodes = new();


	/// <summary>
	/// Initializes a <see cref="LinkViewNodeShape"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LinkViewNodeShape()
		=> _linkViewNodes.CollectionChanged += (_, _) =>
		{
			_canvas.Children.Clear();
			_canvas.Children.AddRange(CreateLinks(_linkViewNodes));
		};


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
	public required ImmutableArray<LinkViewNode> Nodes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _linkViewNodes.ToImmutableArray();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _linkViewNodes.AddRange(value);
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
		other is LinkViewNodeShape comparer && Nodes == comparer.Nodes;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(TypeIdentifier, Nodes);

	/// <inheritdoc/>
	public override Canvas GetControl() => _canvas;

	/// <summary>
	/// Creates a list of <see cref="Shape"/> instances via the specified link view nodes.
	/// </summary>
	/// <param name="nodes">The link view nodes.</param>
	/// <returns>A <see cref="Shape"/> instance.</returns>
	private IEnumerable<Shape> CreateLinks(ObservableCollection<LinkViewNode> nodes)
	{
		var points = new HashSet<Point>();
		var linkArray = nodes.ToArray();
		foreach (var linkNode in linkArray)
		{
			points.Add(PointConversions.GetMouseCenter(PaneSize, OutsideOffset, linkNode.Start));
			points.Add(PointConversions.GetMouseCenter(PaneSize, OutsideOffset, linkNode.End));
		}

		foreach (var conclusion in Conclusions)
		{
			points.Add(PointConversions.GetMousePointInCenter(PaneSize, OutsideOffset, conclusion.Cell, conclusion.Digit));
		}

		var result = new List<Shape>();

		// Iterate on each inference to draw the links and grouped nodes (if so).
		double cs = PointConversions.CandidateSize(PaneSize, OutsideOffset);
		foreach (var (start, end, inference) in linkArray)
		{
			_ = PointConversions.GetMouseCenter(PaneSize, OutsideOffset, start) is var pt1 and var (pt1x, pt1y);
			_ = PointConversions.GetMouseCenter(PaneSize, OutsideOffset, end) is var pt2 and var (pt2x, pt2y);

			var endCap = inference != Inference.Default ? PenLineCap.Triangle : PenLineCap.Flat;
			var doubleCollection = inference switch
			{
				Inference.Strong => new(),
				Inference.Weak => new() { 3, 1.5 },
				_ => new DoubleCollection { 3, 3 }
			};

			if (inference == Inference.Default)
			{
				// Draw the link.
				result.Add(
					new Line()
						.WithStroke(Preference.LinkColor)
						.WithStrokeThickness(2)
						.WithStrokeDashArray(doubleCollection)
						.WithPoints(pt1, pt2)
						.WithStrokeEndLineCap(endCap)
				);
			}
			else
			{
				// If the distance of two points is lower than the one of two adjacent candidates,
				// the link will be emitted to draw because of too narrow.
				double distance = Sqrt((pt1x - pt2x) * (pt1x - pt2x) + (pt1y - pt2y) * (pt1y - pt2y));
				if (distance <= cs * SqrtOf2 + OutsideOffset || distance <= cs * SqrtOf2 + OutsideOffset)
				{
					continue;
				}

				// Check if another candidate lies in the direct line.
				double deltaX = pt2x - pt1x, deltaY = pt2y - pt1y;
				double alpha = Atan2(deltaY, deltaX);
				double dx1 = deltaX, dy1 = deltaY;
				bool through = false;
				adjust(pt1, pt2, out var p1, out _, alpha, cs, OutsideOffset);
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
				cut(ref pt1, ref pt2, OutsideOffset, cs, pt1x, pt1y, pt2x, pt2y);

				if (through)
				{
					double bezierLength = 20;

					// The end points are rotated 45 degrees
					// (counterclockwise for the start point, clockwise for the end point).
					Point oldPt1 = new(pt1x, pt1y), oldPt2 = new(pt2x, pt2y);
					rotate(oldPt1, ref pt1, -RotateAngle);
					rotate(oldPt2, ref pt2, RotateAngle);

					double aAlpha = alpha - RotateAngle;
					double bx1 = pt1.X + bezierLength * Cos(aAlpha), by1 = pt1.Y + bezierLength * Sin(aAlpha);

					aAlpha = alpha + RotateAngle;
					double bx2 = pt2.X - bezierLength * Cos(aAlpha), by2 = pt2.Y - bezierLength * Sin(aAlpha);

					result.Add(
						new Path()
							.WithStroke(Preference.LinkColor)
							.WithStrokeThickness(2)
							.WithStrokeDashArray(doubleCollection)
							.WithStrokeEndLineCap(endCap)
							.WithData(
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
							)
					);
				}
				else
				{
					// Draw the link.
					result.Add(
						new Line()
							.WithStroke(Preference.LinkColor)
							.WithStrokeThickness(2)
							.WithStrokeDashArray(doubleCollection)
							.WithPoints(pt1, pt2)
							.WithStrokeEndLineCap(endCap)
					);
				}
			}
		}

		return result;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void rotate(scoped in Point pt1, scoped ref Point pt2, double angle)
		{
			// Translate 'pt2' to (0, 0).
			pt2.X -= pt1.X;
			pt2.Y -= pt1.Y;

			// Rotate.
			double sinAngle = Sin(angle), cosAngle = Cos(angle);
			double xAct = pt2.X, yAct = pt2.Y;
			pt2.X = (float)(xAct * cosAngle - yAct * sinAngle);
			pt2.Y = (float)(xAct * sinAngle + yAct * cosAngle);

			pt2.X += pt1.X;
			pt2.Y += pt1.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void adjust(
			scoped in Point pt1, scoped in Point pt2, out Point p1, out Point p2,
			double alpha, double candidateSize, double offset)
		{
			p1 = pt1;
			p2 = pt2;
			double tempDelta = candidateSize / 2 + offset;
			int px = (int)(tempDelta * Cos(alpha)), py = (int)(tempDelta * Sin(alpha));

			p1.X += px;
			p1.Y += py;
			p2.X -= px;
			p2.Y -= py;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void cut(
			scoped ref Point pt1, scoped ref Point pt2, double offset, double cs,
			double pt1x, double pt1y, double pt2x, double pt2y)
		{
			double slope = Abs((pt2y - pt1y) / (pt2x - pt1x));
			double x = cs / (float)Sqrt(1 + slope * slope);
			double y = cs * (float)Sqrt(slope * slope / (1 + slope * slope));

			double o = offset / 8;
			if (pt1y > pt2y && pt1x.NearlyEquals(pt2x)) { pt1.Y -= cs / 2 - o; pt2.Y += cs / 2 - o; }
			else if (pt1y < pt2y && pt1x.NearlyEquals(pt2x)) { pt1.Y += cs / 2 - o; pt2.Y -= cs / 2 - o; }
			else if (pt1y.NearlyEquals(pt2y) && pt1x > pt2x) { pt1.X -= cs / 2 - o; pt2.X += cs / 2 - o; }
			else if (pt1y.NearlyEquals(pt2y) && pt1x < pt2x) { pt1.X += cs / 2 - o; pt2.X -= cs / 2 - o; }
			else if (pt1y > pt2y && pt1x > pt2x)
			{
				pt1.X -= x / 2 - o; pt1.Y -= y / 2 - o;
				pt2.X += x / 2 - o; pt2.Y += y / 2 - o;
			}
			else if (pt1y > pt2y && pt1x < pt2x)
			{
				pt1.X += x / 2 - o; pt1.Y -= y / 2 - o;
				pt2.X -= x / 2 - o; pt2.Y += y / 2 - o;
			}
			else if (pt1y < pt2y && pt1x > pt2x)
			{
				pt1.X -= x / 2 - o; pt1.Y += y / 2 - o;
				pt2.X += x / 2 - o; pt2.Y -= y / 2 - o;
			}
			else if (pt1y < pt2y && pt1x < pt2x)
			{
				pt1.X += x / 2 - o; pt1.Y += y / 2 - o;
				pt2.X -= x / 2 - o; pt2.Y -= y / 2 - o;
			}
		}
	}
}
