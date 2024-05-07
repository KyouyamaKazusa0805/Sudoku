namespace SudokuStudio.Views.Pages.Formulae;

/// <summary>
/// Represents a user-defined formulae page.
/// </summary>
public sealed partial class UserDefinedFormulaePage : Page
{
	/// <summary>
	/// Initializes a <see cref="UserDefinedFormulaePage"/> instance.
	/// </summary>
	public UserDefinedFormulaePage() => InitializeComponent();


	private void AddFormulaButton_Click(object sender, RoutedEventArgs e) => App.GetMainWindow(this).NavigateToPage<AddFormulaPage>();

	private void Page_Loaded(object sender, RoutedEventArgs e)
	{

	}
}
