namespace SudokuStudio.Views.Pages.ContentDialogs;

/// <summary>
/// Represents "add one puzzle" dialog content.
/// </summary>
public sealed partial class AddOnePuzzleDialogContent : Page
{
	/// <summary>
	/// Initializes an <see cref="AddOnePuzzleDialogContent"/> instance.
	/// </summary>
	public AddOnePuzzleDialogContent() => InitializeComponent();


	private void TextCodeInput_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (sender is TextBox { Text: var input })
		{
			WarningTextDisplayer.Visibility = input switch
			{
				"" => Visibility.Collapsed,
				_ when Grid.TryParse(input, out _) => Visibility.Collapsed,
				_ => Visibility.Visible
			};
		}
	}
}
