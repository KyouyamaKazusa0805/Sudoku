namespace SudokuStudio.Views.Windows;

/// <summary>
/// Provides with a <see cref="Window"/> instance that is running as main instance of the program.
/// </summary>
/// <seealso cref="Window"/>
public sealed partial class MainWindow : Window
{
	/// <summary>
	/// The default navigation options.
	/// </summary>
	private static readonly FrameNavigationOptions NavigationOptions = new() { TransitionInfoOverride = new EntranceNavigationTransitionInfo() };


	/// <summary>
	/// Initializes a <see cref="MainWindow"/> instance.
	/// </summary>
	public MainWindow()
	{
		InitializeComponent();
		TrySetMicaBackdrop();
		InitializeAppWindow();
	}


	/// <summary>
	/// Try to navigate to the target page.
	/// </summary>
	/// <param name="pageType">The target page type.</param>
	private void NavigateToPage(Type? pageType)
	{
		if (pageType is not null)
		{
			NavigationViewFrame.NavigateToType(pageType, null, NavigationOptions);
			MainNavigationView.Header = GetStringNullable($"{nameof(MainWindow)}_{pageType.Name}{nameof(Title)}") ?? string.Empty;
		}
	}


	private void NavigationView_Loaded(object sender, RoutedEventArgs e) => NavigateToPage(typeof(AnalyzePage));

	private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		=> NavigateToPage(
			args switch
			{
				{ InvokedItemContainer: var container } when container == AnalyzePageItem => typeof(AnalyzePage),
				_ => null
			}
		);
}
