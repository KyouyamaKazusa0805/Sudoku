using System.Drawing;

namespace Sudoku.Drawing.Layers
{
	/// <summary>
	/// Provides a background layer.
	/// </summary>
	public sealed class BackLayer : Layer
	{
		/// <summary>
		/// Indicates the background color.
		/// </summary>
		private readonly Color _color;


		/// <summary>
		/// Initializes an instance with the specified point converter and the color.
		/// </summary>
		/// <param name="pointConverter">The point converter.</param>
		/// <param name="color">The color.</param>
		public BackLayer(PointConverter pointConverter, Color color)
			: base(pointConverter) => _color = color;


		/// <inheritdoc/>
		public override int Priority => 1;


		/// <inheritdoc/>
		protected override void Draw()
		{
			var result = new Bitmap((int)Width, (int)Height);
			using var g = Graphics.FromImage(result);
			g.Clear(_color);
			Target = result;
		}
	}
}
