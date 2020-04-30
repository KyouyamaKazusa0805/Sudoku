using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing.Extensions;
using Sudoku.Drawing.Layers;
using Sudoku.Extensions;
using Sudoku.Solving;
using static System.Math;
using static Sudoku.Solving.ConclusionType;

namespace Sudoku.Drawing
{
	public sealed class CustomViewLayer : Layer
	{
		/// <summary>
		/// The square root of 2.
		/// </summary>
		private const float SqrtOf2 = 1.41421356F;

		/// <summary>
		/// The rotate angle (45 degrees, i.e. <c><see cref="PI"/> / 4</c>).
		/// </summary>
		private const float RotateAngle = .78539816F;


		/// <summary>
		/// Indicates the elimination color.
		/// </summary>
		private readonly Color _eliminationColor;

		/// <summary>
		/// Indicates the cannibalism color.
		/// </summary>
		private readonly Color _cannibalismColor;

		/// <summary>
		/// Indicates the chain color.
		/// </summary>
		private readonly Color _chainColor;

		/// <summary>
		/// The technique information.
		/// </summary>
		private readonly MutableView _view;

		/// <summary>
		/// Indicates the color dictionary.
		/// </summary>
		private readonly IDictionary<int, Color> _colorDic;

		/// <summary>
		/// The all conclusions.
		/// </summary>
		private readonly IEnumerable<Conclusion>? _conclusions;


		/// <summary>
		/// Initializes an instance with the specified view and the color dictionary.
		/// </summary>
		/// <param name="pointConverter">The point converter.</param>
		/// <param name="view">The view.</param>
		/// <param name="conclusions">The conclusions.</param>
		/// <param name="colorDic">The color dictionary.</param>
		/// <param name="eliminationColor">The elimination color.</param>
		/// <param name="cannibalismColor">The cannibalism color.</param>
		/// <param name="chainColor">The chain color.</param>
		public CustomViewLayer(
			PointConverter pointConverter, MutableView view, IEnumerable<Conclusion>? conclusions,
			IDictionary<int, Color> colorDic, Color eliminationColor, Color cannibalismColor,
			Color chainColor) : base(pointConverter) =>
			(_view, _conclusions, _colorDic, _eliminationColor, _cannibalismColor, _chainColor) = (view, conclusions, colorDic, eliminationColor, cannibalismColor, chainColor);


		/// <inheritdoc/>
		public override int Priority => 5;


		/// <inheritdoc/>
		protected override void Draw()
		{
			const float offset = 6F;
			var bitmap = new Bitmap((int)Width, (int)Height);
			using var g = Graphics.FromImage(bitmap);
			g.SmoothingMode = SmoothingMode.AntiAlias;

			if (!(_view is null))
			{
				DrawCells(g);
				DrawCandidates(g, offset);
				DrawRegions(g, offset);
				DrawLinks(g, offset);
			}

			DrawEliminations(g, offset);

			Target = bitmap;
		}

		/// <summary>
		/// Draw eliminations.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="offset">The offset.</param>
		private void DrawEliminations(Graphics g, float offset)
		{
			using var eliminationBrush = new SolidBrush(_eliminationColor);
			using var cannibalBrush = new SolidBrush(_cannibalismColor);
			foreach (var (t, c, d) in _conclusions ?? Array.Empty<Conclusion>())
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
							_view?.CandidateOffsets?.Any(pair => pair._candidate == c * 9 + d) ?? false
								? cannibalBrush
								: eliminationBrush,
							_pointConverter.GetMousePointRectangle(c, d).Zoom(-offset / 3));
						break;
					}
				}
			}
		}

		/// <summary>
		/// To draw links.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="offset">The offset.</param>
		private void DrawLinks(Graphics g, float offset)
		{
			if (_view.Links is null)
			{
				return;
			}

			// Record all points used.
			var points = new HashSet<PointF>();
			foreach (var inference in _view.Links)
			{
				points.Add(_pointConverter.GetMouseCenterOfCandidates(inference.Start.CandidatesMap));
				points.Add(_pointConverter.GetMouseCenterOfCandidates(inference.End.CandidatesMap));
			}
			points.AddRange(
				from conclusion in _conclusions ?? Array.Empty<Conclusion>()
				select _pointConverter.GetMousePointInCenter(conclusion.CellOffset, conclusion.Digit));

			// Iterate on each inference to draw the links and grouped nodes (if so).
			var (cw, ch) = _pointConverter.CandidateSize;
			using var pen = new Pen(_chainColor, 2F)
			{
				CustomEndCap = new AdjustableArrowCap(cw / 4F, ch / 3F)
			};
			using var groupedNodeBrush = new SolidBrush(Color.FromArgb(64, Color.Yellow));
			foreach (var inference in _view.Links)
			{
				var ((startCandidates, startNodeType), (endCandidates, endNodeType)) = inference;
				pen.DashStyle = true switch
				{
					_ when inference.IsStrong => DashStyle.Solid,
					_ when inference.IsWeak => DashStyle.Dot,
					_ => DashStyle.Dash
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
						&& (dx1 == 0 || dy1 == 0 || (dx1 / dy1).NearlyEquals(dx2 / dy2, 1e-1)))
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

					// The end points are rotated 45 degrees (counterclockwise
					// for the start point, clockwise for the end point).
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

		/// <summary>
		/// Draw regions.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="offset">The offset.</param>
		private void DrawRegions(Graphics g, float offset)
		{
			foreach (var (id, region) in _view.RegionOffsets ?? Array.Empty<(int, int)>())
			{
				if (_colorDic.TryGetValue(id, out var color))
				{
					using var brush = new SolidBrush(Color.FromArgb(32, color));
					g.FillRectangle(brush, _pointConverter.GetMousePointRectangleForRegion(region).Zoom(-offset / 3));
				}
			}
		}

		/// <summary>
		/// To draw candidates.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="offset">The offset.</param>
		private void DrawCandidates(Graphics g, float offset)
		{
			foreach (var (id, candidate) in _view.CandidateOffsets ?? Array.Empty<(int, int)>())
			{
				int cell = candidate / 9, digit = candidate % 9;
				if (!(
					_conclusions?.Any(
						c => c.CellOffset == cell && c.Digit == digit && c.ConclusionType == Elimination) ?? false)
					&& _colorDic.TryGetValue(id, out var color))
				{
					var (cw, ch) = _pointConverter.CandidateSize;
					var (x, y) = _pointConverter.GetMousePointInCenter(cell, digit);
					using var brush = new SolidBrush(color);
					g.FillEllipse(brush, _pointConverter.GetMousePointRectangle(cell, digit).Zoom(-offset / 3));
				}
			}
		}

		/// <summary>
		/// To draw cells.
		/// </summary>
		/// <param name="g">The graphics.</param>
		private void DrawCells(Graphics g)
		{
			foreach (var (id, cell) in _view.CellOffsets ?? Array.Empty<(int, int)>())
			{
				if (_colorDic.TryGetValue(id, out var color))
				{
					var (cw, ch) = _pointConverter.CellSize;
					var (x, y) = _pointConverter.GetMousePointInCenter(cell);
					using var brush = new SolidBrush(Color.FromArgb(64, color));
					g.FillRectangle(brush, _pointConverter.GetMousePointRectangle(cell)/*.Zoom(-offset)*/);
				}
			}
		}


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
			PointF pt1, PointF pt2, out PointF p1, out PointF p2, double alpha,
			double candidateSize, float offset)
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
