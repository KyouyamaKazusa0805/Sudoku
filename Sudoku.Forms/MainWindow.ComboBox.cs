using System.Windows;
using System.Windows.Controls;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void ComboBoxMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// When initializing, the selection index will be changed to 0.
			// During changing, the label or combo box may be null in this case.
			// So here need null checking.
			if (sender is ComboBox comboBox
				&& !(_labelSymmetry is null) && !(_comboBoxSymmetry is null))
			{
				switch (comboBox.SelectedIndex)
				{
					case 0: // Symmetry mode.
					{
						_labelSymmetry.Visibility = Visibility.Visible;
						_comboBoxSymmetry.Visibility = Visibility.Visible;
						return;
					}
					case 1: // Hard pattern mode.
					{
						_labelSymmetry.Visibility = Visibility.Hidden;
						_comboBoxSymmetry.Visibility = Visibility.Hidden;
						return;
					}
					default:
					{
						// What the hell is this selection???
						e.Handled = true;
						return;
					}
				}
			}
		}
	}
}
