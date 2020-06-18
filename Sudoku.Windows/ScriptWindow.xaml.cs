using System.Windows;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>ScriptWindow.xaml</c>.
	/// </summary>
	public partial class ScriptWindow : Window
	{
		/// <summary>
		/// Indicates the base window.
		/// </summary>
		private readonly MainWindow _baseWindow;


		/// <summary>
		/// Initializes an instance with the specified base window.
		/// </summary>
		/// <param name="baseWindow">The base window.</param>
		public ScriptWindow(MainWindow baseWindow)
		{
			InitializeComponent();

			_baseWindow = baseWindow;
		}

		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
