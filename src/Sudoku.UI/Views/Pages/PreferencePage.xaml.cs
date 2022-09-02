namespace Sudoku.UI.Views.Pages;

/// <summary>
/// The preference page.
/// </summary>
[Page]
public sealed partial class PreferencePage : Page
{
	/// <summary>
	/// Initializes a <see cref="PreferencePage"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PreferencePage() => InitializeComponent();


	/// <summary>
	/// The default transition information.
	/// </summary>
	private EntranceNavigationTransitionInfo TransitionInfo => new();


	/// <summary>
	/// Triggers when the hyper link to basic options page is clicked.
	/// </summary>
	/// <param name="sender">The object triggering this event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void BasicOptionsPageRouteButton_Click(object sender, RoutedEventArgs e)
		=> ((App)Application.Current).RuntimeInfo.MainWindow.OnNavigate(nameof(BasicOptionsPage), TransitionInfo);

	/// <summary>
	/// Triggers when the hyper link to basic options page is clicked.
	/// </summary>
	/// <param name="sender">The object triggering this event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void SolvingOptionsPageRouteButton_Click(object sender, RoutedEventArgs e)
		=> ((App)Application.Current).RuntimeInfo.MainWindow.OnNavigate(nameof(SolvingOptionsPage), TransitionInfo);

	/// <summary>
	/// Triggers when the hyper link to basic options page is clicked.
	/// </summary>
	/// <param name="sender">The object triggering this event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void RenderingOptionsPageRouteButton_Click(object sender, RoutedEventArgs e)
		=> ((App)Application.Current).RuntimeInfo.MainWindow.OnNavigate(nameof(RenderingOptionsPage), TransitionInfo);

	/// <summary>
	/// Triggers when the hyper link to basic options page is clicked.
	/// </summary>
	/// <param name="sender">The object triggering this event.</param>
	/// <param name="e">The event arguments provided.</param>
	private void MiscellaneousOptionsPageRouteButton_Click(object sender, RoutedEventArgs e)
		=> ((App)Application.Current).RuntimeInfo.MainWindow.OnNavigate(nameof(MiscellaneousOptionsPage), TransitionInfo);
}
