namespace Sudoku.Gdip;

partial class GridImageGenerator
{
	/// <summary>
	/// Draw cells.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawCells(Graphics g)
	{
		if (View is null)
		{
			return;
		}

		foreach (var cellNode in View.OfType<CellViewNode>())
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
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawCandidates(Graphics g)
	{
		if (this is not
			{
				View: { } view,
				Calculator: { CellSize.Width: var cellWidth, CandidateSize.Width: var candidateWidth } calc,
				Conclusions: var conclusions,
				Preferences:
				{
					CandidateColor: var cColor,
					CandidateFontName: var cFontName,
					CandidateScale: var cScale,
					CandidateFontStyle: var cFontStyle,
					ShowCandidates: var showCandidates
				}
			})
		{
			return;
		}

		var vOffsetCandidate = candidateWidth / 9; // The vertical offset of rendering each candidate.

		using var bCandidate = new SolidBrush(cColor);
		using var bCandidateLighter = new SolidBrush(cColor.QuarterAlpha());
		using var fCandidate = GetFont(cFontName, cellWidth / 2F, cScale, cFontStyle);

		foreach (var candidateNode in view.OfType<CandidateViewNode>())
		{
			var candidate = candidateNode.Candidate;
			var id = candidateNode.Identifier;

			var isOverlapped = false;
			if (conclusions is [])
			{
				goto IsOverlapped;
			}

			foreach (var (concType, concCandidate) in conclusions)
			{
				if (concType == Elimination && concCandidate == candidate)
				{
					isOverlapped = true;
					break;
				}
			}

		IsOverlapped:
			if (!isOverlapped)
			{
				var cell = candidate / 9;
				var digit = candidate % 9;
				var overlaps = view.UnknownOverlaps(cell);

				var color = GetColor(id);
				using var brush = new SolidBrush(overlaps ? color.QuarterAlpha() : color);
				g.FillEllipse(brush, calc.GetMouseRectangle(cell, digit));

				// In direct view, candidates should be drawn also.
				if (!showCandidates)
				{
					d(cell, digit, vOffsetCandidate, overlaps ? bCandidateLighter : bCandidate);
				}
			}
		}

		if (!showCandidates && conclusions is not [])
		{
			foreach (var (type, cell, digit) in conclusions)
			{
				var overlaps = view.UnknownOverlaps(cell);
				if (type == Elimination)
				{
					d(cell, digit, vOffsetCandidate, overlaps ? bCandidateLighter : bCandidate);
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void d(int cell, int digit, float vOffsetCandidate, Brush brush)
		{
			var originalPoint = calc.GetMousePointInCenter(cell, digit);
			var point = originalPoint with { Y = originalPoint.Y + vOffsetCandidate };
			g.DrawValue(digit + 1, fCandidate, brush, point, StringLocating);
		}
	}

	/// <summary>
	/// Draw houses.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawHouses(Graphics g)
	{
		if (this is not { Calculator: { CellSize: var cs } calc, View: { } view, Preferences.ShowLightHouse: var showLightHouse })
		{
			return;
		}

		foreach (var houseNode in view.OfType<HouseViewNode>())
		{
			var house = houseNode.House;
			var id = houseNode.Identifier;
			var color = GetColor(id);
			if (showLightHouse)
			{
				using var pen = new Pen(color, 4F);
				switch (house)
				{
					case >= 0 and < 9:
					{
						g.DrawRoundedRectangle(pen, calc.GetMouseRectangleViaHouse(house), 6);

						break;
					}
					case >= 9 and < 27:
					{
						var (l, r) = calc.GetAnchorsViaHouse(house);
						l += cs / 2;
						r -= cs / 2;

						g.DrawLine(pen, l, r);

						break;
					}
				}
			}
			else
			{
				using var brush = new SolidBrush(Color.FromArgb(64, color));
				g.FillRectangle(brush, calc.GetMouseRectangleViaHouse(house));
			}
		}
	}

	/// <summary>
	/// Draw links.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawLinks(Graphics g)
	{
		if (this is not
			{
				View: { } view,
				Conclusions: var conclusions,
				Calculator: { CandidateSize: var (cw, ch) } calc,
				Preferences.ChainColor: var chainColor
			})
		{
			return;
		}

		// Gather all points used.
		var points = new HashSet<PointF>();
		var linkArray = view.OfType<LinkViewNode>().ToArray();
		foreach (var linkNode in linkArray)
		{
			points.Add(calc.GetMouseCenter(linkNode.Start));
			points.Add(calc.GetMouseCenter(linkNode.End));
		}

		if (conclusions is not [])
		{
			foreach (var conclusion in conclusions)
			{
				points.Add(calc.GetMousePointInCenter(conclusion.Cell, conclusion.Digit));
			}
		}

		// Iterate on each inference to draw the links and grouped nodes (if so).
		using var linePen = new Pen(chainColor, 2F);
		using var arrowPen = new Pen(chainColor, 2F) { CustomEndCap = new AdjustableArrowCap(cw / 4F, ch / 3F) };

		foreach (var linkNode in linkArray)
		{
			if (linkNode is not { Start: var start, End: var end, Inference: var inference })
			{
				continue;
			}

			arrowPen.DashStyle = inference switch { Inference.Strong => DashStyle.Solid, Inference.Weak => DashStyle.Dot, _ => DashStyle.Dash };

			_ = (calc.GetMouseCenter(start), calc.GetMouseCenter(end)) is (var pt1 and var (pt1x, pt1y), var pt2 and var (pt2x, pt2y));

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
				if (distance <= cw * SqrtOf2 || distance <= ch * SqrtOf2)
				{
					continue;
				}

				// Check if another candidate lies in the direct line.
				var deltaX = pt2x - pt1x;
				var deltaY = pt2y - pt1y;
				var alpha = Atan2(deltaY, deltaX);
				var dx1 = deltaX;
				var dy1 = deltaY;
				var through = false;
				adjust(pt1, pt2, out var p1, out _, alpha, cw);
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
						&& (dx1 == 0 || dy1 == 0 || (dx1 / dy1).NearlyEquals(dx2 / dy2, epsilon: 1E-1F)))
					{
						through = true;
						break;
					}
				}

				// Now cut the link.
				cut(ref pt1, ref pt2, cw, ch, pt1x, pt1y, pt2x, pt2y);

				if (through)
				{
					var bezierLength = 20F;

					// The end points are rotated 45 degrees
					// (counterclockwise for the start point, clockwise for the end point).
					rotate(new(pt1x, pt1y), ref pt1, -RotateAngle);
					rotate(new(pt2x, pt2y), ref pt2, RotateAngle);

					var aAlpha = alpha - RotateAngle;
					var bx1 = pt1.X + bezierLength * Cos(aAlpha);
					var by1 = pt1.Y + bezierLength * Sin(aAlpha);

					aAlpha = alpha + RotateAngle;
					var bx2 = pt2.X - bezierLength * Cos(aAlpha);
					var by2 = pt2.Y - bezierLength * Sin(aAlpha);

					g.DrawBezier(penToDraw, pt1.X, pt1.Y, bx1, by1, bx2, by2, pt2.X, pt2.Y);
				}
				else
				{
					// Draw the link.
					g.DrawLine(penToDraw, pt1, pt2);
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void rotate(PointF pt1, scoped ref PointF pt2, float angle)
		{
			// Translate 'pt2' to (0, 0).
			pt2.X -= pt1.X;
			pt2.Y -= pt1.Y;

			// Rotate.
			var sinAngle = Sin(angle);
			var cosAngle = Cos(angle);
			var xAct = pt2.X;
			var yAct = pt2.Y;
			pt2.X = (float)(xAct * cosAngle - yAct * sinAngle);
			pt2.Y = (float)(xAct * sinAngle + yAct * cosAngle);

			pt2.X += pt1.X;
			pt2.Y += pt1.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void adjust(PointF pt1, PointF pt2, out PointF p1, out PointF p2, float alpha, float candidateSize)
		{
			p1 = pt1;
			p2 = pt2;
			var tempDelta = candidateSize / 2;
			var px = (int)(tempDelta * Cos(alpha));
			var py = (int)(tempDelta * Sin(alpha));

			p1.X += px;
			p1.Y += py;
			p2.X -= px;
			p2.Y -= py;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void cut(scoped ref PointF pt1, scoped ref PointF pt2, float cw, float ch, float pt1x, float pt1y, float pt2x, float pt2y)
		{
			var slope = Abs((pt2y - pt1y) / (pt2x - pt1x));
			var x = cw / (float)Sqrt(1 + slope * slope);
			var y = ch * (float)Sqrt(slope * slope / (1 + slope * slope));

			if (pt1y > pt2y && pt1x.NearlyEquals(pt2x)) { pt1.Y -= ch / 2; pt2.Y += ch / 2; }
			else if (pt1y < pt2y && pt1x.NearlyEquals(pt2x)) { pt1.Y += ch / 2; pt2.Y -= ch / 2; }
			else if (pt1y.NearlyEquals(pt2y) && pt1x > pt2x) { pt1.X -= cw / 2; pt2.X += cw / 2; }
			else if (pt1y.NearlyEquals(pt2y) && pt1x < pt2x) { pt1.X += cw / 2; pt2.X -= cw / 2; }
			else if (pt1y > pt2y && pt1x > pt2x) { pt1.X -= x / 2; pt1.Y -= y / 2; pt2.X += x / 2; pt2.Y += y / 2; }
			else if (pt1y > pt2y && pt1x < pt2x) { pt1.X += x / 2; pt1.Y -= y / 2; pt2.X -= x / 2; pt2.Y += y / 2; }
			else if (pt1y < pt2y && pt1x > pt2x) { pt1.X -= x / 2; pt1.Y += y / 2; pt2.X += x / 2; pt2.Y -= y / 2; }
			else if (pt1y < pt2y && pt1x < pt2x) { pt1.X += x / 2; pt1.Y += y / 2; pt2.X -= x / 2; pt2.Y -= y / 2; }
		}
	}

	/// <summary>
	/// Draw chutes.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawChute(Graphics g)
	{
		if (this is not { View: { } view, Preferences.ShowLightHouse: var showLightHouse })
		{
			return;
		}

		foreach (var chuteNode in view.OfType<ChuteViewNode>())
		{
			if (chuteNode is not { ChuteIndex: var chute, Identifier: var id })
			{
				continue;
			}

			var color = GetColor(id);
			using var brush = new SolidBrush(showLightHouse ? color.QuarterAlpha() : color);

			if (chute is >= 0 and < 3)
			{
				var (pt1, _) = Calculator.GetAnchorsViaHouse(9 + chute * 3);
				var (_, pt2) = Calculator.GetAnchorsViaHouse(8 + (chute + 1) * 3);
				var rect = RectangleCreator.Create(pt1, pt2);
				g.FillRectangle(brush, rect);
			}
			else
			{
				var (pt1, _) = Calculator.GetAnchorsViaHouse(18 + (chute - 3) * 3);
				var (_, pt2) = Calculator.GetAnchorsViaHouse(17 + (chute - 2) * 3);
				var rect = RectangleCreator.Create(pt1, pt2);
				g.FillRectangle(brush, rect);
			}
		}
	}

	/// <summary>
	/// Draw unknown values.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawUnknownValue(Graphics g)
	{
		if (this is not
			{
				View: { } view,
				Calculator: { CellSize.Width: var cellWidth } calc,
				Preferences:
				{
					UnknownIdentifierColor: var uColor,
					ValueScale: var vScale,
					UnknownFontStyle: var uFontStyle,
					UnknownFontName: var uFontName
				}
			})
		{
			return;
		}

		var vOffsetValue = cellWidth / (AnchorsCount / 3); // The vertical offset of rendering each value.
		var halfWidth = cellWidth / 2F;

		using var brush = new SolidBrush(uColor);
		using var font = GetFont(uFontName, halfWidth, vScale, uFontStyle);

		foreach (var unknownNode in view.OfType<BabaGroupViewNode>())
		{
			var cell = unknownNode.Cell;
			var character = unknownNode.UnknownValueChar;

			// Draw values.
			var originalPoint = calc.GetMousePointInCenter(cell);
			var point = originalPoint with { Y = originalPoint.Y + vOffsetValue };
			g.DrawValue(character, font, brush, point, StringLocating);
		}
	}
}
