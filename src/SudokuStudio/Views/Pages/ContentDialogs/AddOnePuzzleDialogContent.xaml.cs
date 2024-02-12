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
		if (sender is not TextBox { Text: var input })
		{
			return;
		}

		switch (input)
		{
			case "":
			{
				return;
			}
			case var _ when Grid.TryParse(input, out _):
			{
				WarningTextDisplayer.Visibility = Visibility.Collapsed;
				break;
			}
			default:
			{
				WarningTextDisplayer.Visibility = Visibility.Visible;
				break;
			}
		}
	}
}
