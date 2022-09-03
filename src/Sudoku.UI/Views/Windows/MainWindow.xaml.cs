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
	private static readonly NavigationInfo[] NavigationPairs = NavigationInfo.GetPages();


	/// <summary>
	/// Indicates the helper type instance that is used for ensuring the dispatcher queue is not null.
	/// </summary>
	private readonly WinsysDispatcherQueueHelper _wsdqHelper = new();

	/// <summary>
	/// Indicates the mica controller instance.
	/// </summary>
	private MicaController? _micaController;

	/// <summary>
	/// Indicates the system backdrop configuration instance.
	/// </summary>
	private SystemBackdropConfiguration? _configurationSource;


	/// <summary>
	/// Initializes a <see cref="MainWindow"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MainWindow()
	{
		InitializeComponent();
		AddHandlerForCheckingBatteryStatusIfWorth();
		EnsureDispatcherQueueExists();
		SetMicaBackdropIfSupports();
		SetProgramNameToTitle();
		SetIconFromAssetFile();
		LoadGlobalPreferenceIfExistsAsync();
	}


	/// <summary>
	/// Try to navigate the pages.
	/// </summary>
	/// <param name="tag">The specified tag of the navigate page item.</param>
	/// <param name="transitionInfo">The transition information.</param>
	internal void OnNavigate(string tag, NavigationTransitionInfo transitionInfo)
	{
		var (_, pageType) = Array.Find(NavigationPairs, p => p.ViewItemTag == tag);

		// Get the page type before navigation so you can prevent duplicate entries in the back-stack.
		// Only navigate if the selected page isn't currently loaded.
		var preNavPageType = _cViewRouterFrame.CurrentSourcePageType;
		if (pageType is not null && preNavPageType != pageType)
		{
			_cViewRouterFrame.Navigate(pageType, null, transitionInfo);
		}
	}

	/// <summary>
	/// Add handlers that checking for battery status.
	/// </summary>
	private void AddHandlerForCheckingBatteryStatusIfWorth()
	{
		if (!((App)Application.Current).UserPreference.CheckBatteryStatusWhenOpen)
		{
			return;
		}

		PowerManager.BatteryStatusChanged += BatteryRelatedItemsStatusChangedAsync;
		PowerManager.RemainingChargePercentChanged += BatteryRelatedItemsStatusChangedAsync;
		PowerManager.PowerSupplyStatusChanged += BatteryRelatedItemsStatusChangedAsync;
		PowerManager.PowerSourceKindChanged += BatteryRelatedItemsStatusChangedAsync;
	}

	/// <summary>
	/// Remove handlers that checking for battery status.
	/// </summary>
	private void RemoveHandlerForCheckingBatteryStatusIfWorth()
	{
		if (!((App)Application.Current).UserPreference.CheckBatteryStatusWhenOpen)
		{
			return;
		}

		PowerManager.BatteryStatusChanged -= BatteryRelatedItemsStatusChangedAsync;
		PowerManager.RemainingChargePercentChanged -= BatteryRelatedItemsStatusChangedAsync;
		PowerManager.PowerSupplyStatusChanged -= BatteryRelatedItemsStatusChangedAsync;
		PowerManager.PowerSourceKindChanged -= BatteryRelatedItemsStatusChangedAsync;
	}

	/// <summary>
	/// Sets the icon from assets file.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetIconFromAssetFile() => this.GetAppWindow().SetIcon(@"Assets\Logo.ico");

	/// <summary>
	/// To ensure the dispatcher queue exists.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void EnsureDispatcherQueueExists() => _wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

	/// <summary>
	/// Try to set the title.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetProgramNameToTitle() => Title = R["ProgramName"];

	/// <summary>
	/// Try to set the Mica backdrop. The method is used as an entry to set Mica backdrop,
	/// which is called by the constructor of the current type.
	/// </summary>
	private void SetMicaBackdropIfSupports()
	{
		if (_micaController is not null)
		{
			_micaController.Dispose();
			_micaController = null;
		}

		Activated -= UserDefined_Window_Activated;
		Closed -= UserDefined_Window_Closed;

		_configurationSource = null;

		if (Supportability.Mica)
		{
			// Hooking up the policy object.
			_configurationSource = new();
			Activated += UserDefined_Window_Activated;
			Closed += UserDefined_Window_Closed;
			((FrameworkElement)Content).ActualThemeChanged += UserDefined_Window_ThemeChanged;

			// Initial configuration state.
			_configurationSource.IsInputActive = true;
			SetConfigurationSourceTheme();

			_micaController = new();

			// I tested the case that I run the program at the machine lower than 22H1,
			// the method will always throw an exception whose the inner message is difficult to understand.
			// The reason why the method throws an exception is that
			// the window doesn't support the Mica backdrop in earlier versions.
			_micaController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());

			// Enable the system backdrop.
			_micaController.SetSystemBackdropConfiguration(_configurationSource);
		}
	}

	/// <summary>
	/// Checks for the battery status if worth.
	/// </summary>
	/// <returns>The task that handles for the current operation.</returns>
	private async Task ReportBatteryLowerStatusAsync()
	{
		var batteryStatus = PowerManager.BatteryStatus;
		int remainingCharge = PowerManager.RemainingChargePercent;
		var powerStatus = PowerManager.PowerSupplyStatus;
		var powerSource = PowerManager.PowerSourceKind;

		if (powerSource == PowerSourceKind.DC && batteryStatus == BatteryStatus.Discharging && remainingCharge < 50
			|| powerSource == PowerSourceKind.AC && powerStatus == PowerSupplyStatus.Inadequate)
		{
			await SimpleControlFactory.CreateErrorDialog(_cViewRouter)
				.WithTitle(R["BatteryStatusIsNotGood"]!)
				.WithContent(R["BatteryStatusIsNotGood_Detail"]!)
				.ShowAsync();
		}
	}

	/// <summary>
	/// Used for methods <see cref="AddHandlerForCheckingBatteryStatusIfWorth()"/>
	/// and <see cref="RemoveHandlerForCheckingBatteryStatusIfWorth()"/>
	/// </summary>
	private async void BatteryRelatedItemsStatusChangedAsync(object? _, object __) => await ReportBatteryLowerStatusAsync();

	/// <summary>
	/// Try to set the theme to field <see cref="_configurationSource"/>. The method requires the field
	/// <see cref="_configurationSource"/> be not <see langword="null"/>.
	/// </summary>
	/// <seealso cref="_configurationSource"/>
	private void SetConfigurationSourceTheme()
	{
		if (((FrameworkElement)Content).ActualTheme is var elementTheme && !Enum.IsDefined(elementTheme))
		{
			return;
		}

		Debug.Assert(_configurationSource is not null);

		_configurationSource.Theme = elementTheme switch
		{
			ElementTheme.Dark => SystemBackdropTheme.Dark,
			ElementTheme.Light => SystemBackdropTheme.Light,
			ElementTheme.Default => SystemBackdropTheme.Default
		};
	}

	/// <summary>
	/// Loads the local preference file if the file exists.
	/// </summary>
	/// <returns>The task.</returns>
	private async void LoadGlobalPreferenceIfExistsAsync()
	{
		var initialInfo = ((App)Application.Current).InitialInfo;
		if (!initialInfo.FromPreferenceFile)
		{
			var up = await PreferenceSavingLoading.LoadAsync();
			((App)Application.Current).UserPreference.CoverPreferenceBy(up);
		}
	}

	/// <summary>
	/// Saves the global preference file to the local path.
	/// </summary>
	/// <returns>The task.</returns>
	private async Task SaveGlobalPreferenceFileAsync()
	{
		var initialInfo = ((App)Application.Current).InitialInfo;
		if (!initialInfo.FromPreferenceFile)
		{
			var up = ((App)Application.Current).UserPreference;
			await PreferenceSavingLoading.SaveAsync(up);
		}
	}


	/// <summary>
	/// To clear the content of the specified <see cref="AutoSuggestBox"/> instance.
	/// </summary>
	/// <param name="autoSuggestBox">The <see cref="AutoSuggestBox"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ClearAutoSuggestBoxValue(AutoSuggestBox autoSuggestBox) => autoSuggestBox.Text = string.Empty;


	/// <summary>
	/// Triggers when the window is closed.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private async void Window_ClosedAsync(object sender, WindowEventArgs args) => await SaveGlobalPreferenceFileAsync();

	/// <summary>
	/// Triggers when the window is activated.
	/// This method requires the field <see cref="_configurationSource"/> being not <see langword="null"/>.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="args">The event arguments provided.</param>
	/// <seealso cref="_configurationSource"/>
	private void UserDefined_Window_Activated(object sender, MsWindowActivatedEventArgs args)
	{
		Debug.Assert(_configurationSource is not null);

		_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
	}

	/// <summary>
	/// Triggers when the window is closed.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="args">The event arguments provided.</param>
	/// <remarks>
	/// Make sure the Mica controller is disposed so it doesn't try to use this closed window.
	/// </remarks>
	private void UserDefined_Window_Closed(object sender, WindowEventArgs args)
	{
		if (_micaController is not null)
		{
			_micaController.Dispose();
			_micaController = null;
		}

		Activated -= UserDefined_Window_Activated;
		_configurationSource = null;

		RemoveHandlerForCheckingBatteryStatusIfWorth();
	}

	/// <summary>
	/// Triggers when the theme of the window is changed.
	/// </summary>
	/// <param name="sender">The object that triggers the event.</param>
	/// <param name="args">The event arguments provided.</param>
	private void UserDefined_Window_ThemeChanged(FrameworkElement sender, object args)
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
	private async void ViewRouter_LoadedAsync(object sender, RoutedEventArgs e)
	{
		await ReportBatteryLowerStatusAsync();

		var navigationInfo = new EntranceNavigationTransitionInfo();
		switch ((App)Application.Current)
		{
			case { UserPreference.IsFirstMeet: true }:
			{
				OnNavigate(nameof(HomePage), navigationInfo);

				((App)Application.Current).UserPreference.IsFirstMeet = false;

				break;
			}
			case { UserPreference.AlwaysShowHomePageWhenOpen: var homePageAlways, InitialInfo: var initInfo }:
			{
				OnNavigate(
					initInfo.RouteToPageName() is var pageName && homePageAlways && pageName == nameof(HomePage)
						? nameof(HomePage)
						: pageName,
					navigationInfo
				);

				break;
			}
		}
	}

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
#pragma warning disable IDE0055
		if ((sender, e, _cViewRouter) is not (
				Frame { SourcePageType: not null },
				{ SourcePageType: var sourcePageType },
				{ MenuItems: var menuItems, FooterMenuItems: var footerMenuItems }
			))
#pragma warning restore IDE0055
		{
			return;
		}

		var (tag, _) = Array.Find(NavigationPairs, p => p.PageType == sourcePageType);
		var item = (MenuItemTemplateData)menuItems.Concat(footerMenuItems).First(menuItemChoser);
		_cViewRouter.SelectedItem = item;
		_cTitleBar.TitleText = item switch { { Title: string localTitle } => localTitle, _ => string.Empty };


		bool menuItemChoser(object n) => n is MenuItemTemplateData { Tag: string localTag } && tag == localTag;
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
}
