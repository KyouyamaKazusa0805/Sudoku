using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Data.Stepping;
using Sudoku.Drawing.Extensions;
using Sudoku.Extensions;
using static System.Drawing.Drawing2D.DashStyle;
using static System.Drawing.StringAlignment;
using static System.Math;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;
using static Sudoku.Data.LinkType;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Indicates the grid painter.
	/// </summary>
	/// <param name="PointConverter">Indicates the <see cref="Drawing.PointConverter"/> instance.</param>
	/// <param name="Settings">Indicates the <see cref="Drawing.Settings"/> instance.</param>
	/// <param name="Width">Indicates the width.</param>
	/// <param name="Height">Indicates the height.</param>
	/// <param name="Grid">Indicates the grid.</param>
	/// <param name="FocusedCells">Indicates the focused cells.</param>
	/// <param name="View">Indicates the view.</param>
	/// <param name="CustomView">Indicates the custom view.</param>
	/// <param name="Conclusions">All conclusions.</param>
	/// <remarks>
	/// <para>
	/// Please note that eliminations will be colored with red, but all assignments won't be colored,
	/// because they will be colored using another method (draw candidates).
	/// </para>
	/// <para>
	/// In addition, please use the constructor <see cref="GridPainter(PointConverter, Settings, UndoableGrid)"/>
	/// instead of <see langword="init"/> <see cref="PointConverter"/> or <see langword="with"/>
	/// expression to assign the value on <see cref="PointConverter"/>, to ensure the points
	/// can be re-calculated.
	/// </para>
	/// </remarks>
	/// <seealso cref="GridPainter(PointConverter, Settings, UndoableGrid)"/>
	/// <seealso cref="Width"/>
	/// <seealso cref="Height"/>
	/// <seealso cref="PointConverter"/>
	public sealed record GridPainter(
		PointConverter PointConverter, Settings Settings, float Width, float Height, UndoableGrid Grid,
		GridMap? FocusedCells, View? View, MutableView? CustomView, IEnumerable<Conclusion>? Conclusions)
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
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="pointConverter">The point converter.</param>
		/// <param name="settings">The instance.</param>
		/// <param name="grid">The grid.</param>
		public GridPainter(PointConverter pointConverter, Settings settings, UndoableGrid grid)
			: this(pointConverter, settings, default, default, grid, null, null, null, null) =>
			(Width, Height) = pointConverter.ControlSize;


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
			float cellWidth = PointConverter.CellSize.Width;
			float candidateWidth = PointConverter.CandidateSize.Width;
			float vOffsetValue = cellWidth / 9; // The vertical offset of rendering each value.
			float vOffsetCandidate = candidateWidth / 9; // The vertical offset of rendering each candidate.
			float halfWidth = cellWidth / 2F;

			using var bGiven = new SolidBrush(Settings.GivenColor);
			using var bModifiable = new SolidBrush(Settings.ModifiableColor);
			using var bCandidate = new SolidBrush(Settings.CandidateColor);
			using var fGiven = GetFontByScale(Settings.GivenFontName, halfWidth, Settings.ValueScale);
			using var fModifiable = GetFontByScale(Settings.ModifiableFontName, halfWidth, Settings.ValueScale);
			using var fCandidate = GetFontByScale(Settings.CandidateFontName, halfWidth, Settings.CandidateScale);
			using var sf = new StringFormat { Alignment = Center, LineAlignment = Center };

			for (int cell = 0; cell < 81; cell++)
			{
				short mask = grid.GetMask(cell);
				var status = (CellStatus)(mask >> 9 & (int)All);
				switch (status)
				{
					case Empty when Settings.ShowCandidates:
					{
						// Draw candidates.
						short candidateMask = (short)(mask & SudokuGrid.MaxCandidatesMask);
						foreach (int digit in candidateMask)
						{
							var point = PointConverter.GetMousePointInCenter(cell, digit);
							point.Y += vOffsetCandidate;
							g.DrawString((digit + 1).ToString(), fCandidate, bCandidate, point, sf);
						}

						break;
					}
					case Modifiable or Given when PointConverter.GetMousePointInCenter(cell) is var point:
					{
						// Draw values.
						point.Y += vOffsetValue;
						g.DrawString(
							(grid[cell] + 1).ToString(), status == Given ? fGiven : fModifiable,
							status == Given ? bGiven : bModifiable, point, sf);

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
			if (view.DirectLines is IEnumerable<(GridMap, GridMap)> directLines) DrawDirectLines(g, directLines, offset);
		}

		/// <summary>
		/// Draw focused cells.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="focusedCells">(<see langword="in"/> parameter) The focused cells.</param>
		private void DrawFocusedCells(Graphics g, in GridMap focusedCells)
		{
			using var b = new SolidBrush(Settings.FocusedCellColor);
			foreach (int cell in focusedCells)
			{
				g.FillRectangle(b, PointConverter.GetMouseRectangleViaCell(cell));
			}
		}

		/// <summary>
		/// Draw the background, where the color is specified in <see cref="Settings.BackgroundColor"/>.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <seealso cref="Settings.BackgroundColor"/>
		private void DrawBackground(Graphics g) => g.Clear(Settings.BackgroundColor);

		/// <summary>
		/// Draw grid lines and block lines.
		/// </summary>
		/// <param name="g">The graphics.</param>
		private void DrawGridAndBlockLines(Graphics g)
		{
			using var pg = new Pen(Settings.GridLineColor, Settings.GridLineWidth);
			using var pb = new Pen(Settings.BlockLineColor, Settings.BlockLineWidth);
			var gridPoints = PointConverter.GridPoints;
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
			using var eliminationBrush = new SolidBrush(Settings.EliminationColor);
			using var cannibalBrush = new SolidBrush(Settings.CannibalismColor);
			unsafe
			{
				foreach (var (t, c, d) in from c in conclusions where c.ConclusionType == Elimination select c)
				{
					g.FillEllipse(
						View?.Candidates?.Any(&overlapping, c, d) ?? false ? cannibalBrush : eliminationBrush,
						PointConverter.GetMouseRectangle(c, d).Zoom(-offset / 3));
				}
			}

			static bool overlapping(DrawingInfo pair, in int c, in int d) => pair.Value == c * 9 + d;
		}

		/// <summary>
		/// Draw direct lines. The direct lines are the information for hidden singles and naked singles.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="directLines">The direct lines.</param>
		/// <param name="offset">The drawing offset.</param>
		private void DrawDirectLines(Graphics g, IEnumerable<(GridMap, GridMap)> directLines, float offset)
		{
			if (Settings.ShowCandidates)
			{
				// Non-direct view (without candidates) don't show this function.
				return;
			}

			foreach (var (start, end) in directLines)
			{
				// Draw start cells (may be a capsule-like shape to block them).
				if (start.IsNotEmpty)
				{
					// Step 1: Get the left-up cell and right-down cell to construct a rectangle.
					var p1 = PointConverter.GetMousePointInCenter(start.SetAt(0)) - PointConverter.CellSize / 2;
					var p2 = PointConverter.GetMousePointInCenter(start.SetAt(^1)) + PointConverter.CellSize / 2;
					var rect = RectangleEx.CreateInstance(p1, p2).Zoom(-offset);

					// Step 2: Draw capsule.
					using var pen = new Pen(Settings.CrosshatchingOutlineColor, 3F);
					using var brush = new SolidBrush(Settings.CrosshatchingInnerColor);
					g.DrawEllipse(pen, rect);
					g.FillEllipse(brush, rect);
				}

				// Draw end cells (may be using cross sign to represent the current cell can't fill that digit).
				foreach (int cell in end)
				{
					// Step 1: Get the left-up cell and right-down cell to construct a rectangle.
					var rect = PointConverter.GetMouseRectangleViaCell(cell).Zoom(-offset * 2);

					// Step 2: Draw cross sign.
					using var pen = new Pen(Settings.CrossSignColor, 5F);
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
				SudokuMap map1 = new() { startCand }, map2 = new() { endCand };

				points.Add(PointConverter.GetMouseCenter(map1));
				points.Add(PointConverter.GetMouseCenter(map2));
			}

			if (Conclusions is not null)
			{
				points.AddRange(
					from conclusion in Conclusions
					select PointConverter.GetMousePointInCenter(conclusion.Cell, conclusion.Digit));
			}

			// Iterate on each inference to draw the links and grouped nodes (if so).
			var (cw, ch) = PointConverter.CandidateSize;
			using var arrowPen = new Pen(Settings.ChainColor, 2F)
			{
				CustomEndCap = new AdjustableArrowCap(cw / 4F, ch / 3F)
			};
			using var linePen = new Pen(Settings.ChainColor, 2F);

#if OBSOLETE
			// This brush is used for drawing grouped nodes.
			using var groupedNodeBrush = new SolidBrush(Color.FromArgb(64, Color.Yellow));
#endif
			foreach (var link in links)
			{
				var (start, end, type) = link;
				arrowPen.DashStyle = type switch { Strong => Solid, Weak => Dot, Default => Dash, _ => Dash };

				var pt1 = PointConverter.GetMouseCenter(new() { start });
				var pt2 = PointConverter.GetMouseCenter(new() { end });
				var (pt1x, pt1y) = pt1;
				var (pt2x, pt2y) = pt2;

#if OBSOLETE
				// Draw grouped node regions.
				if (startMap.Count != 1)
				{
					g.FillRoundedRectangle(
						groupedNodeBrush,
						PointConverter.GetMouseRectangleOfCandidates(startFullMap),
						offset);
				}
				if (endMap.Count != 1)
				{
					g.FillRoundedRectangle(
						groupedNodeBrush,
						PointConverter.GetMouseRectangleOfCandidates(endFullMap),
						offset);
				}
#endif

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
				AdjustPoint(pt1, pt2, out var p1, out var p2, alpha, cw, offset);
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
				CutLink(ref pt1, ref pt2, offset, cw, ch, pt1x, pt1y, pt2x, pt2y);

				var penToDraw = type != Line ? arrowPen : linePen;
				if (through)
				{
					double bezierLength = 20;

					// The end points are rotated 45 degrees
					// (counterclockwise for the start point, clockwise for the end point).
					PointF oldPt1 = new(pt1x, pt1y), oldPt2 = new(pt2x, pt2y);
					RotatePoint(oldPt1, ref pt1, -RotateAngle);
					RotatePoint(oldPt2, ref pt2, RotateAngle);

					double aAlpha = alpha - RotateAngle;
					double bx1 = pt1.X + bezierLength * Cos(aAlpha), by1 = pt1.Y + bezierLength * Sin(aAlpha);
					aAlpha = alpha + RotateAngle;
					double bx2 = pt2.X - bezierLength * Cos(aAlpha), by2 = pt2.Y - bezierLength * Sin(aAlpha);

					g.DrawBezier(
						penToDraw, pt1.X, pt1.Y, (float)bx1, (float)by1, (float)bx2, (float)by2, pt2.X, pt2.Y);
				}
				else
				{
					// Draw the link.
					g.DrawLine(penToDraw, pt1, pt2);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static void RotatePoint(in PointF pt1, ref PointF pt2, double angle)
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
			static void AdjustPoint(
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
			static void CutLink(
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
					var rect = PointConverter.GetMouseRectangleViaRegion(region).Zoom(-offset / 3);
					using var brush = new SolidBrush(color);
					//using var pen = new Pen(color, 6F);
					//g.DrawRectangle(pen, rect.Truncate());
					g.FillRectangle(brush, rect);
				}
				else if (Settings.PaletteColors.TryGetValue(id, out var color))
				{
					var rect = PointConverter.GetMouseRectangleViaRegion(region).Zoom(-offset / 3);
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
			float cellWidth = PointConverter.CellSize.Width;
			float candidateWidth = PointConverter.CandidateSize.Width;
			float vOffsetCandidate = candidateWidth / 9; // The vertical offset of rendering each candidate.

			using var bCandidate = new SolidBrush(Settings.CandidateColor);
			using var fCandidate = GetFontByScale(Settings.CandidateFontName, cellWidth / 2F, Settings.CandidateScale);
			using var sf = new StringFormat { Alignment = Center, LineAlignment = Center };

			unsafe
			{
				static bool overlaps(Conclusion conc, in int cell, in int digit)
				{
					var (ttt, ccc, ddd) = conc;
					return (ttt, ccc, ddd) == (Elimination, cell, digit);
				}

				foreach (var (id, candidate) in candidates)
				{
					int cell = candidate / 9, digit = candidate % 9;
					if (!(Conclusions?.Any(&overlaps, cell, digit) ?? false))
					{
						if (ColorId.IsCustomColorId(id, out byte aWeight, out byte rWeight, out byte gWeight, out byte bWeight))
						{
							using var brush = new SolidBrush(Color.FromArgb(aWeight, rWeight, gWeight, bWeight));
							g.FillEllipse(brush, PointConverter.GetMouseRectangle(cell, digit).Zoom(-offset / 3));

							// In direct view, candidates should be drawn also.
							if (!Settings.ShowCandidates)
							{
								d(cell, digit, vOffsetCandidate);
							}
						}
						else if (Settings.PaletteColors.TryGetValue(id, out var color))
						{
							// In the normal case, I'll draw these circles.
							using var brush = new SolidBrush(color);
							g.FillEllipse(brush, PointConverter.GetMouseRectangle(cell, digit).Zoom(-offset / 3));

							// In direct view, candidates should be drawn also.
							if (!Settings.ShowCandidates)
							{
								d(cell, digit, vOffsetCandidate);
							}
						}
					}
				}
			}

			if (!Settings.ShowCandidates && Conclusions is not null)
			{
				foreach (var (type, cell, digit) in Conclusions)
				{
					if (type == Elimination)
					{
						d(cell, digit, vOffsetCandidate);
					}
				}
			}

			void d(int cell, int digit, float vOffsetCandidate)
			{
				var point = PointConverter.GetMousePointInCenter(cell, digit);
				point.Y += vOffsetCandidate;
				g.DrawString((digit + 1).ToString(), fCandidate, bCandidate, point, sf);
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
				if (
					ColorId.IsCustomColorId(
						id, out byte aWeight, out byte rWeight, out byte gWeight, out byte bWeight))
				{
					var (cw, ch) = PointConverter.CellSize;
					var (x, y) = PointConverter.GetMousePointInCenter(cell);
					using var brush = new SolidBrush(Color.FromArgb(aWeight, rWeight, gWeight, bWeight));
					g.FillRectangle(brush, PointConverter.GetMouseRectangleViaCell(cell)/*.Zoom(-offset)*/);
				}
				else if (Settings.PaletteColors.TryGetValue(id, out var color))
				{
					var (cw, ch) = PointConverter.CellSize;
					var (x, y) = PointConverter.GetMousePointInCenter(cell);
					using var brush = new SolidBrush(Color.FromArgb(64, color));
					g.FillRectangle(brush, PointConverter.GetMouseRectangleViaCell(cell)/*.Zoom(-offset)*/);
				}
			}
		}


		/// <summary>
		/// Initialize custom view if the current property is <see langword="null"/>.
		/// </summary>
		/// <param name="painter">(<see langword="ref"/> parameter) The current painter instance.</param>
		[MemberNotNull(nameof(CustomView))]
		public static void InitializeCustomViewIfNull(ref GridPainter painter)
		{
			if (painter.CustomView is null)
			{
				painter = painter with { CustomView = new() };
			}
		}

		/// <summary>
		/// Get the font via name, size and the scale.
		/// </summary>
		/// <param name="fontName">The font name.</param>
		/// <param name="size">The size.</param>
		/// <param name="scale">The scale.</param>
		/// <returns>The font.</returns>
		private static Font GetFontByScale(string fontName, float size, decimal scale) =>
			new(fontName, size * (float)scale, FontStyle.Regular);
	}
}
