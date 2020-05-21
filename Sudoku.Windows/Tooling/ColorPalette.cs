using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Xml.Serialization;
using Sudoku.Extensions;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// The color palette.
	/// </summary>
	[Serializable]
	public class ColorPalette
	{
		/// <summary>
		/// The number of first color swatches.
		/// </summary>
		[XmlIgnore]
		protected const int NumColorsFirstSwatch = 39;

		/// <summary>
		/// The number of custom color swatches.
		/// </summary>
		[XmlIgnore]
		protected const int NumColorsCustomSwatch = 44;

		/// <summary>
		/// The number of second color swatches.
		/// </summary>
		[XmlIgnore]
		protected const int NumColorsSecondSwatch = 112;


		/// <include file='../../../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public ColorPalette()
		{
			BuiltInColors = new List<ColorSwatchItem>();
			CustomColors = new List<ColorSwatchItem>();
		}


		/// <summary>
		/// Indicates the built-in colors list.
		/// </summary>
		public ICollection<ColorSwatchItem> BuiltInColors { get; set; }

		/// <summary>
		/// Indicates the custom colors list.
		/// </summary>
		public ICollection<ColorSwatchItem> CustomColors { get; set; }


		/// <summary>
		/// Initialize default values.
		/// </summary>
		public void InitializeDefaults()
		{
			BuiltInColors.Clear();
			BuiltInColors.AddRange(
				GetColorSwatchItems(
					new List<Color>()
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
					}));

			CustomColors.Clear();
			CustomColors.AddRange(
				from x in Enumerable.Repeat(Colors.White, NumColorsCustomSwatch)
				select new ColorSwatchItem() { Color = x, HexString = x.ToHexString() });
		}

		/// <summary>
		/// Get swatch items.
		/// </summary>
		/// <param name="colors">The colors to iterate on.</param>
		/// <returns>The list of colors.</returns>
		protected IEnumerable<ColorSwatchItem> GetColorSwatchItems(IReadOnlyList<Color> colors) =>
			from x in colors select new ColorSwatchItem() { Color = x, HexString = x.ToHexString() };
	}
}
