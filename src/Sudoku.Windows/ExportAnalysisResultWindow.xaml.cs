using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.IO;
using Sudoku.Solving;

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
		private static readonly Dictionary<AnalysisResultFormattingOptions, bool> FormatOptions = new()
		{
			[AnalysisResultFormattingOptions.ShowSeparators] = false,
			[AnalysisResultFormattingOptions.ShowStepLabel] = false,
			[AnalysisResultFormattingOptions.ShowSimple] = true,
			[AnalysisResultFormattingOptions.ShowBottleneck] = false,
			[AnalysisResultFormattingOptions.ShowDifficulty] = false,
			[AnalysisResultFormattingOptions.ShowStepsAfterBottleneck] = true,
			[AnalysisResultFormattingOptions.ShowAttributes] = false,
			[AnalysisResultFormattingOptions.ShowBackdoors] = false,
			[AnalysisResultFormattingOptions.ShowStepDetail] = false,
			[AnalysisResultFormattingOptions.ShowSteps] = true
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
				control.IsChecked = FormatOptions[
					(AnalysisResultFormattingOptions)int.Parse(control.Tag.ToString()!)];
			}

			_analysisResult = analysisResult;
			_settings = settings;
		}


		/// <summary>
		/// Create format string.
		/// </summary>
		/// <returns>The result format string.</returns>
		private AnalysisResultFormattingOptions CreateFormat()
		{
			var options = AnalysisResultFormattingOptions.None;
			foreach (var key in from pair in FormatOptions where pair.Value select pair.Key)
			{
				options |= key;
			}

			return options;
		}


		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonExport_Click(object sender, RoutedEventArgs e) =>
			_textBoxAnalysisResult.Text = _analysisResult.ToString(
				CreateFormat(),
				((WindowsSettings)_settings).LanguageCode
			);

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowSeparators_Click(object sender, RoutedEventArgs e) =>
			FormatOptions[AnalysisResultFormattingOptions.ShowSeparators] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowStepIndices_Click(object sender, RoutedEventArgs e) =>
			FormatOptions[AnalysisResultFormattingOptions.ShowStepLabel] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowLogic_Click(object sender, RoutedEventArgs e) =>
			FormatOptions[AnalysisResultFormattingOptions.ShowSimple] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowBottleneck_Click(object sender, RoutedEventArgs e) =>
			FormatOptions[AnalysisResultFormattingOptions.ShowBottleneck] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowDifficulty_Click(object sender, RoutedEventArgs e) =>
			FormatOptions[AnalysisResultFormattingOptions.ShowDifficulty] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckboxShowStepsAfterBottleneck_Click(object sender, RoutedEventArgs e) =>
			FormatOptions[AnalysisResultFormattingOptions.ShowStepsAfterBottleneck] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowAttributesOfPuzzle_Click(object sender, RoutedEventArgs e) =>
			FormatOptions[AnalysisResultFormattingOptions.ShowAttributes] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowMagicCells_Click(object sender, RoutedEventArgs e) =>
			FormatOptions[AnalysisResultFormattingOptions.ShowBackdoors] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowDifficultyDetail_Click(object sender, RoutedEventArgs e) =>
			FormatOptions[AnalysisResultFormattingOptions.ShowStepDetail] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void CheckBoxShowTechniqueSteps_Click(object sender, RoutedEventArgs e) =>
			FormatOptions[AnalysisResultFormattingOptions.ShowSteps] ^= true;

		/// <inheritdoc cref="Events.Click(object?, EventArgs)"/>
		private void ButtonOutputAnalysisResult_Click(object sender, RoutedEventArgs e)
		{
			var sfd = new SaveFileDialog
			{
				Filter = "Word document|*.docx|Text file|*.txt",
				FilterIndex = 0,
				Title = (string)Application.Current.Resources["TitleSavingPuzzles"]
			};

			if (sfd.ShowDialog() is true)
			{
				if
				(
					new AnalysisResultFileOutput(
						_analysisResult, _settings
					).TryExport(
						sfd.FileName,
						1000,
						_checkBoxOutputStepGrids.IsChecked ?? false,
						CreateFormat(),
						PictureFileType.Png,
						(AnalysisResultOutputType)(sfd.FilterIndex - 1),
						Alignment.Middle
					)
				)
				{
					Messagings.SaveSuccess();
				}
			}
		}
	}
}
