namespace SudokuStudio.Views.Windows;

/// <summary>
/// Provides with a <see cref="Window"/> instance that is running as main instance of the program.
/// </summary>
/// <seealso cref="Window"/>
public sealed partial class MainWindow : Window
{
	/// <summary>
	/// Initializes a <see cref="MainWindow"/> instance.
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();
		InitializeField();

#if UI_FEATURE_CUSTOMIZED_TITLE_BAR
		InitializeAppWindow();
		SetAppTitleBarState();
		SetAppIcon();
#endif
	}

	/// <summary>
	/// Try to navigate to the target page.
	/// </summary>
	/// <param name="pageType">The target page type.</param>
	internal void NavigateToPage(Type pageType)
	{
		if (NavigationPage.NavigationViewFrame.SourcePageType != pageType)
		{
			NavigationPage.NavigationViewFrame.Navigate(pageType, null, DefaultNavigationTransitionInfo);
			NavigationPage.SetFrameDisplayTitle(pageType);
		}
	}

	/// <summary>
	/// Try to navigate to the target page.
	/// </summary>
	/// <typeparam name="T">The type of the data.</typeparam>
	/// <param name="pageType">The target page type.</param>
	/// <param name="data">The data.</param>
	internal void NavigateToPage<T>(Type pageType, T? data) where T : notnull
	{
		if (NavigationPage.NavigationViewFrame.SourcePageType != pageType)
		{
			NavigationPage.NavigationViewFrame.Navigate(pageType, data, DefaultNavigationTransitionInfo);
			NavigationPage.SetFrameDisplayTitle(pageType);
		}
	}

	/// <summary>
	/// Try to navigate to the target page via its type specified as type argument.
	/// </summary>
	/// <typeparam name="TPage">The type of the page.</typeparam>
	internal void NavigateToPage<TPage>() where TPage : Page => NavigateToPage(typeof(TPage));

	/// <summary>
	/// Try to navigate to the target page via its type specified as type argument, and routing data specified as argument.
	/// </summary>
	/// <typeparam name="TPage">The type of the page.</typeparam>
	/// <typeparam name="TData">The type of the data.</typeparam>
	/// <param name="data">The data.</param>
	internal void NavigateToPage<TPage, TData>(TData? data) where TPage : Page where TData : notnull => NavigateToPage(typeof(TPage), data);

	/// <summary>
	/// Initializes for fields.
	/// </summary>
	private void InitializeField() => NavigationPage.ParentWindow = this;

	/// <summary>
	/// Saves for preferences.
	/// </summary>
	private void SavePreference() => ProgramPreferenceFileHandler.Write(CommonPaths.UserPreference, ((App)Application.Current).Preference);

	/// <summary>
	/// Saves for puzzle generating history.
	/// </summary>
	private void SavePuzzleGeneratingHistory()
	{
		if (Application.Current is not App
			{
				Preference.UIPreferences.SavePuzzleGeneratingHistory: true,
				PuzzleGeneratingHistory: { Puzzles: { Count: not 0 } puzzles } history
			})
		{
			return;
		}

		if (File.Exists(CommonPaths.GeneratingHistory) && PuzzleGeneratingHistoryFileHandler.Read(CommonPaths.GeneratingHistory) is { } @base)
		{
			@base.Puzzles.AddRange(puzzles);

			PuzzleGeneratingHistoryFileHandler.Write(CommonPaths.GeneratingHistory, @base);
		}
		else
		{
			PuzzleGeneratingHistoryFileHandler.Write(CommonPaths.GeneratingHistory, history);
		}
	}

	/// <summary>
	/// Sets the state of app title bars conditionally.
	/// </summary>
	private void SetAppTitleBarState()
	{
#if SEARCH_AUTO_SUGGESTION_BOX
		AppTitleBar.Visibility = Visibility.Visible;
		AppTitleBarWithoutAutoSuggestBox.Visibility = Visibility.Collapsed;
#else
		AppTitleBar.Visibility = Visibility.Collapsed;
		AppTitleBarWithoutAutoSuggestBox.Visibility = Visibility.Visible;
#endif
	}

	/// <summary>
	/// Update value for open-pane length into preference.
	/// </summary>
	private void UpdateOpenPaneLengthToPreference()
		=> ((App)Application.Current).Preference.UIPreferences.MainNavigationPageOpenPaneLength = (decimal)Round(NavigationPage.MainNavigationView.OpenPaneLength, 1);

#if UI_FEATURE_CUSTOMIZED_TITLE_BAR
	/// <summary>
	/// Initializes for property <see cref="Window.AppWindow"/>.
	/// </summary>
	/// <remarks>
	/// For more information please visit <see href="https://learn.microsoft.com/en-us/windows/apps/develop/title-bar">this link</see>.
	/// This passage is for full customization of application title bar.
	/// </remarks>
	private void InitializeAppWindow()
	{
		AppWindow.Changed += appWindowChangedHandler;

		// Check to see if customization is supported. Currently only supported on Windows 11.
		if (AppWindowTitleBar.IsCustomizationSupported())
		{
			var titleBar = AppWindow.TitleBar;
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

#if SEARCH_AUTO_SUGGESTION_BOX
			AppTitleBar.Loaded += (_, _) => SetDragRegionForCustomTitleBar(AppWindow);
			AppTitleBar.SizeChanged += (_, _) => SetDragRegionForCustomTitleBar(AppWindow);
#else
			AppTitleBarWithoutAutoSuggestBox.Loaded += (_, _) => SetDragRegionForCustomTitleBar(AppWindow);
			AppTitleBarWithoutAutoSuggestBox.SizeChanged += (_, _) => SetDragRegionForCustomTitleBar(AppWindow);
#endif
		}
		else
		{
			// Title bar customization using these APIs is currently supported only on Windows 11.
			// In other cases, hide the custom title bar element.
#if SEARCH_AUTO_SUGGESTION_BOX
			AppTitleBar.Visibility = Visibility.Collapsed;
#else
			AppTitleBarWithoutAutoSuggestBox.Visibility = Visibility.Collapsed;
#endif
		}


		void appWindowChangedHandler(AppWindow sender, AppWindowChangedEventArgs args)
		{
			if ((sender, args) is not ({ Presenter.Kind: var kind, TitleBar: var titleBar }, { DidPresenterChange: var didPresenterChange }))
			{
				return;
			}

			if (didPresenterChange && AppWindowTitleBar.IsCustomizationSupported())
			{
				switch (kind)
				{
					case AppWindowPresenterKind.CompactOverlay:
					{
						// Compact overlay - hide custom title bar and use the default system title bar instead.
#if SEARCH_AUTO_SUGGESTION_BOX
						AppTitleBar.Visibility = Visibility.Collapsed;
#else
						AppTitleBarWithoutAutoSuggestBox.Visibility = Visibility.Collapsed;
#endif
						titleBar.ResetToDefault();
						break;
					}
					case AppWindowPresenterKind.FullScreen:
					{
						// Full screen - hide the custom title bar and the default system title bar.
#if SEARCH_AUTO_SUGGESTION_BOX
						AppTitleBar.Visibility = Visibility.Collapsed;
#else
						AppTitleBarWithoutAutoSuggestBox.Visibility = Visibility.Collapsed;
#endif
						titleBar.ExtendsContentIntoTitleBar = true;
						break;
					}
					case AppWindowPresenterKind.Overlapped:
					{
						// Normal - hide the system title bar and use the custom title bar instead.
#if SEARCH_AUTO_SUGGESTION_BOX
						AppTitleBar.Visibility = Visibility.Visible;
#else
						AppTitleBarWithoutAutoSuggestBox.Visibility = Visibility.Visible;
#endif
						titleBar.ExtendsContentIntoTitleBar = true;
						SetDragRegionForCustomTitleBar(sender);
						break;
					}
					default:
					{
						// Use the default system title bar.
						titleBar.ResetToDefault();
						break;
					}
				}
			}
		}
	}

	/// <summary>
	/// Sets the draggable region that is used for the whole window.
	/// </summary>
	/// <param name="appWindow">The <see cref="AppWindow"/> instance.</param>
	private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
	{
		if (AppWindowTitleBar.IsCustomizationSupported() && appWindow.TitleBar.ExtendsContentIntoTitleBar)
		{
#if SEARCH_AUTO_SUGGESTION_BOX
			var scaleAdjustment = GetScaleAdjustment();

			RightPaddingColumn.Width = new(appWindow.TitleBar.RightInset / scaleAdjustment);
			LeftPaddingColumn.Width = new(appWindow.TitleBar.LeftInset / scaleAdjustment);

			RectInt32 dragRectL;
			dragRectL.X = (int)(LeftPaddingColumn.ActualWidth * scaleAdjustment);
			dragRectL.Y = 0;
			dragRectL.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
			dragRectL.Width = (int)((IconColumn.ActualWidth + TitleColumn.ActualWidth + LeftDragColumn.ActualWidth) * scaleAdjustment);

			RectInt32 dragRectR;
			dragRectR.X = (int)((LeftPaddingColumn.ActualWidth + IconColumn.ActualWidth + TitleTextBlock.ActualWidth + LeftDragColumn.ActualWidth + SearchColumn.ActualWidth) * scaleAdjustment);
			dragRectR.Y = 0;
			dragRectR.Height = (int)(AppTitleBar.ActualHeight * scaleAdjustment);
			dragRectR.Width = (int)(RightDragColumn.ActualWidth * scaleAdjustment);

			appWindow.TitleBar.SetDragRectangles([dragRectL, dragRectR]);
#else
			var scaleAdjustment = GetScaleAdjustment();

			RectInt32 rect;
			rect.X = 0;
			rect.Y = 0;
			rect.Width = (int)(AppTitleBarWithoutAutoSuggestBox.ActualWidth * scaleAdjustment);
			rect.Height = (int)(AppTitleBarWithoutAutoSuggestBox.ActualHeight * scaleAdjustment);
			appWindow.TitleBar.SetDragRectangles([rect]);
#endif
		}
	}
#endif

#if UI_FEATURE_CUSTOMIZED_TITLE_BAR
	/// <summary>
	/// Try to set icon of the program.
	/// </summary>
	private void SetAppIcon() => AppWindow.SetIcon(@"Assets\images\Logo.ico");

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
		var result = Interoperability.GetDpiForMonitor(hMonitor, MonitorDpiType.MDT_Default, out var dpiX, out _);
		if (result != 0)
		{
			throw new InvalidOperationException("Could not get DPI for monitor.");
		}

		var scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
		return scaleFactorPercent / 100.0;
	}
#endif


	private void Window_Closed(object sender, WindowEventArgs args)
	{
		UpdateOpenPaneLengthToPreference();
		SavePreference();
		SavePuzzleGeneratingHistory();
	}
}
