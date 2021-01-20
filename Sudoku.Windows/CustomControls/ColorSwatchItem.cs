using System.Windows.Media;

namespace Sudoku.Windows.CustomControls
{
	/// <summary>
	/// The swatch item.
	/// </summary>
	public sealed class ColorSwatchItem
	{
		/// <summary>
		/// Indicates the color.
		/// </summary>
		public Color Color { get; set; }

		/// <summary>
		/// Indicates the hex string.
		/// </summary>
		public string? HexString { get; set; }
	}
}
