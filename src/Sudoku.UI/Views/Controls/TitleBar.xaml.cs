namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a customized title bar.
/// </summary>
public sealed partial class TitleBar : UserControl
{
	/// <summary>
	/// Indicates the application window.
	/// </summary>
	private AppWindow _appWindow = null!;


	/// <summary>
	/// Initializes a <see cref="TitleBar"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TitleBar() => InitializeComponent();


	/// <summary>
	/// Triggers when the control is loaded.
	/// </summary>
	/// <param name="sender">The instance which triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void UserControl_Loaded(object sender, RoutedEventArgs e)
	{
		_appWindow = ((App)Application.Current).InitialPageInfo.MainWindow.GetAppWindow();

		// Check to see if customization is supported.
		// Currently only supported on Windows 11.
		if (AppWindowTitleBar.IsCustomizationSupported() && _appWindow is { TitleBar: var titleBar })
		{
			// Hide default title bar.
			titleBar.ExtendsContentIntoTitleBar = true;

			// Sets the background color on "those" three buttons to transparent.
			titleBar.ButtonBackgroundColor = Colors.Transparent;
			titleBar.ButtonForegroundColor = Colors.Black;
			titleBar.ButtonHoverBackgroundColor = Colors.LightGray;
			titleBar.ButtonHoverForegroundColor = Colors.Black;
			titleBar.ButtonPressedBackgroundColor = Colors.DimGray;
			titleBar.ButtonPressedForegroundColor = Colors.Black;
			titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
			titleBar.ButtonInactiveForegroundColor = Colors.Gray;
		}
		else
		{
			// Title bar customization using these APIs is currently
			// supported only on Windows 11. In other cases, hide
			// the custom title bar element.
			_cAppTitleBar.Visibility = Visibility.Collapsed;
		}
	}
}
