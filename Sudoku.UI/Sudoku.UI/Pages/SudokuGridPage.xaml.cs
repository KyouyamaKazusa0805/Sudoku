using Microsoft.UI.Xaml.Controls;

namespace Sudoku.UI.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SudokuGridPage : Page
	{
		/// <summary>
		/// Initializes a <see cref="SudokuGridPage"/> instance with the default instantiation behavior.
		/// </summary>
		public SudokuGridPage() => InitializeComponent();

		/// <summary>
		/// Initializes an instance with the specified base window.
		/// </summary>
		/// <param name="baseWindow">The base window.</param>
		public SudokuGridPage(object baseWindow) : this() => BaseWindow = (MainWindow)baseWindow;


		/// <summary>
		/// Indicates the base window.
		/// </summary>
		public MainWindow BaseWindow { get; internal set; } = null!;
	}
}
