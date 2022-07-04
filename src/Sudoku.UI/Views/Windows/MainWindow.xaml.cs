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
	/// Indicates the gathered keywords.
	/// </summary>
	private (string Key, string Value, string OriginalValue)[] _gatheredQueryKeywords = null!;

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
		EnsureDispatcherQueueExists();
		SetMicaBackdropIfSupports();
		SetProgramNameToTitle();
		LoadGlobalPreferenceIfExistsAsync();
		ConstructPreferenceItems();
	}


	/// <summary>
	/// Constructs the preference items.
	/// </summary>
	/// <exception cref="Exception">Throws when invalid data has encountered.</exception>
	private void ConstructPreferenceItems()
	{
		const string invokeMethodName = nameof(IDynamicCreatableItem<SliderSettingItem>.DynamicCreate);
		((App)Application.Current).RuntimeInfo.Value = (
			from propertyInfo in typeof(Preference).GetProperties()
			where propertyInfo is { CanRead: true, CanWrite: true }
			let returnParameter = propertyInfo.SetMethod!.ReturnParameter
			let modReqs = returnParameter.GetRequiredCustomModifiers()
			where !modReqs.CanFind(static modReq => modReq == typeof(IsExternalInit))
			let genericArgTypes = propertyInfo.GetGenericAttributeTypeArguments(typeof(PreferenceAttribute<>))
			where genericArgTypes.Length == 1
			let genericArgType = genericArgTypes[0]
			let preferenceAttributeType = typeof(PreferenceAttribute<>).MakeGenericType(genericArgType)
			let prefAttribute = propertyInfo.GetCustomAttribute(preferenceAttributeType)
			where prefAttribute is not null
			let dynamicInvokeMethodInfo = genericArgType.GetMethod(invokeMethodName)
			where dynamicInvokeMethodInfo is not null
			let settingItem = (SettingItem?)dynamicInvokeMethodInfo.Invoke(null, new object?[] { propertyInfo.Name })
			where settingItem is not null
			let groupAttribute = propertyInfo.GetCustomAttribute<PreferenceGroupAttribute>()
			where groupAttribute is not null
			let groupName = groupAttribute.Name
			let groupOrderingIndex = groupAttribute.OrderingIndex
			let gnr = R[$"SettingsPage_GroupItemName_{groupName}"] ?? throw new()
			let gnd = R[$"SettingsPage_GroupItemDescription_{groupName}"]
			select (Resource: gnr, DescriptionResource: gnd, Item: settingItem, Ordering: groupOrderingIndex) into tuple
			group tuple by (tuple.Resource, tuple.DescriptionResource) into groupedTuple
			let pair = groupedTuple.Key
			let settingItems = (from tuple in groupedTuple orderby tuple.Ordering select tuple.Item).ToArray()
			select new SettingGroupItem
			{
				Name = pair.Resource,
				Description = pair.DescriptionResource,
				SettingItem = settingItems
			}
		).ToArray();
	}

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

		if (Supportable.Mica)
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
			ElementTheme.Default => SystemBackdropTheme.Default,
			_ => default
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
	private async void Window_ClosedAsync(object sender, WindowEventArgs args)
		=> await SaveGlobalPreferenceFileAsync();

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
	private void ViewRouter_Loaded(object sender, RoutedEventArgs e)
		=> OnNavigate(((App)Application.Current).InitialInfo.RouteToPageName(), new EntranceNavigationTransitionInfo());

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

		var (tag, _, _) = Array.Find(NavigationPairs, p => p.PageType == sourcePageType);
		var item = menuItems.Concat(footerMenuItems).OfType<NavigationViewItem>().First(n => n.Tag as string == tag);
		_cViewRouter.SelectedItem = item;
		_cViewRouter.Header = item.Content?.ToString();
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
			if (splitText.All(key => keywordsSplit.CanFind(k => arrayPredicate(k, key))))
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
