using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Sudoku.Data;
using Sudoku.Drawing.Extensions;
using Sudoku.Solving;

namespace Sudoku.Drawing.Layers
{
	/// <summary>
	/// Provides a technique information layer (only shows a view).
	/// </summary>
	public sealed class ViewLayer : Layer
	{
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
			Color chainColor)
			: base(pointConverter) =>
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

			foreach (var (id, candidate) in _view.CandidateOffsets ?? Array.Empty<(int, int)>())
			{
				if (_colorDic.TryGetValue(id, out var color))
				{
					int cell = candidate / 9, digit = candidate % 9;
					var (cw, ch) = _pointConverter.CandidateSize;
					var (x, y) = _pointConverter.GetMousePointInCenter(cell, digit);
					using var brush = new SolidBrush(color);
					g.FillEllipse(
						brush, _pointConverter.GetMousePointRectangle(cell, digit).Zoom(-offset / 3));
				}
			}

			foreach (var (id, region) in _view.RegionOffsets ?? Array.Empty<(int, int)>())
			{
				if (_colorDic.TryGetValue(id, out var color))
				{
					using var brush = new SolidBrush(Color.FromArgb(32, color));
					g.FillRectangle(
						brush, _pointConverter.GetMousePointRectangleForRegion(region).Zoom(-offset / 3));
				}
			}

			if (!(_view.Links is null))
			{
				float width = _pointConverter.CandidateSize.Width;
				using var pen = new Pen(_chainColor, 3)
				{
					CustomEndCap = new AdjustableArrowCap(width / 6, width / 3)
				};
				foreach (var ((startCandidates, startNodeType), (endCandidates, endNodeType)) in _view.Links)
				{
					g.DrawLine(
						pen,
						_pointConverter.GetMouseCenterOfCandidates(startCandidates),
						_pointConverter.GetMouseCenterOfCandidates(endCandidates));
				}
			}

			using var eliminationBrush = new SolidBrush(_eliminationColor);
			using var cannibalBrush = new SolidBrush(_cannibalismColor);
			foreach (var (t, c, d) in _conclusions)
			{
				switch (t)
				{
					//case ConclusionType.Assignment:
					//{
					//	// Every assignment conclusion will be painted
					//	// in its technique information view.
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

			Target = bitmap;
		}
	}
}
