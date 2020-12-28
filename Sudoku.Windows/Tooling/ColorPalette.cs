using System;
using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Xml.Serialization;
using Sudoku.DocComments;
using Sudoku.Windows.Extensions;
using DColor = System.Drawing.Color;
using WColor = System.Windows.Media.Color;

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


		/// <inheritdoc cref="DefaultConstructor"/>
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
					new List<WColor>()
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

						WColor.FromArgb(255, 5, 5, 5),
						WColor.FromArgb(255, 15, 15, 15),
						WColor.FromArgb(255, 35, 35, 35),
						WColor.FromArgb(255, 55, 55, 55),
						WColor.FromArgb(255, 75, 75, 75),
						WColor.FromArgb(255, 95, 95, 95),
						WColor.FromArgb(255, 115, 115, 115),
						WColor.FromArgb(255, 135, 135, 135),
						WColor.FromArgb(255, 155, 155, 155),
						WColor.FromArgb(255, 175, 175, 175),
						WColor.FromArgb(255, 195, 195, 195),
						WColor.FromArgb(255, 215, 215, 215),
						WColor.FromArgb(255, 235, 235, 235),
					}));

			CustomColors.Clear();
			foreach (var color in Enumerable.Repeat(Colors.White, NumColorsCustomSwatch))
			{
				CustomColors.Add(new() { Color = color, HexString = color.ToHexString() });
			}
		}

		/// <summary>
		/// Get swatch items.
		/// </summary>
		/// <param name="colors">The colors to iterate on.</param>
		/// <returns>The list of colors.</returns>
		protected ColorSwatchItem[] GetColorSwatchItems(IReadOnlyList<WColor> colors)
		{
			var result = new ColorSwatchItem[colors.Count];
			int i = 0;
			foreach (var color in colors)
			{
				result[i++] = new() { Color = color, HexString = color.ToHexString() };
			}

			return result;
		}

#pragma warning disable IDE0051
		/// <summary>
		/// Get all colors especially used for HTML.
		/// </summary>
		/// <returns>All colors.</returns>
		private static IEnumerable<WColor> GetWebColors() =>
			from @property in typeof(DColor).GetProperties(BindingFlags.Public | BindingFlags.Static)
			where @property.PropertyType == typeof(DColor)
			select DColor.FromName(@property.Name).ToWColor();
#pragma warning restore IDE0051
	}
}
