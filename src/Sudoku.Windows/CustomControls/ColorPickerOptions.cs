namespace Sudoku.Windows.CustomControls;

/// <summary>
/// Indicates the option using in initialization for a <see cref="ColorPickerWindow"/>.
/// </summary>
[Flags, Closed]
public enum ColorPickerOptions
{
	/// <summary>
	/// Indicates the initialization is default.
	/// </summary>
	None = 0,
	
	/// <summary>
	/// Indicates the initialization should show the colors only.
	/// </summary>
	SimpleView = 1,

	/// <summary>
	/// Indicates the initialization will display the custom palette.
	/// </summary>
	LoadCustomPalette = 2
}
