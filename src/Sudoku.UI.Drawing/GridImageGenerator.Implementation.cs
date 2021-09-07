namespace Sudoku.UI.Drawing;

partial record GridImageGenerator
{
	/// <summary>
	/// Draw givens, modifiables and candidates, where the values are specified as a grid.
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	partial void DrawValue(Grid g)
	{
		if (Puzzle.IsUndefined)
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

						g.AddTextBlock(r, c, digit, overlaps ? bCandLighter : bCand, fpCand.Font, fpCand.FontSize);
					}

					break;
				}
				case CellStatus.Modifiable:
				{
					// Draw modifiables.
					var (r, c) = Calculator.GetGridRowAndColumn(cell);

					g.AddTextBlock(r, c, Puzzle[cell] + 1, bGiven, fpGiven.Font, fpGiven.FontSize);

					break;
				}
				case CellStatus.Given:
				{
					// Draw givens.
					var (r, c) = Calculator.GetGridRowAndColumn(cell);

					g.AddTextBlock(r, c, Puzzle[cell] + 1, bMod, fpMod.Font, fpMod.FontSize);

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
		if (FocusedCells.IsEmpty)
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
	partial void DrawBackground(Grid g) => g.AddCanvasBackground(Preferences.BackgroundColor, Calculator);

	/// <summary>
	/// Draw grid lines and block lines.
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	partial void DrawGridAndBlockLines(Grid g)
	{
		const int length = PointCalculator.AnchorsCount + 1;
		var gridPoints = Calculator.GridPoints;
		for (int i = 0; i < length; i += PointCalculator.AnchorsCount / 9)
		{
			var (rx1, ry1) = gridPoints[i, 0];
			var (rx2, ry2) = gridPoints[i, PointCalculator.AnchorsCount];
			var (cx1, cy1) = gridPoints[0, i];
			var (cx2, cy2) = gridPoints[PointCalculator.AnchorsCount, i];
			var l1 = new Line
			{
				X1 = rx1,
				Y1 = ry1,
				X2 = rx2,
				Y2 = ry2,
				Stroke = new SolidColorBrush(Preferences.GridLineColor),
				StrokeThickness = Preferences.GridLineWidth
			};
			var l2 = new Line
			{
				X1 = cx1,
				Y1 = cy1,
				X2 = cx2,
				Y2 = cy2,
				Stroke = new SolidColorBrush(Preferences.GridLineColor),
				StrokeThickness = Preferences.GridLineWidth
			};

			g.Children.Add(l1);
			g.Children.Add(l2);
		}

		for (int i = 0; i < length; i += PointCalculator.AnchorsCount / 3)
		{
			var (rx1, ry1) = gridPoints[i, 0];
			var (rx2, ry2) = gridPoints[i, PointCalculator.AnchorsCount];
			var (cx1, cy1) = gridPoints[0, i];
			var (cx2, cy2) = gridPoints[PointCalculator.AnchorsCount, i];

			var l1 = new Line
			{
				X1 = rx1,
				Y1 = ry1,
				X2 = rx2,
				Y2 = ry2,
				Stroke = new SolidColorBrush(Preferences.BlockLineColor),
				StrokeThickness = Preferences.BlockLineWidth
			};
			var l2 = new Line
			{
				X1 = cx1,
				Y1 = cy1,
				X2 = cx2,
				Y2 = cy2,
				Stroke = new SolidColorBrush(Preferences.BlockLineColor),
				StrokeThickness = Preferences.BlockLineWidth
			};

			g.Children.Add(l1);
			g.Children.Add(l2);
		}
	}

	/// <summary>
	/// Draw eliminations.
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	partial void DrawEliminations(Grid g)
	{
		if (Conclusions is null)
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
		if (this is not { View.DirectLines: { } directLines, Preferences.ShowCandidates: false })
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
		if (View.Cells is not { } cells)
		{
			return;
		}

		foreach (var (cell, id) in cells)
		{
			var (row, column) = Calculator.GetGridRowAndColumn(cell);
			if (id is { UseId: false, A: var cA, R: var cR, G: var cG, B: var cB })
			{
				g.AddRectangle(
					row, column, 3, 3, new SolidColorBrush(Color.FromArgb(cA, cR, cG, cB))
				);
			}
			else if (((IPreference)Preferences).TryGetColor(id, out var color))
			{
				g.AddRectangle(
					row, column, 3, 3, new SolidColorBrush(Color.FromArgb(64, color.R, color.G, color.B))
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
					var brush = new SolidColorBrush(
						overlaps ? Color.FromArgb((byte)(color.A >> 2), color.R, color.G, color.B) : color
					);
					g.AddCircle(row, column, size, brush);

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
			var (candFont, candSize) = GetFont(Preferences.CandidateFontName, cellWidth / 2, Preferences.CandidateScale);
			var (r, c) = Calculator.GetGridRowAndColumn(cell, digit);
			g.AddTextBlock(r, c, digit, brush, candFont, candSize);
		}
	}

	/// <summary>
	/// Draw regions.
	/// </summary>
	/// <param name="g">The <see cref="Grid"/> instance.</param>
	partial void DrawRegions(Grid g)
	{
		if (View.Regions is not { } regions)
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
					case >= 0 and < 9:
					{
						var (row, column) = Calculator.GetGridRowAndColumn(RegionFirst[region]);

						g.AddRectangle(row, column, 3, 3, brush, TextOffset, TextOffset);

						break;
					}
					case >= 9 and < 27:
					{
						var (w, h) = Calculator.CellSize;
						var (l, r) = Calculator.GetAnchorsViaRegion(region);
						w /= 2;
						h /= 2;
						l = l.WithOffset(w, h);
						r = r.WithOffset(-w, -h);

						var line = new Line
						{
							X1 = l.X,
							X2 = r.X,
							Y1 = l.Y,
							Y2 = r.Y,
							Stroke = brush,
							StrokeThickness = TextOffset / 2
						};

						g.Children.Add(line);

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

				var brush = new SolidColorBrush(Color.FromArgb(byte.MaxValue >> 2, color.R, color.G, color.B));
				g.AddRectangle(row, column, rowSpan, columnSpan, brush);
			}
		}
	}
}
