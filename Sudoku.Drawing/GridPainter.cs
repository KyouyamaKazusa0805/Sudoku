using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Extensions;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Stepping;
using Sudoku.Drawing.Extensions;
using static System.Math;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Indicates the grid painter.
	/// </summary>
	/// <param name="Converter">Indicates the <see cref="PointConverter"/> instance.</param>
	/// <param name="Preferences">Indicates the <see cref="Settings"/> instance.</param>
	/// <param name="Grid">Indicates the grid.</param>
	/// <remarks>
	/// Please note that eliminations will be colored with red, but all assignments won't be colored,
	/// because they will be colored using another method (draw candidates).
	/// </remarks>
	public sealed record GridPainter(PointConverter Converter, Settings Preferences, UndoableGrid Grid)
	{
		/// <summary>
		/// The square root of 2.
		/// </summary>
		private const float SqrtOf2 = 1.41421356F;

		/// <summary>
		/// The rotate angle (45 degrees, i.e. <c><see cref="PI"/> / 4</c>).
		/// This field is used for rotate the chains if some of them are overlapped.
		/// </summary>
		/// <seealso cref="PI"/>
		private const float RotateAngle = .78539816F;


		/// <summary>
		/// Indicates the drawing width.
		/// </summary>
		public float Width => Converter.ControlSize.Width;

		/// <summary>
		/// Indicates the drawing height.
		/// </summary>
		public float Height => Converter.ControlSize.Height;

		/// <summary>
		/// Indicates the focused cells.
		/// </summary>
		public Cells? FocusedCells { get; set; }

		/// <summary>
		/// Indicates the view.
		/// </summary>
		public View? View { get; set; }

		/// <summary>
		/// Indicates the custom view.
		/// </summary>
		public MutableView? CustomView { get; set; }

		/// <summary>
		/// Indicates all conclusions.
		/// </summary>
		public IEnumerable<Conclusion>? Conclusions { get; set; }


		/// <summary>
		/// To draw the image.
		/// </summary>
		public Bitmap Draw()
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
		public Bitmap Draw(Bitmap? bitmap, Graphics g)
		{
			bitmap ??= new((int)Width, (int)Height);

			DrawBackground(g);
			DrawGridAndBlockLines(g);

			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.CompositingQuality = CompositingQuality.HighQuality;

			const float offset = 6F;
			DrawViewIfNeed(g, offset);
			DrawCustomViewIfNeed(g, offset);
			if (FocusedCells.HasValue) DrawFocusedCells(g, FocusedCells.Value);
			if (Conclusions is not null) DrawEliminations(g, Conclusions, offset);
			if (Grid != SudokuGrid.Undefined) DrawValue(g, Grid);

			return bitmap;
		}

		/// <summary>
		/// Draw givens, modifiables and candidates, where the values are specified as a grid.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="grid">The grid.</param>
		private void DrawValue(Graphics g, UndoableGrid grid)
		{
			float cellWidth = Converter.CellSize.Width;
			float candidateWidth = Converter.CandidateSize.Width;
			float vOffsetValue = cellWidth / 9; // The vertical offset of rendering each value.
			float vOffsetCandidate = candidateWidth / 9; // The vertical offset of rendering each candidate.
			float halfWidth = cellWidth / 2F;

			using var bGiven = new SolidBrush(Preferences.GivenColor);
			using var bModifiable = new SolidBrush(Preferences.ModifiableColor);
			using var bCandidate = new SolidBrush(Preferences.CandidateColor);
			using var fGiven =
				GetFontByScale(Preferences.GivenFontName, halfWidth, Preferences.ValueScale);
			using var fModifiable =
				GetFontByScale(Preferences.ModifiableFontName, halfWidth, Preferences.ValueScale);
			using var fCandidate =
				GetFontByScale(Preferences.CandidateFontName, halfWidth, Preferences.CandidateScale);
			using var sf = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};

			for (int cell = 0; cell < 81; cell++)
			{
				short mask = grid.GetMask(cell);
				var status = SudokuGrid.MaskGetStatus(mask);
				switch (status)
				{
					case CellStatus.Empty when Preferences.ShowCandidates:
					{
						// Draw candidates.
						short candidateMask = (short)(mask & SudokuGrid.MaxCandidatesMask);
						foreach (int digit in candidateMask)
						{
							var point = Converter.GetMousePointInCenter(cell, digit);
							point.Y += vOffsetCandidate;
							g.DrawInt32(digit + 1, fCandidate, bCandidate, point, sf);
						}

						break;
					}
					case CellStatus.Modifiable:
					case CellStatus.Given:
					{
						// Draw values.
						var point = Converter.GetMousePointInCenter(cell);
						point.Y += vOffsetValue;
						g.DrawInt32(
							grid[cell] + 1, status == CellStatus.Given ? fGiven : fModifiable,
							status == CellStatus.Given ? bGiven : bModifiable, point, sf);

						break;
					}
				}
			}
		}

		/// <summary>
		/// Draw custom view if <see cref="CustomView"/> is not <see langword="null"/>.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="offset">The drawing offset.</param>
		/// <seealso cref="CustomView"/>
		private void DrawCustomViewIfNeed(Graphics g, float offset)
		{
			if (CustomView is not null)
			{
				DrawViewIfNeedInternal(g, offset, CustomView);
			}
		}

		/// <summary>
		/// Draw custom view if <see cref="View"/> is not <see langword="null"/>.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="offset">The drawing offset.</param>
		/// <seealso cref="View"/>
		private void DrawViewIfNeed(Graphics g, float offset)
		{
			if (View is not null)
			{
				DrawViewIfNeedInternal(g, offset, View);
			}
		}

		/// <summary>
		/// Draw the specified view.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="offset">The drawing offset.</param>
		/// <param name="view">The view instance.</param>
		private void DrawViewIfNeedInternal(Graphics g, float offset, dynamic view)
		{
			if (view.Regions is IEnumerable<DrawingInfo> regions) DrawRegions(g, regions, offset);
			if (view.Cells is IEnumerable<DrawingInfo> cells) DrawCells(g, cells);
			if (view.Candidates is IEnumerable<DrawingInfo> candidates) DrawCandidates(g, candidates, offset);
			if (view.Links is IEnumerable<Link> links) DrawLinks(g, links, offset);
			if (view.DirectLines is IEnumerable<(Cells, Cells)> directLines) DrawDirectLines(g, directLines, offset);
		}

		/// <summary>
		/// Draw focused cells.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="focusedCells">(<see langword="in"/> parameter) The focused cells.</param>
		private void DrawFocusedCells(Graphics g, in Cells focusedCells)
		{
			using var b = new SolidBrush(Preferences.FocusedCellColor);
			foreach (int cell in focusedCells)
			{
				g.FillRectangle(b, Converter.GetMouseRectangleViaCell(cell));
			}
		}

		/// <summary>
		/// Draw the background, where the color is specified in <see cref="Settings.BackgroundColor"/>.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <seealso cref="Settings.BackgroundColor"/>
		private void DrawBackground(Graphics g) => g.Clear(Preferences.BackgroundColor);

		/// <summary>
		/// Draw grid lines and block lines.
		/// </summary>
		/// <param name="g">The graphics.</param>
		private void DrawGridAndBlockLines(Graphics g)
		{
			using var pg = new Pen(Preferences.GridLineColor, Preferences.GridLineWidth);
			using var pb = new Pen(Preferences.BlockLineColor, Preferences.BlockLineWidth);
			var gridPoints = Converter.GridPoints;
			for (int i = 0; i < 28; i += 3)
			{
				g.DrawLine(pg, gridPoints[i, 0], gridPoints[i, 27]);
				g.DrawLine(pg, gridPoints[0, i], gridPoints[27, i]);
			}

			for (int i = 0; i < 28; i += 9)
			{
				g.DrawLine(pb, gridPoints[i, 0], gridPoints[i, 27]);
				g.DrawLine(pb, gridPoints[0, i], gridPoints[27, i]);
			}
		}

		/// <summary>
		/// Draw eliminations.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="offset">The drawing offset.</param>
		private void DrawEliminations(Graphics g, IEnumerable<Conclusion> conclusions, float offset)
		{
			using var eliminationBrush = new SolidBrush(Preferences.EliminationColor);
			using var cannibalBrush = new SolidBrush(Preferences.CannibalismColor);
			foreach (var (t, c, d) in conclusions)
			{
				if (t != ConclusionType.Elimination)
				{
					continue;
				}

				bool isCannibalism = false;
				if (View is null or { Candidates: null })
				{
					goto Drawing;
				}

				foreach (var (_, value) in View.Candidates)
				{
					if (value == c * 9 + d)
					{
						isCannibalism = true;
						break;
					}
				}

			Drawing:
				g.FillEllipse(
					isCannibalism ? cannibalBrush : eliminationBrush,
					Converter.GetMouseRectangle(c, d).Zoom(-offset / 3));
			}
		}

		/// <summary>
		/// Draw direct lines. The direct lines are the information for hidden singles and naked singles.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="directLines">The direct lines.</param>
		/// <param name="offset">The drawing offset.</param>
		private void DrawDirectLines(Graphics g, IEnumerable<(Cells, Cells)> directLines, float offset)
		{
			if (Preferences.ShowCandidates)
			{
				// Non-direct view (without candidates) don't show this function.
				return;
			}

			foreach (var (start, end) in directLines)
			{
				// Draw start cells (may be a capsule-like shape to block them).
				if (!start.IsEmpty)
				{
					// Step 1: Get the left-up cell and right-down cell to construct a rectangle.
					var p1 = Converter.GetMousePointInCenter(start[0]) - Converter.CellSize / 2;
					var p2 = Converter.GetMousePointInCenter(start[^1]) + Converter.CellSize / 2;
					var rect = RectangleEx.CreateInstance(p1, p2).Zoom(-offset);

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
					var rect = Converter.GetMouseRectangleViaCell(cell).Zoom(-offset * 2);

					// Step 2: Draw cross sign.
					using var pen = new Pen(Preferences.CrossSignColor, 5F);
					g.DrawCrossSign(pen, rect);
				}
			}
		}

		/// <summary>
		/// Draw links.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="links">The links.</param>
		/// <param name="offset">The offset.</param>
		private void DrawLinks(Graphics g, IEnumerable<Link> links, float offset)
		{
			// Gather all points used.
			var points = new HashSet<PointF>();
			foreach (var (startCand, endCand) in links)
			{
				Candidates map1 = new() { startCand }, map2 = new() { endCand };

				points.Add(Converter.GetMouseCenter(map1));
				points.Add(Converter.GetMouseCenter(map2));
			}

			if (Conclusions is not null)
			{
				foreach (var conclusion in Conclusions)
				{
					points.Add(Converter.GetMousePointInCenter(conclusion.Cell, conclusion.Digit));
				}
			}

			// Iterate on each inference to draw the links and grouped nodes (if so).
			var (cw, ch) = Converter.CandidateSize;
			using var arrowPen = new Pen(Preferences.ChainColor, 2F)
			{
				CustomEndCap = new AdjustableArrowCap(cw / 4F, ch / 3F)
			};
			using var linePen = new Pen(Preferences.ChainColor, 2F);

#if OBSOLETE
			// This brush is used for drawing grouped nodes.
			using var groupedNodeBrush = new SolidBrush(Color.FromArgb(64, Color.Yellow));
#endif
			foreach (var (start, end, type) in links)
			{
				arrowPen.DashStyle = type switch
				{
					LinkType.Strong => DashStyle.Solid,
					LinkType.Weak => DashStyle.Dot,
					//LinkType.Default => DashStyle.Dash,
					_ => DashStyle.Dash
				};

				var pt1 = Converter.GetMouseCenter(new() { start });
				var pt2 = Converter.GetMouseCenter(new() { end });
				var (pt1x, pt1y) = pt1;
				var (pt2x, pt2y) = pt2;

#if OBSOLETE
				// Draw grouped node regions.
				if (startMap.Count != 1)
				{
					g.FillRoundedRectangle(
						groupedNodeBrush,
						Converter.GetMouseRectangleOfCandidates(startFullMap),
						offset);
				}
				if (endMap.Count != 1)
				{
					g.FillRoundedRectangle(
						groupedNodeBrush,
						Converter.GetMouseRectangleOfCandidates(endFullMap),
						offset);
				}
#endif

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
					adjust(pt1, pt2, out var p1, out var p2, alpha, cw, offset);
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
							pt2.X, pt2.Y);
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
				in PointF pt1, in PointF pt2, out PointF p1, out PointF p2, double alpha,
				double candidateSize, float offset)
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
				ref PointF pt1, ref PointF pt2, float offset, float cw, float ch,
				float pt1x, float pt1y, float pt2x, float pt2y)
			{
				float slope = Abs((pt2y - pt1y) / (pt2x - pt1x));
				float x = cw / (float)Sqrt(1 + slope * slope);
				float y = ch * (float)Sqrt(slope * slope / (1 + slope * slope));

				float o = offset / 8;
				if (pt1y > pt2y && pt1x == pt2x) { pt1.Y -= ch / 2 - o; pt2.Y += ch / 2 - o; }
				else if (pt1y < pt2y && pt1x == pt2x) { pt1.Y += ch / 2 - o; pt2.Y -= ch / 2 - o; }
				else if (pt1y == pt2y && pt1x > pt2x) { pt1.X -= cw / 2 - o; pt2.X += cw / 2 - o; }
				else if (pt1y == pt2y && pt1x < pt2x) { pt1.X += cw / 2 - o; pt2.X -= cw / 2 - o; }
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
		/// Draw regions.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="regions">The regions.</param>
		/// <param name="offset">The drawing offset.</param>
		/// <remarks>This method is simply implemented, using cell filling.</remarks>
		private void DrawRegions(Graphics g, IEnumerable<DrawingInfo> regions, float offset)
		{
			foreach (var (id, region) in regions)
			{
				if (ColorId.IsCustomColorId(id, out byte aWeight, out byte rWeight, out byte gWeight, out byte bWeight))
				{
					var color = Color.FromArgb(aWeight, rWeight, gWeight, bWeight);
					var rect = Converter.GetMouseRectangleViaRegion(region).Zoom(-offset / 3);
					using var brush = new SolidBrush(color);
					//using var pen = new Pen(color, 6F);
					//g.DrawRectangle(pen, rect.Truncate());
					g.FillRectangle(brush, rect);
				}
				else if (Preferences.PaletteColors.TryGetValue(id, out var color))
				{
					var rect = Converter.GetMouseRectangleViaRegion(region).Zoom(-offset / 3);
					using var brush = new SolidBrush(Color.FromArgb(64, color));
					//using var pen = new Pen(color, 6F);
					//g.DrawRectangle(pen, rect.Truncate());
					g.FillRectangle(brush, rect);
				}
			}
		}

		/// <summary>
		/// Draw candidates.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="candidates">The candidates.</param>
		/// <param name="offset">The drawing offsets.</param>
		private void DrawCandidates(Graphics g, IEnumerable<DrawingInfo> candidates, float offset)
		{
			float cellWidth = Converter.CellSize.Width;
			float candidateWidth = Converter.CandidateSize.Width;
			float vOffsetCandidate = candidateWidth / 9; // The vertical offset of rendering each candidate.

			using var bCandidate = new SolidBrush(Preferences.CandidateColor);
			using var fCandidate =
				GetFontByScale(Preferences.CandidateFontName, cellWidth / 2F, Preferences.CandidateScale);
			using var sf = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};

			foreach (var (id, candidate) in candidates)
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
					if (ColorId.IsCustomColorId(id, out byte aWeight, out byte rWeight, out byte gWeight, out byte bWeight))
					{
						using var brush = new SolidBrush(Color.FromArgb(aWeight, rWeight, gWeight, bWeight));
						g.FillEllipse(brush, Converter.GetMouseRectangle(cell, digit).Zoom(-offset / 3));

						// In direct view, candidates should be drawn also.
						if (!Preferences.ShowCandidates)
						{
							d(cell, digit, vOffsetCandidate);
						}
					}
					else if (Preferences.PaletteColors.TryGetValue(id, out var color))
					{
						// In the normal case, I'll draw these circles.
						using var brush = new SolidBrush(color);
						g.FillEllipse(brush, Converter.GetMouseRectangle(cell, digit).Zoom(-offset / 3));

						// In direct view, candidates should be drawn also.
						if (!Preferences.ShowCandidates)
						{
							d(cell, digit, vOffsetCandidate);
						}
					}
				}
			}

			if (this is { Preferences: { ShowCandidates: false }, Conclusions: not null })
			{
				foreach (var (type, cell, digit) in Conclusions)
				{
					if (type == ConclusionType.Elimination)
					{
						d(cell, digit, vOffsetCandidate);
					}
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void d(int cell, int digit, float vOffsetCandidate)
			{
				var point = Converter.GetMousePointInCenter(cell, digit);
				point.Y += vOffsetCandidate;
				g.DrawInt32(digit + 1, fCandidate, bCandidate, point, sf);
			}
		}

		/// <summary>
		/// Draw cells.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="cells">The cells.</param>
		private void DrawCells(Graphics g, IEnumerable<DrawingInfo> cells)
		{
			foreach (var (id, cell) in cells)
			{
				if (ColorId.IsCustomColorId(id, out byte aWeight, out byte rWeight, out byte gWeight, out byte bWeight))
				{
					var (cw, ch) = Converter.CellSize;
					var (x, y) = Converter.GetMousePointInCenter(cell);
					using var brush = new SolidBrush(Color.FromArgb(aWeight, rWeight, gWeight, bWeight));
					g.FillRectangle(brush, Converter.GetMouseRectangleViaCell(cell)/*.Zoom(-offset)*/);
				}
				else if (Preferences.PaletteColors.TryGetValue(id, out var color))
				{
					var (cw, ch) = Converter.CellSize;
					var (x, y) = Converter.GetMousePointInCenter(cell);
					using var brush = new SolidBrush(Color.FromArgb(64, color));
					g.FillRectangle(brush, Converter.GetMouseRectangleViaCell(cell)/*.Zoom(-offset)*/);
				}
			}
		}


		/// <summary>
		/// Get the font via name, size and the scale.
		/// </summary>
		/// <param name="fontName">The font name.</param>
		/// <param name="size">The size.</param>
		/// <param name="scale">The scale.</param>
		/// <returns>The font.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Font GetFontByScale(string fontName, float size, decimal scale) =>
			new(fontName, size * (float)scale, FontStyle.Regular);
	}
}
