using System;
using System.Windows.Controls;
using Sudoku.DocComments;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		/// <inheritdoc cref="Events.SelectionChanged(object?, EventArgs)"/>
		private void ComboBoxMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// While initializing, the selection index will be changed to 0.
			// During changing, the label or combo box may be null in this case.
			// So here need null checking.
			if (sender is ComboBox comboBox
				&& (_labelSymmetry, _comboBoxSymmetry, _labelBackdoorFilteringDepth, _comboBoxBackdoorFilteringDepth)
				is (not null, not null, not null, not null))
			{
				Settings.GeneratingModeComboBoxSelectedIndex = comboBox.SelectedIndex;
				SwitchOnGeneratingComboBoxesDisplaying();
			}
		}

		/// <inheritdoc cref="Events.SelectionChanged(object?, EventArgs)"/>
		private void ComboBoxSymmetry_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// When initializing, 'Settings' may be null.
			if ((sender, Settings) is (ComboBox comboBox, not null))
			{
				Settings.GeneratingSymmetryModeComboBoxSelectedIndex = comboBox.SelectedIndex;
			}
		}

		/// <inheritdoc cref="Events.SelectionChanged(object?, EventArgs)"/>
		private void ComboBoxBackdoorFilteringDepth_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// When initializing, 'Settings' may be null.
			if ((sender, Settings) is (ComboBox comboBox, not null))
			{
				Settings.GeneratingBackdoorSelectedIndex = comboBox.SelectedIndex;
			}
		}

		/// <inheritdoc cref="Events.SelectionChanged(object?, EventArgs)"/>
		private void ComboBoxDifficulty_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// When initializing, 'Settings' may be null.
			if ((sender, Settings) is (ComboBox comboBox, not null))
			{
				Settings.GeneratingDifficultyLevelSelectedIndex = comboBox.SelectedIndex;
			}
		}
	}
}
