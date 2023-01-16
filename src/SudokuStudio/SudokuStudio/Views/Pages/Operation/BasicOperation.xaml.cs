namespace SudokuStudio.Views.Pages.Operation;

/// <summary>
/// Indicates the basic operation command bar.
/// </summary>
public sealed partial class BasicOperation : Page
{
	/// <summary>
	/// Defines a default puzzle generator.
	/// </summary>
	private static readonly PatternBasedPuzzleGenerator Generator = new();


	/// <summary>
	/// Initializes a <see cref="BasicOperation"/> instance.
	/// </summary>
	public BasicOperation() => InitializeComponent();


	/// <summary>
	/// Indicates the base page.
	/// </summary>
	public AnalyzePage BasePage { get; set; } = null!;


	private async void NewPuzzleButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		BasePage.GeneratorIsNotRunning = false;

		var grid = await Generator.GenerateAsync();

		BasePage.GeneratorIsNotRunning = true;

		BasePage.SudokuPane.Puzzle = grid;
	}
}
