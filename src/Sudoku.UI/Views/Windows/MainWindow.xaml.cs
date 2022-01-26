namespace Sudoku.UI.Views.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a <see cref="Frame"/>.
/// </summary>
/// <seealso cref="Frame"/>
public sealed partial class MainWindow : Window
{
	/// <summary>
	/// Indicates the navigation pairs that controls to route pages.
	/// </summary>
	private readonly (string ViewItemTag, Type PageType)[] _navigationPairs =
	{
		(nameof(MainPage), typeof(MainPage)),
		(nameof(SettingsPage), typeof(SettingsPage))
	};


	/// <summary>
	/// Initializes a <see cref="MainWindow"/> instance.
	/// </summary>
	public MainWindow() => InitializeComponent();


	/// <summary>
	/// Try to navigate the pages.
	/// </summary>
	/// <param name="tag">The specified tag of the navigate page item.</param>
	/// <param name="transitionInfo">The transition information.</param>
	private void OnNavigate(string tag, NavigationTransitionInfo transitionInfo)
	{
		var (_, pageType) = _navigationPairs.FirstOrDefault(p => p.ViewItemTag == tag);

		// Get the page type before navigation so you can prevent duplicate entries in the backstack.
		// Only navigate if the selected page isn't currently loaded.
		var preNavPageType = _viewRouterFrame.CurrentSourcePageType;
		if (pageType is not null && preNavPageType != pageType)
		{
			_viewRouterFrame.Navigate(pageType, null, transitionInfo);
		}
	}


	private void ViewRouterFrame_NavigationFailed([IsDiscard] object sender, NavigationFailedEventArgs e) =>
		throw new InvalidOperationException($"Cannot find the page '{e.SourcePageType.FullName}'.");

	private void ViewRouterFrame_Navigated(object sender, NavigationEventArgs e)
	{
		if (
			(Sender: sender, EventArg: e, Router: _viewRouter) is not (
				Sender: Frame { SourcePageType: not null },
				EventArg: { SourcePageType: var sourcePageType },
				Router: { MenuItems: var menuItems, FooterMenuItems: var footerMenuItems }
			)
		)
		{
			return;
		}

		var (tag, _) = _navigationPairs.FirstOrDefault(tagSelector);
		var item = menuItems.Concat(footerMenuItems).OfType<NavigationViewItem>().First(itemSelector);
		_viewRouter.SelectedItem = item;
		_viewRouter.Header = item.Content?.ToString();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool tagSelector((string, Type PageType) p) => p.PageType == sourcePageType;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool itemSelector(NavigationViewItem n) => n.Tag as string == tag;
	}

	private void ViewRouter_ItemInvoked([IsDiscard] NavigationView sender, NavigationViewItemInvokedEventArgs args)
	{
		if (
			args is not
			{
				InvokedItemContainer.Tag: string itemTag,
				RecommendedNavigationTransitionInfo: var info
			}
		)
		{
			return;
		}

		OnNavigate(itemTag, info);
	}

	private void ViewRouter_SelectionChanged([IsDiscard] NavigationView sender, NavigationViewSelectionChangedEventArgs args)
	{
		if (
			args is not
			{
				SelectedItemContainer.Tag: string itemTag,
				RecommendedNavigationTransitionInfo: var info
			}
		)
		{
			return;
		}

		OnNavigate(itemTag, info);
	}
}
