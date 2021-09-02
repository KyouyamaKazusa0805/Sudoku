namespace Sudoku.UI;

/// <summary>
/// The type that provides with the <see cref="Window"/> instances
/// which the current UI project starts and launches.
/// </summary>
/// <seealso cref="Window"/>
public sealed partial class MainWindow : Window
{
	/// <summary>
	/// Initializes a <see cref="MainWindow"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MainWindow()
	{
		InitializeComponent();

		InitializeControls();
	}


	/// <summary>
	/// Initializes the controls.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InitializeControls()
	{
		Title = Ui.Current.MainWindowTitle;
	}


	/// <summary>
	/// Invokes when the user switches the page to another one in the control <see cref="NavigationView_Main"/>.
	/// </summary>
	/// <param name="sender">The instance which triggered this event.</param>
	/// <param name="args">The arguments provided.</param>
	private void NavigationView_Main_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
	{
		string? tag = args.InvokedItemContainer.Tag as string;
		var (pageType, header) = tag switch
		{
			_ when tag == Ui.Current.MainWindow_NavigationViewItem_Tag_SudokuPanel =>
				(typeof(SudokuPanelPage), Ui.Current.MainWindow_NavigationViewItem_Content_SudokuPanel),
			_ when tag == Ui.Current.MainWindow_NavigationViewItem_Tag_About =>
				(typeof(AboutPage), Ui.Current.MainWindow_NavigationViewItem_Content_About),
			_ => (null, null)
		};

		if (pageType == Frame_NavigationView_Main.CurrentSourcePageType || header is null)
		{
			// No switch.
			return;
		}

		sender.Header = header;
		Frame_NavigationView_Main.NavigateToType(pageType, null, new FrameNavigationOptions
		{
			TransitionInfoOverride = args.RecommendedNavigationTransitionInfo
		});
	}
}
