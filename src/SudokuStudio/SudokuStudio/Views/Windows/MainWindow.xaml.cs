#if MICA_BACKDROP || ACRYLIC_BACKDROP
#define WARNING_WHEN_BOTH_BACKDROP_SET
#endif

namespace SudokuStudio.Views.Windows;

/// <summary>
/// Provides with a <see cref="Window"/> instance that is running as main instance of the program.
/// </summary>
/// <seealso cref="Window"/>
public sealed partial class MainWindow : Window
{
	/// <summary>
	/// The default navigation transition instance that will create animation fallback while switching pages.
	/// </summary>
	private static readonly NavigationTransitionInfo NavigationTransitionInfo = new EntranceNavigationTransitionInfo();


#if CUSTOMIZED_TITLE_BAR
	/// <summary>
	/// Defines an <see cref="AppWindow"/> instance that is used by interaction with core application behaviors,
	/// such as icon, title bars and so on.
	/// </summary>
	/// <seealso cref="AppWindow"/>
	private AppWindow _appWindow;
#endif

	/// <summary>
	/// The navigating data. This field is used by <see cref="SwitchingPage(bool, NavigationViewItemBase)"/>.
	/// </summary>
	/// <seealso cref="SwitchingPage(bool, NavigationViewItemBase)"/>
	private Dictionary<Func<bool, NavigationViewItemBase, bool>, Type> _navigatingData;

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
#if WARNING_WHEN_BOTH_BACKDROP_SET
#warning You should not set both 'MICA_BACKDROP' and 'ACRYLIC_BACKDROP'; acrylic material will be ignored.
#else
#error Cannot set both 'MICA_BACKDROP' and 'ACRYLIC_BACKDROP' at same time.
#endif
#elif MICA_BACKDROP
		TrySetMicaBackdrop();
#elif ACRYLIC_BACKDROP
		TrySetAcrylicBackdrop();
#endif

#if CUSTOMIZED_TITLE_BAR
		InitializeAppWindow();
#endif

		SetAppIcon();
		SetAppTitle();

		LoadProgramPreferenceFromLocal();
	}


	/// <summary>
	/// Initializes fields.
	/// </summary>
	[MemberNotNull(nameof(_navigatingData))]
	private void InitializeField()
		=> _navigatingData = new()
		{
			{ static (isSettingInvokedOrSelected, _) => isSettingInvokedOrSelected, typeof(SettingsPage) },
			{ (_, container) => container == AnalyzePageItem, typeof(AnalyzePage) },
			{ (_, container) => container == AboutPagetItem, typeof(AboutPage) }
		};

#if CUSTOMIZED_TITLE_BAR
	/// <summary>
	/// Initializes for field <see cref="_appWindow"/>.
	/// </summary>
	/// <remarks>
	/// For more information please visit
	/// <see href="https://learn.microsoft.com/en-us/windows/apps/develop/title-bar">this link</see>.
	/// This passage is for full customization of application title bar.
	/// </remarks>
	[MemberNotNull(nameof(_appWindow))]
#if false
	[Conditional("CUSTOMIZED_TITLE_BAR")]
#endif
	private void InitializeAppWindow()
	{
		_appWindow = this.GetAppWindow(out _, out _);
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
						AppTitleBar.Visibility = Visibility.Collapsed;
						titleBar.ResetToDefault();
						break;
					}
					case AppWindowPresenterKind.FullScreen:
					{
						// Full screen - hide the custom title bar and the default system title bar.
						AppTitleBar.Visibility = Visibility.Collapsed;
						titleBar.ExtendsContentIntoTitleBar = true;
						break;
					}
					case AppWindowPresenterKind.Overlapped:
					{
						// Normal - hide the system title bar and use the custom title bar instead.
						AppTitleBar.Visibility = Visibility.Visible;
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

			AppTitleBar.Loaded += (_, _) => { if (AppWindowTitleBar.IsCustomizationSupported()) { SetDragRegionForCustomTitleBar(_appWindow); } };

			AppTitleBar.SizeChanged += (_, _) =>
			{
				if (AppWindowTitleBar.IsCustomizationSupported() && _appWindow.TitleBar.ExtendsContentIntoTitleBar)
				{
					// Update drag region if the size of the title bar changes.
					SetDragRegionForCustomTitleBar(_appWindow);
				}
			};
		}
		else
		{
			// Title bar customization using these APIs is currently supported only on Windows 11.
			// In other cases, hide the custom title bar element.
			AppTitleBar.Visibility = Visibility.Collapsed;
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
	/// Try to navigate to the target page.
	/// </summary>
	/// <param name="pageType">The target page type.</param>
	private void NavigateToPage(Type pageType)
	{
		if (NavigationViewFrame.SourcePageType != pageType)
		{
			NavigationViewFrame.Navigate(pageType, null, NavigationTransitionInfo);
			MainNavigationView.Header = GetStringNullable($"{nameof(MainWindow)}_{pageType.Name}{nameof(Title)}") ?? string.Empty;
		}
	}

	/// <summary>
	/// Try to set icon of the program.
	/// </summary>
	private void SetAppIcon() => _appWindow.SetIcon(@"Resources\images\Logo.ico");

	/// <summary>
	/// Try to set program name onto the title.
	/// </summary>
	private void SetAppTitle() => _appWindow.Title = GetString("_ProgramName");

	/// <summary>
	/// Loads the program preference from local.
	/// </summary>
	private void LoadProgramPreferenceFromLocal()
	{
		var targetPath = CommonPaths.UserPreference;
		if (!File.Exists(targetPath))
		{
			return;
		}

		if (ProgramPreferenceFileHandler.Read(targetPath) is not { } loadedConfig)
		{
			return;
		}

		((App)Application.Current).RunningContext.ProgramPreference.CoverBy(loadedConfig);
	}

	/// <summary>
	/// An outer-layered method to switching pages. This method can be used by both
	/// <see cref="NavigationView_ItemInvoked"/> and <see cref="MainNavigationView_SelectionChanged"/>.
	/// </summary>
	/// <param name="isSettingInvokedOrSelected">Indicates whether the setting menu item is invoked or selected.</param>
	/// <param name="container">The container.</param>
	/// <seealso cref="NavigationView_ItemInvoked"/>
	/// <seealso cref="MainNavigationView_SelectionChanged"/>
	private void SwitchingPage(bool isSettingInvokedOrSelected, NavigationViewItemBase container)
	{
		foreach (var (condition, pageType) in _navigatingData)
		{
			if (condition(isSettingInvokedOrSelected, container))
			{
				NavigateToPage(pageType);
			}
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

#if CUSTOMIZED_TITLE_BAR
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
#endif


#if CUSTOMIZED_TITLE_BAR
	[LibraryImport("Shcore", SetLastError = true)]
	private static partial int GetDpiForMonitor(nint hmonitor, Monitor_DPI_Type dpiType, out uint dpiX, out uint dpiY);
#endif


	private void NavigationView_Loaded(object sender, RoutedEventArgs e) => AnalyzePageItem.IsSelected = true;

	private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		=> SwitchingPage(args.IsSettingsInvoked, args.InvokedItemContainer);

	private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		=> SwitchingPage(args.IsSettingsSelected, args.SelectedItemContainer);

	private void Window_Closed(object sender, WindowEventArgs args)
		=> ProgramPreferenceFileHandler.Write(CommonPaths.UserPreference, ((App)Application.Current).RunningContext.ProgramPreference);
}
