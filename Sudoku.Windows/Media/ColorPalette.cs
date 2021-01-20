using System.Collections.Generic;
using System.Windows.Media;

namespace Sudoku.Windows.Media
{
	/// <summary>
	/// Extracts the fixed colors.
	/// </summary>
	public static class ColorPalette
	{
		/// <summary>
		/// Indicates the color palette that is shown on the control.
		/// </summary>
		public static readonly IReadOnlyCollection<Color> PaletteColors = new[]
		{
			Colors.Black,
			Colors.Red,
			Colors.DarkOrange,
			Colors.Yellow,
			Colors.LawnGreen,
			Colors.Blue,
			Colors.Purple,
			Colors.DeepPink,
			Colors.Aqua,
			Colors.SaddleBrown,
			Colors.Wheat,
			Colors.BurlyWood,
			Colors.Teal,

			Colors.White,
			Colors.OrangeRed,
			Colors.Orange,
			Colors.Gold,
			Colors.LimeGreen,
			Colors.DodgerBlue,
			Colors.Orchid,
			Colors.HotPink,
			Colors.Turquoise,
			Colors.SandyBrown,
			Colors.SeaGreen,
			Colors.SlateBlue,
			Colors.RoyalBlue,

			Colors.Tan,
			Colors.Peru,
			Colors.DarkBlue,
			Colors.DarkGreen,
			Colors.DarkSlateBlue,
			Colors.Navy,
			Colors.MistyRose,
			Colors.LemonChiffon,
			Colors.ForestGreen,
			Colors.Firebrick,
			Colors.DarkViolet,
			Colors.Aquamarine,
			Colors.CornflowerBlue,
			Colors.Bisque,
			Colors.WhiteSmoke,
			Colors.AliceBlue,

			Color.FromArgb(255, 5, 5, 5),
			Color.FromArgb(255, 15, 15, 15),
			Color.FromArgb(255, 35, 35, 35),
			Color.FromArgb(255, 55, 55, 55),
			Color.FromArgb(255, 75, 75, 75),
			Color.FromArgb(255, 95, 95, 95),
			Color.FromArgb(255, 115, 115, 115),
			Color.FromArgb(255, 135, 135, 135),
			Color.FromArgb(255, 155, 155, 155),
			Color.FromArgb(255, 175, 175, 175),
			Color.FromArgb(255, 195, 195, 195),
			Color.FromArgb(255, 215, 215, 215),
			Color.FromArgb(255, 235, 235, 235),
		};
	}
}
