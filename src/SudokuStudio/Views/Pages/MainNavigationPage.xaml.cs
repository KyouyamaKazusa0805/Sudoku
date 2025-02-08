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
		(static @this => @this.HotkeyCheatTablePageItem, typeof(HotkeyCheatTablePage)),
		(static @this => @this.TechniqueGalleryPageItem, typeof(TechniqueGalleryPage)),
		(static @this => @this.CommandBasedDrawingPageItem, typeof(CommandBasedDrawingPage))
	];

	/// <summary>
	/// Indicates the slide navigation information instance, from right.
	/// </summary>
	private static readonly SlideNavigationTransitionInfo SlideNavigationRight = new()
	{
		Effect = SlideNavigationTransitionEffect.FromRight
	};

	/// <summary>
	/// Indicates the slide navigation information instance, from left.
	/// </summary>
	private static readonly SlideNavigationTransitionInfo SlideNavigationLeft = new()
	{
		Effect = SlideNavigationTransitionEffect.FromLeft
	};


	/// <summary>
	/// Initializes a <see cref="MainNavigationPage"/> instance.
	/// </summary>
	public MainNavigationPage()
	{
		InitializeComponent();
		UpdateMemoryWidth();
	}


	/// <summary>
	/// Indicates the parent window.
	/// </summary>
	public MainWindow ParentWindow { get; set; } = null!;

	/// <summary>
	/// Indicates the header bar items.
	/// </summary>
	[DependencyProperty]
	public partial ObservableCollection<PageNavigationBindableSource> HeaderBarItems { get; set; }

	/// <summary>
	/// Sets the navigated page type with custom data.
	/// </summary>
	public (Type PageType, object? Value) PageToWithValue
	{
		set
		{
			var (p, v) = value;
			if (NavigationViewFrame.SourcePageType != p)
			{
				NavigationViewFrame.Navigate(p, v, SlideNavigationRight);
				HeaderBarItems = SR.TryGet(PageTypeResourceKey(p), out var resource, App.CurrentCulture)
					? [new() { PageType = p, PageTitle = resource }]
					: [];
			}
		}
	}

	/// <summary>
	/// Sets the navigated page type with pop pages count.
	/// </summary>
	public (Type PageType, int PopPagesCount) PageToPop
	{
		set
		{
			var (p, v) = value;
			if (NavigationViewFrame.SourcePageType != p)
			{
				NavigationViewFrame.Navigate(p, v, SlideNavigationLeft);
				for (var i = 0; i < v; i++)
				{
					HeaderBarItems.RemoveAt(^1);
				}
			}
		}
	}

	/// <summary>
	/// Sets the navigated page type with a <see cref="bool"/> value indicating whether the page should be stacked.
	/// </summary>
	public (bool StackPage, Type PageType) PageToWithStack
	{
		set
		{
			var p = value.PageType;
			if (NavigationViewFrame.SourcePageType == p)
			{
				return;
			}

			NavigationViewFrame.Navigate(p, null, SlideNavigationRight);

			SR.TryGet(PageTypeResourceKey(p), out var resource, App.CurrentCulture);
			switch (value.StackPage, resource)
			{
				case (true, _):
				{
					HeaderBarItems.Add(new() { PageType = p, PageTitle = resource ?? "<null>" });
					break;
				}
				case (false, not null):
				{
					HeaderBarItems = [new() { PageType = p, PageTitle = resource }];
					break;
				}
				default:
				{
					HeaderBarItems = [];
					break;
				}
			}
		}
	}


	/// <summary>
	/// Sets memory width.
	/// </summary>
	private void UpdateMemoryWidth()
		=> MainNavigationView.OpenPaneLength = (double)Application.Current
			.AsApp()
			.Preference
			.UIPreferences
			.MainNavigationPageOpenPaneLength;

	/// <summary>
	/// Handle navigation.
	/// </summary>
	/// <param name="pageTypeChecker">The method that checks whether the <see cref="Type"/> instance is matched.</param>
	/// <param name="action">The action to handle navigation.</param>
	private void HandleNavigation(
		Func<NavigationViewItemBase, Type, bool> pageTypeChecker,
		Action<NavigationViewItemBase, Type> action
	)
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


	/// <summary>
	/// Returns page type resource key.
	/// </summary>
	/// <param name="type">The type of the page.</param>
	/// <returns>The resource key.</returns>
	private static string PageTypeResourceKey(Type type) => $"{nameof(MainWindow)}_{type.Name}Title";


	private void NavigationView_Loaded(object sender, RoutedEventArgs e) => AnalyzePageItem.IsSelected = true;

	private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
	{
		Action action = args.IsSettingsSelected
			? () => ParentWindow.NavigateToPage(typeof(SettingsPage))
			: () => HandleNavigation((c, _) => c == args.SelectedItemContainer, (_, p) => ParentWindow.NavigateToPage(p));
		action();
	}

	private void MainNavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
	{
		if (NavigationViewFrame is { CanGoBack: true, BackStack: [.., { SourcePageType: var lastPageType }] })
		{
			NavigationViewFrame.GoBack();

			if (HeaderBarItems.Count == 1)
			{
				HeaderBarItems = SR.TryGet(PageTypeResourceKey(lastPageType), out var resource, App.CurrentCulture)
					? [new() { PageType = lastPageType, PageTitle = resource }]
					: [];
			}
			else
			{
				HeaderBarItems.RemoveAt(^1);
			}

			HandleNavigation((_, p) => p == lastPageType, static (c, _) => c.IsSelected = true);
		}
	}

	private void HeaderBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
	{
		var type = ((PageNavigationBindableSource)args.Item).PageType!;
		ParentWindow.NavigateToPage(type, HeaderBarItems.Count - args.Index - 1);
	}
}
