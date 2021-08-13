#pragma warning disable IDE1006

namespace Sudoku.Windows;

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
	/// <param name="puzzle">The puzzle.</param>
	public BugNSearchWindow(in SudokuGrid puzzle)
	{
		InitializeComponent();

		_puzzle = puzzle;
		_labelGrid.Content = $"{LangSource["BugMultipleGrid"]}{_puzzle.ToString("#")}";
	}


	private async void ButtonStartSearching_Click(object sender, RoutedEventArgs e)
	{
		await internalOperation();

		async Task internalOperation()
		{
			_listBoxTrueCandidates.ClearValue(ItemsControl.ItemsSourceProperty);
			_labelStatus.Content = (string)LangSource["BugMultipleWhileSearching"];

			var trueCandidates = await new BugChecker(_puzzle).GetAllTrueCandidatesAsync(64);
			var array = new KeyedTuple<int, string>[trueCandidates.Count];
			int i = 0;
			foreach (int candidate in new Candidates(trueCandidates))
			{
				array[i++] = new(candidate, new Candidates { candidate }.ToString(), 2);
			}

			_labelStatus.ClearValue(ContentProperty);
			int count = array.Length;
			if (count != 0)
			{
				_listBoxTrueCandidates.ItemsSource = array;
				_labelStatus.Content =
					$"{LangSource[count == 1 ? "ThereIs" : "ThereAre"]} " +
					$"{count.ToString()} {LangSource[$"BugMultipleTrueCandidates{(count == 1 ? "Singular" : "Plural")}"]}. " +
					LangSource["BugMultipleSuccessfulCase"];
			}
			else
			{
				_labelStatus.Content = LangSource["BugMultipleFailCase"];
			}
		}
	}
}
