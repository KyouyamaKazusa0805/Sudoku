namespace SudokuStudio.Views.Pages.Operation;

/// <summary>
/// The shuffle operation page.
/// </summary>
public sealed partial class ShuffleOperation : Page, IOperationProviderPage
{
	/// <summary>
	/// Initializes a <see cref="ShuffleOperation"/> instance.
	/// </summary>
	public ShuffleOperation() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;


	private void LeftRightSwapButton_Click(object sender, RoutedEventArgs e)
	{
		var modified = BasePage.SudokuPane.Puzzle;
		modified.MirrorLeftRight();
		BasePage.SudokuPane.Puzzle = modified;
		BasePage.SudokuPane.ViewUnit = null;
	}

	private void TopBottomSwapButton_Click(object sender, RoutedEventArgs e)
	{
		var modified = BasePage.SudokuPane.Puzzle;
		modified.MirrorTopBottom();
		BasePage.SudokuPane.Puzzle = modified;
		BasePage.SudokuPane.ViewUnit = null;
	}

	private void DiagonalSwapButton_Click(object sender, RoutedEventArgs e)
	{
		var modified = BasePage.SudokuPane.Puzzle;
		modified.MirrorDiagonal();
		BasePage.SudokuPane.Puzzle = modified;
		BasePage.SudokuPane.ViewUnit = null;
	}

	private void AntidiagonalSwapButton_Click(object sender, RoutedEventArgs e)
	{
		var modified = BasePage.SudokuPane.Puzzle;
		modified.MirrorAntidiagonal();
		BasePage.SudokuPane.Puzzle = modified;
		BasePage.SudokuPane.ViewUnit = null;
	}

	private void RotateClockwiseButton_Click(object sender, RoutedEventArgs e)
	{
		var modified = BasePage.SudokuPane.Puzzle;
		modified.RotateClockwise();
		BasePage.SudokuPane.Puzzle = modified;
		BasePage.SudokuPane.ViewUnit = null;
	}

	private void RotateCounterclockwiseButton_Click(object sender, RoutedEventArgs e)
	{
		var modified = BasePage.SudokuPane.Puzzle;
		modified.RotateCounterclockwise();
		BasePage.SudokuPane.Puzzle = modified;
		BasePage.SudokuPane.ViewUnit = null;
	}

	private void RotatePiButton_Click(object sender, RoutedEventArgs e)
	{
		var modified = BasePage.SudokuPane.Puzzle;
		modified.RotatePi();
		BasePage.SudokuPane.Puzzle = modified;
		BasePage.SudokuPane.ViewUnit = null;
	}

	private void AdjustToMakeIttoryuButton_Click(object sender, RoutedEventArgs e)
	{
		var modified = BasePage.SudokuPane.Puzzle;
		var techniques = ((App)Application.Current).Preference.AnalysisPreferences.IttoryuSupportedTechniques;
		if (!modified.IsIttoryu([.. techniques], out var digitPath)
			|| digitPath is not { } path)
		{
			InfoDialog_DisorderedIttoryuDigitSequence.IsOpen = true;
			return;
		}

		modified.MakeIttoryu(path);
		BasePage.SudokuPane.Puzzle = modified;
		BasePage.SudokuPane.ViewUnit = null;
	}

	private void ToMinLexButton_Click(object sender, RoutedEventArgs e)
	{
		var modified = BasePage.SudokuPane.Puzzle;
		if (modified.PuzzleType == SudokuType.Sukaku)
		{
			InfoDialog_NotSupportedForSukaku.IsOpen = true;
			return;
		}

		modified.MakeMinLex();
		BasePage.SudokuPane.Puzzle = modified;
		BasePage.SudokuPane.ViewUnit = null;
	}
}
