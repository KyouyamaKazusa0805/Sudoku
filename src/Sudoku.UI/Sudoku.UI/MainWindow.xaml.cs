namespace Sudoku.UI;

/// <summary>
/// The type that provides with the <see cref="Window"/> instances
/// which the current UI project starts and launches.
/// </summary>
/// <seealso cref="Window"/>
public sealed partial class MainWindow : Window
{
	/// <summary>
	/// Indiates the navigation list.
	/// </summary>
	private static readonly (string Tag, Type Type, string Content)[] NavigationInfoList = new[]
	{
		(
			(string)UiResources.Current.MainWindow_NavigationViewItem_Tag_SudokuPanel,
			typeof(SudokuPanelPage),
			(string)UiResources.Current.MainWindow_NavigationViewItem_Content_SudokuPanel
		),
		(
			(string)UiResources.Current.MainWindow_NavigationViewItem_Tag_About,
			typeof(AboutPage),
			(string)UiResources.Current.MainWindow_NavigationViewItem_Content_About
		)
	};


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
		Title = UiResources.Current.MainWindowTitle;
	}


	/// <summary>
	/// Invokes when the user switches the page to another one in the control <see cref="NavigationView_Main"/>.
	/// </summary>
	/// <param name="sender">The instance which triggered this event.</param>
	/// <param name="args">The arguments provided.</param>
	private void NavigationView_Main_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
	{
		if (args is not { InvokedItemContainer.Tag: string tag, RecommendedNavigationTransitionInfo: var info })
		{
			return;
		}

		var type = Frame_NavigationView_Main.CurrentSourcePageType;
		try
		{
			var (_, pageType, header) = NavigationInfoList.FirstOnThrow(triplet =>
			{
				var (a, b, c) = triplet;
				return a == tag && b != type && c is not null;
			});

			sender.Header = header;
			Frame_NavigationView_Main.NavigateToType(pageType, null, new() { TransitionInfoOverride = info });
		}
		catch (ArgumentException)
		{
		}
	}
}
