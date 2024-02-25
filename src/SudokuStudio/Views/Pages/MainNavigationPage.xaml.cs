namespace SudokuStudio.Views.Pages;

/// <summary>
/// The navigation page for main window.
/// </summary>
internal sealed partial class MainNavigationPage : Page
{
	/// <summary>
	/// The navigating data. This dictionary stores the routing data that can be used and controlled
	/// by control <see cref="NavigationViewFrame"/>.
	/// </summary>
	/// <seealso cref="NavigationViewFrame"/>
	private List<(Func<NavigationViewItemBase, bool> PageChecker, Type PageType)> _navigatingData;


	/// <summary>
	/// Initializes a <see cref="MainNavigationPage"/> instance.
	/// </summary>
	public MainNavigationPage()
	{
		InitializeComponent();
		InitializeField();
	}


	/// <summary>
	/// Indicates the parent window.
	/// </summary>
	public MainWindow ParentWindow { get; set; } = null!;


	/// <summary>
	/// Try to set the title of the main navigation frame.
	/// </summary>
	/// <param name="pageType">The page type.</param>
	internal void SetFrameDisplayTitle(Type pageType)
	{
		var titleKey = $"{nameof(MainWindow)}_{pageType.Name}Title";
		MainNavigationView.Header = ResourceDictionary.TryGet(titleKey, out var resource, App.CurrentCulture) ? resource : string.Empty;
	}

	/// <summary>
	/// Initializes fields.
	/// </summary>
	[MemberNotNull(nameof(_navigatingData))]
	private void InitializeField()
	{
		_navigatingData = [
			(container => container == AnalyzePageItem, typeof(AnalyzePage)),
			(container => container == GeneratingStrategyPageItem, typeof(GeneratingStrategyPage)),
			(container => container == AboutPageItem, typeof(AboutPage)),
			(container => container == SingleCountingPageItem, typeof(SingleCountingPracticingPage)),
			(container => container == LibraryPageItem, typeof(LibraryPage)),
			(container => container == HotkeyCheatTablePageItem, typeof(HotkeyCheatTablePage)),
			(container => container == TechniqueGalleryPageItem, typeof(TechniqueGalleryPage))
		];

		MainNavigationView.OpenPaneLength = (double)((App)Application.Current).Preference.UIPreferences.MainNavigationPageOpenPaneLength;
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
			ParentWindow.NavigateToPage(typeof(SettingsPage));
		}
		else
		{
			SwitchingPage(container);
		}
	}

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
				ParentWindow.NavigateToPage(pageType);
				return;
			}
		}
	}


	private void NavigationView_Loaded(object sender, RoutedEventArgs e) => AnalyzePageItem.IsSelected = true;

	private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		=> SwitchingPage(args.InvokedItemContainer, args.IsSettingsInvoked);

	private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		=> SwitchingPage(args.SelectedItemContainer, args.IsSettingsSelected);

	private void MainNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
	{
		if (NavigationViewFrame is { CanGoBack: true, BackStack: [.., { SourcePageType: var lastPageType }] })
		{
			NavigationViewFrame.GoBack();
			SetFrameDisplayTitle(lastPageType);
		}
	}
}
