// Full customization:
// https://learn.microsoft.com/en-us/windows/apps/develop/title-bar

namespace SudokuStudio.Views.Windows;

partial class MainWindow
{
	private AppWindow _appWindow;


	/// <summary>
	/// Initializes for field <see cref="_appWindow"/>.
	/// </summary>
	[MemberNotNull(nameof(_appWindow))]
	private void InitializeAppWindow()
	{
		_appWindow = GetAppWindowForCurrentWindow();
		_appWindow.Changed += AppWindow_Changed;

		// Check to see if customization is supported.
		// Currently only supported on Windows 11.
		if (AppWindowTitleBar.IsCustomizationSupported())
		{
			var titleBar = _appWindow.TitleBar;
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

			AppTitleBar.Loaded += AppTitleBar_Loaded;
			AppTitleBar.SizeChanged += AppTitleBar_SizeChanged;
		}
		else
		{
			// Title bar customization using these APIs is currently supported only on Windows 11.
			// In other cases, hide the custom title bar element.
			AppTitleBar.Visibility = Visibility.Collapsed;

			// Show alternative UI for any functionality in the title bar, such as search.
		}
	}

	/// <summary>
	/// Sets the dragable region that is used for the whole window.
	/// </summary>
	/// <param name="appWindow">The <see cref="AppWindow"/> instance.</param>
	private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
	{
		if (AppWindowTitleBar.IsCustomizationSupported() && appWindow.TitleBar.ExtendsContentIntoTitleBar)
		{
			var scaleAdjustment = GetScaleAdjustment();

			RightPaddingColumn.Width = new(appWindow.TitleBar.RightInset / scaleAdjustment);
			LeftPaddingColumn.Width = new(appWindow.TitleBar.LeftInset / scaleAdjustment);

			var dragRectsList = new List<RectInt32>();

			RectInt32 dragRectL;
			dragRectL.X = (int)(LeftPaddingColumn.ActualWidth * scaleAdjustment);
			dragRectL.Y = 0;
			dragRectL.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
			dragRectL.Width = (int)((IconColumn.ActualWidth + TitleColumn.ActualWidth + LeftDragColumn.ActualWidth) * scaleAdjustment);
			dragRectsList.Add(dragRectL);

			RectInt32 dragRectR;
			dragRectR.X = (int)((LeftPaddingColumn.ActualWidth + IconColumn.ActualWidth + TitleTextBlock.ActualWidth + LeftDragColumn.ActualWidth + SearchColumn.ActualWidth) * scaleAdjustment);
			dragRectR.Y = 0;
			dragRectR.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
			dragRectR.Width = (int)(RightDragColumn.ActualWidth * scaleAdjustment);
			dragRectsList.Add(dragRectR);

			var dragRects = dragRectsList.ToArray();

			appWindow.TitleBar.SetDragRectangles(dragRects);
		}
	}

	/// <summary>
	/// Try to adjust the scaling.
	/// </summary>
	/// <returns>The scaling result value.</returns>
	/// <exception cref="InvalidOperationException">Throws when the computer cannot handle scaling correctly.</exception>
	private double GetScaleAdjustment()
	{
		var hWnd = WindowNative.GetWindowHandle(this);
		var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
		var displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
		var hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

		// Get DPI.
		var result = GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out var dpiX, out _);
		if (result != 0)
		{
			throw new InvalidOperationException("Could not get DPI for monitor.");
		}

		var scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
		return scaleFactorPercent / 100.0;
	}

	/// <summary>
	/// Gets <see cref="AppWindow"/> instance for the current window.
	/// </summary>
	/// <returns>A valid <see cref="AppWindow"/> instance.</returns>
	private AppWindow GetAppWindowForCurrentWindow()
	{
		var hWnd = WindowNative.GetWindowHandle(this);
		var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
		return AppWindow.GetFromWindowId(wndId);
	}


	[LibraryImport("Shcore", SetLastError = true)]
	private static partial int GetDpiForMonitor(IntPtr hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);


	private void AppTitleBar_Loaded(object sender, RoutedEventArgs e)
	{
		if (AppWindowTitleBar.IsCustomizationSupported())
		{
			SetDragRegionForCustomTitleBar(_appWindow);
		}
	}

	private void AppTitleBar_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (AppWindowTitleBar.IsCustomizationSupported() && _appWindow.TitleBar.ExtendsContentIntoTitleBar)
		{
			// Update drag region if the size of the title bar changes.
			SetDragRegionForCustomTitleBar(_appWindow);
		}
	}

	private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
	{
		if (args.DidPresenterChange && AppWindowTitleBar.IsCustomizationSupported())
		{
			switch (sender.Presenter.Kind)
			{
				case AppWindowPresenterKind.CompactOverlay:
				{
					// Compact overlay - hide custom title bar
					// and use the default system title bar instead.
					AppTitleBar.Visibility = Visibility.Collapsed;
					sender.TitleBar.ResetToDefault();
					break;
				}
				case AppWindowPresenterKind.FullScreen:
				{
					// Full screen - hide the custom title bar
					// and the default system title bar.
					AppTitleBar.Visibility = Visibility.Collapsed;
					sender.TitleBar.ExtendsContentIntoTitleBar = true;
					break;
				}
				case AppWindowPresenterKind.Overlapped:
				{
					// Normal - hide the system title bar
					// and use the custom title bar instead.
					AppTitleBar.Visibility = Visibility.Visible;
					sender.TitleBar.ExtendsContentIntoTitleBar = true;
					SetDragRegionForCustomTitleBar(sender);
					break;
				}
				default:
				{
					// Use the default system title bar.
					sender.TitleBar.ResetToDefault();
					break;
				}
			}
		}
	}
}
