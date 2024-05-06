namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents rating formula page.
/// </summary>
public sealed partial class RatingFormulaPage : Page
{
	/// <summary>
	/// Initializes a <see cref="RatingFormulaPage"/> instance.
	/// </summary>
	public RatingFormulaPage() => InitializeComponent();


	private void GoToBuiltInFormulaeButton_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage<BuiltInFormulaePage>();

	private void GoToCustomizedFormulaeButton_Click(object sender, RoutedEventArgs e)
	{

	}

	private void GoToFunctionsDefinedButton_Click(object sender, RoutedEventArgs e)
	{

	}
}
