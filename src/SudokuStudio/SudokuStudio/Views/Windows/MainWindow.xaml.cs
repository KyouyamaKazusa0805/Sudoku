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


	/// <summary>
	/// Initializes a <see cref="MainWindow"/> instance.
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();

#if MICA_BACKDROP
		TrySetMicaBackdrop();
#endif
#if CUSTOMIZED_TITLE_BAR
		InitializeAppWindow();
#endif
		SetAppIcon();
		SetAppTitle();
	}


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
	/// An outer-layered method to switching pages. This method can be used by both
	/// <see cref="NavigationView_ItemInvoked"/> and <see cref="MainNavigationView_SelectionChanged"/>.
	/// </summary>
	/// <param name="container">The container.</param>
	/// <seealso cref="NavigationView_ItemInvoked"/>
	/// <seealso cref="MainNavigationView_SelectionChanged"/>
	private void SwitchingPage(bool isSettingInvokedOrSelected, NavigationViewItemBase container)
	{
		if (isSettingInvokedOrSelected)
		{
			NavigateToPage(typeof(SettingsPage));
		}
		else if (container == AnalyzePageItem)
		{
			NavigateToPage(typeof(AnalyzePage));
		}
	}


	private void NavigationView_Loaded(object sender, RoutedEventArgs e) => AnalyzePageItem.IsSelected = true;

	private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		=> SwitchingPage(args.IsSettingsInvoked, args.InvokedItemContainer);

	private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		=> SwitchingPage(args.IsSettingsSelected, args.SelectedItemContainer);
}
