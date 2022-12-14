namespace Sudoku.Gdip;

partial class GridImageGenerator
{
	/// <summary>
	/// Draw custom view if <see cref="View"/> is not <see langword="null"/>.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	/// <seealso cref="View"/>
	private void DrawView(Graphics g)
	{
		if (View is null)
		{
			return;
		}

		DrawHouses(g);
		DrawCells(g);
		DrawCandidates(g);
		DrawLinks(g);
		DrawUnknownValue(g);
		DrawFigure(g);

		DrawShapeNodes(g);
		DrawGroupedNodes(g);
	}

	/// <summary>
	/// Draw eliminations.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawEliminations(Graphics g)
	{
		if (this is not
			{
				Conclusions: { } conclusions,
				Preferences: { EliminationColor: var eColor, CannibalismColor: var cColor },
				View: var view
			} || !conclusions.Any())
		{
			return;
		}

		using var elimBrush = new SolidBrush(eColor);
		using var cannibalBrush = new SolidBrush(cColor);
		using var elimBrushLighter = new SolidBrush(eColor.QuarterAlpha());
		using var canniBrushLighter = new SolidBrush(cColor.QuarterAlpha());
		foreach (var (t, c, d) in conclusions)
		{
			if (t != ConclusionType.Elimination)
			{
				continue;
			}

			var cannibalism = false;
			if (view is not { CandidateNodes: var candidateNodes })
			{
				goto Drawing;
			}

			foreach (var candidateNode in candidateNodes)
			{
				var value = candidateNode.Candidate;
				if (value == c * 9 + d)
				{
					cannibalism = true;
					break;
				}
			}

		Drawing:
			g.FillEllipse(
				(cannibalism, view.UnknownOverlaps(c)) switch
				{
					(true, true) => canniBrushLighter,
					(true, false) => cannibalBrush,
					(false, true) => elimBrushLighter,
					_ => elimBrush
				},
				Calculator.GetMouseRectangle(c, d)
			);
		}
	}

	/// <summary>
	/// Draw cells.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawCells(Graphics g)
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
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawCandidates(Graphics g)
	{
		if (this is not
			{
				View: { CandidateNodes: var candidateNodes } view,
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

		foreach (var candidateNode in candidateNodes)
		{
			var candidate = candidateNode.Candidate;
			var id = candidateNode.Identifier;

			var isOverlapped = false;
			if (conclusions is null)
			{
				goto IsOverlapped;
			}

			foreach (var (concType, concCandidate) in conclusions)
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
				var cell = candidate / 9;
				var digit = candidate % 9;
				var overlaps = view.UnknownOverlaps(cell);

				switch (id)
				{
					case { Mode: IdentifierColorMode.Raw, A: var alpha, R: var red, G: var green, B: var blue }:
					{
						using var brush = new SolidBrush(Color.FromArgb(overlaps ? alpha : alpha >> 2, red, green, blue));
						g.FillEllipse(brush, calc.GetMouseRectangle(cell, digit));

						// In direct view, candidates should be drawn also.
						if (!showCandidates)
						{
							d(cell, digit, vOffsetCandidate, overlaps ? bCandidateLighter : bCandidate);
						}

						break;
					}
					case { Mode: var mode and (IdentifierColorMode.Id or IdentifierColorMode.Named) }:
					{
						var color = mode switch
						{
							IdentifierColorMode.Id when GetValueById(id, out var c) => c,
							IdentifierColorMode.Named => GetColor(id),
							_ => throw new InvalidOperationException()
						};

						// In the normal case, I'll draw these circles.
						using var brush = new SolidBrush(overlaps ? color.QuarterAlpha() : color);
						g.FillEllipse(brush, calc.GetMouseRectangle(cell, digit));

						// In direct view, candidates should be drawn also.
						if (!showCandidates)
						{
							d(cell, digit, vOffsetCandidate, overlaps ? bCandidateLighter : bCandidate);
						}

						break;
					}
				}
			}
		}

		if (!showCandidates && conclusions is not null)
		{
			foreach (var (type, cell, digit) in conclusions)
			{
				var overlaps = view.UnknownOverlaps(cell);
				if (type == ConclusionType.Elimination)
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
		if (this is not { Calculator: { CellSize: var (w, h) } calc, View.HouseNodes: var houseNodes, Preferences.ShowLightHouse: var showLightHouse })
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
				View: { LinkNodes: var links } view,
				Conclusions: var conclusions,
				Calculator: { CandidateSize: var (cw, ch) } calc,
				Preferences.ChainColor: var chainColor
			})
		{
			return;
		}

		// Gather all points used.
		var points = new HashSet<PointF>();
		var linkArray = links.CastToArray();
		foreach (var linkNode in linkArray)
		{
			points.Add(calc.GetMouseCenter(linkNode.Start));
			points.Add(calc.GetMouseCenter(linkNode.End));
		}

		if (conclusions is not null)
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
	/// Draw unknown values.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawUnknownValue(Graphics g)
	{
		if (this is not
			{
				View.UnknownNodes: var unknownNodes,
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

		foreach (var unknownNode in unknownNodes)
		{
			var cell = unknownNode.Cell;
			var character = unknownNode.UnknownValueChar;

			// Draw values.
			var orginalPoint = calc.GetMousePointInCenter(cell);
			var point = orginalPoint with { Y = orginalPoint.Y + vOffsetValue };
			g.DrawValue(character, font, brush, point, StringLocating);
		}
	}

	/// <summary>
	/// Draw figures.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	private void DrawFigure(Graphics g)
	{
		if (this is not
			{
				View.FigureNodes: var figureNodes,
				Calculator: { CellSize: var (cw, ch) } calc,
				Preferences.FigurePadding: var padding
			})
		{
			return;
		}

		foreach (var figureNode in figureNodes)
		{
			switch (figureNode)
			{
				case (TriangleViewNode or DiamondViewNode or StarViewNode) and (var cell) { Identifier: var identifier }:
				{
					using var brush = new SolidBrush(GetColor(identifier));
					var (x, y) = calc.GetMousePointInCenter(cell);

					using var path = (
						figureNode switch
						{
							TriangleViewNode => triangle,
							DiamondViewNode => diamond,
							StarViewNode => star,
							_ => default(Func<float, float, GraphicsPath>?)!
						}
					)(x, y);
					g.FillPath(brush, path);

					break;
				}
				case (SquareViewNode or CircleViewNode) and (var cell) { Identifier: var identifier }:
				{
					using var brush = new SolidBrush(GetColor(identifier));
					var (x, y) = calc.GetMousePointInCenter(cell);
					(
						figureNode switch
						{
							SquareViewNode => g.FillRectangle,
							CircleViewNode => g.FillEllipse,
							_ => default(Action<Brush, float, float, float, float>?)!
						}
					)(brush, x - cw / 2 + padding, y - ch / 2 + padding, cw - 2 * padding, ch - 2 * padding);

					break;
				}
				case HeartViewNode(var cell) { Identifier: var identifier }:
				{
					// https://mathworld.wolfram.com/HeartCurve.html
					using var brush = new SolidBrush(GetColor(identifier));

					var center = calc.GetMousePointInCenter(cell);

					// Rotating.
					var oldMatrix = g.Transform;
					using var newMatrix = g.Transform.Clone();
					newMatrix.RotateAt(180, center);

					g.Transform = newMatrix;
					g.FillClosedCurve(brush, getPoints());
					g.Transform = oldMatrix;

					break;


					PointF[] getPoints()
					{
						const int maxTrialTimes = 360;

						var (centerX, centerY) = center;
						var result = new PointF[maxTrialTimes];
						for (var i = 0; i < maxTrialTimes; i++)
						{
							var t = 2 * PI / maxTrialTimes * i;
							var x = centerX + 16 * Pow(Sin(t), 3) / (32 + 2 * padding) * cw;
							var y = centerY + (13 * Cos(t) - 5 * Cos(2 * t) - 2 * Cos(3 * t) - Cos(4 * t)) / (32 + 2 * padding) * ch;

							result[i] = new(x, y);
						}

						return result;
					}
				}


				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				GraphicsPath triangle(float x, float y)
				{
					var top = new PointF(x, y - Tan(PI / 3) / 4 * (cw - 2 * padding));
					var left = new PointF(x - (cw - 2 * padding) / 2, y - ch / 2 + ch - padding);
					var right = new PointF(x + (cw - 2 * padding) / 2, y - ch / 2 + ch - padding);

					var path = new GraphicsPath();
					path.AddLine(top, left);
					path.AddLine(left, right);
					path.AddLine(right, top);

					return path;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				GraphicsPath diamond(float x, float y)
				{
					var p1 = new PointF(x, y - ch / 2 + padding);
					var p2 = new PointF(x - cw / 2 + padding, y);
					var p3 = new PointF(x + cw / 2 - padding, y);
					var p4 = new PointF(x, y + ch / 2 - padding);

					var path = new GraphicsPath();
					path.AddLine(p1, p3);
					path.AddLine(p3, p4);
					path.AddLine(p4, p2);
					path.AddLine(p2, p1);

					return path;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				GraphicsPath star(float x, float y)
				{
					var angles1 = getAngles(-PI / 2);
					var angles2 = getAngles(-PI / 2 + PI / 5);
					var points1 = getPoints(x, y, cw / 2 - padding, angles1);
					var points2 = getPoints(x, y, (ch / 2 - padding) / 2, angles2);
					var points = new PointF[points1.Length + points2.Length];
					for (var (i, j) = (0, 0); i < points.Length; i += 2, j++)
					{
						points[i] = points1[j];
						points[i + 1] = points2[j];
					}

					var path = new GraphicsPath();
					for (var i = 0; i < points.Length - 1; i++)
					{
						path.AddLine(points[i], points[i + 1]);
					}
					path.AddLine(points[^1], points[0]);

					return path;


					static float[] getAngles(float startAngle)
					{
						var result = new[] { startAngle, default, default, default, default };
						for (var i = 1; i < 5; i++)
						{
							result[i] = result[i - 1] + 2 * PI / 5;
						}

						return result;
					}

					[MethodImpl(MethodImplOptions.AggressiveInlining)]
					static PointF getPoint(float x, float y, float length, float angle)
						=> new(x + length * Cos(angle), y + length * Sin(angle));

					static PointF[] getPoints(float x, float y, float length, params float[] angles)
					{
						var result = new PointF[angles.Length];
						for (var i = 0; i < result.Length; i++)
						{
							result[i] = getPoint(x, y, length, angles[i]);
						}

						return result;
					}
				}
			}
		}
	}

	/// <summary>
	/// Draw <see cref="ShapeViewNode"/> instances.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	[Conditional("ENHANCED_DRAWING_APIS")]
	private void DrawShapeNodes(Graphics g)
	{
		if (this is not { View.ShapeViewNodes: var nodes, Calculator: { CellSize: (var cw, var ch) cellSize } calc })
		{
			return;
		}

		foreach (var node in nodes)
		{
			_ = (This: this, Node: node) switch
			{
				// Consecutive bar
				{
					This.Preferences: { BorderBarWidth: var barWidth, BorderBarFullyOverlapsGridLine: var fullyOverlapping },
					Node: BorderBarViewNode(var c1, var c2) { Identifier: var identifier }
				} => DrawBorderBar(identifier, barWidth, calc, c1, c2, fullyOverlapping, g),

				// Kropki dot
				{
					This.Preferences: { KropkiDotBorderWidth: var borderWidth, KropkiDotSize: var dotSize, BackgroundColor: var backColor },
					Node: KropkiDotViewNode(var c1, var c2) { Identifier: var identifier, IsSolid: var isSolid }
				} => DrawKropkiDot(identifier, backColor, borderWidth, calc, c1, c2, dotSize, isSolid, g),

				// Greater-than sign
				{
					This.Preferences: { GreaterThanSignFont: var fontData, BackgroundColor: var backColor },
					Node: GreaterThanSignViewNode(var c1, var c2, var isRow)
					{
						Identifier: var identifier,
						IsGreaterThan: var isGreaterThan
					}
				} => DrawGreaterThanSign(fontData, backColor, identifier, isGreaterThan, calc, c1, c2, isRow, g),

				// XV sign
				{
					This.Preferences: { XvSignFont: var fontData, BackgroundColor: var backColor },
					Node: XvSignViewNode(var c1, var c2) { Identifier: var identifier, IsX: var isX }
				} => DrawXvSign(fontData, backColor, identifier, isX, calc, c1, c2, g),

				// Number label
				{
					This.Preferences: { NumberLabelFont: var fontData, BackgroundColor: var backColor },
					Node: NumberLabelViewNode(var c1, var c2) { Identifier: var identifier, Label: var label }
				} => DrawNumberLabel(fontData, backColor, identifier, calc, c1, c2, g, label),

				// Battenburg
				{
					This.Preferences.BattenburgSize: var battenburgSize,
					Node: BattenburgViewNode { Identifier: var identifier, Cells: [.., var lastCell] }
				} => DrawBattenburg(identifier, calc, lastCell, cellSize, battenburgSize, g),

				// Quadruple hint
				{
					This.Preferences: { QuadrupleHintFont: var fontData, BackgroundColor: var backColor },
					Node: QuadrupleHintViewNode { Identifier: var identifier, Cells: [.., var lastCell], Hint: var hint }
				} => DrawQuadrupleHint(fontData, backColor, identifier, g, hint, calc, lastCell, cw, ch),

				// Clockface dot
				{
					This.Preferences:
					{
						ClockfaceDotSize: var dotSize,
						ClockfaceDotBorderWidth: var borderWidth,
						BackgroundColor: var backColor
					},
					Node: ClockfaceDotViewNode { Identifier: var identifier, Cells: [.., var lastCell], IsClockwise: var isClockwise }
				} => DrawClockfaceDot(identifier, borderWidth, backColor, calc, lastCell, isClockwise, g, cw, ch, dotSize),

				// Neighbor sign
				{
					This.Preferences: { NeighborSignsWidth: var width, NeighborSignCellPadding: var padding },
					Node: NeighborSignViewNode(var cell, _) { Identifier: var identifier, IsFourDirections: var isFourDirections }
				} => DrawNeighborSign(identifier, width, calc, cell, cw, ch, padding, isFourDirections, g),

				// Wheel
				{
					This.Preferences:
					{
						WheelFont: var fontData,
						WheelWidth: var width,
						WheelTextColor: var textColor,
						BackgroundColor: var backColor
					},
					Node: WheelViewNode(var cell, _) { Identifier: var identifier, DigitString: var digitString }
				} => DrawWheel(backColor, fontData, textColor, identifier, width, calc, cell, cw, ch, digitString, g),

				// Pencilmark
				{
					This.Preferences: { PencilmarkFont: var fontData, PencilmarkTextColor: var textColor },
					Node: PencilMarkViewNode(var cell, _) { Notation: var notation }
				} => DrawPencilmark(fontData, textColor, g, notation, calc, cell, ch),

				// Triangle sum
				{
					This.Preferences.TriangleSumCellPadding: var padding,
					Node: TriangleSumViewNode(var cell, var directions) { Identifier: var identifier, IsComplement: var isComplement }
				} => DrawTriangleSum(identifier, padding, cell, directions, isComplement, g, calc),

				// Star product star
				{
					This.Preferences.StarProductStarFont: var fontData,
					Node: StarProductStarViewNode(var cell, var direction) { Identifier: var identifier }
				} => DrawStarProductStar(fontData, identifier, g, calc, cell, direction, cw, ch),

				// Cell arrow
				{
					Node: CellArrowViewNode(var cell, var direction) { Identifier: var identifier }
				} => DrawCellArrow(identifier, calc, cell, direction, cw, ch, g),

				// Quadruple max arrow
				{
					This.Preferences.QuadrupleMaxArrowSize: var size,
					Node: QuadrupleMaxArrowViewNode { Cells: [.., var lastCell], Identifier: var identifier, ArrowDirection: var direction }
				} => DrawQuadrupleMaxArrow(identifier, calc, lastCell, cw, ch, direction, g, size),

				// Cell-corner triangle
				{
					This.Preferences: { CellCornerTriangleSize: var size, CellCornerTriangleCellPadding: var padding },
					Node: CellCornerTriangleViewNode { Identifier: var identifier, Cell: var cell, Directions: var direction }
				} => DrawCellCornerTriangle(identifier, calc, cell, direction, cw, ch, padding, size, g),

				// Average bar
				{
					This.Preferences.AverageBarWidth: var width,
					Node: AverageBarViewNode { Identifier: var identifier, Cell: var cell, Type: var type }
				} => DrawAverageBar(identifier, width, calc, cell, type, cw, ch, g),

				// Cell-corner arrow
				{
					This.Preferences.CellCornerArrowWidth: var width,
					Node: CellCornerArrowViewNode { Identifier: var identifier, Cell: var cell, Directions: var directions }
				} => DrawCellCornerArrow(identifier, calc, cell, ch, width, directions, g),

				// Embedded skyscraper arrow
				{
					This.Preferences.EmbeddedSkyscraperArrowFont: var fontData,
					Node: EmbeddedSkyscraperArrowViewNode { Identifier: var identifier, Cell: var cell, Directions: var directions }
				} => DrawEmbeddedSkyscraperArrow(fontData, identifier, calc, cell, directions, g, cw, ch)
			};
		}
	}

	/// <summary>
	/// Draw <see cref="GroupedViewNode"/> instances.
	/// </summary>
	/// <param name="g"><inheritdoc cref="RenderTo(Graphics)" path="/param[@name='g']"/></param>
	[Conditional("ENHANCED_DRAWING_APIS")]
	private void DrawGroupedNodes(Graphics g)
	{
		if (this is not { View.GroupedViewNodes: var nodes, Calculator: { CellSize: var cs, GridSize: var gs } calc })
		{
			return;
		}

		foreach (var node in nodes)
		{
			_ = (This: this, Node: node) switch
			{
				// Diagonal lines
				{
					This.Preferences.DiagonalLinesWidth: var width,
					Node: DiagonalLinesViewNode { Identifier: var identifier }
				} => DrawDiagonalLines(identifier, width, calc, cs, gs, g),

				// Capsule
				{
					This.Preferences: { CapsulePadding: var padding, CapsuleWidth: var width },
					Node: CapsuleViewNode(var head, _) { Identifier: var identifier, AdjacentType: var adjacentType }
				} => DrawCapsule(head, adjacentType, padding, calc, cs, identifier, width, g),

				// Oblique line
				{
					This.Preferences.ObliqueLineWidth: var width,
					Node: ObliqueLineViewNode(var head, _) { Identifier: var identifier, TailCell: var tail }
				} => DrawObliqueLine()
			};
		}
	}
}
