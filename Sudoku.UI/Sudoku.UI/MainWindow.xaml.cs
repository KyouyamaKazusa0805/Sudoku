using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sudoku.UI.Dictionaries;

namespace Sudoku.UI
{
	/// <summary>
	/// The main window of the program. This program will open this window at first.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		/// <summary>
		/// Initializes a <see cref="MainWindow"/> instance with the default instantiation behavior.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();

			InitializeProgramTitle();
		}


		/// <summary>
		/// To initialize the property <see cref="Window.Title"/>.
		/// </summary>
		/// <remarks>
		/// When I filled with this property into the XAML page, it'll give me a complier error
		/// <c>WMC061: Property not found</c>, so I moved that statement to here.
		/// </remarks>
		private void InitializeProgramTitle() => Title = ResourceFinder.Current.ProgramTitle;
	}
}
