using w = System.Windows;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private void ComboBoxMode_SelectionChanged(object sender, w::Controls.SelectionChangedEventArgs e)
		{
			if (sender is w::Controls.ComboBox comboBox
				&& !(_labelSymmetry is null) && !(_comboBoxSymmetry is null))
			{
				switch (_seletedGeneratingMode = comboBox.SelectedIndex)
				{
					case 0: // Symmetry mode.
					{
						_labelSymmetry.Visibility = w::Visibility.Visible;
						_comboBoxSymmetry.Visibility = w::Visibility.Visible;
						return;
					}
					case 1: // Hard pattern mode.
					{
						_labelSymmetry.Visibility = w::Visibility.Hidden;
						_comboBoxSymmetry.Visibility = w::Visibility.Hidden;
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
