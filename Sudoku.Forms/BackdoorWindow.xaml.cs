using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sudoku.Data;
using Sudoku.Solving.Checking;
using Sudoku.Solving.Utils;

namespace Sudoku.Forms
{
	/// <summary>
	/// Interaction logic for <c>BackdoorWindow.xaml</c>.
	/// </summary>
	public partial class BackdoorWindow : Window
	{
		/// <summary>
		/// The puzzle.
		/// </summary>
		private readonly IReadOnlyGrid _puzzle;

		
		/// <summary>
		/// Indicates the searching depth.
		/// </summary>
		private int _depth;


		/// <summary>
		/// Initializes an instance with the specified puzzle.
		/// </summary>
		/// <param name="puzzle">The puzzle.</param>
		public BackdoorWindow(IReadOnlyGrid puzzle)
		{
			InitializeComponent();

			_puzzle = puzzle;
			_labelGrid.Content = $"Grid: {_puzzle:#}";
		}

		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void ButtonStartSearching_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async ValueTask internalOperation()
			{
				_listBoxBackdoors.ClearValue(ItemsControl.ItemsSourceProperty);
				_labelStatus.Content = "Searching... The searching should be slow. Please wait.";

				var collections = await Task.Run(() =>
				{
					try
					{
						return new BackdoorSearcher().SearchForBackdoors(_puzzle, _depth);
					}
					catch (SudokuRuntimeException)
					{
						return null;
					}
				});

				_labelStatus.ClearValue(ContentProperty);
				if (collections is null)
				{
					MessageBox.Show(
						"The specified grid is invalid. The possible case is that the grid has no or multiple solutions.",
						"Warning");

					e.Handled = true;
					return;
				}

				_listBoxBackdoors.ItemsSource =
					from collection in collections
					select ConclusionCollection.ToString(collection);
			}
		}

		private void ComboBoxDepth_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ComboBox comboBox)
			{
				_depth = comboBox.SelectedIndex;
			}
		}
	}
}
