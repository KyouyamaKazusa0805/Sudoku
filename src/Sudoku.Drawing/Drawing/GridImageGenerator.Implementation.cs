using static Sudoku.Drawing.IGridImageGenerator;
using static Sudoku.Drawing.IPointCalculator;

namespace Sudoku.Drawing;

partial record class GridImageGenerator
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

		float cellWidth = Calculator.CellSize.Width;
		float candidateWidth = Calculator.CandidateSize.Width;
		float vOffsetValue = cellWidth / (AnchorsCount / 3); // The vertical offset of rendering each value.
		float vOffsetCandidate = candidateWidth / (AnchorsCount / 3); // The vertical offset of rendering each candidate.
		float halfWidth = cellWidth / 2F;

		using var bGiven = new SolidBrush(Preferences.GivenColor);
		using var bModifiable = new SolidBrush(Preferences.ModifiableColor);
		using var bCandidate = new SolidBrush(Preferences.CandidateColor);
		using var bCandidateLighter = new SolidBrush(
			Color.FromArgb(Preferences.CandidateColor.A >> 2, Preferences.CandidateColor)
		);
		using var fGiven = GetFont(
			Preferences.GivenFontName, halfWidth, Preferences.ValueScale, Preferences.GivenFontStyle
		);
		using var fModifiable = GetFont(
			Preferences.ModifiableFontName, halfWidth, Preferences.ValueScale, Preferences.ModifiableFontStyle
		);
		using var fCandidate = GetFont(
			Preferences.CandidateFontName, halfWidth, Preferences.CandidateScale, Preferences.CandidateFontStyle
		);

		for (int cell = 0; cell < 81; cell++)
		{
			short mask = Puzzle.GetMask(cell);
			switch (mask.MaskToStatus())
			{
				case CellStatus.Empty when Preferences.ShowCandidates:
				{
					// Draw candidates.
					bool overlaps = View.UnknownIdentifierOverlapsWithCell(cell);
					foreach (int digit in (short)(mask & SudokuGrid.MaxCandidatesMask))
					{
						var point = Calculator.GetMousePointInCenter(cell, digit).WithY(vOffsetCandidate);
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
					var point = Calculator.GetMousePointInCenter(cell).WithY(vOffsetValue);
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
		if (View.IsUndefiened)
		{
			return;
		}

		DrawRegions(g, offset);
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
		if (FocusedCells.IsEmpty)
		{
			return;
		}

		using var b = new SolidBrush(Preferences.FocusedCellColor);
		foreach (int cell in FocusedCells)
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
		for (int i = 0; i < length; i += AnchorsCount / 9)
		{
			g.DrawLine(pg, gridPoints[i, 0], gridPoints[i, AnchorsCount]);
			g.DrawLine(pg, gridPoints[0, i], gridPoints[AnchorsCount, i]);
		}

		for (int i = 0; i < length; i += AnchorsCount / 3)
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

		using var elimBrush = new SolidBrush(Preferences.EliminationColor);
		using var cannibalBrush = new SolidBrush(Preferences.CannibalismColor);
		using var elimBrushLighter = new SolidBrush(
			Color.FromArgb(Preferences.EliminationColor.A >> 2, Preferences.EliminationColor)
		);
		using var canniBrushLighter = new SolidBrush(
			Color.FromArgb(Preferences.CannibalismColor.A >> 2, Preferences.CannibalismColor)
		);
		foreach (var (t, c, d) in Conclusions)
		{
			if (t != ConclusionType.Elimination)
			{
				continue;
			}

			bool isCanni = false;
			if (View is not { Candidates: not null })
			{
				goto Drawing;
			}

			foreach (var (value, _) in View.Candidates)
			{
				if (value == c * 9 + d)
				{
					isCanni = true;
					break;
				}
			}

		Drawing:
			bool overlaps = View.UnknownIdentifierOverlapsWithCell(c);
			g.FillEllipse(
				isCanni ? overlaps ? canniBrushLighter : cannibalBrush : overlaps ? elimBrushLighter : elimBrush,
				Calculator.GetMouseRectangle(c, d).Zoom(-offset / 3)
			);
		}
	}

	/// <summary>
	/// Draw direct lines. The direct lines are the information for hidden singles and naked singles.
	/// </summary>
	/// <param name="g">The graphics.</param>
	/// <param name="offset">The drawing offset.</param>
	partial void DrawDirectLines(Graphics g, float offset)
	{
		if (View.DirectLines is not { } directLines)
		{
			return;
		}

		if (Preferences.ShowCandidates)
		{
			// Non-direct view (without candidates) don't show this function.
			return;
		}

		foreach (var ((start, end), _) in directLines)
		{
			// Draw start cells (may be a capsule-like shape to block them).
			if (!start.IsEmpty)
			{
				// Step 1: Get the left-up cell and right-down cell to construct a rectangle.
				var p1 = Calculator.GetMousePointInCenter(start[0]) - Calculator.CellSize / 2;
				var p2 = Calculator.GetMousePointInCenter(start[^1]) + Calculator.CellSize / 2;
				var rect = RectangleMarshal.CreateInstance(p1, p2).Zoom(-offset);

				// Step 2: Draw capsule.
				using var pen = new Pen(Preferences.CrosshatchingOutlineColor, 3F);
				using var brush = new SolidBrush(Preferences.CrosshatchingInnerColor);
				g.DrawEllipse(pen, rect);
				g.FillEllipse(brush, rect);
			}

			// Draw end cells (may be using cross sign to represent the current cell can't fill that digit).
			foreach (int cell in end)
			{
				// Step 1: Get the left-up cell and right-down cell to construct a rectangle.
				var rect = Calculator.GetMouseRectangleViaCell(cell).Zoom(-offset * 2);

				// Step 2: Draw cross sign.
				using var pen = new Pen(Preferences.CrossSignColor, 5F);
				g.DrawCrossSign(pen, rect);
			}
		}
	}

	/// <summary>
	/// Draw cells.
	/// </summary>
	/// <param name="g">The graphics.</param>
	partial void DrawCells(Graphics g)
	{
		if (View.Cells is not { } cells)
		{
			return;
		}

		foreach (var (cell, id) in cells)
		{
			if (id.UseId)
			{
				using var brush = new SolidBrush(Color.FromArgb(id.Color));
				g.FillRectangle(brush, Calculator.GetMouseRectangleViaCell(cell));
			}
			else if (Preferences.TryGetColor(id, out var color))
			{
				using var brush = new SolidBrush(Color.FromArgb(64, color));
				g.FillRectangle(brush, Calculator.GetMouseRectangleViaCell(cell));
			}
		}
	}

	/// <summary>
	/// Draw candidates.
	/// </summary>
	/// <param name="g">The graphics.</param>
	/// <param name="offset">The drawing offsets.</param>
	partial void DrawCandidates(Graphics g, float offset)
	{
		if (View.Candidates is not { } candidates)
		{
			return;
		}

		float cellWidth = Calculator.CellSize.Width;
		float candidateWidth = Calculator.CandidateSize.Width;
		float vOffsetCandidate = candidateWidth / 9; // The vertical offset of rendering each candidate.

		using var bCandidate = new SolidBrush(Preferences.CandidateColor);
		using var bCandidateLighter = new SolidBrush(
			Color.FromArgb(Preferences.CandidateColor.A >> 2, Preferences.CandidateColor)
		);
		using var fCandidate = GetFont(
			Preferences.CandidateFontName, cellWidth / 2F, Preferences.CandidateScale,
			Preferences.CandidateFontStyle
		);

		foreach (var (candidate, id) in candidates)
		{
			bool isOverlapped = false;
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
				bool overlaps = View.UnknownIdentifierOverlapsWithCell(cell);

				if (id.UseId)
				{
					using var brush = new SolidBrush(Color.FromArgb(overlaps ? id.A : id.A >> 2, id.R, id.G, id.B));
					g.FillEllipse(brush, Calculator.GetMouseRectangle(cell, digit).Zoom(-offset / 3));

					// In direct view, candidates should be drawn also.
					if (!Preferences.ShowCandidates)
					{
						d(cell, digit, vOffsetCandidate, overlaps ? bCandidateLighter : bCandidate);
					}
				}
				else if (Preferences.TryGetColor(id, out var color))
				{
					// In the normal case, I'll draw these circles.
					using var brush = new SolidBrush(overlaps ? Color.FromArgb(color.A >> 2, color) : color);
					g.FillEllipse(brush, Calculator.GetMouseRectangle(cell, digit).Zoom(-offset / 3));

					// In direct view, candidates should be drawn also.
					if (!Preferences.ShowCandidates)
					{
						d(cell, digit, vOffsetCandidate, overlaps ? bCandidateLighter : bCandidate);
					}
				}
			}
		}

		if (this is { Preferences.ShowCandidates: false, Conclusions: not null })
		{
			foreach (var (type, cell, digit) in Conclusions)
			{
				bool overlaps = View.UnknownIdentifierOverlapsWithCell(cell);
				if (type == ConclusionType.Elimination)
				{
					d(cell, digit, vOffsetCandidate, overlaps ? bCandidateLighter : bCandidate);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void d(int cell, int digit, float vOffsetCandidate, Brush brush)
		{
			var point = Calculator.GetMousePointInCenter(cell, digit).WithY(vOffsetCandidate);
			g.DrawValue(digit + 1, fCandidate, brush, point, DefaultStringFormat);
		}
	}

	/// <summary>
	/// Draw regions.
	/// </summary>
	/// <param name="g">The graphics.</param>
	/// <param name="offset">The drawing offset.</param>
	/// <remarks>This method is simply implemented, using cell filling.</remarks>
	partial void DrawRegions(Graphics g, float offset)
	{
		if (View.Regions is not { } regions)
		{
			return;
		}

		foreach (var (region, id) in regions)
		{
			bool assigned = false;
			Color color;
			if (id.UseId)
			{
				color = Color.FromArgb(id.Color);
				assigned = true;
			}
			else if (Preferences.TryGetColor(id, out color))
			{
				assigned = true;
			}

			if (!assigned)
			{
				continue;
			}

			if (Preferences.ShowLightRegion)
			{
				using var pen = new Pen(color, offset / 3 * 2);
				switch (region)
				{
					case >= 0 and < 9:
					{
						// Block.
						var rect = Calculator.GetMouseRectangleViaRegion(region).Zoom(-offset);
						g.DrawRoundedRectangle(pen, rect, offset);

						break;
					}
					case >= 9 and < 27:
					{
						var (l, r) = Calculator.GetAnchorsViaRegion(region);
						var (w, h) = Calculator.CellSize;
						w /= 2;
						h /= 2;
						l = l.WithOffset(w, h);
						r = r.WithOffset(-w, -h);

						g.DrawLine(pen, l, r);

						break;
					}
				}
			}
			else
			{
				var rect = Calculator.GetMouseRectangleViaRegion(region).Zoom(-offset / 3);
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
		if (View.Links is not { } links)
		{
			return;
		}

		// Gather all points used.
		var points = new HashSet<PointF>();
		var linkArray = links as (Link, ColorIdentifier)[] ?? links.ToArray();
		foreach (var ((startCand, endCand, _), _) in linkArray)
		{
			Candidates map1 = new() { startCand }, map2 = new() { endCand };

			points.Add(Calculator.GetMouseCenter(map1));
			points.Add(Calculator.GetMouseCenter(map2));
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
		using var linePen = new Pen(Preferences.ChainColor, 2F);
		using var arrowPen = new Pen(Preferences.ChainColor, 2F)
		{
			CustomEndCap = new AdjustableArrowCap(cw / 4F, ch / 3F)
		};

		foreach (var ((start, end, type), _) in linkArray)
		{
			arrowPen.DashStyle = type switch
			{
				LinkType.Strong => DashStyle.Solid,
				LinkType.Weak => DashStyle.Dot,
				_ => DashStyle.Dash
			};

			var pt1 = Calculator.GetMouseCenter(new() { start });
			var pt2 = Calculator.GetMouseCenter(new() { end });
			var (pt1x, pt1y) = pt1;
			var (pt2x, pt2y) = pt2;

			var penToDraw = type != LinkType.Line ? arrowPen : linePen;
			if (type == LinkType.Line)
			{
				// Draw the link.
				g.DrawLine(penToDraw, pt1, pt2);
			}
			else
			{
				// If the distance of two points is lower than the one of two adjacent candidates,
				// the link will be emitted to draw because of too narrow.
				double distance = Sqrt((pt1x - pt2x) * (pt1x - pt2x) + (pt1y - pt2y) * (pt1y - pt2y));
				if (distance <= cw * SqrtOf2 + offset || distance <= ch * SqrtOf2 + offset)
				{
					continue;
				}

				// Check if another candidate lies in the direct line.
				double deltaX = pt2x - pt1x, deltaY = pt2y - pt1y;
				double alpha = Atan2(deltaY, deltaX);
				double dx1 = deltaX, dy1 = deltaY;
				bool through = false;
				adjust(pt1, pt2, out var p1, out _, alpha, cw, offset);
				foreach (var point in points)
				{
					if (point == pt1 || point == pt2)
					{
						// Self...
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

					double aAlpha = alpha - RotateAngle;
					double bx1 = pt1.X + bezierLength * Cos(aAlpha), by1 = pt1.Y + bezierLength * Sin(aAlpha);

					aAlpha = alpha + RotateAngle;
					double bx2 = pt2.X - bezierLength * Cos(aAlpha), by2 = pt2.Y - bezierLength * Sin(aAlpha);

					g.DrawBezier(
						penToDraw, pt1.X, pt1.Y, (float)bx1, (float)by1, (float)bx2, (float)by2,
						pt2.X, pt2.Y
					);
				}
				else
				{
					// Draw the link.
					g.DrawLine(penToDraw, pt1, pt2);
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void rotate(in PointF pt1, ref PointF pt2, double angle)
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
			in PointF pt1,
			in PointF pt2,
			out PointF p1,
			out PointF p2,
			double alpha,
			double candidateSize,
			float offset
		)
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
			ref PointF pt1,
			ref PointF pt2,
			float offset,
			float cw,
			float ch,
			float pt1x,
			float pt1y,
			float pt2x,
			float pt2y
		)
		{
			float slope = Abs((pt2y - pt1y) / (pt2x - pt1x));
			float x = cw / (float)Sqrt(1 + slope * slope);
			float y = ch * (float)Sqrt(slope * slope / (1 + slope * slope));

			float o = offset / 8;
			if (pt1y > pt2y && pt1x.NearlyEquals(pt2x)) { pt1.Y -= ch / 2 - o; pt2.Y += ch / 2 - o; }
			else if (pt1y < pt2y && pt1x.NearlyEquals(pt2x)) { pt1.Y += ch / 2 - o; pt2.Y -= ch / 2 - o; }
			else if (pt1y.NearlyEquals(pt2y) && pt1x > pt2x) { pt1.X -= cw / 2 - o; pt2.X += cw / 2 - o; }
			else if (pt1y.NearlyEquals(pt2y) && pt1x < pt2x) { pt1.X += cw / 2 - o; pt2.X -= cw / 2 - o; }
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

	/// <summary>
	/// Draw unknown values.
	/// </summary>
	/// <param name="g">The graphics.</param>
	partial void DrawUnknownValue(Graphics g)
	{
		if (View.UnknownValues is not { } unknownValues)
		{
			return;
		}

		const string defaultFontName = "Times New Roman";

		float cellWidth = Calculator.CellSize.Width;
		float vOffsetValue = cellWidth / (AnchorsCount / 3); // The vertical offset of rendering each value.
		float halfWidth = cellWidth / 2F;

		using var brush = new SolidBrush(Preferences.UnknownIdentifierColor);
		using var font = GetFont(
			defaultFontName, halfWidth, Preferences.ValueScale, Preferences.UnknownIdentfierFontStyle
		);

		foreach (var ((cell, character, _), _) in unknownValues)
		{
			// Draw values.
			var point = Calculator.GetMousePointInCenter(cell).WithY(vOffsetValue);
			g.DrawValue(character, font, brush, point, DefaultStringFormat);
		}
	}
}
