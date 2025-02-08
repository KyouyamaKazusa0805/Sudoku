namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents command-based drawing page.
/// </summary>
public sealed partial class CommandBasedDrawingPage : Page
{
	/// <summary>
	/// Initializes a <see cref="CommandBasedDrawingPage"/> instance.
	/// </summary>
	public CommandBasedDrawingPage() => InitializeComponent();


	private void SudokuPane_Loaded(object sender, RoutedEventArgs e)
	{
		var app = Application.Current.AsApp();
		app.CoverSettingsToSudokuPaneViaApplicationTheme(SudokuPane);
		app.MainSudokuPane = SudokuPane;
	}

	private void SudokuPane_ActualThemeChanged(FrameworkElement sender, object args)
		=> Application.Current.AsApp().CoverSettingsToSudokuPaneViaApplicationTheme(SudokuPane);

	private void ApplyButton_Click(object sender, RoutedEventArgs e)
	{

	}
}
