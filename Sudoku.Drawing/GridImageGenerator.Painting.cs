#if SUDOKU_UI

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Extensions;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.Drawing.Extensions;

namespace Sudoku.Drawing
{
	partial record GridImageGenerator
	{
		/// <summary>
		/// The square root of 2.
		/// </summary>
		private const float SquareRootOfTwo = 1.414214F;

		/// <summary>
		/// The rotate angle (45 degrees).
		/// This field is used for rotate the chains if some of them are overlapped.
		/// </summary>
		private const float RotateAngle = MathF.PI / 4;


		/// <summary>
		/// To paint background with the color specified in preferences.
		/// </summary>
		/// <param name="g">The graphics instance.</param>
		partial void PaintBackground(Graphics g) => g.Clear(Preferences.BackgroundColor);

		/// <summary>
		/// To paint grid lines and block lines with the color specified in preferences.
		/// </summary>
		/// <param name="g">The graphics instance.</param>
		partial void PaintGridAndBlockLines(Graphics g)
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
		/// To display the presentation data.
		/// </summary>
		/// <param name="g">The graphics instance.</param>
		/// <param name="view">The presentation data that you want to display.</param>
		/// <param name="offset">The offset that is assigned and used in drawing and rendering.</param>
		partial void PaintPresentationData(Graphics g, PresentationData? view, float offset)
		{
			if (view is null)
			{
				return;
			}

			PaintRegions(g, view.Regions, offset);
			PaintCells(g, view.Cells);
			PaintCandidates(g, view.Candidates, offset);
			PaintLinks(g, view.Links, offset);
			PaintDirectLines(g, view.DirectLines, offset);
		}

		/// <summary>
		/// To paint the focused cells.
		/// </summary>
		/// <param name="g">The graphics instance.</param>
		partial void PaintFocusedCells(Graphics g)
		{
			if (FocusedCells.IsEmpty)
			{
				return;
			}

			using var b = new SolidBrush(Preferences.FocusedCellColor);
			foreach (int cell in FocusedCells)
			{
				g.FillRectangle(b, Converter.GetMouseRectangleViaCell(cell));
			}
		}

		/// <summary>
		/// To paint eliminations.
		/// </summary>
		/// <param name="g">The graphics instance.</param>
		/// <param name="offset">
		/// The offset that is used to render. When we draw the eliminations, the algorithm uses
		/// a circle to render the candidate is start to remove. The candidate border line (outline)
		/// and the circle don't intersect with each other. The offset is used for this point.
		/// </param>
		partial void PaintEliminations(Graphics g, float offset)
		{
			if (Conclusions is null)
			{
				return;
			}

			using var eliminationBrush = new SolidBrush(Preferences.EliminationColor);
			using var cannibalBrush = new SolidBrush(Preferences.CannibalismColor);
			foreach (var (t, c, d) in Conclusions)
			{
				if (t != ConclusionType.Elimination)
				{
					continue;
				}

				bool isCannibalism = false;
				if (View is not { Candidates: not null })
				{
					goto Drawing;
				}

				foreach (var (_, _, _, value) in View.Candidates)
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
					Converter.GetMouseRectangleViaCandidate(c, d).Zoom(-offset / 3));
			}
		}

		/// <summary>
		/// To paint values (givens, modifiables and candidates).
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <remarks>
		/// Different with <see cref="PaintEliminations(Graphics, float)"/>, this method will draw text
		/// values of the candidates, while that method only displays the elimination circle
		/// marked on that candidate text.
		/// </remarks>
		/// <seealso cref="PaintEliminations(Graphics, float)"/>
		partial void PaintValues(Graphics g)
		{
			float cellWidth = Converter.CellSize.Width;
			float candidateWidth = Converter.CandidateSize.Width;
			float vOffsetValue = cellWidth / 9; // The vertical offset of rendering each value.
			float vOffsetCandidate = candidateWidth / 9; // The vertical offset of rendering each candidate.
			float halfWidth = cellWidth / 2;

			using var bGiven = new SolidBrush(Preferences.GivenColor);
			using var bModifiable = new SolidBrush(Preferences.ModifiableColor);
			using var bCandidate = new SolidBrush(Preferences.CandidateColor);
			using var fGiven = GetFont(Preferences.GivenFontName, halfWidth, Preferences.ValueScale);
			using var fModifiable = GetFont(Preferences.ModifiableFontName, halfWidth, Preferences.ValueScale);
			using var fCandidate = GetFont(Preferences.CandidateFontName, halfWidth, Preferences.CandidateScale);
			using var sf = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};

			for (int cell = 0; cell < 81; cell++)
			{
				short mask = Grid.GetMask(cell);
				var status = SudokuGrid.MaskGetStatus(mask);
				switch (status)
				{
					case CellStatus.Empty when Preferences.ShowCandidates:
					{
						// Draw candidates.
						short candidateMask = (short)(mask & SudokuGrid.MaxCandidatesMask);
						foreach (int digit in candidateMask)
						{
							var point = Converter.GetMouseCenter(cell, digit);
							point.Y += vOffsetCandidate;
							g.DrawString((digit + 1).ToString(), fCandidate, bCandidate, point, sf);
						}

						break;
					}
					case CellStatus.Modifiable:
					case CellStatus.Given:
					{
						// Draw values.
						var point = Converter.GetMouseCenter(cell);
						point.Y += vOffsetValue;
						g.DrawString(
							(Grid[cell] + 1).ToString(), status == CellStatus.Given ? fGiven : fModifiable,
							status == CellStatus.Given ? bGiven : bModifiable, point, sf);

						break;
					}
				}
			}
		}

		/// <summary>
		/// To paint the cells in the presentation data.
		/// </summary>
		/// <param name="g">The graphics instance.</param>
		/// <param name="cells">The cells specified in the presentation data.</param>
		partial void PaintCells(Graphics g, ICollection<PaintingPair<int>>? cells)
		{
			if (cells is null)
			{
				return;
			}

			foreach (var (usePalette, id, color, cell) in cells)
			{
				if (!usePalette || !Preferences.TryGetPaletteColor(id, out var colorToDraw))
				{
					colorToDraw = color;
				}

				using var brush = new SolidBrush(colorToDraw);
				g.FillRectangle(brush, Converter.GetMouseRectangleViaCell(cell));
			}
		}

		/// <summary>
		/// To paint the candidates in the presentation data.
		/// </summary>
		/// <param name="g">The graphics instance.</param>
		/// <param name="candidates">The candidates specified in the presentation data.</param>
		/// <param name="offset">The offset.</param>
		partial void PaintCandidates(Graphics g, ICollection<PaintingPair<int>>? candidates, float offset)
		{
			if (candidates is null)
			{
				return;
			}

			// The offset values.
			// 'vOffsetCandidate': The vertical offset of rendering each candidate.
			float cellWidth = Converter.CellSize.Width, candidateWidth = Converter.CandidateSize.Width;
			float halfWidth = cellWidth / 2, vOffsetCandidate = candidateWidth / 9;

			using var bCandidate = new SolidBrush(Preferences.CandidateColor);
			using var fCandidate = GetFont(Preferences.CandidateFontName, halfWidth, Preferences.CandidateScale);
			using var sf = new StringFormat
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};

			foreach (var (usePalette, id, color, candidate) in candidates)
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
					if (!usePalette || !Preferences.TryGetPaletteColor(id, out var colorToDraw))
					{
						colorToDraw = color;
					}

					int cell = candidate / 9, digit = candidate % 9;

					using var brush = new SolidBrush(colorToDraw);
					g.FillEllipse(brush, Converter.GetMouseRectangleViaCandidate(cell, digit).Zoom(-offset / 3));

					// Candidates should be painted also.
					if (!Preferences.ShowCandidates)
					{
						d(cell, digit, vOffsetCandidate);
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
				var point = Converter.GetMouseCenter(cell, digit);
				point.Y += vOffsetCandidate;
				g.DrawString((digit + 1).ToString(), fCandidate, bCandidate, point, sf);
			}
		}

		/// <summary>
		/// To paint the regions in the presentation data.
		/// </summary>
		/// <param name="g">The graphics instance.</param>
		/// <param name="regions">The regions specified in the presentation data.</param>
		/// <param name="offset">The offset.</param>
		partial void PaintRegions(Graphics g, ICollection<PaintingPair<int>>? regions, float offset)
		{
			if (regions is null)
			{
				return;
			}

			foreach (var (usePalette, id, color, region) in regions)
			{
				if (!usePalette || !Preferences.TryGetPaletteColor(id, out var colorToDraw))
				{
					colorToDraw = color;
				}

				var rect = Converter.GetMouseRectangleViaRegion(region).Zoom(-offset / 3);
				using var brush = new SolidBrush(colorToDraw);
				g.FillRectangle(brush, rect);
			}
		}

		/// <summary>
		/// To paint the links in the presentation data.
		/// </summary>
		/// <param name="g">The graphics instance.</param>
		/// <param name="links">The links specified in the presentation data.</param>
		/// <param name="offset">The offset.</param>
		partial void PaintLinks(Graphics g, ICollection<PaintingPair<Link>>? links, float offset)
		{
			if (links is null)
			{
				return;
			}

			// Gather all points used.
			var points = new HashSet<PointF>();
			foreach (var (_, _, _, (startCand, endCand, _)) in links)
			{
				Candidates map1 = new() { startCand }, map2 = new() { endCand };

				points.Add(Converter.GetMouseCenter(map1));
				points.Add(Converter.GetMouseCenter(map2));
			}

			if (Conclusions is not null)
			{
				foreach (var (_, cell, digit) in Conclusions)
				{
					points.Add(Converter.GetMouseCenter(cell, digit));
				}
			}

			// Iterate on each inference to draw the links and grouped nodes (if so).
			var (cw, ch) = Converter.CandidateSize;

			// This brush is used for drawing grouped nodes.
			//using var groupedNodeBrush = new SolidBrush(Color.FromArgb(64, Color.Yellow));

			foreach (var (usePalette, id, color, (start, end, type)) in links)
			{
				if (!usePalette || !Preferences.TryGetPaletteColor(id, out var colorToDraw))
				{
					colorToDraw = color;
				}

				using var linePen = new Pen(colorToDraw, 2F);
				using var arrowPen = new Pen(colorToDraw, 2F)
				{
					CustomEndCap = new AdjustableArrowCap(cw / 4F, ch / 3F),
					DashStyle = type switch
					{
						LinkType.Strong => DashStyle.Solid,
						LinkType.Weak => DashStyle.Dot,
						//LinkType.Default => DashStyle.Dash,
						_ => DashStyle.Dash
					}
				};

				var pt1 = Converter.GetMouseCenter(new Candidates() { start });
				var pt2 = Converter.GetMouseCenter(new Candidates() { end });
				var (pt1x, pt1y) = pt1;
				var (pt2x, pt2y) = pt2;

				// Draw grouped node regions.
				//if (startMap.Count != 1)
				//{
				//	g.FillRoundedRectangle(
				//		groupedNodeBrush,
				//		Converter.GetMouseRectangleOfCandidates(startFullMap),
				//		offset);
				//}
				//if (endMap.Count != 1)
				//{
				//	g.FillRoundedRectangle(
				//		groupedNodeBrush,
				//		Converter.GetMouseRectangleOfCandidates(endFullMap),
				//		offset);
				//}

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
					float distance = MathF.Sqrt((pt1x - pt2x) * (pt1x - pt2x) + (pt1y - pt2y) * (pt1y - pt2y));
					if (distance <= cw * SquareRootOfTwo + offset || distance <= ch * SquareRootOfTwo + offset)
					{
						continue;
					}

					// Check if another candidate lies in the direct line.
					float deltaX = pt2x - pt1x, deltaY = pt2y - pt1y;
					float alpha = MathF.Atan2(deltaY, deltaX);
					float dx1 = deltaX, dy1 = deltaY;
					bool through = false;
					adjust(pt1, pt2, out var p1, out _, alpha, cw, offset);
					foreach (var point in points)
					{
						if (point == pt1 || point == pt2)
						{
							// Self...
							continue;
						}

						float dx2 = point.X - p1.X, dy2 = point.Y - p1.Y;
						if (Math.Sign(dx1) == Math.Sign(dx2) && Math.Sign(dy1) == Math.Sign(dy2)
							&& Math.Abs(dx2) <= Math.Abs(dx1) && Math.Abs(dy2) <= Math.Abs(dy1)
							&& (dx1 == 0 || dy1 == 0 || (dx1 / dy1).NearlyEquals(dx2 / dy2, epsilon: 1E-1F)))
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
						double bx1 = pt1.X + bezierLength * Math.Cos(aAlpha);
						double by1 = pt1.Y + bezierLength * Math.Sin(aAlpha);

						aAlpha = alpha + RotateAngle;
						double bx2 = pt2.X - bezierLength * Math.Cos(aAlpha);
						double by2 = pt2.Y - bezierLength * Math.Sin(aAlpha);

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
			static void rotate(in PointF pt1, ref PointF pt2, float angle)
			{
				// Translate 'pt2' to (0, 0).
				pt2.X -= pt1.X;
				pt2.Y -= pt1.Y;

				// Rotate.
				double sinAngle = MathF.Sin(angle), cosAngle = MathF.Cos(angle);
				double xAct = pt2.X, yAct = pt2.Y;
				pt2.X = (float)(xAct * cosAngle - yAct * sinAngle);
				pt2.Y = (float)(xAct * sinAngle + yAct * cosAngle);

				pt2.X += pt1.X;
				pt2.Y += pt1.Y;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static void adjust(
				in PointF pt1, in PointF pt2, out PointF p1, out PointF p2, float alpha,
				double candidateSize, float offset)
			{
				p1 = pt1;
				p2 = pt2;
				double tempDelta = candidateSize / 2 + offset;
				int px = (int)(tempDelta * MathF.Cos(alpha)), py = (int)(tempDelta * MathF.Sin(alpha));

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
				float slope = MathF.Abs((pt2y - pt1y) / (pt2x - pt1x));
				float x = cw / MathF.Sqrt(1 + slope * slope);
				float y = ch * MathF.Sqrt(slope * slope / (1 + slope * slope));

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
		/// To paint the direct lines in the presentation data.
		/// </summary>
		/// <param name="g">The graphics instance.</param>
		/// <param name="directLines">The direct lines specified in the presentation data.</param>
		/// <param name="offset">The offset.</param>
		partial void PaintDirectLines(
			Graphics g, ICollection<PaintingPair<(Cells, Cells)>>? directLines, float offset)
		{
			if (directLines is null)
			{
				return;
			}

			if (Preferences.ShowCandidates)
			{
				// Non-direct view (without candidates) don't show this function.
				return;
			}

			foreach (var (usePalette, id, color, (start, end)) in directLines)
			{
				if (!usePalette || !Preferences.TryGetPaletteColor(id, out var colorToDraw))
				{
					colorToDraw = color;
				}

				// Draw start cells (may be a capsule-like shape to block them).
				if (!start.IsEmpty)
				{
					// Step 1: Get the left-up cell and right-down cell to construct a rectangle.
					var rect = RectangleEx.CreateInstance(
						Converter.GetMouseCenter(start[0]) - Converter.CellSize / 2,
						Converter.GetMouseCenter(start[^1]) + Converter.CellSize / 2
					).Zoom(-offset);

					// Step 2: Draw capsule.
					using var pen = new Pen(colorToDraw, 3F);
					g.DrawEllipse(pen, rect);
				}

				// Draw end cells (may be using cross sign to represent the current cell can't fill that digit).
				foreach (int cell in end)
				{
					// Step 1: Get the left-up cell and right-down cell to construct a rectangle.
					var rect = Converter.GetMouseRectangleViaCell(cell).Zoom(-offset * 2);

					// Step 2: Draw cross sign.
					using var pen = new Pen(colorToDraw, 5F);
					g.DrawCrossSign(pen, rect);
				}
			}
		}
	}
}

#endif