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
	/// Indicates the base window.
	/// </summary>
	private MainWindow _baseWindow = null!;


	/// <summary>
	/// Initializes a <see cref="TitleBar"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TitleBar() => InitializeComponent();


	/// <summary>
	/// Set drag region for the custom title bar.
	/// </summary>
	/// <param name="appWindow">The app window.</param>
	private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
	{
		if (appWindow.TitleBar.ExtendsContentIntoTitleBar)
		{
			double scaleAdjustment = GetScaleAdjustment();
			int width = (int)(ActualWidth * scaleAdjustment);
			int height = (int)(ActualHeight * scaleAdjustment);
			var rect = new RectInt32(0, 0, width, height);
			appWindow.TitleBar.SetDragRectangles(new[] { rect });
		}
	}

	/// <summary>
	/// Gets the scale adjustment.
	/// </summary>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the external API cannot fetch the valid result.
	/// </exception>
	private double GetScaleAdjustment()
	{
		nint hWnd = WindowNative.GetWindowHandle(_baseWindow);
		var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
		var displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
		nint hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

		// Get DPI.
		int result = getDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out _);
		if (result != 0)
		{
			throw new InvalidOperationException("Could not get DPI for monitor.");
		}

		uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
		return scaleFactorPercent / 100.0;


		[DllImport("Shcore", EntryPoint = "GetDpiForMonitor", SetLastError = true)]
		static extern int getDpiForMonitor(nint hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);
	}


	/// <summary>
	/// Triggers when the control is loaded.
	/// </summary>
	/// <param name="sender">The instance which triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void UserControl_Loaded(object sender, RoutedEventArgs e)
	{
		_baseWindow = ((App)Application.Current).RuntimeInfo.MainWindow;
		_appWindow = _baseWindow.GetAppWindow();

		// Check to see if customization is supported.
		// Currently only supported on Windows 11.
		if (Supportability.TitleBar && _appWindow is { TitleBar: var titleBar })
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

			SetDragRegionForCustomTitleBar(_appWindow);

			Loaded += TitleBar_Loaded;
			SizeChanged += TitleBar_SizeChanged;
		}
		else
		{
			// Title bar customization using these APIs is currently
			// supported only on Windows 11. In other cases, hide
			// the custom title bar element.
			_cAppTitleBar.Visibility = Visibility.Collapsed;
		}
	}

	/// <summary>
	/// Triggers when the control is unloaded.
	/// </summary>
	/// <param name="sender">The instance which triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void UserControl_Unloaded(object sender, RoutedEventArgs e)
	{
		if (Supportability.TitleBar)
		{
			Loaded -= TitleBar_Loaded;
			SizeChanged -= TitleBar_SizeChanged;
		}
	}

	/// <summary>
	/// Triggers when the title bar's size is changed.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void TitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (_appWindow.TitleBar.ExtendsContentIntoTitleBar)
		{
			// Update drag region if the size of the title bar changes.
			SetDragRegionForCustomTitleBar(_appWindow);
		}
	}

	/// <summary>
	/// Triggers when the title bar is loaded.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void TitleBar_Loaded(object sender, RoutedEventArgs e) => SetDragRegionForCustomTitleBar(_appWindow);
}
