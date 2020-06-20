using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing.Extensions;
using Sudoku.Extensions;
using Sudoku.Windows;
using static System.Drawing.Drawing2D.DashStyle;
using static System.Drawing.FontStyle;
using static System.Drawing.StringAlignment;
using static System.Math;
using static Sudoku.Data.CellStatus;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Indicates the target painter.
	/// </summary>
	/// <remarks>
	/// This data structure is a little bit heavy.
	/// </remarks>
	public sealed class TargetPainter
	{
		/// <summary>
		/// The square root of 2.
		/// </summary>
		private const float SqrtOf2 = 1.41421356F;

		/// <summary>
		/// The rotate angle (45 degrees, i.e. <c><see cref="PI"/> / 4</c>).
		/// This field is used for rotate the chains if some of them are overlapped.
		/// </summary>
		private const float RotateAngle = .78539816F;


		/// <summary>
		/// Indicates the focused cells.
		/// </summary>
		private readonly GridMap? _focusedCells;

		/// <summary>
		/// Indicates the <see cref="Settings"/> instance.
		/// </summary>
		private readonly Settings _settings;

		/// <summary>
		/// Indicates the <see cref="PointConverter"/> instance.
		/// </summary>
		private readonly PointConverter _pointConverter;

		/// <summary>
		/// Indicates the grid.
		/// </summary>
		private readonly Grid? _grid;

		/// <summary>
		/// Indicates the view.
		/// </summary>
		private readonly View? _view;

		/// <summary>
		/// Indicates the custom view.
		/// </summary>
		private readonly MutableView? _customView;

		/// <summary>
		/// Indicates the conclusions.
		/// </summary>
		private readonly IEnumerable<Conclusion>? _conclusions;


		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="pointConverter">The point converter.</param>
		/// <param name="settings">The instance.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		/// <param name="focusedCells">The focused cells.</param>
		/// <param name="view">The view.</param>
		/// <param name="conclusions">The conclusions.</param>
		public TargetPainter(
			PointConverter pointConverter, Settings settings, int width, int height, Grid? grid,
			GridMap? focusedCells, View? view, MutableView? customView, IEnumerable<Conclusion>? conclusions)
		{
			(Width, Height) = (width, height);
			_settings = settings;
			_pointConverter = pointConverter;
			_grid = grid;
			_focusedCells = focusedCells;
			_view = view;
			_customView = customView;
			_conclusions = conclusions;
		}


		/// <summary>
		/// Indicates the width.
		/// </summary>
		public int Width { get; }

		/// <summary>
		/// Indicates the height.
		/// </summary>
		public int Height { get; }


		/// <summary>
		/// To draw the image.
		/// </summary>
		public Bitmap Draw()
		{
			var result = new Bitmap(Width, Height);
			using var g = Graphics.FromImage(result);

			DrawBackground(g);
			DrawGridAndBlockLines(g);
			if (_focusedCells.HasValue)
			{
				DrawFocusedCells(g, _focusedCells.Value);
			}

			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = TextRenderingHint.AntiAlias;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.CompositingQuality = CompositingQuality.HighQuality;
			const float offset = 6F;
			DrawViewIfNeed(g, offset);
			DrawCustomViewIfNeed(g, offset);

			if (!(_grid is null))
			{
				DrawValue(g, _grid);
			}

			return result;
		}

		private void DrawValue(Graphics g, Grid grid)
		{
			float cellWidth = _pointConverter.CellSize.Width;
			float candidateWidth = _pointConverter.CandidateSize.Width;
			float vOffsetValue = cellWidth / 9; // The vertical offset of rendering each value.
			float vOffsetCandidate = candidateWidth / 9; // The vertical offset of rendering each candidate.

			using var bGiven = new SolidBrush(_settings.GivenColor);
			using var bModifiable = new SolidBrush(_settings.ModifiableColor);
			using var bCandidate = new SolidBrush(_settings.CandidateColor);
			using var fGiven = GetFontByScale(_settings.GivenFontName, cellWidth / 2F, _settings.ValueScale);
			using var fModifiable = GetFontByScale(_settings.ModifiableFontName, cellWidth / 2F, _settings.ValueScale);
			using var fCandidate = GetFontByScale(_settings.CandidateFontName, cellWidth / 2F, _settings.CandidateScale);
			using var sf = new StringFormat { Alignment = Center, LineAlignment = Center };

			for (int cell = 0; cell < 81; cell++)
			{
				short mask = grid.GetMask(cell);

				// Firstly, draw values.
				var status = (CellStatus)(mask >> 9 & (int)All);
				switch (status)
				{
					case Empty when _settings.ShowCandidates:
					{
						// Draw candidates.
						short candidateMask = (short)(~mask & Grid.MaxCandidatesMask);
						foreach (int digit in candidateMask.GetAllSets())
						{
							var point = _pointConverter.GetMousePointInCenter(cell, digit);
							point.Y += vOffsetCandidate;
							g.DrawString((digit + 1).ToString(), fCandidate, bCandidate, point, sf);
						}

						break;
					}
					case Modifiable:
					case Given:
					{
						// Draw values.
						var point = _pointConverter.GetMousePointInCenter(cell);
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
			if (!(_customView is null))
			{
				if (!(_customView.CellOffsets is null)) DrawCells(g, _customView.CellOffsets);
				if (!(_customView.CandidateOffsets is null)) DrawCandidates(g, _customView.CandidateOffsets, offset);
				if (!(_customView.RegionOffsets is null)) DrawRegions(g, _customView.RegionOffsets, offset);
				if (!(_customView.Links is null)) DrawLinks(g, _customView.Links, offset);
			}
		}

		private void DrawViewIfNeed(Graphics g, float offset)
		{
			if (!(_view is null))
			{
				if (!(_view.CellOffsets is null)) DrawCells(g, _view.CellOffsets);
				if (!(_view.CandidateOffsets is null)) DrawCandidates(g, _view.CandidateOffsets, offset);
				if (!(_view.RegionOffsets is null)) DrawRegions(g, _view.RegionOffsets, offset);
				if (!(_view.Links is null)) DrawLinks(g, _view.Links, offset);
			}

			if (!(_conclusions is null)) DrawEliminations(g, _conclusions, offset);
		}

		private void DrawFocusedCells(Graphics g, GridMap focusedCells)
		{
			using var b = new SolidBrush(_settings.FocusedCellColor);
			foreach (int cell in focusedCells)
			{
				g.FillRectangle(b, _pointConverter.GetMousePointRectangle(cell));
			}
		}

		private void DrawBackground(Graphics g) => g.Clear(_settings.BackgroundColor);

		private void DrawGridAndBlockLines(Graphics g)
		{
			using var pg = new Pen(_settings.GridLineColor, Width);
			using var pb = new Pen(_settings.BlockLineColor, Width);
			var gridPoints = _pointConverter.GridPoints;
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
			using var eliminationBrush = new SolidBrush(_settings.EliminationColor);
			using var cannibalBrush = new SolidBrush(_settings.CannibalismColor);
			foreach (var (t, c, d) in conclusions)
			{
				switch (t)
				{
					// Every assignment conclusion will be painted
					// in its technique information view.
					//case Assignment:
					//{
					//	break;
					//}
					case Elimination:
					{
						g.FillEllipse(
							_view?.CandidateOffsets?.Any(pair => pair._candidateOffset == c * 9 + d) ?? false
								? cannibalBrush
								: eliminationBrush,
							_pointConverter.GetMousePointRectangle(c, d).Zoom(-offset / 3));
						break;
					}
				}
			}
		}

		private void DrawLinks(Graphics g, IEnumerable<Inference> links, float offset)
		{
			// Gather all points used.
			var points = new HashSet<PointF>();
			foreach (var inference in links)
			{
				points.Add(_pointConverter.GetMouseCenterOfCandidates(inference.Start.CandidatesMap));
				points.Add(_pointConverter.GetMouseCenterOfCandidates(inference.End.CandidatesMap));
			}

			if (!(_conclusions is null))
			{
				points.AddRange(
					from conclusion in _conclusions
					select _pointConverter.GetMousePointInCenter(conclusion.CellOffset, conclusion.Digit));
			}

			// Iterate on each inference to draw the links and grouped nodes (if so).
			var (cw, ch) = _pointConverter.CandidateSize;
			using var pen = new Pen(_settings.ChainColor, 2F) { CustomEndCap = new AdjustableArrowCap(cw / 4F, ch / 3F) };
			using var groupedNodeBrush = new SolidBrush(Color.FromArgb(64, Color.Yellow));
			foreach (var inference in links)
			{
				var ((startCandidates, startNodeType), (endCandidates, endNodeType)) = inference;
				pen.DashStyle = true switch
				{
					_ when inference.IsStrong => Solid,
					_ when inference.IsWeak => Dot,
					_ => Dash
				};

				var pt1 = _pointConverter.GetMouseCenterOfCandidates(startCandidates);
				var pt2 = _pointConverter.GetMouseCenterOfCandidates(endCandidates);
				var (pt1x, pt1y) = pt1;
				var (pt2x, pt2y) = pt2;

				// Draw grouped node regions.
				if (startNodeType != NodeType.Candidate)
				{
					g.FillRoundedRectangle(
						groupedNodeBrush,
						_pointConverter.GetMouseRectangleOfCandidates(startCandidates),
						offset);
				}
				if (endNodeType != NodeType.Candidate)
				{
					g.FillRoundedRectangle(
						groupedNodeBrush,
						_pointConverter.GetMouseRectangleOfCandidates(endCandidates),
						offset);
				}

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

		private void DrawRegions(Graphics g, IEnumerable<(int, int)> regionOffsets, float offset)
		{
			foreach (var (id, region) in regionOffsets)
			{
				if (_settings.PaletteColors.TryGetValue(id, out var color))
				{
					using var brush = new SolidBrush(Color.FromArgb(32, color));
					g.FillRectangle(brush, _pointConverter.GetMousePointRectangleForRegion(region).Zoom(-offset / 3));
				}
			}
		}

		private void DrawCandidates(Graphics g, IEnumerable<(int, int)> candidateOffsets, float offset)
		{
			foreach (var (id, candidate) in candidateOffsets)
			{
				int cell = candidate / 9, digit = candidate % 9;
				if (!(
					_conclusions?.Any(c =>
					{
						var (ttt, ccc, ddd) = c;
						return ccc == cell && ddd == digit && ttt == Elimination;
					}) ?? false)
					&& _settings.PaletteColors.TryGetValue(id, out var color))
				{
					using var brush = new SolidBrush(color);
					g.FillEllipse(brush, _pointConverter.GetMousePointRectangle(cell, digit).Zoom(-offset / 3));
				}
			}
		}

		private void DrawCells(Graphics g, IEnumerable<(int, int)> cellOffsets)
		{
			foreach (var (id, cell) in cellOffsets)
			{
				if (_settings.PaletteColors.TryGetValue(id, out var color))
				{
					var (cw, ch) = _pointConverter.CellSize;
					var (x, y) = _pointConverter.GetMousePointInCenter(cell);
					using var brush = new SolidBrush(Color.FromArgb(64, color));
					g.FillRectangle(brush, _pointConverter.GetMousePointRectangle(cell)/*.Zoom(-offset)*/);
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
			new Font(fontName, size * (float)scale, Regular);

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
