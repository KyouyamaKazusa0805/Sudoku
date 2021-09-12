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
	/// <remarks>
	/// The triplet contains three values, where:
	/// <list type="table">
	/// <item>
	/// <term><c>Tag</c></term>
	/// <description>
	/// The tag of the <see cref="NavigationViewItem"/> instance, i.e. the property
	/// <see cref="FrameworkElement.Tag"/>.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>Type</c></term>
	/// <description>
	/// The type that the <see cref="Frame"/> instance bounds with a <see cref="NavigationView"/> instance
	/// can navigate and redirect to the specified page.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>Content</c></term>
	/// <description>The content of the page, i.e. the property <see cref="ContentControl.Content"/>.</description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso cref="NavigationView"/>
	/// <seealso cref="NavigationViewItem"/>
	/// <seealso cref="Frame"/>
	/// <seealso cref="FrameworkElement.Tag"/>
	/// <seealso cref="ContentControl.Content"/>
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
	public MainWindow() => InitializeComponent();


	/// <summary>
	/// Invokes when the user switches the page to another one in the control <see cref="NavigationView_Main"/>.
	/// </summary>
	/// <param name="sender">The instance which triggered this event.</param>
	/// <param name="args">The arguments provided.</param>
	private void NavigationView_Main_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
	{
		if (
			(Sender: sender, Args: args) is not
			(
				Sender:
				{
					Header: var navigationViewHeader,
					Content: Frame
					{
						Name: nameof(Frame_NavigationView_Main),
						CurrentSourcePageType: var type
					} frame
				},
				Args:
				{
					InvokedItemContainer.Tag: string tag,
					RecommendedNavigationTransitionInfo: var info,
					IsSettingsInvoked: var settingsInvoked
				}
			)
		)
		{
			return;
		}

		var fno = new FrameNavigationOptions { TransitionInfoOverride = info };
		if (settingsInvoked)
		{
			sender.Header = UiResources.Current.MainWindow_NavigationViewItem_Content_Settings;
			frame.NavigateToType(typeof(SettingsPage), null, fno);
		}
		else
		{
			try
			{
				var (_, pageType, header) = NavigationInfoList.FirstOnThrow(triplet =>
				{
					var (a, b, c) = triplet;
					return a == tag && b != type && c is not null;
				});

				sender.Header = header;
				frame.NavigateToType(pageType, null, fno);
			}
			catch
			{
			}
		}
	}
}
