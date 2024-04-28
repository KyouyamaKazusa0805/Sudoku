namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents a page that generates for pattern-based puzzles.
/// </summary>
public sealed partial class PatternBasedPuzzleGeneratingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="PatternBasedPuzzleGeneratingPage"/> instance.
	/// </summary>
	public PatternBasedPuzzleGeneratingPage() => InitializeComponent();


	private void SudokuPane_Loaded(object sender, RoutedEventArgs e)
		=> ((App)Application.Current).CoverSettingsToSudokuPaneViaApplicationTheme(SudokuPane);

	private void SudokuPane_ActualThemeChanged(FrameworkElement sender, object args)
		=> ((App)Application.Current).CoverSettingsToSudokuPaneViaApplicationTheme(SudokuPane);

	private void SudokuPane_Clicked(SudokuPane sender, GridClickedEventArgs e)
	{
		var cell = e.Cell;
		Console.WriteLine(cell);
	}
}
