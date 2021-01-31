using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sudoku.UI.Dictionaries;

namespace Sudoku.UI.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		/// <summary>
		/// Initializes a <see cref="MainPage"/> instance with the default instantiation behavior.
		/// </summary>
		public MainPage() => InitializeComponent();


		/// <summary>
		/// Triggers when the current page is loaded.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="e">The event arguments provided.</param>
		private void Page_Loaded(object sender, RoutedEventArgs e) =>
			InitializeSettingsPartOfNavigationView(NavigationViewMain);


		/// <summary>
		/// Update settings page navigation view item display text and its tool tip.
		/// </summary>
		private static void InitializeSettingsPartOfNavigationView(NavigationView view)
		{
			var item = (NavigationViewItem)view.SettingsItem;
			item.Content = ResourceFinder.Current.NavigationViewItemSettings;
			ToolTipService.SetToolTip(item, ResourceFinder.Current.NavigationViewItemSettingsToolTip);
		}
	}
}
