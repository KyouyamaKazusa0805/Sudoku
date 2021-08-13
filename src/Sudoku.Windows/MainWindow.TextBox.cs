namespace Sudoku.Windows;

partial class MainWindow
{
	private void TextBoxJumpTo_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (sender is TextBox { Text: var t } && int.TryParse(t, out int value))
		{
			int max = _puzzlesText!.Length;
			LoadPuzzle(_puzzlesText[Settings.CurrentPuzzleNumber = value].TrimEndNewLine());
			UpdateDatabaseControls(value != 0, value != 0, value != max - 1, value != max - 1);

			_labelPuzzleNumber.Content = $"{(value + 1).ToString()}/{max}";
		}
	}
}
