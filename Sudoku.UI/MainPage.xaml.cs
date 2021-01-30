using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Sudoku.UI
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		/// <summary>
		/// Initializes a <see cref="MainPage"/> instance using the default behavior.
		/// </summary>
		public MainPage() => InitializeComponent();


		/// <summary>
		/// Triggers when a item control of <see cref="NavigationViewMain"/> is invoked.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="args">The event arguments provided.</param>
		private void NavigationViewMain_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{
			if (args.IsSettingsInvoked)
			{
				// Settings page.
				//ContentFrame.Navigate(typeof(SettingsPage));
			}
			else
			{
				// Other pages.
				var navViews = sender.MenuItems.OfType<NavigationViewItem>();
				var item = navViews.First(findFirstItem);

				// Then navigate to the specified page.
				switch (item.Tag)
				{
					//case nameof(NaviagtionViewItemSudokuGrid):
					//{
					//	ContentFrame.Navigate(typeof(SudokuGridPage));
					//	break;
					//}
					//case nameof(NavigationViewItemInfo):
					//{
					//	ContentFrame.Navigate(typeof(InfoPage));
					//	break;
					//}
					case nameof(NavigationViewItemQuit):
					{
						break;
					}
				}
			}

			bool findFirstItem(NavigationViewItem x) => (string)x.Content == (string)args.InvokedItem;
		}
	}
}
