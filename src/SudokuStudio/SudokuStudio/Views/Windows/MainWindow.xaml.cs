namespace SudokuStudio.Views.Windows;

/// <summary>
/// Provides with a <see cref="Window"/> instance that is running as main instance of the program.
/// </summary>
/// <seealso cref="Window"/>
public sealed partial class MainWindow : Window
{
#if UI_FEATURE_CUSTOMIZED_TITLE_BAR
	/// <summary>
	/// Defines an <see cref="AppWindow"/> instance that is used by interaction with core application behaviors,
	/// such as icon, title bars and so on.
	/// </summary>
	/// <seealso cref="AppWindow"/>
	private AppWindow _appWindow;
#endif

	/// <summary>
	/// The navigating data. This dictionary stores the routing data that can be used and controlled
	/// by control <see cref="NavigationViewFrame"/>.
	/// </summary>
	/// <seealso cref="NavigationViewFrame"/>
	private Dictionary<Predicate<NavigationViewItemBase>, Type> _navigatingData;

#if MICA_BACKDROP || ACRYLIC_BACKDROP
#if MICA_BACKDROP
	/// <summary>
	/// Indicates the Mica controller instance. This instance is used as core implementation of Mica material of applications.
	/// </summary>
	private MicaController? _micaController;
#elif ACRYLIC_BACKDROP
	/// <summary>
	/// Indicates the acrylic controller instance. This instance is used as core implementation of Mica material of applications.
	/// </summary>
	private DesktopAcrylicController? _acrylicController;
#endif

	/// <summary>
	/// Indicates the material configuration instance. This field controls displaying with a customized material such as Mica and acrylic.
	/// </summary>
	private SystemBackdropConfiguration? _configurationSource;

	/// <summary>
	/// Defines a <see cref="WinSysDispatcherQueueHelper"/> instance used for interaction with <see cref="WinSysDispatcherQueue"/>.
	/// </summary>
	/// <seealso cref="WinSysDispatcherQueueHelper"/>
	/// <seealso cref="WinSysDispatcherQueue"/>
	private WinSysDispatcherQueueHelper? _wsdqHelper;
#endif


	/// <summary>
	/// Initializes a <see cref="MainWindow"/> instance.
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();
		InitializeField();

#if MICA_BACKDROP && ACRYLIC_BACKDROP
#warning You should not set both 'MICA_BACKDROP' and 'ACRYLIC_BACKDROP'; acrylic material will be ignored.
#elif MICA_BACKDROP
		TrySetMicaBackdrop();
#elif ACRYLIC_BACKDROP
#line 1 "Backdrop Configuration"
#warning Acrylic backdrop is not fully supported in the program. Some UI features may not be configured so UI may not represent a good look.
#line default
		TrySetAcrylicBackdrop();
#endif

#if UI_FEATURE_CUSTOMIZED_TITLE_BAR
		InitializeAppWindow();
		SetAppTitleBarStatus();
		SetAppIcon();
		SetAppTitle();
#endif
	}


	/// <summary>
	/// Try to navigate to the target page.
	/// </summary>
	/// <param name="pageType">The target page type.</param>
	internal void NavigateToPage(Type pageType)
	{
		if (NavigationViewFrame.SourcePageType != pageType)
		{
			NavigationViewFrame.Navigate(pageType, null, App.DefaultNavigationTransitionInfo);
			SetFrameDisplayTitle(pageType);
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
		if (NavigationViewFrame.SourcePageType != pageType)
		{
			NavigationViewFrame.Navigate(pageType, data, App.DefaultNavigationTransitionInfo);
			SetFrameDisplayTitle(pageType);
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
	/// Initializes fields.
	/// </summary>
	[MemberNotNull(nameof(_navigatingData))]
	private void InitializeField()
		=> _navigatingData = new()
		{
			{ container => container == AnalyzePageItem, typeof(AnalyzePage) },
			{ container => container == AboutPageItem, typeof(AboutPage) },
			{ container => container == SingleCountingPageItem, typeof(SingleCountingPracticingPage) },
			{ container => container == TechniqueGalleryPageItem, typeof(TechniqueGalleryPage) },
			{ container => container == DrawingPageItem, typeof(DrawingPage) }
		};

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
	/// Sets the status of app title bars conditionally.
	/// </summary>
	private void SetAppTitleBarStatus()
	{
#if SEARCH_AUTO_SUGGESTION_BOX
		AppTitleBar.Visibility = Visibility.Visible;
		AppTitleBarWithoutAutoSuggestBox.Visibility = Visibility.Collapsed;
#else
		AppTitleBar.Visibility = Visibility.Collapsed;
		AppTitleBarWithoutAutoSuggestBox.Visibility = Visibility.Visible;
#endif
	}

#if UI_FEATURE_CUSTOMIZED_TITLE_BAR
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
		_appWindow = this.GetAppWindow();
		_appWindow.Changed += (sender, args) =>
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
		};

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

#if SEARCH_AUTO_SUGGESTION_BOX
			AppTitleBar.Loaded += (_, _) => SetDragRegionForCustomTitleBar(_appWindow);
			AppTitleBar.SizeChanged += (_, _) => SetDragRegionForCustomTitleBar(_appWindow);
#else
			AppTitleBarWithoutAutoSuggestBox.Loaded += (_, _) => SetDragRegionForCustomTitleBar(_appWindow);
			AppTitleBarWithoutAutoSuggestBox.SizeChanged += (_, _) => SetDragRegionForCustomTitleBar(_appWindow);
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
	}

	/// <summary>
	/// Sets the draggable region that is used for the whole window.
	/// </summary>
	/// <param name="appWindow">The <see cref="AppWindow"/> instance.</param>
	private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
	{
		if (AppWindowTitleBar.IsCustomizationSupported() && appWindow.TitleBar.ExtendsContentIntoTitleBar
#if DEBUG
			/**
				This is a bug fix. This bug can be reproduced by Windows Application SDK v1.2.
				If you minimize and maximize the window via task bar icon, the expression value <c>appWindow.TitleBar.RightInset</c>
				will be -24.
			*/
			&& appWindow.TitleBar.RightInset >= 0
#endif
			)
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

			appWindow.TitleBar.SetDragRectangles(new[] { dragRectL, dragRectR });
#else
			var scaleAdjustment = GetScaleAdjustment();

			RectInt32 rect;
			rect.X = 0;
			rect.Y = 0;
			rect.Width = (int)(AppTitleBarWithoutAutoSuggestBox.ActualWidth * scaleAdjustment);
			rect.Height = (int)(AppTitleBarWithoutAutoSuggestBox.ActualHeight * scaleAdjustment);
			appWindow.TitleBar.SetDragRectangles(new[] { rect });
#endif
		}
	}
#endif

#if MICA_BACKDROP || ACRYLIC_BACKDROP
	/// <summary>
	/// Sets <see cref="SystemBackdropConfiguration.Theme"/> to the target value.
	/// </summary>
	/// <remarks>
	/// This method requires the member <see cref="_configurationSource"/> be not <see langword="null"/>.
	/// </remarks>
	/// <exception cref="NotSupportedException">Throws when the actual theme is not defined.</exception>
	/// <seealso cref="SystemBackdropConfiguration.Theme"/>
	/// <seealso cref="_configurationSource"/>
	private void SetConfigurationSourceTheme()
	{
		Debug.Assert(_configurationSource is not null);

		_configurationSource.Theme = f(((FrameworkElement)Content).ActualTheme);


		static SystemBackdropTheme f(ElementTheme elementTheme, [CallerArgumentExpression(nameof(elementTheme))] string? expression = null)
			=> elementTheme switch
			{
				ElementTheme.Dark => SystemBackdropTheme.Dark,
				ElementTheme.Light => SystemBackdropTheme.Light,
				ElementTheme.Default => SystemBackdropTheme.Default,
				_ => throw new NotSupportedException($"The value '{expression}' is not supported.")
			};
	}
#endif

	/// <summary>
	/// Try to set the title of the main navigation frame.
	/// </summary>
	/// <param name="pageType">The page type.</param>
	private void SetFrameDisplayTitle(Type pageType)
		=> MainNavigationView.Header = GetStringNullable($"{nameof(MainWindow)}_{pageType.Name}Title") ?? string.Empty;

#if UI_FEATURE_CUSTOMIZED_TITLE_BAR
	/// <summary>
	/// Try to set icon of the program.
	/// </summary>
	private void SetAppIcon() => _appWindow.SetIcon(@"Resources\images\Logo.ico");

	/// <summary>
	/// Try to set program name onto the title.
	/// </summary>
	private void SetAppTitle() => _appWindow.Title = GetString("_ProgramName");
#endif

	/// <summary>
	/// An outer-layered method to switching pages. This method can be used by both
	/// <see cref="NavigationView_ItemInvoked"/> and <see cref="MainNavigationView_SelectionChanged"/>.
	/// </summary>
	/// <param name="container">The container.</param>
	/// <seealso cref="NavigationView_ItemInvoked"/>
	/// <seealso cref="MainNavigationView_SelectionChanged"/>
	private void SwitchingPage(NavigationViewItemBase container)
	{
		foreach (var (condition, pageType) in _navigatingData)
		{
			if (condition(container))
			{
				NavigateToPage(pageType);
				return;
			}
		}
	}

	/// <summary>
	/// An outer-layered method to switching pages. This method can be used by both
	/// <see cref="NavigationView_ItemInvoked"/> and <see cref="MainNavigationView_SelectionChanged"/>.
	/// </summary>
	/// <param name="container">The container.</param>
	/// <param name="isSettingsNavigationViewItemSelectedOrInvoked">
	/// A <see cref="bool"/> value indicating whether the settings item is invoked or selected.
	/// </param>
	private void SwitchingPage(NavigationViewItemBase container, bool isSettingsNavigationViewItemSelectedOrInvoked)
	{
		if (isSettingsNavigationViewItemSelectedOrInvoked)
		{
			NavigateToPage(typeof(SettingsPage));
		}
		else
		{
			SwitchingPage(container);
		}
	}

#if MICA_BACKDROP || ACRYLIC_BACKDROP
	/// <summary>
	/// Try to set Mica backdrop.
	/// </summary>
	/// <returns>A <see cref="bool"/> result indicating whether the operation is succeeded.</returns>
	[MemberNotNullWhen(true, nameof(_wsdqHelper))]
	private bool TrySetMicaBackdrop()
	{
#if MICA_BACKDROP
		if (MicaController.IsSupported())
#elif ACRYLIC_BACKDROP
		if (DesktopAcrylicController.IsSupported())
#endif
		{
			(_wsdqHelper = new()).EnsureWindowsSystemDispatcherQueueController();

			// Hooking up the policy object.
			_configurationSource = new();

			Activated += onActivated;
			Closed += (_, _) =>
			{
				// Make sure any Mica/Acrylic controller is disposed so it doesn't try to use this closed window.
#if MICA_BACKDROP
				if (_micaController is not null)
				{
					_micaController.Dispose();
					_micaController = null;
				}
#elif ACRYLIC_BACKDROP
				if (_acrylicController is not null)
				{
					_acrylicController.Dispose();
					_acrylicController = null;
				}
#endif

				Activated -= onActivated;
				_configurationSource = null;
			};
			((FrameworkElement)Content).ActualThemeChanged += (_, _) => { if (_configurationSource is not null) SetConfigurationSourceTheme(); };

			// Initial configuration state.
			_configurationSource.IsInputActive = true;
			SetConfigurationSourceTheme();

#if MICA_BACKDROP
			_micaController = new();

			// Enable the system backdrop.
			_micaController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
			_micaController.SetSystemBackdropConfiguration(_configurationSource);
#elif ACRYLIC_BACKDROP
			_acrylicController = new();

			// Enable the system backdrop.
			_acrylicController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
			_acrylicController.SetSystemBackdropConfiguration(_configurationSource);
#endif

			return true; // Succeeded.
		}

		return false; // Mica/Acrylic is not supported on this system.


		void onActivated(object _, MicrosoftXamlWindowActivatedEventArgs args)
		{
			Debug.Assert(_configurationSource is not null);

			_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
		}
	}
#endif

#if UI_FEATURE_CUSTOMIZED_TITLE_BAR
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


	private void NavigationView_Loaded(object sender, RoutedEventArgs e) => AnalyzePageItem.IsSelected = true;

	private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		=> SwitchingPage(args.InvokedItemContainer, args.IsSettingsInvoked);

	private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		=> SwitchingPage(args.SelectedItemContainer, args.IsSettingsSelected);

	private void Window_Closed(object sender, WindowEventArgs args)
	{
		SavePreference();
		SavePuzzleGeneratingHistory();
	}

	private void MainNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
	{
		if (NavigationViewFrame is { CanGoBack: true, BackStack: [.., { SourcePageType: var lastPageType }] })
		{
			NavigationViewFrame.GoBack();

			SetFrameDisplayTitle(lastPageType);
		}
	}
}

/// <include file='../../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Gets <see cref="AppWindow"/> instance for the current <see cref="Window"/> instance.
	/// </summary>
	/// <param name="this">The current <see cref="Window"/> instance.</param>
	/// <returns>A valid <see cref="AppWindow"/> instance.</returns>
	public static AppWindow GetAppWindow(this Window @this)
	{
		var hWnd = WindowNative.GetWindowHandle(@this);
		var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
		return AppWindow.GetFromWindowId(wndId);
	}
}
