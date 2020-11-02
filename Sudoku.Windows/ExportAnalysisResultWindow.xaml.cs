using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Extensions;
using Sudoku.IO;
using Sudoku.Solving;
using Sudoku.Windows.Constants;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for ExportAnalysisResultWindow.xaml
	/// </summary>
	public partial class ExportAnalysisResultWindow : Window
	{
		/// <summary>
		/// The analysis result.
		/// </summary>
		private readonly AnalysisResult _analysisResult;

		/// <summary>
		/// The settings.
		/// </summary>
		private readonly Settings _settings;

		/// <summary>
		/// The internal dictionary of all format characters.
		/// </summary>
		private static readonly IDictionary<char, bool> FormatCharacterList = new Dictionary<char, bool>
		{
			['-'] = false, // Show separators.
			['#'] = false, // Show step indices.
			['@'] = true, // Don't show eliminations.
			['?'] = false, // Show bottleneck.
			['!'] = false, // Show difficulty rating of each step.
			['.'] = true, // Don't show steps after bottleneck.
			['a'] = false, // Show attributes of this puzzle (if exists).
			['b'] = false, // Show magic cells.
			['d'] = false, // Show difficulty details.
			['l'] = true // Show technique steps.
		};


		/// <summary>
		/// Initializes an instance with the specified analysis result.
		/// </summary>
		/// <param name="analysisResult">The analysis result.</param>
		/// <param name="settings">The settings.</param>
		public ExportAnalysisResultWindow(AnalysisResult analysisResult, Settings settings)
		{
			InitializeComponent();

			// Initialize controls.
			foreach (var control in _gridMain.Children.OfType<CheckBox>())
			{
				control.IsChecked = FormatCharacterList[control.Tag.ToString()![0]];
			}

			_analysisResult = analysisResult;
			_settings = settings;
		}


		/// <summary>
		/// Create format string.
		/// </summary>
		/// <returns>The result format string.</returns>
		private string CreateFormatString()
		{
			var format = new StringBuilder();
			foreach (char key in from pair in FormatCharacterList where pair.Value select pair.Key)
			{
				format.Append(key);
			}

			return format.ToString();
		}


		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonExport_Click(object sender, RoutedEventArgs e) =>
			_textBoxAnalysisResult.Text = _analysisResult.ToString(CreateFormatString());

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowSeparators_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['-'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowStepIndices_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['#'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowLogic_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['@'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowBottleneck_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['?'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowDifficulty_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['!'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckboxShowStepsAfterBottleneck_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['.'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowAttributesOfPuzzle_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['a'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowMagicCells_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['b'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowDifficultyDetail_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['d'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowTechniqueSteps_Click(object sender, RoutedEventArgs e) =>
			FormatCharacterList['l'] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonOutputAnalysisResult_Click(object sender, RoutedEventArgs e)
		{
			if (
				new SaveFileDialog
				{
					Filter = "Word document|*.docx|Text file|*.txt",
					FilterIndex = 0,
					Title = (string)Application.Current.Resources["TitleSavingPuzzles"]
				} is var sfd && sfd.ShowDialog() is true
				&& new AnalysisResultFileOutput(_analysisResult, _settings).Export(
					sfd.FileName,
					500,
					_checkBoxOutputStepGrids.IsChecked.GetValueOrDefault(),
					CreateFormatString(),
					PictureFileType.Png,
					(AnalysisResultOutputType)(sfd.FilterIndex - 1),
					Alignment.Middle))
			{
				Messagings.SaveSuccess();
			}
		}
	}
}
