namespace Sudoku.UI.Views.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a <see cref="Frame"/>.
/// </summary>
/// <seealso cref="Frame"/>
public sealed partial class MainWindow : Window
{
	/// <summary>
	/// Indicates the navigation info tuples that controls to route pages.
	/// </summary>
	private static readonly (string ViewItemTag, Type PageType, bool DisplayTitle)[] NavigationPairs =
	{
		(nameof(SettingsPage), typeof(SettingsPage), true),
		(nameof(AboutPage), typeof(AboutPage), true),
		(nameof(SudokuPage), typeof(SudokuPage), false),
		(nameof(DocumentationPage), typeof(DocumentationPage), true)
	};


	/// <summary>
	/// Indicates the gathered keywords.
	/// </summary>
	private (string Key, string Value, string OriginalValue)[] _gatheredQueryKeywords = null!;

	/// <summary>
	/// Indicates the application window.
	/// </summary>
	private AppWindow _appWindow;


	/// <summary>
	/// Initializes a <see cref="MainWindow"/> instance.
	/// </summary>
	public MainWindow()
	{
		// Initializes the controls.
		InitializeComponent();

		// Initializes the field '_appWindow'.
		InitializeAppWindowField();

		// Sets the title of the window.
		Title = R["ProgramName"];

		// To customize the title bar if available.
		CustomizeTitleBar();
	}


	/// <summary>
	/// Initializes the field <see cref="_appWindow"/>.
	/// </summary>
	/// <seealso cref="_appWindow"/>
	[MemberNotNull(nameof(_appWindow))]
	private void InitializeAppWindowField()
	{
		_appWindow = this.GetAppWindow();
		_appWindow.Changed += AppWindow_Changed;
	}

	/// <summary>
	/// Customize the title bar if available.
	/// </summary>
	private void CustomizeTitleBar()
	{
		// Check to see if customization is supported.
		// Currently only supported on Windows 11.
		if (AppWindowTitleBar.IsCustomizationSupported() && _appWindow is { TitleBar: var titleBar })
		{
			// Hide default title bar.
			titleBar.ExtendsContentIntoTitleBar = true;

			// Set events that detects the re-sizing by users.
			_cAppTitleBar.Loaded += AppTitleBar_Loaded;
			_cAppTitleBar.SizeChanged += AppTitleBar_SizeChanged;

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

	/// <summary>
	/// Sets the dragable region for the customized title bar.
	/// </summary>
	/// <param name="appWindow">The application window.</param>
	private void SetDragRegionForCustomTitleBar(AppWindow appWindow)
	{
		if (AppWindowTitleBar.IsCustomizationSupported() && appWindow.TitleBar.ExtendsContentIntoTitleBar)
		{
			double scaleAdjustment = GetScaleAdjustment();

			_cRightPaddingColumn.Width = new GridLength(appWindow.TitleBar.RightInset / scaleAdjustment);
			_cLeftPaddingColumn.Width = new GridLength(appWindow.TitleBar.LeftInset / scaleAdjustment);

			var dragRectsList = new List<RectInt32>();

			RectInt32 dragRectL;
			dragRectL.X = (int)((_cLeftPaddingColumn.ActualWidth) * scaleAdjustment);
			dragRectL.Y = 0;
			dragRectL.Height = (int)(_cAppTitleBar.ActualHeight * scaleAdjustment);
			dragRectL.Width = (int)((_cIconColumn.ActualWidth + _cTitleColumn.ActualWidth + _cLeftDragColumn.ActualWidth) * scaleAdjustment);
			dragRectsList.Add(dragRectL);

			RectInt32 dragRectR;
			dragRectR.X = (int)((_cLeftPaddingColumn.ActualWidth + _cIconColumn.ActualWidth + _cTitleTextBlock.ActualWidth + _cLeftDragColumn.ActualWidth + _cSearchColumn.ActualWidth) * scaleAdjustment);
			dragRectR.Y = 0;
			dragRectR.Height = (int)(_cAppTitleBar.ActualHeight * scaleAdjustment);
			dragRectR.Width = (int)(_cRightDragColumn.ActualWidth * scaleAdjustment);
			dragRectsList.Add(dragRectR);

			var dragRects = dragRectsList.ToArray();
			appWindow.TitleBar.SetDragRectangles(dragRects);
		}
	}

	/// <summary>
	/// Gets the adjustments on scaling.
	/// </summary>
	/// <returns>The percentage of the scaling.</returns>
	/// <exception cref="Exception">Throws when the target DPI is failed to get.</exception>
	private double GetScaleAdjustment()
	{
		var hWnd = WindowNative.GetWindowHandle(this);
		var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
		var displayArea = DisplayArea.GetFromWindowId(wndId, DisplayAreaFallback.Primary);
		var hMonitor = Win32Interop.GetMonitorFromDisplayId(displayArea.DisplayId);

		// Get DPI.
		if (AppWindowMarshal.GetDpiForMonitor(hMonitor, Monitor_DPI_Type.MDT_Default, out uint dpiX, out uint _) != 0)
		{
			throw new Exception("Could not get DPI for monitor.");
		}

		uint scaleFactorPercent = (uint)(((long)dpiX * 100 + (96 >> 1)) / 96);
		return scaleFactorPercent / 100.0;
	}

	/// <summary>
	/// Try to navigate the pages.
	/// </summary>
	/// <param name="tag">The specified tag of the navigate page item.</param>
	/// <param name="transitionInfo">The transition information.</param>
	private void OnNavigate(string tag, NavigationTransitionInfo transitionInfo)
	{
		var (_, pageType, displayTitle) = Array.Find(NavigationPairs, p => p.ViewItemTag == tag);

		// Get the page type before navigation so you can prevent duplicate entries in the backstack.
		// Only navigate if the selected page isn't currently loaded.
		var preNavPageType = _cViewRouterFrame.CurrentSourcePageType;
		if (pageType is not null && preNavPageType != pageType)
		{
			_cViewRouterFrame.Navigate(pageType, null, transitionInfo);
		}

		_cViewRouter.AlwaysShowHeader = displayTitle;
	}


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
				// Compact overlay - hide custom title bar and use the default system title bar instead.
				case AppWindowPresenterKind.CompactOverlay:
				{	
					_cAppTitleBar.Visibility = Visibility.Collapsed;
					sender.TitleBar.ResetToDefault();

					break;
				}

				// Full screen - hide the custom title bar and the default system title bar.
				case AppWindowPresenterKind.FullScreen:
				{
					_cAppTitleBar.Visibility = Visibility.Collapsed;
					sender.TitleBar.ExtendsContentIntoTitleBar = true;

					break;
				}

				// Normal - hide the system title bar and use the custom title bar instead.
				case AppWindowPresenterKind.Overlapped:
				{
					_cAppTitleBar.Visibility = Visibility.Visible;
					sender.TitleBar.ExtendsContentIntoTitleBar = true;
					SetDragRegionForCustomTitleBar(sender);

					break;
				}

				// Use the default system title bar.
				default:
				{
					sender.TitleBar.ResetToDefault();

					break;
				}
			}
		}
	}

	/// <summary>
	/// Switch on presenters. This method is only used when the relative XAML code is enabled.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The event arguments provided.</param>
	[Conditional("FULLSCREEN_MODE_ENABLED")]
	private void SwitchPresenter(object sender, RoutedEventArgs e)
	{
		if ((_appWindow, sender) is not (not null, Button { Name: var buttonName }))
		{
			return;
		}

		var newPresenterKind = buttonName switch
		{
			"_toCompactoverlay" => AppWindowPresenterKind.CompactOverlay,
			"_toFullscreen" => AppWindowPresenterKind.FullScreen,
			"_toOverlapped" => AppWindowPresenterKind.Overlapped,
			_ => AppWindowPresenterKind.Default,
		};

		_appWindow.SetPresenter(
			newPresenterKind == _appWindow.Presenter.Kind
				// If the same presenter button was pressed as the mode we're in, toggle the window back to Default.
				? AppWindowPresenterKind.Default
				// Else request a presenter of the selected kind to be created and applied to the window.
				: newPresenterKind
		);
	}

	/// <summary>
	/// Triggers when the view router control is loaded.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void ViewRouter_Loaded(object sender, RoutedEventArgs e)
		=> OnNavigate(nameof(SudokuPage), new EntranceNavigationTransitionInfo());

	/// <summary>
	/// Triggers when the navigation is failed. The method will be invoked if and only if the routing is invalid.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	/// <exception cref="InvalidOperationException">
	/// Always throws. Because the method is handled with the failure of the navigation,
	/// the throwing is expected.
	/// </exception>
	[DoesNotReturn]
	private void ViewRouterFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
		=> throw new InvalidOperationException($"Cannot find the page '{e.SourcePageType.FullName}'.");

	/// <summary>
	/// Triggers when the frame of the navigation view control has navigated to a certain page.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void ViewRouterFrame_Navigated(object sender, NavigationEventArgs e)
	{
		if (
#pragma warning disable IDE0055
			(sender, e, _cViewRouter) is not (
				Frame { SourcePageType: not null },
				{ SourcePageType: var sourcePageType },
				{ MenuItems: var menuItems, FooterMenuItems: var footerMenuItems }
			)
#pragma warning restore IDE0055
		)
		{
			return;
		}

		var (tag, _, _) = Array.Find(NavigationPairs, tagSelector);
		var item = menuItems.Concat(footerMenuItems).OfType<NavigationViewItem>().First(itemSelector);
		_cViewRouter.SelectedItem = item;
		_cViewRouter.Header = item.Content?.ToString();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool tagSelector((string, Type PageType, bool) p) => p.PageType == sourcePageType;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool itemSelector(NavigationViewItem n) => n.Tag as string == tag;
	}

	/// <summary>
	/// Triggers when a page-related navigation item is clicked and selected.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void ViewRouter_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
	{
		if (args is { InvokedItemContainer.Tag: string tag, RecommendedNavigationTransitionInfo: var info })
		{
			OnNavigate(tag, info);
		}
	}

	/// <summary>
	/// Triggers when the page-related navigation item, as the selection, is changed.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void ViewRouter_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
	{
		if (args is { SelectedItemContainer.Tag: string tag, RecommendedNavigationTransitionInfo: var info })
		{
			OnNavigate(tag, info);
		}
	}

	/// <summary>
	/// Triggers when text of the main auto suggest box has been changed.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
	{
		if ((sender, args) is not ({ Text: var userText }, { Reason: AutoSuggestionBoxTextChangeReason.UserInput }))
		{
			return;
		}

		const string queryPrefix = "Query_";
		string cultureInfoName = CultureInfo.CurrentUICulture.Name;
		bool p(ResourceDictionary d) => d.Source.AbsolutePath.Contains(cultureInfoName, StringComparison.InvariantCultureIgnoreCase);
		var resourceDic = Application.Current.Resources.MergedDictionaries.FirstOrDefault(p);
		_gatheredQueryKeywords ??= (
			from key in resourceDic?.Keys.OfType<string>() ?? Array.Empty<string>()
			where key.StartsWith(queryPrefix) && resourceDic![key] is string
			let originalValue = resourceDic![key[queryPrefix.Length..]] as string
			where originalValue is not null
			select (key, R[key], originalValue)
		).ToArray();

		var suitableItems = new List<object>();
		string[] splitText = userText.ToLower(CultureInfo.CurrentUICulture).Split(" ");
		foreach (var (rawKey, rawValue, originalValue) in _gatheredQueryKeywords)
		{
			if (rawValue.Split('|') is not [var keywords, var resultToDisplay])
			{
				continue;
			}

			string key = rawKey[queryPrefix.Length..];
			string[] keywordsSplit = keywords.Split(';');
			static bool arrayPredicate(string k, string key) => k.ToLower(CultureInfo.CurrentUICulture).Contains(key);
			if (splitText.All(key => Array.FindIndex(keywordsSplit, k => arrayPredicate(k, key)) != -1))
			{
				suitableItems.Add(
					new SearchedResult
					{
						Value = originalValue,
						Location = resultToDisplay.Replace("->", R["Emoji_RightArrow"])
					}
				);
			}
		}
		if (suitableItems.Count == 0)
		{
			suitableItems.Add(R["QueryResult_Empty"]!);
		}

		sender.ItemsSource = suitableItems;
	}

	/// <summary>
	/// Triggers when a suggestion is chosen.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
		=> sender.Text = string.Empty;
}
