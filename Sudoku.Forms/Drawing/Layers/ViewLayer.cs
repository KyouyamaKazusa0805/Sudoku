using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing.Extensions;
using Sudoku.Solving;
using static System.Math;

namespace Sudoku.Drawing.Layers
{
	/// <summary>
	/// Provides a technique information layer (only shows a view).
	/// </summary>
	public sealed partial class ViewLayer : Layer
	{
		/// <summary>
		/// The square root of 2.
		/// </summary>
		private const float SqrtOf2 = 1.41421356F;


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
		private readonly View _view;

		/// <summary>
		/// Indicates the color dictionary.
		/// </summary>
		private readonly IDictionary<int, Color> _colorDic;

		/// <summary>
		/// The all conclusions.
		/// </summary>
		private readonly IEnumerable<Conclusion> _conclusions;


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
		public ViewLayer(
			PointConverter pointConverter, View view, IEnumerable<Conclusion> conclusions,
			IDictionary<int, Color> colorDic, Color eliminationColor, Color cannibalismColor,
			Color chainColor) : base(pointConverter) =>
			(_view, _conclusions, _colorDic, _eliminationColor, _cannibalismColor, _chainColor) = (view, conclusions, colorDic, eliminationColor, cannibalismColor, chainColor);


		/// <inheritdoc/>
		public override int Priority => 4;


		/// <inheritdoc/>
		protected override void Draw()
		{
			const float offset = 6F;
			var bitmap = new Bitmap((int)Width, (int)Height);
			using var g = Graphics.FromImage(bitmap);
			g.SmoothingMode = SmoothingMode.AntiAlias;

			DrawCells(g, offset);
			DrawCandidates(g, offset);
			DrawRegions(g, offset);
			DrawLinks(g, offset);
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
			foreach (var (t, c, d) in _conclusions)
			{
				switch (t)
				{
					// Every assignment conclusion will be painted
					// in its technique information view.
					//case ConclusionType.Assignment:
					//{
					//	break;
					//}
					case ConclusionType.Elimination:
					{
						g.FillEllipse(
							_view.CandidateOffsets.Any(pair => pair._candidateOffset == c * 9 + d)
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

			//var drawedLinks = new List<DrawingLine>();
			var (cw, ch) = _pointConverter.CandidateSize;
			using var pen = new Pen(_chainColor, 1.5F)
			{
				CustomEndCap = new AdjustableArrowCap(cw / 6F, cw / 4F)
			};
			using var groupedNodeBrush = new SolidBrush(Color.FromArgb(64, Color.Yellow));
			foreach (var inference in _view.Links)
			{
				var ((startCandidates, startNodeType), (endCandidates, endNodeType)) = inference;
				pen.DashStyle = true switch
				{
					_ when inference.IsStrong => DashStyle.Solid,
					_ when inference.IsWeak => DashStyle.Dash,
					_ => DashStyle.Dot
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
				// the link will be emitted because of narrow.
				double distance = Sqrt((pt1x - pt2x) * (pt1x - pt2x) + (pt1y - pt2y) * (pt1y - pt2y));
				if (distance <= cw * SqrtOf2 + offset || distance <= ch * SqrtOf2 + offset)
				{
					continue;
				}

				// Now cut the link.
				CutLink(ref pt1, ref pt2, offset, cw, ch, pt1x, pt1y, pt2x, pt2y);

				// Draw the link.
				g.DrawLine(pen, pt1, pt2);

				//var line = new DrawingLine((int)pt1x, (int)pt1y, (int)pt2x, (int)pt2y);
				//bool hasOverlapped = false;
				//foreach (var drawedLink in drawedLinks)
				//{
				//	if (line.Overlaps(drawedLink))
				//	{
				//		hasOverlapped = true;
				//		break;
				//	}
				//}
				//
				//if (hasOverlapped)
				//{
				//
				//}
				//else
				//{
				//	// Now cut the link.
				//	CutLink(ref pt1, ref pt2, offset, cw, ch, pt1x, pt1y, pt2x, pt2y);
				//
				//	// Draw the link.
				//	g.DrawLine(pen, pt1, pt2);
				//}
				//
				//drawedLinks.Add(line);
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
					g.FillRectangle(
						brush, _pointConverter.GetMousePointRectangleForRegion(region).Zoom(-offset / 3));
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
				if (!_conclusions.Any(c => c.CellOffset == cell && c.Digit == digit)
					&& _colorDic.TryGetValue(id, out var color))
				{
					var (cw, ch) = _pointConverter.CandidateSize;
					var (x, y) = _pointConverter.GetMousePointInCenter(cell, digit);
					using var brush = new SolidBrush(color);
					g.FillEllipse(
						brush, _pointConverter.GetMousePointRectangle(cell, digit).Zoom(-offset / 3));
				}
			}
		}

		/// <summary>
		/// To draw cells.
		/// </summary>
		/// <param name="g">The graphics.</param>
		/// <param name="offset">The offset.</param>
		private void DrawCells(Graphics g, float offset)
		{
			foreach (var (id, cell) in _view.CellOffsets ?? Array.Empty<(int, int)>())
			{
				if (_colorDic.TryGetValue(id, out var color))
				{
					var (cw, ch) = _pointConverter.CellSize;
					var (x, y) = _pointConverter.GetMousePointInCenter(cell);
					using var brush = new SolidBrush(Color.FromArgb(32, color));
					g.FillRectangle(brush, _pointConverter.GetMousePointRectangle(cell).Zoom(-offset));
				}
			}
		}


		/// <summary>
		/// To cut the link to let the head and the tail is outside the candidate.
		/// </summary>
		/// <param name="pt1">The point 1.</param>
		/// <param name="pt2">The point 2.</param>
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
	}
}
