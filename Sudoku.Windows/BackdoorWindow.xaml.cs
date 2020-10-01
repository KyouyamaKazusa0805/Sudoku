#pragma warning disable IDE1006

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sudoku.Data.Collections;
using Sudoku.DocComments;
using Sudoku.Runtime;
using Sudoku.Solving.Checking;
using Sudoku.Windows.Constants;
using static Sudoku.Windows.MainWindow;
using Grid = Sudoku.Data.Grid;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>BackdoorWindow.xaml</c>.
	/// </summary>
	public partial class BackdoorWindow : Window
	{
		/// <summary>
		/// The puzzle.
		/// </summary>
		private readonly Grid _puzzle;


		/// <summary>
		/// Indicates the searching depth.
		/// </summary>
		private int _depth;


		/// <summary>
		/// Initializes an instance with the specified puzzle.
		/// </summary>
		/// <param name="puzzle">The puzzle.</param>
		public BackdoorWindow(Grid puzzle)
		{
			InitializeComponent();

			_puzzle = puzzle;
			_labelGrid.Content = $"{LangSource["BackdoorGrid"]}{_puzzle:#}";
		}


		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private async void ButtonStartSearching_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				_listBoxBackdoors.ClearValue(ItemsControl.ItemsSourceProperty);
				_labelStatus.Content = (string)LangSource["BackdoorWhileSearching"];

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
					Messagings.FailedToCheckDueToInvalidPuzzle();

					e.Handled = true;
					return;
				}

				_listBoxBackdoors.ItemsSource =
					from collection in collections select new ConclusionCollection(collection).ToString();
			}
		}

		/// <inheritdoc cref="Events.SelectionChanged(object?, EventArgs)"/>
		private void ComboBoxDepth_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ComboBox comboBox)
			{
				_depth = comboBox.SelectedIndex;
			}
		}
	}
}
