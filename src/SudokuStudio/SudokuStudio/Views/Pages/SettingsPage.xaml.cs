namespace SudokuStudio.Views.Pages;

/// <summary>
/// Defines the settings page.
/// </summary>
public sealed partial class SettingsPage : Page
{
	/// <summary>
	/// The backing field of coordinate displaying modes.
	/// </summary>
	private readonly List<Tuple<CoordinateLabelDisplayMode, string>> _coordinateLabelDisplayModes = new()
	{
		new(CoordinateLabelDisplayMode.None, GetString("SettingsPage_OutsideCoordinateNone")),
		new(CoordinateLabelDisplayMode.UpperAndLeft, GetString("SettingsPage_OutsideCoordinateUpperAndLeft")),
		new(CoordinateLabelDisplayMode.FourDirection, GetString("SettingsPage_OutsideCoordinateFourDirection"))
	};


	/// <summary>
	/// Initializes a <see cref="SettingsPage"/> instance.
	/// </summary>
	public SettingsPage() => InitializeComponent();
}
