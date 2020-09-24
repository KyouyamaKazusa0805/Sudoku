using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing.Extensions;
using Sudoku.Extensions;
using static System.Drawing.Drawing2D.DashStyle;
using static System.Drawing.Drawing2D.InterpolationMode;
using static System.Drawing.FontStyle;
using static System.Drawing.StringAlignment;
using static System.Drawing.Text.TextRenderingHint;
using static System.Math;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Indicates the grid painter.
	/// </summary>
	/// <remarks>
	/// Please note that eliminations will be colored with red, but all assignments won't be colored,
	/// because they will be colored using another method (draw candidates).
	/// </remarks>
	public sealed class GridPainter
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
		/// Indicates the back field of the property <see cref="PointConverter"/>.
		/// </summary>
		private PointConverter _pointConverter = null!;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="pointConverter">The point converter.</param>
		/// <param name="settings">The instance.</param>
		/// <param name="grid">The grid.</param>
		/// <param name="focusedCells">The focused cells.</param>
		/// <param name="view">The view.</param>
		/// <param name="customView">The custom view.</param>
		/// <param name="conclusions">The conclusions.</param>
		public GridPainter(
			PointConverter pointConverter, Settings settings, Grid? grid = null, GridMap? focusedCells = null,
			View? view = null, MutableView? customView = null, IEnumerable<Conclusion>? conclusions = null)
		{
			PointConverter = pointConverter;
			Settings = settings;
			Grid = grid;
			FocusedCells = focusedCells;
			View = view;
			CustomView = customView;
			Conclusions = conclusions;
		}


		/// <summary>
		/// Indicates the width.
		/// </summary>
		public float Width { get; private set; }

		/// <summary>
		/// Indicates the height.
		/// </summary>
		public float Height { get; private set; }

		/// <summary>
		/// Indicates the focused cells.
		/// </summary>
		public GridMap? FocusedCells { get; set; }

		/// <summary>
		/// Indicates the <see cref="Drawing.Settings"/> instance.
		/// </summary>
		public Settings Settings { get; }

		/// <summary>
		/// Indicates the <see cref="Drawing.PointConverter"/> instance.
		/// </summary>
		/// <value>The converter instance.</value>
		public PointConverter PointConverter
		{
			get => _pointConverter;
			set => (Width, Height) = (_pointConverter = value).ControlSize;
		}

		/// <summary>
		/// Indicates the grid.
		/// </summary>
		public Grid? Grid { get; set; }

		/// <summary>
		/// Indicates the view.
		/// </summary>
		public View? View { get; set; }

		/// <summary>
		/// Indicates the custom view.
		/// </summary>
		public MutableView? CustomView { get; set; }

		/// <summary>
		/// Indicates the conclusions.
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
			g.TextRenderingHint = ClearTypeGridFit;
			g.InterpolationMode = HighQualityBicubic;
			g.CompositingQuality = CompositingQuality.HighQuality;
			const float offset = 6F;
			DrawViewIfNeed(g, offset);
			DrawCustomViewIfNeed(g, offset);
			if (FocusedCells.HasValue) DrawFocusedCells(g, FocusedCells.Value);
			if (Grid is not null) DrawValue(g, Grid);

			return bitmap;
		}

		private void DrawValue(Graphics g, Grid grid)
		{
			float cellWidth = PointConverter.CellSize.Width;
			float candidateWidth = PointConverter.CandidateSize.Width;
			float vOffsetValue = cellWidth / 9; // The vertical offset of rendering each value.
			float vOffsetCandidate = candidateWidth / 9; // The vertical offset of rendering each candidate.

			using var bGiven = new SolidBrush(Settings.GivenColor);
			using var bModifiable = new SolidBrush(Settings.ModifiableColor);
			using var bCandidate = new SolidBrush(Settings.CandidateColor);
			using var fGiven = GetFontByScale(Settings.GivenFontName, cellWidth / 2F, Settings.ValueScale);
			using var fModifiable = GetFontByScale(Settings.ModifiableFontName, cellWidth / 2F, Settings.ValueScale);
			using var fCandidate = GetFontByScale(Settings.CandidateFontName, cellWidth / 2F, Settings.CandidateScale);
			using var sf = new StringFormat { Alignment = Center, LineAlignment = Center };

			for (int cell = 0; cell < 81; cell++)
			{
				short mask = grid.GetMask(cell);

				// Firstly, draw values.
				var status = (CellStatus)(mask >> 9 & (int)All);
				switch (status)
				{
					case Empty when Settings.ShowCandidates:
					{
						// Draw candidates.
						short candidateMask = (short)(~mask & Grid.MaxCandidatesMask);
						foreach (int digit in candidateMask.GetAllSets())
						{
							var point = PointConverter.GetMousePointInCenter(cell, digit);
							point.Y += vOffsetCandidate;
							g.DrawString((digit + 1).ToString(), fCandidate, bCandidate, point, sf);
						}

						break;
					}
					case Modifiable or Given:
					{
						// Draw values.
						var point = PointConverter.GetMousePointInCenter(cell);
						point.Y += vOffsetValue;
						g.DrawString(
							(grid[cell] + 1).ToString(), status == Given ? fGiven : fModifiable,
							status == Given ? bGiven : bModifiable, point, sf);

						break;
					}
				}
			}
		}

		private void DrawCustomViewIfNeed(Graphics g, float offset)
		{
			if (CustomView is not null)
			{
				if (CustomView.Cells is not null) DrawCells(g, CustomView.Cells);
				if (CustomView.Candidates is not null) DrawCandidates(g, CustomView.Candidates, offset);
				if (CustomView.Regions is not null) DrawRegions(g, CustomView.Regions, offset);
				if (CustomView.Links is not null) DrawLinks(g, CustomView.Links, offset);
				if (CustomView.DirectLines is not null) DrawDirectLines(g, CustomView.DirectLines, offset);
			}
		}

		private void DrawViewIfNeed(Graphics g, float offset)
		{
			if (View is not null)
			{
				if (View.Cells is not null) DrawCells(g, View.Cells);
				if (View.Candidates is not null) DrawCandidates(g, View.Candidates, offset);
				if (View.Regions is not null) DrawRegions(g, View.Regions, offset);
				if (View.Links is not null) DrawLinks(g, View.Links, offset);
				if (View.DirectLines is not null) DrawDirectLines(g, View.DirectLines, offset);
			}

			if (Conclusions is not null) DrawEliminations(g, Conclusions, offset);
		}

		private void DrawFocusedCells(Graphics g, GridMap focusedCells)
		{
			using var b = new SolidBrush(Settings.FocusedCellColor);
			foreach (int cell in focusedCells)
			{
				g.FillRectangle(b, PointConverter.GetMousePointRectangle(cell));
			}
		}

		private void DrawBackground(Graphics g) => g.Clear(Settings.BackgroundColor);

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

		private void DrawEliminations(Graphics g, IEnumerable<Conclusion> conclusions, float offset)
		{
			using var eliminationBrush = new SolidBrush(Settings.EliminationColor);
			using var cannibalBrush = new SolidBrush(Settings.CannibalismColor);
			foreach (var (t, c, d) in conclusions)
			{
				switch (t)
				{
					// Every assignment conclusion will be painted in its technique information view.
					//case Assignment:
					//{
					//	break;
					//}
					case Elimination:
					{
						g.FillEllipse(
							View?.Candidates?.Any(pair => pair.Value == c * 9 + d) ?? false ? cannibalBrush : eliminationBrush,
							PointConverter.GetMousePointRectangle(c, d).Zoom(-offset / 3));
						break;
					}
				}
			}
		}

		private void DrawDirectLines(Graphics g, IEnumerable<(GridMap, GridMap)> directLines, float offset)
		{
			foreach (var (start, end) in directLines)
			{
				var (cw, ch) = PointConverter.CellSize;

				// Draw start cells (may be a capsule-like shape to block them).
				{
					// Step 1: Get the left-up cell and right-down cell to construct a rectangle.
					var p1 = PointConverter.GetMousePointInCenter(start.SetAt(0));
					var p2 = PointConverter.GetMousePointInCenter(start.SetAt(^1));
					var rect = RectangleEx.CreateInstance(p1, p2).Zoom(-offset / 3);

					// Step 2: Draw capsule.
					using var pen = new Pen(Color.FromArgb(192, Color.Black));
					using var brush = new SolidBrush(Color.FromArgb(64, Color.Black));
					g.DrawCapsule(pen, rect);
					g.FillCapsule(brush, rect);
				}

				// Draw end cells (may be using cross sign to represent the current cell cannot fill that digit).
				{
					foreach (int cell in end)
					{
						// Step 1: Get the left-up cell and right-down cell to construct a rectangle.
						var rect = PointConverter.GetMousePointRectangle(cell).Zoom(-offset / 3);

						// Step 2: Draw cross sign.
						using var pen = new Pen(Color.FromArgb(192, Color.Red), 10F);
						g.DrawCrossSign(pen, rect);
					}
				}
			}
		}

		private void DrawLinks(Graphics g, IEnumerable<Link> links, float offset)
		{
			// Gather all points used.
			var points = new HashSet<PointF>();
			foreach (var (startCand, endCand, _) in links)
			{
				var map1 = new SudokuMap() { startCand };
				var map2 = new SudokuMap() { endCand };

				points.Add(PointConverter.GetMouseCenterOfCandidates(map1));
				points.Add(PointConverter.GetMouseCenterOfCandidates(map2));
			}

			if (Conclusions is not null)
			{
				points.AddRange(
					from conclusion in Conclusions
					select PointConverter.GetMousePointInCenter(conclusion.Cell, conclusion.Digit));
			}

			// Iterate on each inference to draw the links and grouped nodes (if so).
			var (cw, ch) = PointConverter.CandidateSize;
			using var pen = new Pen(Settings.ChainColor, 2F) { CustomEndCap = new AdjustableArrowCap(cw / 4F, ch / 3F) };
			#region Obsolete code
			// This brush is used for drawing grouped nodes.
			//using var groupedNodeBrush = new SolidBrush(Color.FromArgb(64, Color.Yellow));
			#endregion
			foreach (var link in links)
			{
				var (start, end, type) = link;
				pen.DashStyle = type switch
				{
					LinkType.Strong => Solid,
					LinkType.Weak => Dot,
					LinkType.Default => Dash,
					_ => Dash
				};

				var pt1 = PointConverter.GetMouseCenterOfCandidates(new() { start });
				var pt2 = PointConverter.GetMouseCenterOfCandidates(new() { end });
				var (pt1x, pt1y) = pt1;
				var (pt2x, pt2y) = pt2;

				#region Obsolete code
				// Draw grouped node regions.
				//if (startMap.Count != 1)
				//{
				//	g.FillRoundedRectangle(
				//		groupedNodeBrush,
				//		PointConverter.GetMouseRectangleOfCandidates(startFullMap),
				//		offset);
				//}
				//if (endMap.Count != 1)
				//{
				//	g.FillRoundedRectangle(
				//		groupedNodeBrush,
				//		PointConverter.GetMouseRectangleOfCandidates(endFullMap),
				//		offset);
				//}
				#endregion

				// If the distance of two points is lower than the one of two adjacent candidates,
				// the link will be emitted to draw because of too narrow.
				double distance = Sqrt((pt1x - pt2x) * (pt1x - pt2x) + (pt1y - pt2y) * (pt1y - pt2y));
				if (distance <= cw * SqrtOf2 + offset || distance <= ch * SqrtOf2 + offset)
				{
					continue;
				}

				// Check if another candidate lies on the direct line.
				double deltaX = pt2x - pt1x;
				double deltaY = pt2y - pt1y;
				double alpha = Atan2(deltaY, deltaX);
				double dx1 = deltaX;
				double dy1 = deltaY;
				bool through = false;
				AdjustPoint(pt1, pt2, out var p1, out var p2, alpha, cw, offset);
				foreach (var point in points)
				{
					if (point == pt1 || point == pt2)
					{
						// Self...
						continue;
					}

					double dx2 = point.X - p1.X;
					double dy2 = point.Y - p1.Y;
					if (Sign(dx1) == Sign(dx2) && Sign(dy1) == Sign(dy2)
						&& Abs(dx2) <= Abs(dx1) && Abs(dy2) <= Abs(dy1)
						&& (dx1 == 0 || dy1 == 0 || (dx1 / dy1).NearlyEquals(dx2 / dy2, 1E-1)))
					{
						through = true;
						break;
					}
				}

				// Now cut the link.
				CutLink(ref pt1, ref pt2, offset, cw, ch, pt1x, pt1y, pt2x, pt2y);

				if (through)
				{
					double bezierLength = 20;

					// The end points are rotated 45 degrees
					// (counterclockwise for the start point, clockwise for the end point).
					var oldPt1 = new PointF(pt1x, pt1y);
					var oldPt2 = new PointF(pt2x, pt2y);
					RotatePoint(oldPt1, ref pt1, -RotateAngle);
					RotatePoint(oldPt2, ref pt2, RotateAngle);

					double aAlpha = alpha - RotateAngle;
					double bx1 = pt1.X + bezierLength * Cos(aAlpha);
					double by1 = pt1.Y + bezierLength * Sin(aAlpha);
					aAlpha = alpha + RotateAngle;
					double bx2 = pt2.X - bezierLength * Cos(aAlpha);
					double by2 = pt2.Y - bezierLength * Sin(aAlpha);

					g.DrawBezier(pen, pt1.X, pt1.Y, (float)bx1, (float)by1, (float)bx2, (float)by2, pt2.X, pt2.Y);
				}
				else
				{
					// Draw the link.
					g.DrawLine(pen, pt1, pt2);
				}
			}
		}

		private void DrawRegions(Graphics g, IEnumerable<DrawingInfo> regions, float offset)
		{
			foreach (var (id, region) in regions)
			{
				if (Settings.PaletteColors.TryGetValue(id, out var color))
				{
					using var brush = new SolidBrush(Color.FromArgb(32, color));
					g.FillRectangle(brush, PointConverter.GetMousePointRectangleForRegion(region).Zoom(-offset / 3));
				}
			}
		}

		private void DrawCandidates(Graphics g, IEnumerable<DrawingInfo> candidates, float offset)
		{
			foreach (var (id, candidate) in candidates)
			{
				int cell = candidate / 9, digit = candidate % 9;
				if (!(Conclusions?.Any(conc => c(conc, cell, digit)) ?? false)
					&& Settings.PaletteColors.TryGetValue(id, out var color))
				{
					using var brush = new SolidBrush(color);
					g.FillEllipse(brush, PointConverter.GetMousePointRectangle(cell, digit).Zoom(-offset / 3));
				}
			}

			static bool c(in Conclusion conc, int cell, int digit)
			{
				var (ttt, ccc, ddd) = conc;
				return ccc == cell && ddd == digit && ttt == Elimination;
			}
		}

		private void DrawCells(Graphics g, IEnumerable<DrawingInfo> cells)
		{
			foreach (var (id, cell) in cells)
			{
				if (Settings.PaletteColors.TryGetValue(id, out var color))
				{
					var (cw, ch) = PointConverter.CellSize;
					var (x, y) = PointConverter.GetMousePointInCenter(cell);
					using var brush = new SolidBrush(Color.FromArgb(64, color));
					g.FillRectangle(brush, PointConverter.GetMousePointRectangle(cell)/*.Zoom(-offset)*/);
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
		private static Font GetFontByScale(string fontName, float size, decimal scale) =>
			new(fontName, size * (float)scale, Regular);

		/// <summary>
		/// To cut the link to let the head and the tail is outside the candidate.
		/// </summary>
		/// <param name="pt1">(<see langword="ref"/> parameter) The point 1.</param>
		/// <param name="pt2">(<see langword="ref"/> parameter) The point 2.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="cw">The candidate width.</param>
		/// <param name="ch">The candidate height.</param>
		/// <param name="pt1x">The x value of point 1.</param>
		/// <param name="pt1y">The y value of point 1.</param>
		/// <param name="pt2x">The x value of point 2.</param>
		/// <param name="pt2y">The y value of point 2.</param>
		private static void CutLink(
			ref PointF pt1, ref PointF pt2, float offset, float cw, float ch,
			float pt1x, float pt1y, float pt2x, float pt2y)
		{
			float slope = Abs((pt2y - pt1y) / (pt2x - pt1x));
			float x = cw / (float)Sqrt(1 + slope * slope);
			float y = ch * (float)Sqrt(slope * slope / (1 + slope * slope));

			float o = offset / 8;
			if (pt1y > pt2y && pt1x == pt2x)
			{
				pt1.Y -= ch / 2 - o;
				pt2.Y += ch / 2 - o;
			}
			else if (pt1y < pt2y && pt1x == pt2x)
			{
				pt1.Y += ch / 2 - o;
				pt2.Y -= ch / 2 - o;
			}
			else if (pt1y == pt2y && pt1x > pt2x)
			{
				pt1.X -= cw / 2 - o;
				pt2.X += cw / 2 - o;
			}
			else if (pt1y == pt2y && pt1x < pt2x)
			{
				pt1.X += cw / 2 - o;
				pt2.X -= cw / 2 - o;
			}
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

		/// <summary>
		/// Rotate the point.
		/// </summary>
		/// <param name="pt1">The point 1.</param>
		/// <param name="pt2">(<see langword="ref"/> parameter) The point 2.</param>
		/// <param name="angle">The angle.</param>
		private static void RotatePoint(PointF pt1, ref PointF pt2, double angle)
		{
			// Translate 'pt2' to (0, 0).
			pt2.X -= pt1.X;
			pt2.Y -= pt1.Y;

			// Rotate.
			double sinAngle = Sin(angle);
			double cosAngle = Cos(angle);
			double xAct = pt2.X;
			double yAct = pt2.Y;
			pt2.X = (float)(xAct * cosAngle - yAct * sinAngle);
			pt2.Y = (float)(xAct * sinAngle + yAct * cosAngle);

			pt2.X += pt1.X;
			pt2.Y += pt1.Y;
		}

		/// <summary>
		/// Adjust the end points of an arrow: the arrow should start and end outside
		/// the circular background of the candidate.
		/// </summary>
		/// <param name="pt1">The point 1.</param>
		/// <param name="pt2">The point 2.</param>
		/// <param name="p1">(<see langword="out"/> parameter) The point 1.</param>
		/// <param name="p2">(<see langword="out"/> parameter) The point 2.</param>
		/// <param name="alpha">The angle.</param>
		/// <param name="candidateSize">The candidate size.</param>
		/// <param name="offset">The offset.</param>
		private static void AdjustPoint(
			PointF pt1, PointF pt2, out PointF p1, out PointF p2, double alpha, double candidateSize, float offset)
		{
			p1 = pt1;
			p2 = pt2;
			double tempDelta = candidateSize / 2 + offset;
			int px = (int)(tempDelta * Cos(alpha));
			int py = (int)(tempDelta * Sin(alpha));

			p1.X += px;
			p1.Y += py;
			p2.X -= px;
			p2.Y -= py;
		}
	}
}
