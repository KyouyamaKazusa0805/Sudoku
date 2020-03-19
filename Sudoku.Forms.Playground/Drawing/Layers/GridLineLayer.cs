using System.Drawing;

namespace Sudoku.Drawing.Layers
{
	/// <summary>
	/// Provides a grid line layer.
	/// </summary>
	public sealed class GridLineLayer : Layer
	{
		/// <summary>
		/// Indicates the width of each line.
		/// </summary>
		private readonly float _width;

		/// <summary>
		/// Indicates the color of each line.
		/// </summary>
		private readonly Color _color;


		/// <summary>
		/// Initializes an instance with the specified point converter, width and the color.
		/// </summary>
		/// <param name="pointConverter">The point converter.</param>
		/// <param name="width">The width of lines.</param>
		/// <param name="color">The color of lines.</param>
		public GridLineLayer(PointConverter pointConverter, float width, Color color)
			: base(pointConverter) => (_width, _color) = (width, color);


		/// <inheritdoc/>
		public override int Priority => 2;


		/// <inheritdoc/>
		protected override void Draw()
		{
			var bitmap = new Bitmap(Width, Height);
			using (var g = Graphics.FromImage(bitmap))
			using (var pen = new Pen(_color, _width))
			{
				var gridPoints = _pointConverter.GridPoints;
				for (int i = 0; i < 28; i += 3)
				{
					g.DrawLine(pen, gridPoints[i, 0], gridPoints[i, 27]);
					g.DrawLine(pen, gridPoints[0, i], gridPoints[27, i]);
				}

				Target = bitmap;
			}
		}
	}
}
