#pragma warning disable IDE0051

using System;
using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Xml.Serialization;
using Sudoku.DocComments;
using Sudoku.Windows.Extensions;
using Sudoku.Windows.Media;
using DColor = System.Drawing.Color;
using WColor = System.Windows.Media.Color;

namespace Sudoku.Windows.CustomControls
{
	/// <summary>
	/// The color palette.
	/// </summary>
	[Serializable]
	public class ColorPickerPalette
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
		public ColorPickerPalette()
		{
			BuiltInColors = new List<ColorSampleItem>();
			CustomColors = new List<ColorSampleItem>();
		}


		/// <summary>
		/// Indicates the built-in colors list.
		/// </summary>
		public ICollection<ColorSampleItem> BuiltInColors { get; set; }

		/// <summary>
		/// Indicates the custom colors list.
		/// </summary>
		public ICollection<ColorSampleItem> CustomColors { get; set; }


		/// <summary>
		/// Initialize default values.
		/// </summary>
		public void InitializeDefaults()
		{
			BuiltInColors.Clear();
			BuiltInColors.AddRange(
				from color in ColorPalette.PaletteColors
				select new ColorSampleItem() { Color = color, HexString = color.ToHexString() });

			CustomColors.Clear();
			CustomColors.AddRange(
				from color in Enumerable.Repeat(Colors.White, NumColorsCustomSwatch)
				select new ColorSampleItem() { Color = color, HexString = color.ToHexString() });
		}

		/// <summary>
		/// Get all colors especially used for HTML.
		/// </summary>
		/// <returns>All colors.</returns>
		private static IEnumerable<WColor> GetWebColors() =>
			from @property in typeof(DColor).GetProperties(BindingFlags.Public | BindingFlags.Static)
			where @property.PropertyType == typeof(DColor)
			select DColor.FromName(@property.Name).ToWColor();
	}
}
