using System.Windows.Controls;
using System.Windows.Input;
using Sudoku.Solving.Manual;
using static Sudoku.Windows.Constants.Processings;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		private void TextBoxJumpTo_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (sender is TextBox textBox && int.TryParse(textBox.Text, out int value))
			{
				int max = _puzzlesText!.Length;
				LoadPuzzle(_puzzlesText[Settings.CurrentPuzzleNumber = value].TrimEnd(Splitter));
				UpdateDatabaseControls(value != 0, value != 0, value != max - 1, value != max - 1);

				_labelPuzzleNumber.Content = $"{value + 1}/{max}";
			}
		}
	}
}
