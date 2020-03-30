using System.Diagnostics;
using System.Drawing;
using Sudoku.Data;
using Sudoku.Drawing.Layers;
using PointConverter = Sudoku.Drawing.PointConverter;

namespace Sudoku.Forms.Drawing.Layers
{
	/// <summary>
	/// Provides a focus layer.
	/// </summary>
	[DebuggerStepThrough]
	public sealed class FocusLayer : Layer
	{
		/// <summary>
		/// The color.
		/// </summary>
		private readonly Color _color;

		/// <summary>
		/// The internal list of focused cells.
		/// </summary>
		private readonly GridMap _cells;


		/// <summary>
		/// Initializes an instance with the point converter and focused cells.
		/// </summary>
		/// <param name="pointConverter">The point converter.</param>
		/// <param name="cells">The cells.</param>
		/// <param name="color">The colors.</param>
		public FocusLayer(
			PointConverter pointConverter, GridMap cells, Color color)
			: base(pointConverter) => (_color, _cells) = (color, cells);


		/// <inheritdoc/>
		public override int Priority => 4;


		/// <inheritdoc/>
		protected override void Draw()
		{
			var bitmap = new Bitmap((int)Width, (int)Height);
			using var g = Graphics.FromImage(bitmap);
			using var brush = new SolidBrush(_color);
			foreach (int cell in _cells.Offsets)
			{
				g.FillRectangle(brush, _pointConverter.GetMousePointRectangle(cell));
			}

			Target = bitmap;
		}
	}
}
