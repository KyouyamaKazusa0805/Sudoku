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
	private static readonly (string ViewItemTag, Type PageType, bool DisplayTitle)[] NavigationPairs;


	/// <summary>
	/// Indicates the gathered keywords.
	/// </summary>
	private (string Key, string Value, string OriginalValue)[] _gatheredQueryKeywords = null!;

	/// <summary>
	/// Indicates the helper instance.
	/// </summary>
	/// <remarks>
	/// For more information about the Mica material, please visit
	/// <see href="https://docs.microsoft.com/en-us/windows/apps/design/style/mica">this link</see>
	/// to learn more.
	/// </remarks>
	private WindowsSystemDispatcherQueueHelper _wsdqHelper = null!;

	/// <summary>
	/// The Mica material controller.
	/// </summary>
	/// <remarks>
	/// For more information about the Mica material, please visit
	/// <see href="https://docs.microsoft.com/en-us/windows/apps/design/style/mica">this link</see>
	/// to learn more.
	/// </remarks>
	private MicaController _micaController = null!;

	/// <summary>
	/// Indicates the configuration source instance for Mica material.
	/// </summary>
	/// <remarks>
	/// For more information about the Mica material, please visit
	/// <see href="https://docs.microsoft.com/en-us/windows/apps/design/style/mica">this link</see>
	/// to learn more.
	/// </remarks>
	private SystemBackdropConfiguration _configurationSource = null!;


	/// <summary>
	/// Initializes a <see cref="MainWindow"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MainWindow()
	{
		// Initializes the controls.
		InitializeComponent();

#if false
		// Try to set the window with Mica material.
		// For more information, please visit this link:
		// https://github.com/microsoft/WinUI-Gallery/blob/main/WinUIGallery/SamplePages/SampleSystemBackdropsWindow.xaml.cs
		TrySetMicaBackdrop();
#endif

		// Sets the title of the window.
		// If we set the value to the XAML file, that file cannot be compiled successfully:
		// WMC0615: Type 'StaticResource' used after '{' must be a Markup Extension. Error code 0x09c4.
		// We must set the value here instead.
		Title = R["ProgramName"];
	}


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static MainWindow()
		=> NavigationPairs = (
			from type in typeof(MainWindow).Assembly.GetDerivedTypes<Page>()
			let attribute = type.GetCustomAttribute<PageAttribute>()
			where attribute is not null
			select (type.Name, type, attribute.DisplayTitle)
		).ToArray();


	/// <summary>
	/// Try to navigate the pages.
	/// </summary>
	/// <param name="tag">The specified tag of the navigate page item.</param>
	/// <param name="transitionInfo">The transition information.</param>
	private void OnNavigate(string tag, NavigationTransitionInfo transitionInfo)
	{
		var (_, pageType, displayTitle) = Array.Find(NavigationPairs, p => p.ViewItemTag == tag);

		// Get the page type before navigation so you can prevent duplicate entries in the back-stack.
		// Only navigate if the selected page isn't currently loaded.
		var preNavPageType = _cViewRouterFrame.CurrentSourcePageType;
		if (pageType is not null && preNavPageType != pageType)
		{
			_cViewRouterFrame.Navigate(pageType, null, transitionInfo);
		}

		_cViewRouter.AlwaysShowHeader = displayTitle;
	}

	/// <summary>
	/// Try to set the configuration source with the theme value.
	/// </summary>
	private void SetConfigurationSourceTheme()
	{
		if (((FrameworkElement)Content).ActualTheme is var actualTheme && Enum.IsDefined(actualTheme))
		{
			_configurationSource.Theme = actualTheme switch
			{
				ElementTheme.Dark => SystemBackdropTheme.Dark,
				ElementTheme.Light => SystemBackdropTheme.Light,
				ElementTheme.Default => SystemBackdropTheme.Default,
				_ => default
			};
		}
	}

	/// <summary>
	/// Try to set Mica backdrop.
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating whether the operation is succeeded.</returns>
	/// <remarks>
	/// For more information about the Mica material, please visit
	/// <see href="https://docs.microsoft.com/en-us/windows/apps/design/style/mica">this link</see>
	/// to learn more.
	/// </remarks>
	[MemberNotNullWhen(true, nameof(_wsdqHelper), nameof(_configurationSource), nameof(_micaController))]
	private bool TrySetMicaBackdrop()
	{
		if (MicaController.IsSupported())
		{
			_wsdqHelper = new();
			_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

			// Hooking up the policy object.
			_configurationSource = new();
			Activated += Window_Activated;
			Closed += Window_Closed;
			((FrameworkElement)Content).ActualThemeChanged += Window_ThemeChanged;

			// Initial configuration state.
			_configurationSource.IsInputActive = true;
			SetConfigurationSourceTheme();

			_micaController = new();

			// Enable the system backdrop.
			_micaController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
			_micaController.SetSystemBackdropConfiguration(_configurationSource);

			return true; // Succeeded.
		}

		return false; // Mica is not supported on this system.
	}


	/// <summary>
	/// To clear the content of the specified <see cref="AutoSuggestBox"/> instance.
	/// </summary>
	/// <param name="autoSuggestBox">The <see cref="AutoSuggestBox"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ClearAutoSuggestBoxValue(AutoSuggestBox autoSuggestBox) => autoSuggestBox.Text = string.Empty;


	/// <summary>
	/// Triggers when the window is activated.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void Window_Activated(object sender, MsWindowActivatedEventArgs args)
		=> _configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;

	/// <summary>
	/// Triggers when the window is closed.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void Window_Closed(object sender, WindowEventArgs args)
	{
		// Make sure any Mica/Acrylic controller is disposed so it doesn't try to use this closed window.
		if (_micaController is not null)
		{
			_micaController.Dispose();
			_micaController = null!;
		}

		Activated -= Window_Activated;
		_configurationSource = null!;
	}

	/// <summary>
	/// Triggers when the theme is changed.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void Window_ThemeChanged(FrameworkElement sender, object args)
	{
		if (_configurationSource is not null)
		{
			SetConfigurationSourceTheme();
		}
	}

	/// <summary>
	/// Triggers when the view router control is loaded.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void ViewRouter_Loaded(object sender, RoutedEventArgs e)
		=> OnNavigate(
			((App)Application.Current).InitialInfo switch
			{
				{ FirstGrid: not null } => nameof(SudokuPage),
#if AUTHOR_FEATURE_CELL_MARKS
				{ DrawingDataRawValue: not null } => nameof(SudokuPage),
#endif
				{ FirstPageTypeName: var firstPageTypeName } => firstPageTypeName,
				_ => throw new InvalidOperationException("The initialization information is invalid.")
			},
			new EntranceNavigationTransitionInfo()
		);

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
		=> ClearAutoSuggestBoxValue(sender);
}
