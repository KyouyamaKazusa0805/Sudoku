using System;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Indicates the option using in initialization for a <see cref="ColorPicker"/>.
	/// </summary>
	[Flags]
	/*closed*/
	public enum ColorPickerOptions
	{
		/// <summary>
		/// Indicates the initialization is default.
		/// </summary>
		None,
		
		/// <summary>
		/// Indicates the initialization should show the colors only.
		/// </summary>
		SimpleView,

		/// <summary>
		/// Indicates the initialization will display the custom palette.
		/// </summary>
		LoadCustomPalette
	}
}
