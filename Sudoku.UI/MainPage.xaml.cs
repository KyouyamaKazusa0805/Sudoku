using System.Linq;
using Sudoku.UI.Pages;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static Sudoku.UI.Dictionaries.DictionaryResources;

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
		/// Triggers when <see cref="NavigationViewMain"/> is loaded.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="e">The event arguments provided.</param>
		private void NavigationViewMain_Loaded(object sender, RoutedEventArgs e)
		{
			if (sender is not NavigationView view)
			{
				return;
			}

			var item = (NavigationViewItem)view.SettingsItem;
			item.Content = LangSource["NavigationViewItemSettings"];
			ToolTipService.SetToolTip(item, LangSource["NavigationViewItemSettingsToolTip"]);
		}

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
					//	// The sudoku grid page.
					//	FrameToShowTheSpecifiedPage.Navigate(typeof(SudokuGridPage));
					//	break;
					//}
					case nameof(NavigationViewItemInfo):
					{
						// The information page.
						FrameToShowTheSpecifiedPage.Navigate(typeof(InfoPage));
						break;
					}
					case nameof(NavigationViewItemQuit):
					{
						// To exit the program.
						CoreApplication.Exit();
						break;
					}
				}
			}

			bool findFirstItem(NavigationViewItem x) => (string)x.Content == (string)args.InvokedItem;
		}
	}
}
