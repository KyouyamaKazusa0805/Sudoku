#pragma warning disable IDE1006

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Solving.Checking;
using static Sudoku.Windows.MainWindow;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>BugNSearchWindow.xaml</c>.
	/// </summary>
	public partial class BugNSearchWindow : Window
	{
		/// <summary>
		/// The puzzle.
		/// </summary>
		private readonly SudokuGrid _puzzle;


		/// <summary>
		/// Initializes an instance with the specified puzzle.
		/// </summary>
		/// <param name="puzzle">(<see langword="in"/> paramter) The puzzle.</param>
		public BugNSearchWindow(in SudokuGrid puzzle)
		{
			InitializeComponent();

			_puzzle = puzzle;
			_labelGrid.Content = $"{LangSource["BugMultipleGrid"]}{_puzzle:#}";
		}


		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private async void ButtonStartSearching_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				_listBoxTrueCandidates.ClearValue(ItemsControl.ItemsSourceProperty);
				_labelStatus.Content = (string)LangSource["BugMultipleWhileSearching"];

				var array = (
					from candidate in await new BugChecker(_puzzle).GetAllTrueCandidatesAsync(64)
					orderby candidate
					let str = new SudokuMap { candidate }.ToString()
					select new KeyedTuple<int, string>(candidate, str, 2)).ToArray();

				_labelStatus.ClearValue(ContentProperty);
				if (array.Length is var count && count == 0)
				{
					_labelStatus.Content = (string)LangSource["BugMultipleFailCase"];
				}
				else
				{
					_listBoxTrueCandidates.ItemsSource = array;
					string singularOrPlural = count == 1 ? string.Empty : "s";
					_labelStatus.Content =
						$"{LangSource[count == 1 ? "ThereIs" : "ThereAre"]} " +
						$"{count} {LangSource[$"BugMultipleTrueCandidates{(count == 1 ? "Singular" : "Plural")}"]}. " +
						LangSource["BugMultipleSuccessfulCase"];
				}
			}
		}
	}
}
