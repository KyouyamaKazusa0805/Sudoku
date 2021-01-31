using Microsoft.UI;
using Windows.UI;

namespace Sudoku.Painting
{
	partial class PreferencesBase
	{
		/// <summary>
		/// Indicates the grid line width. The default value is 1.5.
		/// </summary>
		public double GridLineWidth { get; set; } = 1.5;

		/// <summary>
		/// Indicates the block line width. The default value is 3.
		/// </summary>
		public double BlockLineWidth { get; set; } = 3;

		/// <summary>
		/// Indicates the background color of the grid. The default value is <see cref="Colors.White"/>.
		/// </summary>
		public Color BackgroundColor { get; set; } = Colors.White;

		/// <summary>
		/// Indicates the grid line color of the grid. The default value is <see cref="Colors.Gray"/>.
		/// </summary>
		public Color GridLineColor { get; set; } = Colors.Gray;

		/// <summary>
		/// Indicates the block line color of the grid. The default value is <see cref="Colors.Gray"/>.
		/// </summary>
		public Color BlockLineColor { get; set; } = Colors.Gray;
	}
}
