using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sudoku.UI.Data;
using Sudoku.UI.Dictionaries;

namespace Sudoku.UI.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		/// <summary>
		/// Indicates the command name that used for quitting this program.
		/// </summary>
		private static readonly string QuitCommandName = nameof(NavigationViewItemQuit)[^4..];


		/// <summary>
		/// Initializes a <see cref="MainPage"/> instance with the default instantiation behavior.
		/// </summary>
		public MainPage() => InitializeComponent();


		/// <summary>
		/// Indicates the base window.
		/// </summary>
		public MainWindow BaseWindow { get; internal set; } = null!;

		/// <summary>
		/// Indicates the preferences used in the base window.
		/// </summary>
		public PagePreferences Preferences => BaseWindow.Preferences;


		/// <summary>
		/// Indicates the event that triggered when the base window is closing.
		/// </summary>
		/// <remarks>
		/// This event is used for saving configuration files, when user clicked the navigation view item
		/// called <c>NavigationViewItemQuit</c>.
		/// </remarks>
		public event EventHandler? BaseWindowClosing;


		/// <summary>
		/// Update settings page navigation view item display text and its tool tip.
		/// </summary>
		private static void InitializeSettingsPartOfNavigationView(NavigationView view)
		{
			var item = (NavigationViewItem)view.SettingsItem;
			item.Content = ResourceFinder.Current.NavigationViewItemSettings;
			ToolTipService.SetToolTip(item, ResourceFinder.Current.NavigationViewItemSettingsToolTip);
		}


		/// <summary>
		/// Triggers when the current page is loaded.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="e">The event arguments provided.</param>
		private void Page_Loaded(object sender, RoutedEventArgs e) =>
			InitializeSettingsPartOfNavigationView(NavigationViewMain);

		/// <summary>
		/// Triggers when the menu item of the control <see cref="NavigationViewMain"/>
		/// is invoked.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="args">The event arguments provided.</param>
		private void NavigationViewMain_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{
			if (args.IsSettingsInvoked) // Settings page.
			{
				//FrameToShowTheSpecifiedPage.Navigate(typeof(SettingsPage));
			}
			else // Other pages.
			{
				var allViews = sender.MenuItems.OfType<NavigationViewItem>();

				// Then navigate to the specified page.
				object tag = allViews.First(findFirstItem).Tag;
				if (tag is not string t)
				{
					return;
				}

				if (t == QuitCommandName) // To exit the program.
				{
					BaseWindowClosing?.Invoke(this, EventArgs.Empty);
					Application.Current.Exit();
				}
				else if (Navigation.GetPageType(t) is { } type) // Switch to other pages.
				{
					FrameToShowTheSpecifiedPage.Navigate(type);
				}
			}

			bool findFirstItem(NavigationViewItem x) => (string)x.Content == (string)args.InvokedItem;
		}

		/// <summary>
		/// Triggers when the selection of <see cref="NavigationViewMain"/> is changed.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="args">The event arguments provided.</param>
		private void NavigationViewMain_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			if (sender is NavigationView view)
			{
				InitializeSettingsPartOfNavigationView(view);
			}
		}
	}
}
