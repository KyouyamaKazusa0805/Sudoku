using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sudoku.Data;
using Sudoku.Data.Collections;
using Sudoku.Solving.Checking;
using static Sudoku.Windows.Constants.Processings;

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
		private readonly IReadOnlyGrid _puzzle;


		/// <summary>
		/// Initializes an instance with the specified puzzle.
		/// </summary>
		/// <param name="puzzle">The puzzle.</param>
		public BugNSearchWindow(IReadOnlyGrid puzzle)
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
					from candidate in await Task.Run(() => new BugChecker(_puzzle).GetAllTrueCandidates(64))
					orderby candidate
					let Str = new CandidateCollection(candidate).ToString()
					select new PrimaryElementTuple<int, string>(candidate, Str, 2)).ToArray();

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
