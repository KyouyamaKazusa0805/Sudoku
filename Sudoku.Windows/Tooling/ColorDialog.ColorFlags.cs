using System;

namespace Sudoku.Windows.Tooling
{
	public partial class ColorDialog
	{
		/// <summary>
		/// The color flags.
		/// </summary>
		[Flags]
		private enum ColorFlags : uint
		{
			AnyColor = 0x100,
			FullOpen = 2,
			PreventFullOpen = 4,
			RgbInit = 1,
			SolidColor = 0x80
		}
	}
}
