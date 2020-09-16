using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sudoku.Data;
using Sudoku.Solving.Checking;
using static Sudoku.Windows.Constants.Processings;
using Grid = Sudoku.Data.Grid;

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
		private readonly Grid _puzzle;


		/// <summary>
		/// Initializes an instance with the specified puzzle.
		/// </summary>
		/// <param name="puzzle">The puzzle.</param>
		public BugNSearchWindow(Grid puzzle)
		{
			InitializeComponent();

			_puzzle = puzzle;
			_labelGrid.Content = $"{LangSource["BugMultipleGrid"]}{_puzzle:#}";
		}


		[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
		private async void ButtonStartSearching_Click(object sender, RoutedEventArgs e)
		{
			await internalOperation();

			async Task internalOperation()
			{
				_listBoxTrueCandidates.ClearValue(ItemsControl.ItemsSourceProperty);
				_labelStatus.Content = (string)LangSource["BugMultipleWhileSearching"];

				var array = (
					from Candidate in await new BugChecker(_puzzle).GetAllTrueCandidatesAsync(64)
					orderby Candidate
					let Str = new SudokuMap { Candidate }.ToString()
					select new KeyedTuple<int, string>(Candidate, Str, 2)).ToArray();

				_labelStatus.ClearValue(ContentProperty);
				int count = array.Length;
				if (count == 0)
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
