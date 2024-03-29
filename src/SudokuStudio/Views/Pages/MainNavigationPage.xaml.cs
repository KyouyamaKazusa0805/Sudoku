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
	private List<(Func<NavigationViewItemBase, bool>, Type)> _navigatingData;


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
			(container => container == ConstraintPageItem, typeof(GeneratedPuzzleConstraintPage)),
			(container => container == TechniqueDataPageItem, typeof(TechniqueInfoModifierPage)),
			(container => container == AboutPageItem, typeof(AboutPage)),
			(container => container == SingleCountingPageItem, typeof(SingleCountingPracticingPage)),
			(container => container == LibraryPageItem, typeof(LibraryPage)),
			(container => container == HotkeyCheatTablePageItem, typeof(HotkeyCheatTablePage)),
			(container => container == TechniqueGalleryPageItem, typeof(TechniqueGalleryPage))
		];

		MainNavigationView.OpenPaneLength = (double)((App)Application.Current).Preference.UIPreferences.MainNavigationPageOpenPaneLength;
	}

	private void NavigationView_Loaded(object sender, RoutedEventArgs e) => AnalyzePageItem.IsSelected = true;

	private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
	{
		if (args.IsSettingsSelected)
		{
			ParentWindow.NavigateToPage<SettingsPage>();
		}
		else
		{
			foreach (var (match, pageType) in _navigatingData)
			{
				if (match(args.SelectedItemContainer))
				{
					ParentWindow.NavigateToPage(pageType);
					return;
				}
			}
		}
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
