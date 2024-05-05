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
	private static readonly (Func<MainNavigationPage, NavigationViewItemBase>, Type)[] NavigatingArray = [
		(static @this => @this.AnalyzePageItem, typeof(AnalyzePage)),
		(static @this => @this.ConstraintPageItem, typeof(GeneratedPuzzleConstraintPage)),
		(static @this => @this.TechniqueDataPageItem, typeof(TechniqueInfoModifierPage)),
		(static @this => @this.AboutPageItem, typeof(AboutPage)),
		(static @this => @this.SingleCountingPageItem, typeof(SingleCountingPracticingPage)),
		(static @this => @this.LibraryPageItem, typeof(LibraryPage)),
		(static @this => @this.GeneratorPageItem, typeof(PatternBasedPuzzleGeneratingPage)),
		(static @this => @this.RatingFormulaPageItem, typeof(RatingFormulaPage)),
		(static @this => @this.HotkeyCheatTablePageItem, typeof(HotkeyCheatTablePage)),
		(static @this => @this.TechniqueGalleryPageItem, typeof(TechniqueGalleryPage))
	];


	/// <summary>
	/// Initializes a <see cref="MainNavigationPage"/> instance.
	/// </summary>
	public MainNavigationPage()
	{
		InitializeComponent();
		SetMemoryWidth();
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
	/// Sets memory width.
	/// </summary>
	private void SetMemoryWidth()
		=> MainNavigationView.OpenPaneLength = (double)((App)Application.Current).Preference.UIPreferences.MainNavigationPageOpenPaneLength;

	/// <summary>
	/// Handle navigation.
	/// </summary>
	/// <param name="pageTypeChecker">The method that checks whether the <see cref="Type"/> instance is matched.</param>
	/// <param name="action">The action to handle navigation.</param>
	private void HandleNavigation(Func<NavigationViewItemBase, Type, bool> pageTypeChecker, Action<NavigationViewItemBase, Type> action)
	{
		foreach (var (match, pageType) in NavigatingArray)
		{
			if (pageTypeChecker(match(this), pageType))
			{
				action(match(this), pageType);
				return;
			}
		}
	}


	private void NavigationView_Loaded(object sender, RoutedEventArgs e) => AnalyzePageItem.IsSelected = true;

	private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		=> (
			args.IsSettingsSelected
				? (Action)ParentWindow.NavigateToPage<SettingsPage>
				: () => HandleNavigation((c, _) => c == args.SelectedItemContainer, (_, p) => ParentWindow.NavigateToPage(p))
		)();

	private void MainNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
	{
		if (NavigationViewFrame is { CanGoBack: true, BackStack: [.., { SourcePageType: var lastPageType }] })
		{
			NavigationViewFrame.GoBack();
			SetFrameDisplayTitle(lastPageType);

			HandleNavigation((_, p) => p == lastPageType, static (c, _) => c.IsSelected = true);
		}
	}
}
