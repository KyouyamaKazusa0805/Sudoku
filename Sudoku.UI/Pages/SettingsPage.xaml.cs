using System;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Sudoku.UI.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SettingsPage : Page
	{
		/// <summary>
		/// Initializes a <see cref="SettingsPage"/> instance using the default behavior.
		/// </summary>
		public SettingsPage() => InitializeComponent();


		/// <summary>
		/// Triggers when a item control of <see cref="NavigationViewMain"/> is invoked.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="args">The event arguments provided.</param>
		private void NavigationViewMain_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{
			// Other pages.
			var allViews = sender.MenuItems.OfType<NavigationViewItem>();

			// Then navigate to the specified page.
			string tag = (string)allViews.First(findFirstItem).Tag;
			if (Type.GetType($"Sudoku.UI.Pages.Settings{tag}Page") is not { } type)
			{
				return;
			}

			FrameToShowTheSpecifiedPage.Navigate(type);

			bool findFirstItem(NavigationViewItem x) => (string)x.Content == (string)args.InvokedItem;
		}

		/// <summary>
		/// Triggers when the selected item has been changed.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="args">The event arguments provided.</param>
		private void NavigationViewMain_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			var item = (NavigationViewItem)args.SelectedItem;
			sender.Header = item.Content;
		}
	}
}
