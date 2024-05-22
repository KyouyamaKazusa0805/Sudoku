namespace Sudoku.Drawing;

public partial class GridCanvas
{
	/// <summary>
	/// Draw cell view nodes onto the canvas.
	/// </summary>
	/// <param name="nodes">The nodes to be drawn.</param>
	public partial void DrawCellViewNodes(ReadOnlySpan<CellViewNode> nodes)
	{
		foreach (var node in nodes)
		{
			var cell = node.Cell;
			using var brush = new SolidBrush(GetColor(node.Identifier));
			_g.FillRectangle(brush, _calculator.GetMouseRectangleViaCell(cell));
		}
	}

	/// <summary>
	/// Draw candidate view nodes onto the canvas, with confliction check on conclusions.
	/// </summary>
	/// <param name="nodes">The nodes to be drawn.</param>
	/// <param name="conclusions">The conclusions to be checked.</param>
	public partial void DrawCandidateViewNodes(ReadOnlySpan<CandidateViewNode> nodes, ReadOnlySpan<Conclusion> conclusions)
	{
		if (this is not
			{
				_calculator: { CellSize.Width: var cellWidth, CandidateSize.Width: var candidateWidth },
				_settings:
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

		var vOffsetCandidate = candidateWidth / 9; // The vertical offset of drawing each candidate.
		using var bCandidate = new SolidBrush(cColor);
		using var bCandidateLighter = new SolidBrush(cColor.QuarterAlpha());
		using var fCandidate = GetFont(cFontName, cellWidth / 2F, cScale, cFontStyle);
		foreach (var node in nodes)
		{
			var candidate = node.Candidate;
			var isOverlapped = false;
			if (conclusions.Length == 0)
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
				var overlaps = nodes.Any((ref readonly CandidateViewNode node) => node.Cell == cell);
				var color = GetColor(node.Identifier);
				using var brush = new SolidBrush(overlaps ? color.QuarterAlpha() : color);
				_g.FillEllipse(brush, _calculator.GetMouseRectangle(cell, digit));

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
				var overlaps = nodes.Any((ref readonly CandidateViewNode node) => node.Cell == cell);
				if (type == Elimination)
				{
					d(cell, digit, vOffsetCandidate, overlaps ? bCandidateLighter : bCandidate);
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void d(Cell cell, Digit digit, float vOffsetCandidate, Brush brush)
		{
			var originalPoint = _calculator.GetMousePointInCenter(cell, digit);
			var point = originalPoint with { Y = originalPoint.Y + vOffsetCandidate };
			_g.DrawValue(digit + 1, fCandidate, brush, point, _stringAligner);
		}
	}

	/// <summary>
	/// Draw house view nodes onto the canvas.
	/// </summary>
	/// <param name="nodes">The nodes to be drawn.</param>
	public partial void DrawHouseViewNodes(ReadOnlySpan<HouseViewNode> nodes)
	{
		var showLightHouse = _settings.ShowLightHouse;
		foreach (var node in nodes)
		{
			var house = node.House;
			var color = GetColor(node.Identifier);
			if (showLightHouse)
			{
				using var pen = new Pen(color, 4F);
				switch (house)
				{
					case >= 0 and < 9:
					{
						_g.DrawRoundedRectangle(pen, _calculator.GetMouseRectangleViaHouse(house), 6);
						break;
					}
					case >= 9 and < 27:
					{
						var (l, r) = _calculator.GetAnchorsViaHouse(house);
						_g.DrawLine(pen, l + _calculator.CellSize / 2, r - _calculator.CellSize / 2);
						break;
					}
				}
			}
			else
			{
				using var brush = new SolidBrush(Color.FromArgb(64, color));
				_g.FillRectangle(brush, _calculator.GetMouseRectangleViaHouse(house));
			}
		}
	}

	/// <summary>
	/// Draw link view nodes onto the canvas, with confliction check on conclusions.
	/// If any conclusions is lying on the line of the link drawn, the link will be automatically adjusted to a curve.
	/// </summary>
	/// <param name="nodes">The nodes to be drawn.</param>
	/// <param name="conclusions">The conclusion to be checked.</param>
	public partial void DrawLinkViewNodes(ReadOnlySpan<LinkViewNode> nodes, ReadOnlySpan<Conclusion> conclusions)
	{
		// Collect all points used.
		var (cw, ch) = _calculator.CandidateSize;
		var chainColor = _settings.ChainColor;
		var points = new HashSet<PointF>();
		var linkArray = nodes.ToArray();
		foreach (var linkNode in linkArray)
		{
			points.Add(_calculator.GetMouseCenter(linkNode.Start));
			points.Add(_calculator.GetMouseCenter(linkNode.End));
		}
		foreach (var conclusion in conclusions)
		{
			points.Add(_calculator.GetMousePointInCenter(conclusion.Cell, conclusion.Digit));
		}

		// Iterate on each inference to draw the links and grouped nodes (if so).
		using var linePen = new Pen(chainColor, 2F);
		using var arrowPen = new Pen(chainColor, 2F) { CustomEndCap = new AdjustableArrowCap(cw / 4F, ch / 3F) };
		foreach (var node in linkArray)
		{
			if (node is not { Start: var start, End: var end, Inference: var inference })
			{
				continue;
			}

			arrowPen.DashStyle = inference switch
			{
				Inference.Strong => DashStyle.Solid,
				Inference.Weak => DashStyle.Dot,
				_ => DashStyle.Dash
			};

			_ = (_calculator.GetMouseCenter(in start), _calculator.GetMouseCenter(in end)) is (var pt1 and var (pt1x, pt1y), var pt2 and var (pt2x, pt2y));

			var penToDraw = inference != Inference.Default ? arrowPen : linePen;
			if (inference == Inference.Default)
			{
				// Draw the link.
				_g.DrawLine(penToDraw, pt1, pt2);
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

					_g.DrawBezier(penToDraw, pt1.X, pt1.Y, bx1, by1, bx2, by2, pt2.X, pt2.Y);
				}
				else
				{
					// Draw the link.
					_g.DrawLine(penToDraw, pt1, pt2);
				}
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void rotate(PointF pt1, ref PointF pt2, float angle)
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
		static void cut(ref PointF pt1, ref PointF pt2, float cw, float ch, float pt1x, float pt1y, float pt2x, float pt2y)
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
	/// Draw chute view nodes onto the canvas.
	/// </summary>
	/// <param name="nodes">The nodes to be drawn.</param>
	public partial void DrawChuteViewNodes(ReadOnlySpan<ChuteViewNode> nodes)
	{
		foreach (var node in nodes)
		{
			var chute = node.ChuteIndex;
			var color = GetColor(node.Identifier);
			using var brush = new SolidBrush(_settings.ShowLightHouse ? color.QuarterAlpha() : color);
			if (chute is >= 0 and < 3)
			{
				var (pt1, _) = _calculator.GetAnchorsViaHouse(9 + chute * 3);
				var (_, pt2) = _calculator.GetAnchorsViaHouse(8 + (chute + 1) * 3);
				var rect = RectangleCreator.Create(pt1, pt2);
				_g.FillRectangle(brush, rect);
			}
			else
			{
				var (pt1, _) = _calculator.GetAnchorsViaHouse(18 + (chute - 3) * 3);
				var (_, pt2) = _calculator.GetAnchorsViaHouse(17 + (chute - 2) * 3);
				var rect = RectangleCreator.Create(pt1, pt2);
				_g.FillRectangle(brush, rect);
			}
		}
	}

	/// <summary>
	/// Draw baba grouping view nodes onto the canvas.
	/// </summary>
	/// <param name="nodes">The nodes to be drawn.</param>
	public partial void DrawBabaGroupingViewNodes(ReadOnlySpan<BabaGroupViewNode> nodes)
	{
		if (_settings is not
			{
				BabaGroupingCharacterColor: var uColor,
				ValueScale: var vScale,
				BabaGroupCharacterFontStyle: var uFontStyle,
				BabaGroupingFontName: var uFontName
			})
		{
			return;
		}

		var cellWidth = _calculator.CellSize.Width;
		var vOffsetValue = cellWidth / (PointCalculator.AnchorsCount / 3); // The vertical offset of drawing each value.
		var halfWidth = cellWidth / 2F;
		using var brush = new SolidBrush(uColor);
		using var font = GetFont(uFontName, halfWidth, vScale, uFontStyle);
		foreach (var node in nodes)
		{
			var cell = node.Cell;
			var character = node.UnknownValueChar;

			// Draw values.
			var originalPoint = _calculator.GetMousePointInCenter(cell);
			var point = originalPoint with { Y = originalPoint.Y + vOffsetValue };
			_g.DrawValue(character, font, brush, point, _stringAligner);
		}
	}
}
