namespace SudokuStudio.Views.Pages.Formulae;

/// <summary>
/// Represents a page that can add formulae.
/// </summary>
public sealed partial class AddFormulaPage : Page
{
	/// <summary>
	/// Initializes an <see cref="AddFormulaPage"/> instance.
	/// </summary>
	public AddFormulaPage() => InitializeComponent();


	private void CancelButton_Click(object sender, RoutedEventArgs e)
		=> App.GetMainWindow(this).NavigateToPage<UserDefinedFormulaePage>();
}
