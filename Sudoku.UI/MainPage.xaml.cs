#nullable enable annotations

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
		/// Indicates the preferences used.
		/// </summary>
		private Preferences? _preferences;


		/// <summary>
		/// Initializes a <see cref="MainPage"/> instance using the default behavior.
		/// </summary>
		public MainPage() => InitializeComponent();


		/// <summary>
		/// Update settings page navigation view item display text and its tool tip.
		/// </summary>
		/// <param name="view">The navigation view to change.</param>
		private static void UpdateSettingsDisplayTextAndItsToolTip(NavigationView view)
		{
			var item = (NavigationViewItem)view.SettingsItem;
			item.Content = LangSource["NavigationViewItemSettings"];
			ToolTipService.SetToolTip(item, LangSource["NavigationViewItemSettingsToolTip"]);
		}


		/// <summary>
		/// Triggers when the page is loaded.
		/// </summary>
		/// <param name="sender">The object to trigger this event.</param>
		/// <param name="e">The event arguments provided.</param>
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			JsonSerializer.
		}
	}
}
