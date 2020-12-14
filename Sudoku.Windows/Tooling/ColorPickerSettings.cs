using System;
using System.IO;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Indicates the color picker settings.
	/// </summary>
	public static class ColorPickerSettings
	{
		/// <summary>
		/// Indicates whether the current environment can use custom palette.
		/// </summary>
		internal static bool UsingCustomPalette;

		/// <summary>
		/// Indicates the XML file name.
		/// </summary>
		public static readonly string CustomColorsFilename = "CustomColorPalette.xml";

		/// <summary>
		/// Indicates the directory that custom colors saved.
		/// </summary>
		public static readonly string CustomColorsDirectory = Environment.CurrentDirectory;


		/// <summary>
		/// Indicates the custom palette file name.
		/// </summary>
		public static string CustomPaletteFilename => Path.Combine(CustomColorsDirectory, CustomColorsFilename);
	}
}
