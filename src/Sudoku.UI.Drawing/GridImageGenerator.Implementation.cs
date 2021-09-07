namespace Sudoku.UI.Drawing;

partial class GridImageGenerator
{
	/// <summary>
	/// Draw givens, modifiables and candidates, where the values are specified as a grid.
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	partial void DrawValue(Grid g)
	{
		if (this is not { Puzzle.IsUndefined: false, Calculator: not null, Preferences: not null })
		{
			return;
		}

		double cellWidth = Calculator.CellSize.Width, halfWidth = cellWidth / 2;

		var (cA, cR, cG, cB) = Preferences.CandidateColor;
		SolidColorBrush
			bGiven = new(Preferences.GivenColor),
			bMod = new(Preferences.ModifiableColor),
			bCand = new(Preferences.CandidateColor),
			bCandLighter = new(Color.FromArgb((byte)(cA >> 2), cR, cG, cB));
		(FontFamily Font, double FontSize)
			fpGiven = GetFont(Preferences.GivenFontName, halfWidth, Preferences.ValueScale),
			fpMod = GetFont(Preferences.ModifiableFontName, halfWidth, Preferences.ValueScale),
			fpCand = GetFont(Preferences.CandidateFontName, halfWidth, Preferences.CandidateScale);

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
						var (r, c) = Calculator.GetGridRowAndColumn(cell, digit);

						g.AddText(
							r, c, digit, overlaps ? bCandLighter : bCand, fpCand.Font, fpCand.FontSize,
							fontStyle: Preferences.CandidateFontStyle
						);
					}

					break;
				}
				case CellStatus.Modifiable:
				{
					// Draw modifiables.
					var (r, c) = Calculator.GetGridRowAndColumn(cell);

					g.AddText(
						r, c, Puzzle[cell] + 1, bGiven, fpGiven.Font, fpGiven.FontSize,
						fontStyle: Preferences.ModifiableFontStyle
					);

					break;
				}
				case CellStatus.Given:
				{
					// Draw givens.
					var (r, c) = Calculator.GetGridRowAndColumn(cell);

					g.AddText(
						r, c, Puzzle[cell] + 1, bMod, fpMod.Font, fpMod.FontSize,
						fontStyle: Preferences.GivenFontStyle
					);

					break;
				}
			}
		}
	}

	/// <summary>
	/// Draw the view (i.e. property <see cref="View"/>).
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	/// <seealso cref="View"/>
	partial void DrawView(Grid g)
	{
		if (!View.IsUndefiened)
		{
			DrawRegions(g);
			DrawCells(g);
			DrawCandidates(g);
			DrawLinks(g);
			DrawDirectLines(g);
			DrawUnknownValue(g);
		}
	}

	/// <summary>
	/// Draw focused cells.
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	partial void DrawFocusedCells(Grid g)
	{
		if (this is not { FocusedCells.IsEmpty: false, Calculator: not null, Preferences: not null })
		{
			return;
		}

		var bFocusedCells = new SolidColorBrush(Preferences.FocusedCellColor);
		foreach (int cell in FocusedCells)
		{
			var (r, c) = Calculator.GetGridRowAndColumn(cell);

			g.AddRectangle(r, c, 3, 3, bFocusedCells);
		}
	}

	/// <summary>
	/// Draw the background where the color is specified in <see cref="IPreference.BackgroundColor"/>.
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	/// <seealso cref="IPreference.BackgroundColor"/>
	partial void DrawBackground(Grid g)
	{
		if (this is not { Calculator.ControlSize: var (x, y), Preferences.BackgroundColor: var color })
		{
			return;
		}

		g.AddBackground(x, y, new SolidColorBrush(color));
	}

	/// <summary>
	/// Draw grid lines and block lines.
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	partial void DrawGridAndBlockLines(Grid g)
	{
		if (this is not { Calculator: not null, Preferences: not null })
		{
			return;
		}

		const int length = PointCalculator.AnchorsCount + 1;
		var gridPoints = Calculator.GridPoints;
		for (int i = 0; i < length; i += PointCalculator.AnchorsCount / 9)
		{
			g.AddLine(
				gridPoints[i, 0], gridPoints[i, PointCalculator.AnchorsCount],
				new SolidColorBrush(Preferences.GridLineColor), Preferences.GridLineWidth
			);
			g.AddLine(
				gridPoints[0, i], gridPoints[PointCalculator.AnchorsCount, i],
				new SolidColorBrush(Preferences.GridLineColor), Preferences.GridLineWidth
			);
		}

		for (int i = 0; i < length; i += PointCalculator.AnchorsCount / 3)
		{
			g.AddLine(
				gridPoints[i, 0], gridPoints[i, PointCalculator.AnchorsCount],
				new SolidColorBrush(Preferences.BlockLineColor), Preferences.BlockLineWidth
			);
			g.AddLine(
				gridPoints[0, i], gridPoints[PointCalculator.AnchorsCount, i],
				new SolidColorBrush(Preferences.BlockLineColor), Preferences.BlockLineWidth
			);
		}
	}

	/// <summary>
	/// Draw eliminations.
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	partial void DrawEliminations(Grid g)
	{
		if (this is not { Conclusions: not null, Preferences: not null, Calculator: not null })
		{
			return;
		}

		var (eA, eR, eG, eB) = Preferences.EliminationColor;
		var (cA, cR, cG, cB) = Preferences.CannibalismColor;
		var elimBrush = new SolidColorBrush(Preferences.EliminationColor);
		var cannibalBrush = new SolidColorBrush(Preferences.CannibalismColor);
		var elimBrushLighter = new SolidColorBrush(Color.FromArgb((byte)(eA >> 2), eR, eG, eB));
		var canniBrushLighter = new SolidColorBrush(Color.FromArgb((byte)(cA >> 2), cR, cG, cB));
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
			var (row, column) = Calculator.GetGridRowAndColumn(c, d);
			var (width, _) = Calculator.CandidateSize;
			g.AddCircle(row, column, width, (IsCannibal: isCanni, Overlaps: overlaps) switch
			{
				(IsCannibal: true, Overlaps: true) => canniBrushLighter,
				(IsCannibal: true, Overlaps: false) => cannibalBrush,
				(IsCannibal: false, Overlaps: true) => elimBrushLighter,
				(IsCannibal: false, Overlaps: false) => elimBrush
			});
		}
	}

	/// <summary>
	/// Draw direct lines. The direct lines are the information for hidden singles and naked singles.
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	partial void DrawDirectLines(Grid g)
	{
		if (
			this is not
			{
				Calculator: not null,
				View.DirectLines: { } directLines,
				Preferences.ShowCandidates: false
			}
		)
		{
			return;
		}

		const double o = 5;
		foreach (var ((start, end), _) in directLines)
		{
			// Draw start cells (may be a capsule-like shape to block them).
			if (!start.IsEmpty)
			{
				var (row, column) = Calculator.GetGridRowAndColumn(start[0]);
				var (size, _) = Calculator.CellSize;

				g.AddCircle(
					row, column, size,
					new SolidColorBrush(Preferences.CrosshatchingOutlineColor),
					new SolidColorBrush(Preferences.CrosshatchingInnerColor),
					Preferences.CrosshatchingOutlineWidth
				);
			}

			// Draw end cells (may be using cross sign to represent the current cell can't fill that digit).
			foreach (int cell in end)
			{
				var (row, column) = Calculator.GetGridRowAndColumn(cell);
				var (p1X, p1Y) = Calculator.GridPoints[row, column];
				var (p2X, p2Y) = Calculator.GridPoints[row + 1, column + 1];
				var (p3X, p3Y) = Calculator.GridPoints[row, column + 1];
				var (p4X, p4Y) = Calculator.GridPoints[row + 1, column];

				g.AddCross(
					p1X + o, p1Y + o, p2X - o, p2Y - o, p3X - o, p3Y + o, p4X + o, p4Y - o,
					new SolidColorBrush(Preferences.CrossSignColor),
					Preferences.CrossSignWidth
				);
			}
		}
	}

	/// <summary>
	/// Draw cells.
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	partial void DrawCells(Grid g)
	{
		if (this is not { View.Cells: { } cells, Calculator: not null, Preferences: not null })
		{
			return;
		}

		const int z = PointCalculator.AnchorsCount / 9;
		foreach (var (cell, id) in cells)
		{
			var (row, column) = Calculator.GetGridRowAndColumn(cell);
			if (id is { UseId: false, A: var cA, R: var cR, G: var cG, B: var cB })
			{
				g.AddRectangle(
					row, column, z, z, new SolidColorBrush(Color.FromArgb(cA, cR, cG, cB))
				);
			}
			else if (((IPreference)Preferences).TryGetColor(id, out var color))
			{
				g.AddRectangle(
					row, column, z, z, new SolidColorBrush(Color.FromArgb(64, color.R, color.G, color.B))
				);
			}
		}
	}

	/// <summary>
	/// Draw candidates.
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	partial void DrawCandidates(Grid g)
	{
		if (
			this is not
			{
				Calculator.CellSize.Width: var cellWidth,
				View.Candidates: { } candidates,
				Preferences.CandidateColor: var (cA, cR, cG, cB)
			}
		)
		{
			return;
		}

		var bCand = new SolidColorBrush(Preferences.CandidateColor);
		var bCandLighter = new SolidColorBrush(Color.FromArgb((byte)(cA >> 2), cR, cG, cB));

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
				var (row, column) = Calculator.GetGridRowAndColumn(cell, digit);
				var (size, _) = Calculator.CellSize;
				bool overlaps = View.UnknownIdentifierOverlapsWithCell(cell);

				if (id is { UseId: false, A: var idA, R: var idR, G: var idG, B: var idB })
				{
					var brush = new SolidColorBrush(
						Color.FromArgb((byte)(overlaps ? idA : idA >> 2), idR, idG, idB)
					);

					g.AddCircle(row, column, size, brush);

					// In direct view, candidates should be drawn also.
					if (!Preferences.ShowCandidates)
					{
						d(cell, digit, overlaps ? bCandLighter : bCand);
					}
				}
				else if (((IPreference)Preferences).TryGetColor(id, out var color))
				{
					// In the normal case, I'll draw these circles.
					g.AddCircle(
						row, column, size,
						new SolidColorBrush(
							overlaps ? Color.FromArgb((byte)(color.A >> 2), color.R, color.G, color.B) : color
						)
					);

					// In direct view, candidates should be drawn also.
					if (!Preferences.ShowCandidates)
					{
						d(cell, digit, overlaps ? bCandLighter : bCand);
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
					d(cell, digit, overlaps ? bCandLighter : bCand);
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void d(int cell, int digit, Brush brush)
		{
			var (cf, cs) = GetFont(Preferences.CandidateFontName, cellWidth / 2, Preferences.CandidateScale);
			var (r, c) = Calculator.GetGridRowAndColumn(cell, digit);
			g.AddText(r, c, digit, brush, cf, cs, fontStyle: Preferences.CandidateFontStyle);
		}
	}

	/// <summary>
	/// Draw regions.
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	partial void DrawRegions(Grid g)
	{
		if (this is not { View.Regions: { } regions, Preferences: not null, Calculator: not null })
		{
			return;
		}

		foreach (var (region, id) in regions)
		{
			bool assigned = false;
			Color color;
			if (id is { UseId: false, A: var rA, R: var rR, G: var rG, B: var rB })
			{
				color = Color.FromArgb(rA, rR, rG, rB);
				assigned = true;
			}
			else if (((IPreference)Preferences).TryGetColor(id, out color))
			{
				assigned = true;
			}

			if (!assigned)
			{
				continue;
			}

			if (Preferences.ShowLightRegion)
			{
				var brush = new SolidColorBrush(color);
				switch (region)
				{
					case >= 0 and < 9 when (
						Pair: Calculator.GetGridRowAndColumn(RegionFirst[region]),
						Calculator.Offset
					) is var ((row, column), offset):
					{
						g.AddRectangle(row, column, 3, 3, brush, offset, offset);

						break;
					}
					case >= 9 and < 27 when (
						Calculator.CellSize,
						Calculator.GetAnchorsViaRegion(region),
						Calculator.Offset
					) is var ((w, h), (l, r), offset):
					{
						w /= 2;
						h /= 2;

						g.AddLine(l.WithOffset(w, h), r.WithOffset(-w, -h), brush, offset / 2);

						break;
					}
				}
			}
			else
			{
				var (row, column) = Calculator.GetGridRowAndColumn(RegionFirst[region]);
				var (rowSpan, columnSpan) = (region / 9) switch
				{
					0 => (PointCalculator.AnchorsCount / 3, PointCalculator.AnchorsCount / 3),
					1 => (1, PointCalculator.AnchorsCount),
					2 => (PointCalculator.AnchorsCount, 1)
				};

				g.AddRectangle(
					row, column, rowSpan, columnSpan,
					new SolidColorBrush(Color.FromArgb(byte.MaxValue >> 2, color.R, color.G, color.B))
				);
			}
		}
	}

	/// <summary>
	/// Draw links.
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	partial void DrawLinks(Grid g)
	{
		if (
			this is not
			{
				Preferences: not null,
				View.Links: { } links,
				Calculator: { Offset: var offset, CandidateSize: var (cw, ch) }
			}
		)
		{
			return;
		}

		// Gather all points used.
		var points = new HashSet<Point>();
		var linkArray = links as (Link, ColorIdentifier)[] ?? links.ToArray();
		foreach (var ((startCand, endCand, _), _) in linkArray)
		{
			points.Add(Calculator.GetCenterPoint(startCand));
			points.Add(Calculator.GetCenterPoint(endCand));
		}

		foreach (var (_, candidate) in Conclusions ?? Array.Empty<Conclusion>())
		{
			points.Add(Calculator.GetCenterPoint(candidate));
		}

		foreach (var ((start, end, type), _) in linkArray)
		{
			const double strokeThickness = 2;
			var penToDraw = new SolidColorBrush(Preferences.ChainColor);
			var dashStyleArray = type switch
			{
				ChainLinkType.Strong => null,
				ChainLinkType.Weak => new DoubleCollection { 3 },
				_ => new DoubleCollection { 6 }
			};

			Point pt1 = Calculator.GetCenterPoint(start), pt2 = Calculator.GetCenterPoint(end);
			var (pt1x, pt1y) = pt1;
			var (pt2x, pt2y) = pt2;

			if (type == ChainLinkType.Line)
			{
				g.AddLine(pt1, pt2, penToDraw, strokeThickness, PenLineCap.Triangle, dashStyleArray);
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
				adjust(pt1, out var p1, alpha, cw, offset);
				foreach (var point in points)
				{
					if (point.NearlyEquals(pt1, 1E-1) || point.NearlyEquals(pt2, 1E-1))
					{
						// Self...
						continue;
					}

					double dx2 = point.X - p1.X, dy2 = point.Y - p1.Y;
					if (Sign(dx1) == Sign(dx2) && Sign(dy1) == Sign(dy2)
						&& Abs(dx2) <= Abs(dx1) && Abs(dy2) <= Abs(dy1)
						&& (dx1 == 0 || dy1 == 0 || (dx1 / dy1).NearlyEquals(dx2 / dy2, 1E-1)))
					{
						through = true;
						break;
					}
				}

				// Now cut the link.
				cut(ref pt1, ref pt2, offset, cw, ch, pt1x, pt1y, pt2x, pt2y);

				if (through)
				{
					const double bezierLength = 20;

					// The end points are rotated 45 degrees. The details are:
					//     1) Counterclockwise for the start point,
					//     2) Clockwise for the end point.
					Point oldPt1 = new(pt1x, pt1y), oldPt2 = new(pt2x, pt2y);
					rotate(oldPt1, ref pt1, -RotateAngle);
					rotate(oldPt2, ref pt2, RotateAngle);

					double aAlpha = alpha - RotateAngle;
					double bx1 = pt1.X + bezierLength * Cos(aAlpha), by1 = pt1.Y + bezierLength * Sin(aAlpha);

					aAlpha = alpha + RotateAngle;
					double bx2 = pt2.X - bezierLength * Cos(aAlpha), by2 = pt2.Y - bezierLength * Sin(aAlpha);

					g.AddBezier(
						pt1, new(bx1, by1), new(bx2, by2), pt2, strokeThickness,
						PenLineCap.Triangle, dashStyleArray
					);
				}
				else
				{
					g.AddLine(pt1, pt2, penToDraw, strokeThickness, PenLineCap.Triangle, dashStyleArray);
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void rotate(in Point pt1, ref Point pt2, double angle)
		{
			// Translate 'pt2' to (0, 0).
			pt2.X -= pt1.X;
			pt2.Y -= pt1.Y;

			// Rotate.
			double sinAngle = Sin(angle), cosAngle = Cos(angle);
			double xAct = pt2.X, yAct = pt2.Y;
			pt2.X = xAct * cosAngle - yAct * sinAngle;
			pt2.Y = xAct * sinAngle + yAct * cosAngle;

			pt2.X += pt1.X;
			pt2.Y += pt1.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void adjust(in Point pt1, out Point p1, double alpha, double candidateSize, double offset)
		{
			p1 = pt1;
			double tempDelta = candidateSize / 2 + offset;

			p1.X += (int)(tempDelta * Cos(alpha));
			p1.Y += (int)(tempDelta * Sin(alpha));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void cut(
			ref Point pt1, ref Point pt2, double offset, double cw, double ch,
			double pt1x, double pt1y, double pt2x, double pt2y)
		{
			double slope = Abs((pt2y - pt1y) / (pt2x - pt1x));
			double x = cw / Sqrt(1 + slope * slope);
			double y = ch * Sqrt(slope * slope / (1 + slope * slope));

			double o = offset / 8;
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
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	partial void DrawUnknownValue(Grid g)
	{
		if (
			this is not
			{
				View.UnknownValues: { } unknownValues,
				Calculator.CellSize: var (width, _),
				Preferences: { UnknownIdentifierColor: var unknownColor, ValueScale: var valueScale }
			}
		)
		{
			return;
		}

		const string defaultFontName = "Times New Roman";

		var brush = new SolidColorBrush(unknownColor);
		var (ff, fs) = GetFont(defaultFontName, width / 2, valueScale);

		foreach (var ((cell, character, _), _) in unknownValues)
		{
			var (row, column) = Calculator.GetGridRowAndColumn(cell);

			g.AddText(row, column, character, brush, ff, fs, fontStyle: Preferences.UnknownIdentfierFontStyle);
		}
	}
}
