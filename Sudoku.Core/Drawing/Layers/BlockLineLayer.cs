using System.Drawing;

namespace Sudoku.Drawing.Layers
{
	/// <summary>
	/// Provides a block line layer.
	/// </summary>
	public sealed class BlockLineLayer : Layer
	{
		/// <summary>
		/// Indicates the width of the block line.
		/// </summary>
		private readonly float _width;

		/// <summary>
		/// Indicates the color of the line.
		/// </summary>
		private readonly Color _color;


		/// <summary>
		/// Initializes an instance with the specified point converter, width and the color.
		/// </summary>
		/// <param name="pointConverter">The point converter.</param>
		/// <param name="width">The width of lines.</param>
		/// <param name="color">The color of lines.</param>
		public BlockLineLayer(PointConverter pointConverter, float width, Color color)
			: base(pointConverter) => (_width, _color) = (width, color);


		/// <inheritdoc/>
		public override int Priority => 3;


		/// <inheritdoc/>
		protected override void Draw()
		{
			var bitmap = new Bitmap(Width, Height);
			using var g = Graphics.FromImage(bitmap);
			using var pen = new Pen(_color, _width);
			var gridPoints = _pointConverter.GridPoints;
			for (int i = 0; i < 28; i += 9)
			{
				g.DrawLine(pen, gridPoints[i, 0], gridPoints[i, 27]);
				g.DrawLine(pen, gridPoints[0, i], gridPoints[27, i]);
			}

			Target = bitmap;
		}
	}
}
