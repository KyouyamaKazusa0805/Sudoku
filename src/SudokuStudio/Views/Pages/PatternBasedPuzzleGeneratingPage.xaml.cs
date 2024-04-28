namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents a page that generates for pattern-based puzzles.
/// </summary>
[DependencyProperty<CellMap>("SelectedCells", Accessibility = Accessibility.Internal, DocSummary = "Indicates the selected cells.")]
public sealed partial class PatternBasedPuzzleGeneratingPage : Page
{
	/// <summary>
	/// Defines a user-defined view that will be used.
	/// </summary>
	/// <seealso cref="SudokuPane.CurrentPaneMode"/>
	private readonly ViewUnitBindableSource _userColoringView = new();


	/// <summary>
	/// Initializes a <see cref="PatternBasedPuzzleGeneratingPage"/> instance.
	/// </summary>
	public PatternBasedPuzzleGeneratingPage() => InitializeComponent();


	[Callback]
	private static void SelectedCellsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is not (PatternBasedPuzzleGeneratingPage { _userColoringView: var view, SudokuPane: var pane } page, { NewValue: CellMap newValue }))
		{
			return;
		}

		var (a, r, g, b) = App.CurrentTheme switch
		{
			ApplicationTheme.Light => (Color)page.Resources["SelectedCellColorLight"]!,
			_ => (Color)page.Resources["SelectedCellColorDark"]!
		};
		view.View.Clear();
		view.View.AddRange(
			from cell in newValue
			select new CellViewNode(new ColorColorIdentifier(a, r, g, b), cell)
			{
				RenderingMode = RenderingMode.BothDirectAndPencilmark
			}
		);

		pane.ViewUnit = null;
		pane.ViewUnit = view;
	}


	private void SudokuPane_Loaded(object sender, RoutedEventArgs e)
		=> ((App)Application.Current).CoverSettingsToSudokuPaneViaApplicationTheme(SudokuPane);

	private void SudokuPane_ActualThemeChanged(FrameworkElement sender, object args)
		=> ((App)Application.Current).CoverSettingsToSudokuPaneViaApplicationTheme(SudokuPane);

	private void SudokuPane_Clicked(SudokuPane sender, GridClickedEventArgs e)
	{
		if (e is not { Cell: var cell and not -1 })
		{
			return;
		}

		var newValue = SelectedCells;
		newValue.Toggle(cell);
		SelectedCells = newValue;
	}

	private void SudokuPane_DigitInput(SudokuPane sender, DigitInputEventArgs e) => e.Cancel = true;
}
