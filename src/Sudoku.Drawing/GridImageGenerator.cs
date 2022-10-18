namespace Sudoku.Drawing;

/// <summary>
/// Defines and encapsulates a data structure that provides the operations to draw a sudoku puzzle.
/// </summary>
/// <param name="Calculator">
/// Indicates the <see cref="IPointCalculator"/> instance that calculates the pixels to help the inner
/// methods to handle and draw the picture used for displaying onto the UI projects.
/// </param>
/// <param name="Preferences">
/// Indicates the <see cref="IPreference"/> instance that stores the default preferences
/// that decides the drawing behavior.
/// </param>
/// <param name="Puzzle">Indicates the puzzle.</param>
public partial record GridImageGenerator(IPointCalculator Calculator, IPreference Preferences, scoped in Grid Puzzle) : IGridImageGenerator
{
	/// <inheritdoc/>
	public float Width
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Calculator.Width;
	}

	/// <inheritdoc/>
	public float Height
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Calculator.Height;
	}

	/// <inheritdoc/>
	public CellMap FocusedCells { get; set; }

	/// <inheritdoc/>
	public View? View { get; set; }

	/// <inheritdoc/>
	public IEnumerable<Conclusion>? Conclusions { get; set; }


	/// <inheritdoc/>
	public Image DrawManually()
	{
		var result = new Bitmap((int)Width, (int)Height);
		using var g = Graphics.FromImage(result);
		return Draw(result, g);
	}

	/// <summary>
	/// To draw the image.
	/// </summary>
	/// <param name="bitmap">The bitmap result.</param>
	/// <param name="g">The graphics instance.</param>
	/// <returns>
	/// The return value is the same as the parameter <paramref name="bitmap"/> when
	/// this parameter is not <see langword="null"/>.
	/// </returns>
	public Image Draw([AllowNull] Image bitmap, Graphics g)
	{
		bitmap ??= new Bitmap((int)Width, (int)Height);

		DrawBackground(g);
		DrawGridAndBlockLines(g);

		g.SmoothingMode = SmoothingMode.AntiAlias;
		g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
		g.InterpolationMode = InterpolationMode.HighQualityBicubic;
		g.CompositingQuality = CompositingQuality.HighQuality;

		DrawView(g, TextOffset);
		DrawFocusedCells(g);
		DrawEliminations(g, TextOffset);
		DrawValue(g);

		return bitmap;
	}

	/// <summary>
	/// Gets the color value.
	/// </summary>
	/// <param name="id">The identifier instance.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Throws when the specified value is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Color GetColor(Identifier id)
		=> id switch
		{
			{ Mode: IdentifierColorMode.Raw, A: var alpha, R: var red, G: var green, B: var blue }
				=> Color.FromArgb(alpha, red, green, blue),
			{ Mode: IdentifierColorMode.Id } when Preferences.TryGetColor(id, out var color)
				=> Color.FromArgb(64, color),
			{ Mode: IdentifierColorMode.Named, NamedKind: var namedKind }
				=> namedKind switch
				{
					DisplayColorKind.Normal => Preferences.Color1,
					DisplayColorKind.Elimination => Preferences.EliminationColor,
					DisplayColorKind.Exofin => Preferences.Color2,
					DisplayColorKind.Endofin => Preferences.Color3,
					DisplayColorKind.Cannibalism => Preferences.CannibalismColor,
					DisplayColorKind.Link => Preferences.ChainColor,
					_ => throw new InvalidOperationException("Such displaying color kind is invalid.")
				},
			_ => throw new InvalidOperationException("Such identifier instance contains invalid value.")
		};


	partial void DrawGridAndBlockLines(Graphics g);
	partial void DrawBackground(Graphics g);
	partial void DrawValue(Graphics g);
	partial void DrawFocusedCells(Graphics g);
	partial void DrawView(Graphics g, float offset);
	partial void DrawEliminations(Graphics g, float offset);
	partial void DrawCells(Graphics g);
	partial void DrawCandidates(Graphics g, float offset);
	partial void DrawHouses(Graphics g, float offset);
	partial void DrawLinks(Graphics g, float offset);
	partial void DrawDirectLines(Graphics g, float offset);
	partial void DrawUnknownValue(Graphics g);
}

partial record GridImageGenerator
{
	/// <summary>
	/// Draw givens, modifiables and candidates, where the values are specified as a grid.
	/// </summary>
	/// <param name="g">The graphics.</param>
	partial void DrawValue(Graphics g)
	{
		if (Puzzle.IsUndefined)
		{
			return;
		}

		var cellWidth = Calculator.CellSize.Width;
		var candidateWidth = Calculator.CandidateSize.Width;
		var vOffsetValue = cellWidth / (AnchorsCount / 3); // The vertical offset of rendering each value.
		var vOffsetCandidate = candidateWidth / (AnchorsCount / 3); // The vertical offset of rendering each candidate.
		var halfWidth = cellWidth / 2F;

		using SolidBrush
			bGiven = new(Preferences.GivenColor),
			bModifiable = new(Preferences.ModifiableColor),
			bCandidate = new(Preferences.CandidateColor),
			bCandidateLighter = new(Color.FromArgb(Preferences.CandidateColor.A >> 2, Preferences.CandidateColor));
		using Font
			fGiven = GetFont(Preferences.GivenFontName, halfWidth, Preferences.ValueScale, Preferences.GivenFontStyle),
			fModifiable = GetFont(Preferences.ModifiableFontName, halfWidth, Preferences.ValueScale, Preferences.ModifiableFontStyle),
			fCandidate = GetFont(Preferences.CandidateFontName, halfWidth, Preferences.CandidateScale, Preferences.CandidateFontStyle);

		for (var cell = 0; cell < 81; cell++)
		{
			var mask = Puzzle.GetMask(cell);
			switch (MaskToStatus(mask))
			{
				case CellStatus.Empty when Preferences.ShowCandidates:
				{
					// Draw candidates.
					var overlaps = View.UnknownOverlaps(cell);
					foreach (var digit in (short)(mask & Grid.MaxCandidatesMask))
					{
						var originalPoint = Calculator.GetMousePointInCenter(cell, digit);
						var point = originalPoint with { Y = originalPoint.Y + vOffsetCandidate };
						g.DrawValue(
							digit + 1, fCandidate, overlaps ? bCandidateLighter : bCandidate,
							point, DefaultStringFormat
						);
					}

					break;
				}
				case var status and (CellStatus.Modifiable or CellStatus.Given):
				{
					// Draw values.
					var originalPoint = Calculator.GetMousePointInCenter(cell);
					var point = originalPoint with { Y = originalPoint.Y + vOffsetValue };
					g.DrawValue(
						Puzzle[cell] + 1, status == CellStatus.Given ? fGiven : fModifiable,
						status == CellStatus.Given ? bGiven : bModifiable, point, DefaultStringFormat
					);

					break;
				}
			}
		}
	}

	/// <summary>
	/// Draw custom view if <see cref="View"/> is not <see langword="null"/>.
	/// </summary>
	/// <param name="g">The graphics.</param>
	/// <param name="offset">The drawing offset.</param>
	/// <seealso cref="View"/>
	partial void DrawView(Graphics g, float offset)
	{
		if (View is null)
		{
			return;
		}

		DrawHouses(g, offset);
		DrawCells(g);
		DrawCandidates(g, offset);
		DrawLinks(g, offset);
		DrawDirectLines(g, offset);
		DrawUnknownValue(g);
	}

	/// <summary>
	/// Draw focused cells.
	/// </summary>
	/// <param name="g">The graphics.</param>
	partial void DrawFocusedCells(Graphics g)
	{
		if (!FocusedCells)
		{
			return;
		}

		using var b = new SolidBrush(Preferences.FocusedCellColor);
		foreach (var cell in FocusedCells)
		{
			g.FillRectangle(b, Calculator.GetMouseRectangleViaCell(cell));
		}
	}

	/// <summary>
	/// Draw the background, where the color is specified in <see cref="IPreference.BackgroundColor"/>.
	/// </summary>
	/// <param name="g">The graphics.</param>
	/// <seealso cref="IPreference.BackgroundColor"/>
	partial void DrawBackground(Graphics g) => g.Clear(Preferences.BackgroundColor);

	/// <summary>
	/// Draw grid lines and block lines.
	/// </summary>
	/// <param name="g">The graphics.</param>
	partial void DrawGridAndBlockLines(Graphics g)
	{
		using var pg = new Pen(Preferences.GridLineColor, Preferences.GridLineWidth);
		using var pb = new Pen(Preferences.BlockLineColor, Preferences.BlockLineWidth);
		const int length = AnchorsCount + 1;
		var gridPoints = Calculator.GridPoints;
		for (var i = 0; i < length; i += AnchorsCount / 9)
		{
			g.DrawLine(pg, gridPoints[i, 0], gridPoints[i, AnchorsCount]);
			g.DrawLine(pg, gridPoints[0, i], gridPoints[AnchorsCount, i]);
		}

		for (var i = 0; i < length; i += AnchorsCount / 3)
		{
			g.DrawLine(pb, gridPoints[i, 0], gridPoints[i, AnchorsCount]);
			g.DrawLine(pb, gridPoints[0, i], gridPoints[AnchorsCount, i]);
		}
	}

	/// <summary>
	/// Draw eliminations.
	/// </summary>
	/// <param name="g">The graphics.</param>
	/// <param name="offset">The drawing offset.</param>
	partial void DrawEliminations(Graphics g, float offset)
	{
		if (Conclusions is null)
		{
			return;
		}

		using SolidBrush
			elimBrush = new(Preferences.EliminationColor),
			cannibalBrush = new(Preferences.CannibalismColor),
			elimBrushLighter = new(Color.FromArgb(Preferences.EliminationColor.A >> 2, Preferences.EliminationColor)),
			canniBrushLighter = new(Color.FromArgb(Preferences.CannibalismColor.A >> 2, Preferences.CannibalismColor));
		foreach (var (t, c, d) in Conclusions)
		{
			if (t != ConclusionType.Elimination)
			{
				continue;
			}

			var isCanni = false;
			if (View is not { CandidateNodes: var candidateNodes })
			{
				goto Drawing;
			}

			foreach (var candidateNode in candidateNodes)
			{
				var value = candidateNode.Candidate;
				if (value == c * 9 + d)
				{
					isCanni = true;
					break;
				}
			}

		Drawing:
			var overlaps = View.UnknownOverlaps(c);
			g.FillEllipse(
				isCanni ? overlaps ? canniBrushLighter : cannibalBrush : overlaps ? elimBrushLighter : elimBrush,
				Calculator.GetMouseRectangle(c, d).Zoom(-offset / 3));
		}
	}

	/// <summary>
	/// Draw cells.
	/// </summary>
	/// <param name="g">The graphics.</param>
	partial void DrawCells(Graphics g)
	{
		if (View is not { CellNodes: var cellNodes })
		{
			return;
		}

		foreach (var cellNode in cellNodes)
		{
			var cell = cellNode.Cell;
			var id = cellNode.Identifier;
			using var brush = new SolidBrush(GetColor(id));

			g.FillRectangle(brush, Calculator.GetMouseRectangleViaCell(cell));
		}
	}

	/// <summary>
	/// Draw candidates.
	/// </summary>
	/// <param name="g">The graphics.</param>
	/// <param name="offset">The drawing offsets.</param>
	partial void DrawCandidates(Graphics g, float offset)
	{
		if (View is not { CandidateNodes: var candidateNodes })
		{
			return;
		}

		var cellWidth = Calculator.CellSize.Width;
		var candidateWidth = Calculator.CandidateSize.Width;
		var vOffsetCandidate = candidateWidth / 9; // The vertical offset of rendering each candidate.

		using SolidBrush
			bCandidate = new(Preferences.CandidateColor),
			bCandidateLighter = new(Color.FromArgb(Preferences.CandidateColor.A >> 2, Preferences.CandidateColor));
		using var fCandidate = GetFont(Preferences.CandidateFontName, cellWidth / 2F, Preferences.CandidateScale, Preferences.CandidateFontStyle);

		foreach (var candidateNode in candidateNodes)
		{
			var candidate = candidateNode.Candidate;
			var id = candidateNode.Identifier;

			var isOverlapped = false;
			if (Conclusions is null)
			{
				goto IsOverlapped;
			}

			foreach (var (concType, concCandidate) in Conclusions)
			{
				if (concType == ConclusionType.Elimination && concCandidate == candidate)
				{
					isOverlapped = true;
					break;
				}
			}

		IsOverlapped:
			if (!isOverlapped)
			{
				int cell = candidate / 9, digit = candidate % 9;
				var overlaps = View.UnknownOverlaps(cell);

				switch (id)
				{
					case { Mode: IdentifierColorMode.Raw, A: var alpha, R: var red, G: var green, B: var blue }:
					{
						using var brush = new SolidBrush(Color.FromArgb(overlaps ? alpha : alpha >> 2, red, green, blue));
						g.FillEllipse(brush, Calculator.GetMouseRectangle(cell, digit).Zoom(-offset / 3));

						// In direct view, candidates should be drawn also.
						if (!Preferences.ShowCandidates)
						{
							d(cell, digit, vOffsetCandidate, overlaps ? bCandidateLighter : bCandidate);
						}

						break;
					}
					case { Mode: var mode and (IdentifierColorMode.Id or IdentifierColorMode.Named) }:
					{
						var color = mode == IdentifierColorMode.Id && Preferences.TryGetColor(id, out var c)
							? c
							: mode == IdentifierColorMode.Named
								? GetColor(id)
								: throw new InvalidOperationException();

						// In the normal case, I'll draw these circles.
						using var brush = new SolidBrush(overlaps ? Color.FromArgb(color.A >> 2, color) : color);
						g.FillEllipse(brush, Calculator.GetMouseRectangle(cell, digit).Zoom(-offset / 3));

						// In direct view, candidates should be drawn also.
						if (!Preferences.ShowCandidates)
						{
							d(cell, digit, vOffsetCandidate, overlaps ? bCandidateLighter : bCandidate);
						}

						break;
					}
				}
			}
		}

		if (this is { Preferences.ShowCandidates: false, Conclusions: not null })
		{
			foreach (var (type, cell, digit) in Conclusions)
			{
				var overlaps = View.UnknownOverlaps(cell);
				if (type == ConclusionType.Elimination)
				{
					d(cell, digit, vOffsetCandidate, overlaps ? bCandidateLighter : bCandidate);
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void d(int cell, int digit, float vOffsetCandidate, Brush brush)
		{
			var originalPoint = Calculator.GetMousePointInCenter(cell, digit);
			var point = originalPoint with { Y = originalPoint.Y + vOffsetCandidate };
			g.DrawValue(digit + 1, fCandidate, brush, point, DefaultStringFormat);
		}
	}

	/// <summary>
	/// Draw houses.
	/// </summary>
	/// <param name="g">The graphics.</param>
	/// <param name="offset">The drawing offset.</param>
	/// <remarks>This method is simply implemented, using cell filling.</remarks>
	partial void DrawHouses(Graphics g, float offset)
	{
		if (View is not { HouseNodes: var houseNodes })
		{
			return;
		}

		foreach (var houseNode in houseNodes)
		{
			var house = houseNode.House;
			var id = houseNode.Identifier;

			Color? tempColor;
			try
			{
				tempColor = GetColor(id);
			}
			catch (InvalidOperationException)
			{
				tempColor = null;
			}
			if (tempColor is not { } color)
			{
				continue;
			}

			if (Preferences.ShowLightHouse)
			{
				using var pen = new Pen(color, offset / 3 * 2);
				switch (house)
				{
					case >= 0 and < 9:
					{
						// Block.
						var rect = Calculator.GetMouseRectangleViaHouse(house).Zoom(-offset);
						g.DrawRoundedRectangle(pen, rect, offset);

						break;
					}
					case >= 9 and < 27:
					{
						var (l, r) = Calculator.GetAnchorsViaHouse(house);
						var (w, h) = Calculator.CellSize;
						w /= 2;
						h /= 2;
						l = l with { X = l.X + w, Y = l.Y + h };
						r = r with { X = r.X - w, Y = r.Y - h };

						g.DrawLine(pen, l, r);

						break;
					}
				}
			}
			else
			{
				var rect = Calculator.GetMouseRectangleViaHouse(house).Zoom(-offset / 3);
				using var brush = new SolidBrush(Color.FromArgb(64, color));
				g.FillRectangle(brush, rect);
			}
		}
	}

	/// <summary>
	/// Draw links.
	/// </summary>
	/// <param name="g">The graphics.</param>
	/// <param name="offset">The offset.</param>
	partial void DrawLinks(Graphics g, float offset)
	{
		if (View is not { LinkNodes: var links })
		{
			return;
		}

		// Gather all points used.
		var points = new HashSet<PointF>();
		var linkArray = links as LinkViewNode[] ?? links.ToArray();
		foreach (var linkNode in linkArray)
		{
			points.Add(Calculator.GetMouseCenter(linkNode.Start));
			points.Add(Calculator.GetMouseCenter(linkNode.End));
		}

		if (Conclusions is not null)
		{
			foreach (var conclusion in Conclusions)
			{
				points.Add(Calculator.GetMousePointInCenter(conclusion.Cell, conclusion.Digit));
			}
		}

		// Iterate on each inference to draw the links and grouped nodes (if so).
		var (cw, ch) = Calculator.CandidateSize;
		using Pen
			linePen = new(Preferences.ChainColor, 2F),
			arrowPen = new(Preferences.ChainColor, 2F) { CustomEndCap = new AdjustableArrowCap(cw / 4F, ch / 3F) };

		foreach (var linkNode in linkArray)
		{
			var start = linkNode.Start;
			var end = linkNode.End;
			var inference = linkNode.Inference;

			arrowPen.DashStyle = inference switch
			{
				Inference.Strong => DashStyle.Solid,
				Inference.Weak => DashStyle.Dot,
				_ => DashStyle.Dash
			};

			PointF pt1, pt2;
			var (pt1x, pt1y) = pt1 = Calculator.GetMouseCenter(start);
			var (pt2x, pt2y) = pt2 = Calculator.GetMouseCenter(end);

			var penToDraw = inference != Inference.Default ? arrowPen : linePen;
			if (inference == Inference.Default)
			{
				// Draw the link.
				g.DrawLine(penToDraw, pt1, pt2);
			}
			else
			{
				// If the distance of two points is lower than the one of two adjacent candidates,
				// the link will be emitted to draw because of too narrow.
				var distance = Sqrt((pt1x - pt2x) * (pt1x - pt2x) + (pt1y - pt2y) * (pt1y - pt2y));
				if (distance <= cw * SqrtOf2 + offset || distance <= ch * SqrtOf2 + offset)
				{
					continue;
				}

				// Check if another candidate lies in the direct line.
				double deltaX = pt2x - pt1x, deltaY = pt2y - pt1y;
				var alpha = Atan2(deltaY, deltaX);
				double dx1 = deltaX, dy1 = deltaY;
				var through = false;
				adjust(pt1, pt2, out var p1, out _, alpha, cw, offset);
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
				cut(ref pt1, ref pt2, offset, cw, ch, pt1x, pt1y, pt2x, pt2y);

				if (through)
				{
					double bezierLength = 20;

					// The end points are rotated 45 degrees
					// (counterclockwise for the start point, clockwise for the end point).
					PointF oldPt1 = new(pt1x, pt1y), oldPt2 = new(pt2x, pt2y);
					rotate(oldPt1, ref pt1, -RotateAngle);
					rotate(oldPt2, ref pt2, RotateAngle);

					var aAlpha = alpha - RotateAngle;
					double bx1 = pt1.X + bezierLength * Cos(aAlpha), by1 = pt1.Y + bezierLength * Sin(aAlpha);

					aAlpha = alpha + RotateAngle;
					double bx2 = pt2.X - bezierLength * Cos(aAlpha), by2 = pt2.Y - bezierLength * Sin(aAlpha);

					g.DrawBezier(penToDraw, pt1.X, pt1.Y, (float)bx1, (float)by1, (float)bx2, (float)by2, pt2.X, pt2.Y);
				}
				else
				{
					// Draw the link.
					g.DrawLine(penToDraw, pt1, pt2);
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void rotate(scoped in PointF pt1, scoped ref PointF pt2, double angle)
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
			scoped in PointF pt1, scoped in PointF pt2, out PointF p1, out PointF p2,
			double alpha, double candidateSize, float offset)
		{
			p1 = pt1;
			p2 = pt2;
			var tempDelta = candidateSize / 2 + offset;
			int px = (int)(tempDelta * Cos(alpha)), py = (int)(tempDelta * Sin(alpha));

			p1.X += px;
			p1.Y += py;
			p2.X -= px;
			p2.Y -= py;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void cut(
			scoped ref PointF pt1, scoped ref PointF pt2, float offset, float cw, float ch,
			float pt1x, float pt1y, float pt2x, float pt2y)
		{
			var slope = Abs((pt2y - pt1y) / (pt2x - pt1x));
			var x = cw / (float)Sqrt(1 + slope * slope);
			var y = ch * (float)Sqrt(slope * slope / (1 + slope * slope));

			var o = offset / 8;
			if (pt1y > pt2y && pt1x.NearlyEquals(pt2x)) { pt1.Y -= ch / 2 - o; pt2.Y += ch / 2 - o; }
			else if (pt1y < pt2y && pt1x.NearlyEquals(pt2x)) { pt1.Y += ch / 2 - o; pt2.Y -= ch / 2 - o; }
			else if (pt1y.NearlyEquals(pt2y) && pt1x > pt2x) { pt1.X -= cw / 2 - o; pt2.X += cw / 2 - o; }
			else if (pt1y.NearlyEquals(pt2y) && pt1x < pt2x) { pt1.X += cw / 2 - o; pt2.X -= cw / 2 - o; }
			else if (pt1y > pt2y && pt1x > pt2x) { pt1.X -= x / 2 - o; pt1.Y -= y / 2 - o; pt2.X += x / 2 - o; pt2.Y += y / 2 - o; }
			else if (pt1y > pt2y && pt1x < pt2x) { pt1.X += x / 2 - o; pt1.Y -= y / 2 - o; pt2.X -= x / 2 - o; pt2.Y += y / 2 - o; }
			else if (pt1y < pt2y && pt1x > pt2x) { pt1.X -= x / 2 - o; pt1.Y += y / 2 - o; pt2.X += x / 2 - o; pt2.Y -= y / 2 - o; }
			else if (pt1y < pt2y && pt1x < pt2x) { pt1.X += x / 2 - o; pt1.Y += y / 2 - o; pt2.X -= x / 2 - o; pt2.Y -= y / 2 - o; }
		}
	}

	/// <summary>
	/// Draw unknown values.
	/// </summary>
	/// <param name="g">The graphics.</param>
	partial void DrawUnknownValue(Graphics g)
	{
		if (View is not { UnknownNodes: var unknownNodes })
		{
			return;
		}

		const string defaultFontName = "Times New Roman";

		var cellWidth = Calculator.CellSize.Width;
		var vOffsetValue = cellWidth / (AnchorsCount / 3); // The vertical offset of rendering each value.
		var halfWidth = cellWidth / 2F;

		using var brush = new SolidBrush(Preferences.UnknownIdentifierColor);
		using var font = GetFont(defaultFontName, halfWidth, Preferences.ValueScale, Preferences.UnknownFontStyle);

		foreach (var unknownNode in unknownNodes)
		{
			var cell = unknownNode.Cell;
			var character = unknownNode.UnknownValueChar;

			// Draw values.
			var orginalPoint = Calculator.GetMousePointInCenter(cell);
			var point = orginalPoint with { Y = orginalPoint.Y + vOffsetValue };
			g.DrawString(character.ToString(), font, brush, point, DefaultStringFormat);
		}
	}
}
