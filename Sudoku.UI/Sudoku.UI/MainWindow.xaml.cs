using Microsoft.UI.Xaml;

namespace Sudoku.UI
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		/// <summary>
		/// Initializes a <see cref="MainWindow"/> instance with the default instantiation behavior.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();

			InitializeTitle();
		}


		/// <summary>
		/// To initialize the property <see cref="Window.Title"/>.
		/// </summary>
		/// <remarks>
		/// When I filled with this property into the XAML page, it'll give me a complier error
		/// <c>WMC061: Property not found</c>, so I moved that statement to here.
		/// </remarks>
		private void InitializeTitle() => Title = "Sunnie's Sudoku Solution";
	}
}
