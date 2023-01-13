#if CUSTOMIZED_TITLE_BAR
namespace SudokuStudio.Views.Windows;

partial class MainWindow
{
	/// <summary>
	/// Defines an <see cref="AppWindow"/> instance that is used by interaction with core application behaviors,
	/// such as icon, title bars and so on.
	/// </summary>
	/// <seealso cref="AppWindow"/>
	private AppWindow _appWindow;


	/// <summary>
	/// Initializes for field <see cref="_appWindow"/>.
	/// </summary>
	/// <remarks>
	/// For more information please visit
	/// <see href="https://learn.microsoft.com/en-us/windows/apps/develop/title-bar">this link</see>.
	/// This passage is for full customization of application title bar.
	/// </remarks>
	[MemberNotNull(nameof(_appWindow))]
	private void InitializeAppWindow()
	{
		_appWindow = this.GetAppWindow(out _, out _);
		_appWindow.Changed += onChanged;

		// Check to see if customization is supported. Currently only supported on Windows 11.
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

			AppTitleBar.Loaded += onLoaded;
			AppTitleBar.SizeChanged += onSizeChanged;
		}
		else
		{
			// Title bar customization using these APIs is currently supported only on Windows 11.
			// In other cases, hide the custom title bar element.
			AppTitleBar.Visibility = Visibility.Collapsed;

			// Show alternative UI for any functionality in the title bar, such as search.
		}


		void onLoaded(object sender, RoutedEventArgs e)
		{
			if (AppWindowTitleBar.IsCustomizationSupported())
			{
				SetDragRegionForCustomTitleBar(_appWindow);
			}
		}

		void onSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (AppWindowTitleBar.IsCustomizationSupported() && _appWindow.TitleBar.ExtendsContentIntoTitleBar)
			{
				// Update drag region if the size of the title bar changes.
				SetDragRegionForCustomTitleBar(_appWindow);
			}
		}

		void onChanged(AppWindow sender, AppWindowChangedEventArgs args)
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

	/// <summary>
	/// Sets the dragable region that is used for the whole window.
	/// </summary>
	/// <param name="appWindow">The <see cref="AppWindow"/> instance.</param>
	private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
	{
		if (AppWindowTitleBar.IsCustomizationSupported() && appWindow.TitleBar.ExtendsContentIntoTitleBar
#if DEBUG
			/// This is a bug fix. This bug can be reproduced by Windows Application SDK v1.2.
			/// If you minimize and maximize the window via task bar icon, the expression value <c>appWindow.TitleBar.RightInset</c>
			/// will be -24.
			&& appWindow.TitleBar.RightInset >= 0
#endif
			)
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


	[LibraryImport("Shcore", SetLastError = true)]
	private static partial int GetDpiForMonitor(nint hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);
}
#endif