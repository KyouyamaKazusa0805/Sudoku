using System.Windows;

namespace Sudoku.UI
{
	/// <summary>
	/// Interaction logic for <see cref="MainWindow"/>.
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Initializes an instance with the normal instantiation behavior.
		/// </summary>
		public MainWindow() => InitializeComponent();


		/// <summary>
		/// Triggers when the quit command is triggered.
		/// </summary>
		/// <param name="sender">The object to trigger the event.</param>
		/// <param name="e">The event arguments provided.</param>
		private void ButtonQuit_Click(object sender, RoutedEventArgs e) => Close();
	}
}
