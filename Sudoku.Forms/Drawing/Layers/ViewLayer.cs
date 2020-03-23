using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Sudoku.Data;
using Sudoku.Drawing.Extensions;

namespace Sudoku.Drawing.Layers
{
	/// <summary>
	/// Provides a view layer.
	/// </summary>
	public sealed class ViewLayer : Layer
	{
		/// <summary>
		/// Indicates the color dictionary.
		/// </summary>
		private readonly IDictionary<int, Color> _colorDic;

		/// <summary>
		/// The view.
		/// </summary>
		private readonly View _view;


		/// <summary>
		/// Initializes an instance with the specified view and the color dictionary.
		/// </summary>
		/// <param name="pointConverter">The point converter.</param>
		/// <param name="view">The view.</param>
		/// <param name="colorDic">The color dictionary.</param>
		public ViewLayer(
			PointConverter pointConverter, View view, IDictionary<int, Color> colorDic)
			: base(pointConverter) => (_view, _colorDic) = (view, colorDic);


		/// <inheritdoc/>
		public override int Priority => 4;


		/// <inheritdoc/>
		protected override void Draw()
		{
			var bitmap = new Bitmap((int)Width, (int)Height);
			using var g = Graphics.FromImage(bitmap);
			g.SmoothingMode = SmoothingMode.AntiAlias;

			foreach (var (id, cell) in _view.CellOffsets ?? Array.Empty<(int, int)>())
			{
				if (_colorDic.TryGetValue(id, out var color))
				{
					var (cw, ch) = _pointConverter.CellSize;
					var (x, y) = _pointConverter.GetMousePointInCenter(cell);
					using var brush = new SolidBrush(color);
					g.FillRectangle(
						brush, _pointConverter.GetMousePointRectangle(cell));
				}
			}

			foreach (var (id, candidate) in _view.CandidateOffsets ?? Array.Empty<(int, int)>())
			{
				if (_colorDic.TryGetValue(id, out var color))
				{
					var (cw, ch) = _pointConverter.CandidateSize;
					var (x, y) = _pointConverter.GetMousePointInCenter(candidate / 9, candidate % 9);
					using var brush = new SolidBrush(color);
					g.FillEllipse(
						brush, _pointConverter.GetMousePointRectangle(candidate / 9, candidate % 9));
				}
			}

			foreach (var (id, region) in _view.CandidateOffsets ?? Array.Empty<(int, int)>())
			{
				// TODO: Draw regions.
			}

			foreach (var inference in _view.Links ?? Array.Empty<Inference>())
			{
				// TODO: Draw chains.
			}

			Target = bitmap;
		}
	}
}
